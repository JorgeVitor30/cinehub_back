namespace CinehubBack.Data.Movie;

public class ReadHomeMovieDto
{
    public required ReadMovieDto[] PopularMovies { get; set; }
    public required ReadMovieDto[] NewReleaseMovies { get; set; }
    public required ReadMovieDto[] ClassicMovies { get; set; }
}
