using API_Investimentos.Application.Common;
using API_Investimentos.Domain.Interfaces;
using MediatR;

namespace API_Investimentos.Application.Queries.Simulacao;

/// <summary>
/// Handler para obter simulações agrupadas por produto e dia
/// </summary>
public class ObterSimulacoesPorProdutoDiaQueryHandler
    : IRequestHandler<ObterSimulacoesPorProdutoDiaQuery, Result<List<SimulacaoPorProdutoDiaResponse>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ObterSimulacoesPorProdutoDiaQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<SimulacaoPorProdutoDiaResponse>>> Handle(
        ObterSimulacoesPorProdutoDiaQuery request,
        CancellationToken cancellationToken)
    {
        var estatisticas = await _unitOfWork.Simulacoes.ObterEstatisticasPorProdutoDiaAsync(
            request.DataInicio,
            request.DataFim,
            cancellationToken);

        var response = estatisticas
            .Where(e => string.IsNullOrEmpty(request.TipoProduto) ||
                        e.ProdutoNome.Contains(request.TipoProduto, StringComparison.OrdinalIgnoreCase))
            .Select(e => new SimulacaoPorProdutoDiaResponse
            {
                Produto = e.ProdutoNome,
                Data = e.Data,
                QuantidadeSimulacoes = e.Quantidade,
                MediaValorFinal = Math.Round(e.MediaValorFinal, 2)
            })
            .OrderByDescending(r => r.Data)
            .ThenBy(r => r.Produto)
            .ToList();

        return Result<List<SimulacaoPorProdutoDiaResponse>>.Ok(response);
    }
}
