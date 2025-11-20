namespace AuthService.Api.Data.Entities;

/// <summary>
/// Entidade para armazenar refresh tokens
/// </summary>
public class RefreshToken
{
    public long Id { get; protected set; }
    public DateTime CriadoEm { get; protected set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; protected set; }

    public string Token { get; private set; } = string.Empty;
    public long UsuarioId { get; private set; }
    public DateTime ExpiraEm { get; private set; }
    public bool Revogado { get; private set; }
    public DateTime? RevogadoEm { get; private set; }
    public string? SubstituidoPor { get; private set; }

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
        Revogado = false;
    }

    public bool EstaAtivo => !Revogado && ExpiraEm > DateTime.UtcNow;

    public void Revogar(string? substituidoPor = null)
    {
        Revogado = true;
        RevogadoEm = DateTime.UtcNow;
        SubstituidoPor = substituidoPor;
        AtualizadoEm = DateTime.UtcNow;
    }
}
