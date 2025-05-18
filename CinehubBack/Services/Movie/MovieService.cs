using System.Linq.Expressions;
using System.Net;
using AutoMapper;
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
        
    public MovieService(IRepository<Model.Movie> repository, IMapper mapper, IRepository<Favorites> favoritesRepository, IRepository<Model.Rate> rateRepository)
    {
        _repository = repository;
        _rateRepository = rateRepository;
        _mapper = mapper;
        _favoritesRepository = favoritesRepository;
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
            var title = parameter.Get<string>("title");
            if (title != null)
            {
                query = query.Where(m => EF.Functions.Like(m.Title, $"%{title}%"));
            }
            var genre = parameter.Get<string>("genre");
            if (genre != null)
            {
                query = query.Where(m => m.Genres.ToLower().Contains(genre.ToLower()));
            }
            var note = parameter.Get<decimal>("note");
            if (note != 0)
            {
                query = query.Where(m => m.VoteAverage >= note);
            }
            if (Guid.TryParse(userId, out var userGuid))
            {
                query = query.Where(m => !_favoritesRepository.Queryable.Where(f => f.UserId == userGuid)
                    .Any(f => f.UserId == userGuid && f.MovieId == m.Id));
                
                query = query.Where(m => !_rateRepository.Queryable.Where(r => r.UserId == userGuid).Any(r => r.MovieId == m.Id));
            } else { throw new BaseException(ErrorCode.BadRequest(), HttpStatusCode.BadRequest, "UserId is not valid");}   
            
            return query.Select(m => new ReadMovieDto {Id = m.Id, Title = m.Title, Overview = m.Overview, VoteCount = m.VoteCount, VoteAverage = m.VoteAverage, ReleaseDate = m.ReleaseDate, Revenue = m.Revenue, RunTime = m.RunTime, Adult = m.Adult, Budget = m.Budget, PosterPhotoUrl = m.PosterPhotoUrl, BackPhotoUrl = m.BackPhotoUrl, OriginalLanguage = m.OriginalLanguage, Popularity = m.Popularity, Tagline = m.Tagline, KeyWords = m.KeyWords, Productions = m.Productions, Genres = m.Genres});
        }, parameter);
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
            query => query.OrderByDescending(m => m.Popularity).Take(10)
                .Select(m => new ReadMovieDto { Id = m.Id, Title = m.Title, Overview = m.Overview, VoteCount = m.VoteCount, VoteAverage = m.VoteAverage, ReleaseDate = m.ReleaseDate, Revenue = m.Revenue, RunTime = m.RunTime, Adult = m.Adult, Budget = m.Budget, PosterPhotoUrl = m.PosterPhotoUrl, BackPhotoUrl = m.BackPhotoUrl, OriginalLanguage = m.OriginalLanguage, Popularity = m.Popularity, Tagline = m.Tagline, KeyWords = m.KeyWords, Productions = m.Productions, Genres = m.Genres })
        );

        var newReleases = _repository.GetAllList<ReadMovieDto>(
            query => query.OrderByDescending(m => m.ReleaseDate).Take(10)
                .Select(m => new ReadMovieDto { Id = m.Id, Title = m.Title, Overview = m.Overview, VoteCount = m.VoteCount, VoteAverage = m.VoteAverage, ReleaseDate = m.ReleaseDate, Revenue = m.Revenue, RunTime = m.RunTime, Adult = m.Adult, Budget = m.Budget, PosterPhotoUrl = m.PosterPhotoUrl, BackPhotoUrl = m.BackPhotoUrl, OriginalLanguage = m.OriginalLanguage, Popularity = m.Popularity, Tagline = m.Tagline, KeyWords = m.KeyWords, Productions = m.Productions, Genres = m.Genres })
        );
        
        var classicMovies = _repository.GetAllList<ReadMovieDto>(
            query => query.Where(m=> m.VoteAverage > 8 && m.Adult.Equals(false)).OrderBy(m => m.ReleaseDate).Take(10)
                .Select(m => new ReadMovieDto { Id = m.Id, Title = m.Title, Overview = m.Overview, VoteCount = m.VoteCount, VoteAverage = m.VoteAverage, ReleaseDate = m.ReleaseDate, Revenue = m.Revenue, RunTime = m.RunTime, Adult = m.Adult, Budget = m.Budget, PosterPhotoUrl = m.PosterPhotoUrl, BackPhotoUrl = m.BackPhotoUrl, OriginalLanguage = m.OriginalLanguage, Popularity = m.Popularity, Tagline = m.Tagline, KeyWords = m.KeyWords, Productions = m.Productions, Genres = m.Genres })
        );
        
        return new ReadHomeMovieDto {PopularMovies = popularMovies.ToArray(), NewReleaseMovies = newReleases.ToArray(), ClassicMovies = classicMovies.ToArray()};
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
}