using AuthService.Api.Common;
using MediatR;

namespace AuthService.Api.Features.Login;

/// <summary>
/// Comando para realizar login
/// </summary>
public record LoginCommand : IRequest<AuthResponse>
{
    public string Email { get; init; } = string.Empty;
    public string Senha { get; init; } = string.Empty;
}
