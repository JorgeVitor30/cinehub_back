namespace CinehubBack.Data.Dtos.Rate;

public class DeleteRateDto
{
    public required Guid MovieId { get; set; }
    public required Guid UserId { get; set; }
}