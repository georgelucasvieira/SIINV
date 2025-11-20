using AuthService.Api.Data.Entities;

namespace AuthService.Api.Shared;

/// <summary>
/// Interface do serviço de geração de tokens JWT
/// </summary>
public interface IJwtService
{
    string GerarToken(Usuario usuario);
    string GerarRefreshToken();
    bool ValidarToken(string token);
}
