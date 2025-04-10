using System.Reflection.Metadata;

namespace CinehubBack.Model;

public class User : BaseEntity
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required Role Role { get; set; }
    public required string Password { get; set; }
}