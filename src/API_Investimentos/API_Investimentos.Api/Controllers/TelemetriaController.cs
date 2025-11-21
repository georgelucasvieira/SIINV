using API_Investimentos.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_Investimentos.Api.Controllers;

/// <summary>
/// Controller para telemetria e métricas de uso da API
/// </summary>
[ApiController]
[Route("api/v1")]
[Produces("application/json")]
[Authorize]
public class TelemetriaController : ControllerBase
{
    private readonly ITelemetriaService _telemetriaService;
    private readonly ILogger<TelemetriaController> _logger;

    public TelemetriaController(ITelemetriaService telemetriaService, ILogger<TelemetriaController> logger)
    {
        _telemetriaService = telemetriaService;
        _logger = logger;
    }

    /// <summary>
    /// Obtém métricas de telemetria dos endpoints da API
    /// </summary>
    /// <param name="inicio">Data inicial do período (opcional, padrão: últimos 30 dias)</param>
    /// <param name="fim">Data final do período (opcional, padrão: hoje)</param>
    /// <returns>Métricas de uso dos endpoints</returns>
    /// <response code="200">Métricas retornadas com sucesso</response>
    /// <response code="401">Não autorizado</response>
    [HttpGet("telemetria")]
    [ProducesResponseType(typeof(TelemetriaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult ObterTelemetria(
        [FromQuery] DateTime? inicio,
        [FromQuery] DateTime? fim)
    {
        _logger.LogInformation("Consultando telemetria. Período: {Inicio} a {Fim}",
            inicio?.ToString("yyyy-MM-dd") ?? "últimos 30 dias",
            fim?.ToString("yyyy-MM-dd") ?? "hoje");

        var telemetria = _telemetriaService.ObterTelemetria(inicio, fim);

        _logger.LogInformation("Telemetria retornada com {Quantidade} serviços", telemetria.Servicos.Count);

        return Ok(telemetria);
    }
}
