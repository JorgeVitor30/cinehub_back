namespace CinehubBack.Data.Favorite;

public class CreateFavoriteDto
{
    public required Guid UserId { get; set; }
    public required Guid MovieId { get; set; }
}