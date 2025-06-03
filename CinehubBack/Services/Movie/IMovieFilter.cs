namespace CinehubBack.Data.Movie;

public interface IMovieFilter
{
    IQueryable<Model.Movie> Apply(IQueryable<Model.Movie> query, Parameter parameter, string userId);
}

public interface IMovieSorter
{
    bool CanHandle(string sortBy);
    IQueryable<Model.Movie> Apply(IQueryable<Model.Movie> query, string sortOrder);
}