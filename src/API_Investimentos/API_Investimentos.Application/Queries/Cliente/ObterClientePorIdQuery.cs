using API_Investimentos.Application.Common;
using API_Investimentos.Application.DTOs.Responses;
using MediatR;

namespace API_Investimentos.Application.Queries.Cliente;

/// <summary>
/// Query para obter um cliente por ID
/// </summary>
public class ObterClientePorIdQuery : IRequest<Result<ClienteResponse>>
{
    public long Id { get; set; }
}
