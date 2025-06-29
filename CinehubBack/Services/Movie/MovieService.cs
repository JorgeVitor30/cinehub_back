using System.Linq.Expressions;
using System.Net;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CinehubBack.Data;
using CinehubBack.Data.Movie;
using CinehubBack.Expections;
using CinehubBack.Model;
using Microsoft.EntityFrameworkCore;

namespace CinehubBack.Services.Movie;

public class MovieService: IMovieService
{
    private readonly IRepository<Model.Movie> _repository;
    private readonly IMapper _mapper;
    private readonly IRepository<Favorites> _favoritesRepository;
    private readonly IRepository<Model.Rate> _rateRepository;
    private readonly IRepository<Model.User> _userRepository;
    private readonly IImageUploadService _imageUploadService;
    private readonly IEnumerable<IMovieFilter> _movieFilters;
    private const int DefaultPageSize = 10;
        
    public MovieService(IRepository<Model.Movie> repository, IMapper mapper, IRepository<Favorites> favoritesRepository, IRepository<Model.Rate> rateRepository, IRepository<Model.User> userRepository, IImageUploadService imageUploadService, IEnumerable<IMovieFilter> movieFilters)
    {
        _repository = repository;
        _rateRepository = rateRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _favoritesRepository = favoritesRepository;
        _imageUploadService = imageUploadService;
        _movieFilters = movieFilters;
    }
    
    public ReadMovieDto Create(CreateMovieDto createMovieDto)
    {
        var movie = _mapper.Map<Model.Movie>(createMovieDto);
        
        CheckForDuplicate(m => m.Title == movie.Title, "Movie with this title already exists");
        if (movie.RunTime < 1) { throw new BaseException(ErrorCode.BadRequest(), HttpStatusCode.BadRequest, "RunTime must be greater than 0");}
        
        _repository.Create(movie);
        _repository.SaveChanges();
        return _mapper.Map<ReadMovieDto>(movie);
    }

    public Page<ReadMovieDto> GetAll(Parameter parameter, string userId)
    {
        return _repository.GetAll<ReadMovieDto>(query =>
        {
            query = ApplyFilters(query, parameter, userId);
            
            var sortBy = parameter.Get<string>("sortBy")?.ToLower();

            query = sortBy switch
            {
                "title" => query.OrderBy(m => m.Title),
                "releasedate" => query.OrderByDescending(m => m.ReleaseDate),
                "voteaverage" => query.OrderByDescending(m => m.VoteAverage),
                _ => query.OrderByDescending(m => m.Popularity)
            };
            
            return query.ProjectTo<ReadMovieDto>(_mapper.ConfigurationProvider);
        }, parameter);
    }
    
    private IQueryable<Model.Movie> ApplyFilters(IQueryable<Model.Movie> query, Parameter parameter, string userId)
    {
        foreach (var filter in _movieFilters)
        {
            query = filter.Apply(query, parameter, userId);
        }

        return query;
    }
    
    public ReadMovieDto GetById(Guid id)
    {
        return _mapper.Map<ReadMovieDto>(GetByIdOrThrow(id));
    }
    
    private Model.Movie GetByIdOrThrow(Guid id)
    {
        return _repository.GetById(id)
               ?? throw new BaseException(
                   ErrorCode.NotFound<Model.Movie>(),
                   HttpStatusCode.NotFound,
                   "Movie not found"
               );
    }
    
    public ReadHomeMovieDto GetHome()
    {
        var popularMovies = _repository.GetAllList<ReadMovieDto>(
            query => query
                .OrderByDescending(m => m.Popularity)
                .Take(DefaultPageSize)
                .ProjectTo<ReadMovieDto>(_mapper.ConfigurationProvider)
        );

        var newReleases = _repository.GetAllList<ReadMovieDto>(
            query => query
                .OrderByDescending(m => m.ReleaseDate)
                .Take(DefaultPageSize)
                .ProjectTo<ReadMovieDto>(_mapper.ConfigurationProvider)
        );

        var classicMovies = _repository.GetAllList<ReadMovieDto>(
            query => query
                .Where(m => m.VoteAverage > 8 && !m.Adult)
                .OrderBy(m => m.ReleaseDate)
                .Take(DefaultPageSize)
                .ProjectTo<ReadMovieDto>(_mapper.ConfigurationProvider)
        );

        return new ReadHomeMovieDto
        {
            PopularMovies = popularMovies.ToArray(),
            NewReleaseMovies = newReleases.ToArray(),
            ClassicMovies = classicMovies.ToArray()
        };
    }
    
    public void DeleteById(Guid id)
    {
        _repository.DeleteById(id);
        _repository.SaveChanges();
    }
    
    private void CheckForDuplicate(Expression<Func<Model.Movie, bool>> predicate, string errorMessage)
    {
        var exists = _repository.Raw<Model.Movie?>(query => query.FirstOrDefault(predicate));

        if (exists is not null)
        {
            throw new BaseException(
                ErrorCode.BadRequest(),
                HttpStatusCode.BadRequest,
                errorMessage
            );
        }
    }

    public ResponseUploadImgDto AddPhotoMovies(Guid id, AddMoviePhotosDto addMoviePhotos)
    {
        var movie = _repository.GetById(id);
        if (movie is null)
        {
            throw new BaseException("404", HttpStatusCode.NotFound, "Movie not found");
        }
        
        var responsePhotos = _imageUploadService.UploadImage(addMoviePhotos.PosterPhoto, addMoviePhotos.BackPhoto);
        
        movie.BackPhotoUrl = responsePhotos.BackPhotoUrl;
        movie.PosterPhotoUrl = responsePhotos.PosterPhotoUrl;
        _repository.Update(movie);
        _repository.SaveChanges();
        
        return new ResponseUploadImgDto
        {
            BackPhotoUrl = responsePhotos.BackPhotoUrl,
            PosterPhotoUrl = responsePhotos.PosterPhotoUrl
        };
    }

    public List<ReadMovieDto> MovieRecommends(Guid id)
    {
        var user = _userRepository.GetById(id) ?? throw new BaseException("404", HttpStatusCode.NotFound, "User not found");

        var rates = _rateRepository.Raw(query => query.Where(f => f.UserId.Equals(user.Id))).ToList();
        if (rates.Count() < 10)
        {
            throw new BaseException("400", HttpStatusCode.BadRequest,
                "User has not 10 favorites movies to get Recommends");
        }

        var topFavoritesRate = rates.Where(f => f.RateValue >= 7).ToList();
        var favoriteMovies = topFavoritesRate.Select(f => _repository.GetById(f.MovieId)).ToList();
        var allGenres = favoriteMovies
            .SelectMany(m => m.Genres?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>())
            .Select(g => g.Trim())
            .GroupBy(g => g)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .ToList();        
        var allProductions = favoriteMovies
            .SelectMany(m => m.Productions?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>())
            .Select(p => p.Trim())
            .GroupBy(p => p)
            .OrderByDescending(p => p.Count())
            .Select(p => p.Key)
            .ToList();
        var allKeywords = favoriteMovies
            .SelectMany(m => m.KeyWords?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>())
            .Select(k => k.Trim())
            .GroupBy(k => k)
            .OrderByDescending(k => k.Count())
            .Select(k => k.Key)
            .ToList();

        var preferredLanguage = favoriteMovies.GroupBy(m => m.OriginalLanguage).OrderByDescending(g => g.Count()).First().Key;
        
        var avgRuntime = favoriteMovies.Average(m => m.RunTime);
        var avgPopularity = (double)favoriteMovies.Average(m => m.Popularity);
        var avgBudget = (double)favoriteMovies.Average(m => m.Budget);

        var ratedMovieIds = rates.Select(r => r.MovieId).ToHashSet();
        var candidateMovies = _repository.GetAllList(query => query.Where(m => !ratedMovieIds.Contains(m.Id)));
        
        var scoredMovies = new List<(Model.Movie movie, double score)>();
        foreach (var movie in candidateMovies)
        {
            double score = 0;

            var movieGenres = movie.Genres?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(g => g.Trim()).ToList() ?? new List<string>();
            var movieKeywords = movie.KeyWords?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(k => k.Trim()).ToList() ?? new List<string>();
            var movieProductions = movie.Productions?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToList() ?? new List<string>();

            score += 2.0 * FuzzyListMatch(allGenres, movieGenres);
            score += 1.5 * FuzzyListMatch(allKeywords, movieKeywords);
            score += 1.0 * (movie.OriginalLanguage == preferredLanguage ? 1 : 0);
            score += 1.0 * FuzzyListMatch(allProductions, movieProductions);
            score += 1.5 * FuzzySimilarity(avgRuntime, movie.RunTime);
            score += 1.0 * FuzzySimilarity(avgPopularity,(double)movie.Popularity);
            score += 0.5 * FuzzySimilarity(avgBudget, (double)movie.Budget);

            scoredMovies.Add((movie, score));
        }
        
        var topRecommendations = scoredMovies
            .OrderByDescending(s => s.score)
            .Take(10)
            .Select(s => _mapper.Map<ReadMovieDto>(s.movie))
            .ToList();

        return topRecommendations;
    }

    private double FuzzySimilarity(double a, double b)
    {
        if (a == 0) return 0;
        return Math.Max(0, 1 - Math.Abs(a - b) / a);
    }
    
    private double FuzzyListMatch(List<string> userList, List<string> candidateList)
    {
        if (userList == null || userList.Count == 0) return 0;
        var matches = userList.Intersect(candidateList).Count();
        return (double)matches / userList.Count;
    }

}