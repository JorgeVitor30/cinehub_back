using CinehubBack.Data;
using CinehubBack.Data.Movie;

namespace CinehubBack.Services.Movie;

public interface IMovieService
{
    ReadMovieDto Create(CreateMovieDto createMovieDto);
    Page<ReadMovieDto> GetAll(Parameter parameter, string userId);
    ReadMovieDto GetById(Guid id);
    ReadHomeMovieDto GetHome();
    void DeleteById(Guid id);
    ResponseUploadImgDto AddPhotoMovies(Guid id, AddMoviePhotosDto addMoviePhotos);
}