namespace AuthService.Api.Common;

/// <summary>
/// Resposta padrão de autenticação
/// </summary>
public record AuthResponse
{
    public bool Sucesso { get; init; }
    public string? Token { get; init; }
    public string? RefreshToken { get; init; }
    public DateTime? ExpiraEm { get; init; }
    public UsuarioDto? Usuario { get; init; }
    public string? MensagemErro { get; init; }

    public static AuthResponse Falha(string mensagem) => new()
    {
        Sucesso = false,
        MensagemErro = mensagem
    };

    public static AuthResponse Ok(string token, string refreshToken, DateTime expiraEm, UsuarioDto usuario) => new()
    {
        Sucesso = true,
        Token = token,
        RefreshToken = refreshToken,
        ExpiraEm = expiraEm,
        Usuario = usuario
    };
}

/// <summary>
/// DTO do usuário para resposta
/// </summary>
public record UsuarioDto
{
    public long Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
}
