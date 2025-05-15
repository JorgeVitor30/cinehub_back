using CinehubBack.Data.Dtos.User;
using CinehubBack.Data.Movie;

namespace CinehubBack.Data.Rate;

public class ReadRateDto
{
    public required ReadMovieDto Movie { get; set; }
    public required int Rate { get; set; }
    public required string Comment { get; set; } = null!;
}