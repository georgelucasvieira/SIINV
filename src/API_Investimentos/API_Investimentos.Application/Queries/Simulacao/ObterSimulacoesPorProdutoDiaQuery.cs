using API_Investimentos.Application.Common;
using MediatR;

namespace API_Investimentos.Application.Queries.Simulacao;

/// <summary>
/// Query para obter simulações agrupadas por produto e dia
/// </summary>
public class ObterSimulacoesPorProdutoDiaQuery : IRequest<Result<List<SimulacaoPorProdutoDiaResponse>>>
{
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public string? TipoProduto { get; set; }
}

/// <summary>
/// Response com simulações agrupadas por produto e dia
/// </summary>
public class SimulacaoPorProdutoDiaResponse
{
    public string Produto { get; set; } = string.Empty;
    public DateTime Data { get; set; }
    public int QuantidadeSimulacoes { get; set; }
    public decimal MediaValorFinal { get; set; }
}
