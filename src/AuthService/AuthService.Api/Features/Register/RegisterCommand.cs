using AuthService.Api.Common;
using MediatR;

namespace AuthService.Api.Features.Register;

/// <summary>
/// Comando para registrar novo usuário
/// </summary>
public record RegisterCommand : IRequest<AuthResponse>
{
    public string Nome { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Senha { get; init; } = string.Empty;
    public string ConfirmacaoSenha { get; init; } = string.Empty;
}
