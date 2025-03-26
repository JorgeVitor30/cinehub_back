namespace CinehubBack.Encrypt;

public class BCryptEncoder : IPasswordEncoder
{
    public bool Verify(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }

    public string Encode(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
}