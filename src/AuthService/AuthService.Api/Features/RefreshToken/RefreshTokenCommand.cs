using AuthService.Api.Common;
using MediatR;

namespace AuthService.Api.Features.RefreshToken;

/// <summary>
/// Comando para renovar token usando refresh token
/// </summary>
public record RefreshTokenCommand : IRequest<AuthResponse>
{
    public string RefreshToken { get; init; } = string.Empty;
}
