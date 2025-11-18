using API_Investimentos.Application.Common;
using API_Investimentos.Application.DTOs.Responses;
using MediatR;

namespace API_Investimentos.Application.Queries.Simulacao;

/// <summary>
/// Query para obter simulações de um cliente
/// </summary>
public class ObterSimulacoesQuery : IRequest<Result<List<SimulacaoResponse>>>
{
    public long? ClienteId { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
}
