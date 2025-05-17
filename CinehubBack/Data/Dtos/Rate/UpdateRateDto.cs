namespace CinehubBack.Data.Rate;

public class UpdateRateDto
{
    public required int RateValue { get; set; }
    public required string Comment { get; set; } = null!;
}