using API_Investimentos.Application.Commands.Simulacao;
using API_Investimentos.Application.DTOs.Requests;
using API_Investimentos.Application.Queries.Simulacao;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API_Investimentos.Api.Controllers;

/// <summary>
/// Controller para simulações de investimento
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class SimulacoesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<SimulacoesController> _logger;

    public SimulacoesController(IMediator mediator, ILogger<SimulacoesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Simula um investimento
    /// </summary>
    /// <param name="request">Dados da simulação</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da simulação</returns>
    /// <response code="200">Simulação realizada com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost("simular")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SimularInvestimento(
        [FromBody] SimularInvestimentoRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Iniciando simulação de investimento para cliente {ClienteId}, valor {Valor}, prazo {Prazo} meses, tipo {Tipo}",
            request.ClienteId, request.Valor, request.PrazoMeses, request.TipoProduto);

        var command = new SimularInvestimentoCommand
        {
            ClienteId = request.ClienteId,
            Valor = request.Valor,
            PrazoMeses = request.PrazoMeses,
            TipoProduto = request.TipoProduto
        };

        var resultado = await _mediator.Send(command, cancellationToken);

        if (!resultado.Sucesso)
        {
            _logger.LogWarning("Erro ao simular investimento: {Mensagem}", resultado.Mensagem);
            return BadRequest(resultado);
        }

        _logger.LogInformation("Simulação realizada com sucesso. ID: {SimulacaoId}", resultado.Dados?.Id);
        return Ok(resultado);
    }

    /// <summary>
    /// Obtém todas as simulações ou filtra por cliente/período
    /// </summary>
    /// <param name="clienteId">ID do cliente (opcional)</param>
    /// <param name="dataInicio">Data inicial (opcional)</param>
    /// <param name="dataFim">Data final (opcional)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de simulações</returns>
    /// <response code="200">Lista de simulações retornada com sucesso</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObterSimulacoes(
        [FromQuery] long? clienteId,
        [FromQuery] DateTime? dataInicio,
        [FromQuery] DateTime? dataFim,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Obtendo simulações. ClienteId: {ClienteId}, Período: {DataInicio} a {DataFim}",
            clienteId, dataInicio, dataFim);

        var query = new ObterSimulacoesQuery
        {
            ClienteId = clienteId,
            DataInicio = dataInicio,
            DataFim = dataFim
        };

        var resultado = await _mediator.Send(query, cancellationToken);

        if (!resultado.Sucesso)
        {
            return BadRequest(resultado);
        }

        return Ok(resultado);
    }
}
