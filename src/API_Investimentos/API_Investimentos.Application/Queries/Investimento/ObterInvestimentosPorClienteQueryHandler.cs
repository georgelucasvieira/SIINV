using API_Investimentos.Application.Common;
using API_Investimentos.Application.DTOs.Responses;
using API_Investimentos.Domain.Interfaces;
using MediatR;

namespace API_Investimentos.Application.Queries.Investimento;

/// <summary>
/// Handler para obter investimentos de um cliente
/// </summary>
public class ObterInvestimentosPorClienteQueryHandler
    : IRequestHandler<ObterInvestimentosPorClienteQuery, Result<List<InvestimentoResponse>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ObterInvestimentosPorClienteQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<InvestimentoResponse>>> Handle(
        ObterInvestimentosPorClienteQuery request,
        CancellationToken cancellationToken)
    {
        var investimentos = await _unitOfWork.HistoricoInvestimentos.ObterPorClienteAsync(
            request.ClienteId,
            cancellationToken);

        var response = investimentos.Select(i => new InvestimentoResponse
        {
            Id = i.Id,
            Tipo = i.TipoProduto.ToString(),
            Valor = i.Valor.Valor,
            Rentabilidade = i.Rentabilidade.Valor,
            Data = i.DataInvestimento
        }).ToList();

        return Result<List<InvestimentoResponse>>.Ok(response);
    }
}
