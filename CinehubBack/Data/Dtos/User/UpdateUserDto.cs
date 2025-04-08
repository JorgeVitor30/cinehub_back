using System.ComponentModel.DataAnnotations;

namespace CinehubBack.Data.Dtos.User;

public class UpdateUserDto
{
     public string Name { get; set; }
    [EmailAddress] public string Email { get; set; }
    public string Password { get; set; }
    public bool VisibilityPublic { get; set; }
}