using API_Investimentos.Application.Queries.PerfilRisco;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API_Investimentos.Api.Controllers;

/// <summary>
/// Controller para perfil de risco dos clientes
/// </summary>
[ApiController]
[Route("api/v1/perfil-risco")]
[Produces("application/json")]
public class PerfilRiscoController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PerfilRiscoController> _logger;

    public PerfilRiscoController(IMediator mediator, ILogger<PerfilRiscoController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Obtém o perfil de risco de um cliente
    /// </summary>
    /// <param name="clienteId">ID do cliente</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Perfil de risco do cliente</returns>
    /// <response code="200">Perfil de risco retornado com sucesso</response>
    /// <response code="404">Perfil de risco não encontrado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("{clienteId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObterPerfilRisco(
        long clienteId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Obtendo perfil de risco do cliente {ClienteId}", clienteId);

        var query = new ObterPerfilRiscoQuery { ClienteId = clienteId };
        var resultado = await _mediator.Send(query, cancellationToken);

        if (!resultado.Sucesso)
        {
            _logger.LogWarning("Perfil de risco não encontrado para cliente {ClienteId}", clienteId);
            return NotFound(resultado);
        }

        return Ok(resultado);
    }
}
