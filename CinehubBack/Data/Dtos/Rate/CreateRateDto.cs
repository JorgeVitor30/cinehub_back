namespace CinehubBack.Data.Rate;

public class CreateRateDto
{
    public required Guid MovieId { get; set; }
    public required Guid UserId { get; set; }
    public required int Rate { get; set; }
    public required string Comment { get; set; } = null!;
}