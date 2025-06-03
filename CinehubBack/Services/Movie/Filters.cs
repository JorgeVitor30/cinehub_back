using System.Net;
using CinehubBack.Data;
using CinehubBack.Data.Movie;
using CinehubBack.Expections;
using CinehubBack.Model;
using Microsoft.EntityFrameworkCore;

namespace CinehubBack.Services.Movie;

public class TitleFilter : IMovieFilter
{
    public IQueryable<Model.Movie> Apply(IQueryable<Model.Movie> query, Parameter parameter, string userId)
    {
        var title = parameter.Get<string>("title");
        if (!string.IsNullOrEmpty(title))
        {
            query = query.Where(m => EF.Functions.Like(m.Title, $"%{title}%"));
        }
        return query;
    }
}

public class GenreFilter : IMovieFilter
{
    public IQueryable<Model.Movie> Apply(IQueryable<Model.Movie> query, Parameter parameter, string userId)
    {
        var genre = parameter.Get<string>("genre");
        if (!string.IsNullOrEmpty(genre))
        {
            query = query.Where(m => m.Genres.ToLower().Contains(genre.ToLower()));
        }
        return query;
    }
}

public class VoteAverageFilter : IMovieFilter
{
    public IQueryable<Model.Movie> Apply(IQueryable<Model.Movie> query, Parameter parameter, string userId)
    {
        var note = parameter.Get<decimal>("note");
        if (note > 0)
        {
            query = query.Where(m => m.VoteAverage >= note);
        }
        return query;
    }
}

public class ExcludeRatedAndFavoriteFilter : IMovieFilter
{
    private readonly IRepository<Favorites> _favoritesRepository;
    private readonly IRepository<Model.Rate> _rateRepository;

    public ExcludeRatedAndFavoriteFilter(IRepository<Favorites> favoritesRepository, IRepository<Model.Rate> rateRepository)
    {
        _favoritesRepository = favoritesRepository;
        _rateRepository = rateRepository;
    }

    public IQueryable<Model.Movie> Apply(IQueryable<Model.Movie> query, Parameter parameter, string userId)
    {
        if (!Guid.TryParse(userId, out var userGuid))
            throw new BaseException(ErrorCode.BadRequest(), HttpStatusCode.BadRequest, "UserId is not valid");

        query = query.Where(m => !_favoritesRepository.Queryable
            .Where(f => f.UserId == userGuid)
            .Any(f => f.MovieId == m.Id));

        query = query.Where(m => !_rateRepository.Queryable
            .Where(r => r.UserId == userGuid)
            .Any(r => r.MovieId == m.Id));

        return query;
    }
}
