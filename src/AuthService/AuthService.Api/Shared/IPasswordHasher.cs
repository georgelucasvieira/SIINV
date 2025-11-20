namespace AuthService.Api.Shared;

/// <summary>
/// Interface para hashing de senhas
/// </summary>
public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}
