using API_Investimentos.Application.Common;
using API_Investimentos.Application.DTOs.Responses;
using MediatR;

namespace API_Investimentos.Application.Queries.Produto;

/// <summary>
/// Query para obter produtos de investimento
/// </summary>
public class ObterProdutosQuery : IRequest<Result<List<ProdutoResponse>>>
{
    public string? Tipo { get; set; }
    public bool? ApenasAtivos { get; set; } = true;
}
