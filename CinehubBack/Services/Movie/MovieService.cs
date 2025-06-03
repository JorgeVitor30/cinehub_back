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
    private readonly IImageUploadService _imageUploadService;
    private readonly IEnumerable<IMovieFilter> _movieFilters;
    private const int DefaultPageSize = 10;
        
    public MovieService(IRepository<Model.Movie> repository, IMapper mapper, IRepository<Favorites> favoritesRepository, IRepository<Model.Rate> rateRepository, IImageUploadService imageUploadService, IEnumerable<IMovieFilter> movieFilters)
    {
        _repository = repository;
        _rateRepository = rateRepository;
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
}