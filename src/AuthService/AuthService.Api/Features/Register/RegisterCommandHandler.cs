using AuthService.Api.Common;
using AuthService.Api.Data;
using AuthService.Api.Data.Entities;
using AuthService.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AuthService.Api.Features.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
{
    private readonly AuthDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly JwtSettings _jwtSettings;

    public RegisterCommandHandler(
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

    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.ToLowerInvariant();

        var emailExiste = await _context.Usuarios
            .AnyAsync(u => u.Email == email, cancellationToken);

        if (emailExiste)
        {
            return AuthResponse.Falha("Email já cadastrado");
        }

        var senhaHash = _passwordHasher.Hash(request.Senha);

        var usuario = new Usuario(request.Nome, email, senhaHash, Roles.Usuario);

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync(cancellationToken);

        usuario.RegistrarLogin();

        var token = _jwtService.GerarToken(usuario);
        var refreshTokenValue = _jwtService.GerarRefreshToken();
        var expiraEm = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiracaoMinutos);

        var refreshToken = new Data.Entities.RefreshToken(
            refreshTokenValue,
            usuario.Id,
            DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiracaoDias)
        );

        _context.RefreshTokens.Add(refreshToken);
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
