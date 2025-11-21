using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace API_Investimentos.Api.Controllers;

/// <summary>
/// Controller proxy para autenticação - redireciona para AuthService
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AuthController> _logger;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public AuthController(IHttpClientFactory httpClientFactory, ILogger<AuthController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    /// <summary>
    /// Realiza login do usuário
    /// </summary>
    /// <param name="request">Credenciais do usuário</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Token JWT e refresh token</returns>
    /// <response code="200">Login realizado com sucesso</response>
    /// <response code="401">Credenciais inválidas</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Tentativa de login para: {Email}", request.Email);

        try
        {
            var client = _httpClientFactory.CreateClient("AuthService");
            var content = new StringContent(
                JsonSerializer.Serialize(request, _jsonOptions),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync("/api/auth/login", content, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var authResponse = JsonSerializer.Deserialize<AuthResponse>(responseContent, _jsonOptions);
                _logger.LogInformation("Login bem-sucedido para: {Email}", request.Email);
                return Ok(authResponse);
            }

            _logger.LogWarning("Falha no login para: {Email}", request.Email);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return Unauthorized(new { mensagem = "Credenciais inválidas" });
            }

            var errorResponse = JsonSerializer.Deserialize<AuthResponse>(responseContent, _jsonOptions);
            return BadRequest(errorResponse);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Erro ao conectar com AuthService");
            return StatusCode(503, new { mensagem = "Serviço de autenticação indisponível" });
        }
    }

    /// <summary>
    /// Atualiza o token JWT usando o refresh token
    /// </summary>
    /// <param name="request">Refresh token</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Novo token JWT e refresh token</returns>
    /// <response code="200">Token atualizado com sucesso</response>
    /// <response code="401">Refresh token inválido ou expirado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RefreshToken(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Tentativa de refresh token");

        try
        {
            var client = _httpClientFactory.CreateClient("AuthService");
            var content = new StringContent(
                JsonSerializer.Serialize(request, _jsonOptions),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync("/api/auth/refresh", content, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var authResponse = JsonSerializer.Deserialize<AuthResponse>(responseContent, _jsonOptions);
                _logger.LogInformation("Refresh token bem-sucedido");
                return Ok(authResponse);
            }

            _logger.LogWarning("Falha no refresh token");

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return Unauthorized(new { mensagem = "Refresh token inválido ou expirado" });
            }

            var errorResponse = JsonSerializer.Deserialize<AuthResponse>(responseContent, _jsonOptions);
            return BadRequest(errorResponse);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Erro ao conectar com AuthService");
            return StatusCode(503, new { mensagem = "Serviço de autenticação indisponível" });
        }
    }
}

#region DTOs

public record LoginRequest
{
    public string Email { get; init; } = string.Empty;
    public string Senha { get; init; } = string.Empty;
}

public record RefreshTokenRequest
{
    public string RefreshToken { get; init; } = string.Empty;
}

public record AuthResponse
{
    public bool Sucesso { get; init; }
    public string? Token { get; init; }
    public string? RefreshToken { get; init; }
    public DateTime? ExpiraEm { get; init; }
    public UsuarioDto? Usuario { get; init; }
    public string? MensagemErro { get; init; }
}

public record UsuarioDto
{
    public long Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
}

#endregion
