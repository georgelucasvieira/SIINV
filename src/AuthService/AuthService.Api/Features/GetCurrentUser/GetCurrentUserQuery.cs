using AuthService.Api.Common;
using MediatR;

namespace AuthService.Api.Features.GetCurrentUser;

/// <summary>
/// Query para obter o usuário atual logado
/// </summary>
public record GetCurrentUserQuery : IRequest<UsuarioDto?>
{
    public long UsuarioId { get; init; }
}
