namespace CinehubBack.Encrypt;

public interface IPasswordEncoder
{
    bool Verify(string password, string hash);
    string Encode(string password);
}