namespace AuthService.Api.Data.Entities;

/// <summary>
/// Entidade para armazenar refresh token (1 por usuário)
/// </summary>
public class RefreshToken
{
    public long Id { get; protected set; }
    public DateTime CriadoEm { get; protected set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; protected set; }

    public string Token { get; private set; } = string.Empty;
    public long UsuarioId { get; private set; }
    public DateTime ExpiraEm { get; private set; }

    protected RefreshToken() { }

    public RefreshToken(string token, long usuarioId, DateTime expiraEm)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token é obrigatório", nameof(token));

        if (usuarioId <= 0)
            throw new ArgumentException("UsuarioId inválido", nameof(usuarioId));

        Token = token;
        UsuarioId = usuarioId;
        ExpiraEm = expiraEm;
    }

    public bool EstaAtivo => ExpiraEm > DateTime.UtcNow;

    /// <summary>
    /// Atualiza o token com novo valor e expiração
    /// </summary>
    public void Atualizar(string novoToken, DateTime novaExpiracao)
    {
        if (string.IsNullOrWhiteSpace(novoToken))
            throw new ArgumentException("Token é obrigatório", nameof(novoToken));

        Token = novoToken;
        ExpiraEm = novaExpiracao;
        AtualizadoEm = DateTime.UtcNow;
    }
}
