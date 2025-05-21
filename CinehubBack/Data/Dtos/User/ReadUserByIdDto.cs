using CinehubBack.Data.Movie;
using CinehubBack.Data.Rate;

namespace CinehubBack.Data.Dtos.User;

public class ReadUserByIdDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Role { get; set; }
    public required bool VisibilityPublic { get; set; }
    public string? Photo { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required List<ReadMovieDto> Favorites { get; set; } = new();
    public required List<ReadRateDto?> RatedList { get; set; } = new();
}