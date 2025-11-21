using AuthService.Api.Common;
using AuthService.Api.Data;
using AuthService.Api.Data.Entities;
using AuthService.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AuthService.Api.Features.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    private readonly AuthDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly JwtSettings _jwtSettings;

    public RefreshTokenCommandHandler(
        AuthDbContext context,
        IJwtService jwtService,
        IOptions<JwtSettings> jwtSettings)
    {
        _context = context;
        _jwtService = jwtService;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {

        var refreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken, cancellationToken);

        if (refreshToken == null)
        {
            return AuthResponse.Falha("Refresh token inválido");
        }

        if (!refreshToken.EstaAtivo)
        {
            return AuthResponse.Falha("Refresh token expirado");
        }


        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Id == refreshToken.UsuarioId && u.Ativo, cancellationToken);

        if (usuario == null)
        {
            return AuthResponse.Falha("Usuário não encontrado ou inativo");
        }


        var token = _jwtService.GerarToken(usuario);
        var novoRefreshTokenValue = _jwtService.GerarRefreshToken();
        var expiraEm = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiracaoMinutos);
        var refreshExpiraEm = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiracaoDias);


        refreshToken.Atualizar(novoRefreshTokenValue, refreshExpiraEm);

        await _context.SaveChangesAsync(cancellationToken);

        return AuthResponse.Ok(
            token,
            novoRefreshTokenValue,
            expiraEm,
            new UsuarioDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Role = usuario.Role
            }
        );
    }
}
