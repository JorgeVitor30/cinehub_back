using System.ComponentModel.DataAnnotations;

namespace CinehubBack.Data.Dtos.User;

public class UpdateUserDto
{
     public required string Name { get; set; }
    [EmailAddress] public required string Email { get; set; }
    public bool VisibilityPublic { get; set; }
}