namespace CinehubBack.Data.Favorite;

public class DeleteFavoriteDto
{
    public required Guid UserId { get; set; }
    public required Guid MovieId { get; set; }
}