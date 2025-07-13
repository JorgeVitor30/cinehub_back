using CinehubBack.Data.Movie;
using CinehubBack.Data.Rate;

namespace CinehubBack.Data.Dtos.User;

public class ReadAllUserDto
{
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Role { get; set; }
        public required bool VisibilityPublic { get; set; }
        public string? Photo { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required string Genre { get; set; }
        public required List<ReadRateDto?> RatedList { get; set; } = new();
        public required RankingUser RankingUser { get; set; }
        public List<string> TopGenres { get; set; } = new();
        public int RateCount { get; set; }
        public required string Description { get; set; }
}