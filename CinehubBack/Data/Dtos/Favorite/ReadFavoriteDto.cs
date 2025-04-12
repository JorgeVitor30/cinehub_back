namespace CinehubBack.Data.Favorite;

public class ReadFavoriteDto
{
    public required Guid UserId { get; set; }
    public required Guid MovieId { get; set; }
}