using API_Investimentos.Application.Commands.Cliente;
using API_Investimentos.Application.Queries.Cliente;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_Investimentos.Api.Controllers;

/// <summary>
/// Controller para gerenciamento de clientes
/// </summary>
[ApiController]
[Route("api/v1/clientes")]
[Produces("application/json")]
[Authorize]
public class ClientesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ClientesController> _logger;

    public ClientesController(IMediator mediator, ILogger<ClientesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Cadastra um novo cliente
    /// </summary>
    /// <param name="request">Dados do cliente</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Cliente cadastrado</returns>
    /// <response code="200">Cliente cadastrado com sucesso</response>
    /// <response code="400">Dados inválidos ou CPF já cadastrado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CadastrarCliente(
        [FromBody] CadastrarClienteRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cadastrando cliente: {Nome}", request.Nome);

        var command = new CadastrarClienteCommand
        {
            Nome = request.Nome,
            Cpf = request.Cpf,
            Telefone = request.Telefone
        };

        var resultado = await _mediator.Send(command, cancellationToken);

        if (!resultado.Sucesso)
        {
            _logger.LogWarning("Erro ao cadastrar cliente: {Mensagem}", resultado.Mensagem);
            return BadRequest(resultado);
        }

        _logger.LogInformation("Cliente cadastrado com sucesso. ID: {ClienteId}", resultado.Dados?.Id);
        return Ok(resultado);
    }

    /// <summary>
    /// Obtém um cliente pelo ID
    /// </summary>
    /// <param name="id">ID do cliente</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Dados do cliente</returns>
    /// <response code="200">Cliente encontrado</response>
    /// <response code="404">Cliente não encontrado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObterClientePorId(
        long id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Obtendo cliente por ID: {ClienteId}", id);

        var query = new ObterClientePorIdQuery { Id = id };
        var resultado = await _mediator.Send(query, cancellationToken);

        if (!resultado.Sucesso)
        {
            _logger.LogWarning("Cliente não encontrado: {ClienteId}", id);
            return NotFound(resultado);
        }

        return Ok(resultado);
    }

    /// <summary>
    /// Obtém todos os clientes
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de clientes</returns>
    /// <response code="200">Lista de clientes retornada com sucesso</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObterClientes(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Obtendo todos os clientes");

        var query = new ObterClientesQuery();
        var resultado = await _mediator.Send(query, cancellationToken);

        if (!resultado.Sucesso)
        {
            return BadRequest(resultado);
        }

        _logger.LogInformation("Retornados {Quantidade} clientes", resultado.Dados?.Count ?? 0);
        return Ok(resultado);
    }
}

public record CadastrarClienteRequest
{
    public string Nome { get; init; } = string.Empty;
    public string Cpf { get; init; } = string.Empty;
    public string Telefone { get; init; } = string.Empty;
}
