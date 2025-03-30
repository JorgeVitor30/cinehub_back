using CinehubBack.Data;
using CinehubBack.Data.Movie;

namespace CinehubBack.Services.Movie;

public interface IMovieService
{
    Page<ReadMovieDto> GetAll(Parameter parameter);
    ReadMovieDto GetById(Guid id);
    void DeleteById(Guid id);
}