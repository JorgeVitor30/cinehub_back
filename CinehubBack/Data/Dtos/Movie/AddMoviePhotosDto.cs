namespace CinehubBack.Data.Movie;

public class AddMoviePhotosDto
{
    public required IFormFile PosterPhoto { get; set; } 
    public required IFormFile BackPhoto { get; set; }
}