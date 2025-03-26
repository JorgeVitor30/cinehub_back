using System.ComponentModel.DataAnnotations;

namespace CinehubBack.Data.Dtos.User;

public class CreateUserDto
{
    [Required] public string Name { get; set; } = null!;
    [Required, EmailAddress] public string Email { get; set; } = null!;
    [Required, MinLength(3)] public string Password { get; set; } = null!;
}