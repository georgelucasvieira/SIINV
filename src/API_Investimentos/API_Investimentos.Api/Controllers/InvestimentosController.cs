using API_Investimentos.Application.Commands.Investimento;
using API_Investimentos.Application.Queries.Investimento;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_Investimentos.Api.Controllers;

/// <summary>
/// Controller para gerenciamento de investimentos
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Authorize]
public class InvestimentosController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<InvestimentosController> _logger;

    public InvestimentosController(IMediator mediator, ILogger<InvestimentosController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Cria um investimento a partir de uma simulação
    /// </summary>
    /// <param name="request">Dados do investimento</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Investimento criado</returns>
    /// <response code="200">Investimento criado com sucesso</response>
    /// <response code="400">Dados inválidos ou simulação não encontrada</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CriarInvestimento(
        [FromBody] CriarInvestimentoRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Criando investimento para cliente {ClienteId} a partir da simulação {SimulacaoId}",
            request.ClienteId, request.SimulacaoId);

        var command = new CriarInvestimentoCommand
        {
            ClienteId = request.ClienteId,
            SimulacaoId = request.SimulacaoId
        };

        var resultado = await _mediator.Send(command, cancellationToken);

        if (!resultado.Sucesso)
        {
            _logger.LogWarning("Erro ao criar investimento: {Mensagem}", resultado.Mensagem);
            return BadRequest(resultado);
        }

        _logger.LogInformation("Investimento criado com sucesso. ID: {InvestimentoId}", resultado.Dados?.Id);
        return Ok(resultado);
    }

    /// <summary>
    /// Obtém o histórico de investimentos de um cliente
    /// </summary>
    /// <param name="clienteId">ID do cliente</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de investimentos do cliente</returns>
    /// <response code="200">Lista de investimentos retornada com sucesso</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("{clienteId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObterInvestimentosPorCliente(
        long clienteId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Obtendo investimentos do cliente {ClienteId}", clienteId);

        var query = new ObterInvestimentosPorClienteQuery { ClienteId = clienteId };
        var resultado = await _mediator.Send(query, cancellationToken);

        if (!resultado.Sucesso)
        {
            return BadRequest(resultado);
        }

        _logger.LogInformation("Retornados {Quantidade} investimentos para o cliente {ClienteId}",
            resultado.Dados?.Count ?? 0, clienteId);
        return Ok(resultado);
    }
}

public record CriarInvestimentoRequest
{
    public long ClienteId { get; init; }
    public long SimulacaoId { get; init; }
}
