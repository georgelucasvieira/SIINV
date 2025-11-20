using AuthService.Api.Common;
using AuthService.Api.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Api.Features.GetCurrentUser;

public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, UsuarioDto?>
{
    private readonly AuthDbContext _context;

    public GetCurrentUserQueryHandler(AuthDbContext context)
    {
        _context = context;
    }

    public async Task<UsuarioDto?> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var usuario = await _context.Usuarios
            .Where(u => u.Id == request.UsuarioId && u.Ativo)
            .Select(u => new UsuarioDto
            {
                Id = u.Id,
                Nome = u.Nome,
                Email = u.Email,
                Role = u.Role
            })
            .FirstOrDefaultAsync(cancellationToken);

        return usuario;
    }
}
