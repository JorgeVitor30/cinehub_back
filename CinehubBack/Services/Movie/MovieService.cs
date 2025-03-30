using System.Net;
using AutoMapper;
using CinehubBack.Data;
using CinehubBack.Data.Movie;
using CinehubBack.Expections;
using Microsoft.EntityFrameworkCore;

namespace CinehubBack.Services.Movie;

public class MovieService: IMovieService
{
    private readonly IRepository<Model.Movie> _repository;
    private readonly IMapper _mapper;

    public MovieService(IRepository<Model.Movie> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public Page<ReadMovieDto> GetAll(Parameter parameter)
    {
        return _repository.GetAll<ReadMovieDto>(query =>
        {
            var title = parameter.Get<string>("title");
            if (title != null)
            {
                query = query.Where(m => EF.Functions.Like(m.Title, $"%{title}%"));
            }
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
    
    public void DeleteById(Guid id)
    {
        _repository.DeleteById(id);
        _repository.SaveChanges();
    }
}