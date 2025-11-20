namespace AuthService.Api.Shared;

/// <summary>
/// Configurações do JWT
/// </summary>
public class JwtSettings
{
    public const string SectionName = "JwtSettings";

    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpiracaoMinutos { get; set; } = 60;
    public int RefreshTokenExpiracaoDias { get; set; } = 7;
}
