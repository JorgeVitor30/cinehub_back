namespace CinehubBack.Model;

public class Movie: BaseEntity
{
    public required string Title { get; set; }
    public required string Overview { get; set; }
    public required int VoteCount { get; set; }
    public required decimal VoteAverage { get; set; }
    public required DateTime ReleaseDate { get; set; }
    public required decimal Revenue { get; set; }
    public required int RunTime { get; set; }
    public required bool Adult { get; set; }
    public required decimal Budget { get; set; }
    public required string PosterPhotoUrl { get; set; }
    public required string BackPhotoUrl { get; set; }
    public required string OriginalLanguage { get; set; }
    public required decimal Popularity { get; set; }
    public required string Tagline { get; set; }
    public required string KeyWords { get; set; }
}