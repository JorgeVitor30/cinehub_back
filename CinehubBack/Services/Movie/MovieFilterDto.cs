namespace CinehubBack.Services.Movie;

public class MovieFilterDto
{
    public string? Title { get; set; }
    public string? Genre { get; set; }
    public decimal Note { get; set; }
    public string? SortBy { get; set; }
    public int Size { get; set; } = 10;
    public int Page { get; set; } = 0;
}