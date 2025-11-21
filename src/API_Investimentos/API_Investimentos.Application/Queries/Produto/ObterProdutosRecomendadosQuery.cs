using API_Investimentos.Application.Common;
using MediatR;

namespace API_Investimentos.Application.Queries.Produto;

/// <summary>
/// Query para obter produtos recomendados por perfil
/// </summary>
public class ObterProdutosRecomendadosQuery : IRequest<Result<List<ProdutoRecomendadoResponse>>>
{
    public string Perfil { get; set; } = string.Empty;
}

/// <summary>
/// Response de produto recomendado
/// </summary>
public class ProdutoRecomendadoResponse
{
    public long Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public decimal Rentabilidade { get; set; }
    public string Risco { get; set; } = string.Empty;
}
