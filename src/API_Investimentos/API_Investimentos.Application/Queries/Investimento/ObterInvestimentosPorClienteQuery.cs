using API_Investimentos.Application.Common;
using API_Investimentos.Application.DTOs.Responses;
using MediatR;

namespace API_Investimentos.Application.Queries.Investimento;

/// <summary>
/// Query para obter investimentos de um cliente
/// </summary>
public class ObterInvestimentosPorClienteQuery : IRequest<Result<List<InvestimentoResponse>>>
{
    public long ClienteId { get; set; }
}
