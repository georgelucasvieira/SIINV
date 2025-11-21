using API_Investimentos.Application.Queries.Produto;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_Investimentos.Api.Controllers;

/// <summary>
/// Controller para produtos de investimento
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Authorize]
public class ProdutosController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProdutosController> _logger;

    public ProdutosController(IMediator mediator, ILogger<ProdutosController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Obtém todos os produtos ou filtra por tipo
    /// </summary>
    /// <param name="tipo">Tipo do produto (CDB, TesouroSelic, etc.) - opcional</param>
    /// <param name="apenasAtivos">Retornar apenas produtos ativos - padrão: true</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de produtos</returns>
    /// <response code="200">Lista de produtos retornada com sucesso</response>
    /// <response code="400">Tipo de produto inválido</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObterProdutos(
        [FromQuery] string? tipo,
        [FromQuery] bool? apenasAtivos,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Obtendo produtos. Tipo: {Tipo}, ApenasAtivos: {ApenasAtivos}",
            tipo, apenasAtivos);

        var query = new ObterProdutosQuery
        {
            Tipo = tipo,
            ApenasAtivos = apenasAtivos
        };

        var resultado = await _mediator.Send(query, cancellationToken);

        if (!resultado.Sucesso)
        {
            _logger.LogWarning("Erro ao obter produtos: {Mensagem}", resultado.Mensagem);
            return BadRequest(resultado);
        }

        _logger.LogInformation("Retornados {Quantidade} produtos", resultado.Dados?.Count ?? 0);
        return Ok(resultado);
    }
}
