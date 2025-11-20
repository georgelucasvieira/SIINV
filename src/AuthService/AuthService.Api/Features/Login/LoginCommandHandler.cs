using AuthService.Api.Common;
using AuthService.Api.Data;
using AuthService.Api.Data.Entities;
using AuthService.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AuthService.Api.Features.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly AuthDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly JwtSettings _jwtSettings;

    public LoginCommandHandler(
        AuthDbContext context,
        IJwtService jwtService,
        IPasswordHasher passwordHasher,
        IOptions<JwtSettings> jwtSettings)
    {
        _context = context;
        _jwtService = jwtService;
        _passwordHasher = passwordHasher;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.ToLowerInvariant();

        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        if (usuario == null)
        {
            return AuthResponse.Falha("Email ou senha inválidos");
        }

        if (!usuario.Ativo)
        {
            return AuthResponse.Falha("Usuário inativo");
        }

        if (usuario.EstaBloqueado())
        {
            return AuthResponse.Falha($"Conta bloqueada. Tente novamente após {usuario.BloqueadoAte:HH:mm}");
        }

        if (!_passwordHasher.Verify(request.Senha, usuario.SenhaHash))
        {
            usuario.RegistrarTentativaFalha();
            await _context.SaveChangesAsync(cancellationToken);
            return AuthResponse.Falha("Email ou senha inválidos");
        }

        // Login bem-sucedido
        usuario.RegistrarLogin();

        // Gerar tokens
        var token = _jwtService.GerarToken(usuario);
        var refreshTokenValue = _jwtService.GerarRefreshToken();
        var expiraEm = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiracaoMinutos);
        var refreshExpiraEm = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiracaoDias);

        // Buscar ou criar refresh token (1 por usuário)
        var refreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.UsuarioId == usuario.Id, cancellationToken);

        if (refreshToken == null)
        {
            refreshToken = new Data.Entities.RefreshToken(
                refreshTokenValue,
                usuario.Id,
                refreshExpiraEm
            );
            _context.RefreshTokens.Add(refreshToken);
        }
        else
        {
            refreshToken.Atualizar(refreshTokenValue, refreshExpiraEm);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return AuthResponse.Ok(
            token,
            refreshTokenValue,
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
