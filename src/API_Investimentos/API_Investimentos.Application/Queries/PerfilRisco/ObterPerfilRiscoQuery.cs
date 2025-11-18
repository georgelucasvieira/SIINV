using API_Investimentos.Application.Common;
using API_Investimentos.Application.DTOs.Responses;
using MediatR;

namespace API_Investimentos.Application.Queries.PerfilRisco;

/// <summary>
/// Query para obter perfil de risco de um cliente
/// </summary>
public class ObterPerfilRiscoQuery : IRequest<Result<PerfilRiscoResponse>>
{
    public long ClienteId { get; set; }
}
