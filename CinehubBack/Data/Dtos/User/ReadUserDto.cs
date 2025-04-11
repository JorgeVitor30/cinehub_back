namespace CinehubBack.Data.Dtos.User;

public class ReadUserDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Role { get; set; }
    public required bool VisibilityPublic { get; set; }
    public string? Photo { get; set; }
    public required DateTime CreatedAt { get; set; }
}