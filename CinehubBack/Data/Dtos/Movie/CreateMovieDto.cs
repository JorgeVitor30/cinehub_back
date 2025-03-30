namespace CinehubBack.Data.Movie;

public record CreateMovieDto()
{
    public required string Title { get; set; }
    public required string Overview { get; set; }
    public required DateTime ReleaseDate { get; set; }
    public required int RunTime { get; set; }
    public bool Adult { get; set; }
    public required decimal Budget { get; set; }
    public required string OriginalLanguage { get; set; }
    public required string Tagline { get; set; }
    public required string KeyWords { get; set; }
    public required string Productions { get; set; }
    public required string Genres { get; set; }
}