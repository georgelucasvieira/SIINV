using API_Investimentos.Application.Common;
using API_Investimentos.Application.DTOs.Responses;
using MediatR;

namespace API_Investimentos.Application.Queries.Cliente;

/// <summary>
/// Query para obter todos os clientes
/// </summary>
public class ObterClientesQuery : IRequest<Result<List<ClienteResponse>>>
{
}
