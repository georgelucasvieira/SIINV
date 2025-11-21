using API_Investimentos.Application.Common;
using API_Investimentos.Application.DTOs.Responses;
using MediatR;

namespace API_Investimentos.Application.Commands.Investimento;

/// <summary>
/// Command para criar um investimento a partir de uma simulação
/// </summary>
public class CriarInvestimentoCommand : IRequest<Result<InvestimentoResponse>>
{
    public long ClienteId { get; set; }
    public long SimulacaoId { get; set; }
}
