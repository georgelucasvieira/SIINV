using API_Investimentos.Application.Common;
using API_Investimentos.Application.DTOs.Responses;
using MediatR;

namespace API_Investimentos.Application.Commands.Simulacao;

/// <summary>
/// Command para simular um investimento
/// </summary>
public class SimularInvestimentoCommand : IRequest<Result<SimulacaoResponse>>
{
    public long ClienteId { get; set; }
    public decimal Valor { get; set; }
    public int PrazoMeses { get; set; }
    public string TipoProduto { get; set; } = string.Empty;
}
