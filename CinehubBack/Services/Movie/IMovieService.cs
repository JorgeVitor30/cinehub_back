using CinehubBack.Data;
using CinehubBack.Data.Movie;

namespace CinehubBack.Services.Movie;

public interface IMovieService
{
    ReadMovieDto Create(CreateMovieDto createMovieDto);
    Page<ReadMovieDto> GetAll(Parameter parameter);
    ReadMovieDto GetById(Guid id);
    ReadHomeMovieDto GetHome();
    void DeleteById(Guid id);
}