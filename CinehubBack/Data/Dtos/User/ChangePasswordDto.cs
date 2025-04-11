namespace CinehubBack.Data.Dtos.User;

public class ChangePasswordDto
{
    public required string LastPassword { get; set; }
    public required string NewPassword { get; set; }
}