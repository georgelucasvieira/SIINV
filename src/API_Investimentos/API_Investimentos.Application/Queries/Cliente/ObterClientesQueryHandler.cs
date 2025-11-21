using API_Investimentos.Application.Common;
using API_Investimentos.Application.DTOs.Responses;
using API_Investimentos.Domain.Interfaces;
using MediatR;

namespace API_Investimentos.Application.Queries.Cliente;

/// <summary>
/// Handler para obter todos os clientes
/// </summary>
public class ObterClientesQueryHandler : IRequestHandler<ObterClientesQuery, Result<List<ClienteResponse>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ObterClientesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<ClienteResponse>>> Handle(
        ObterClientesQuery request,
        CancellationToken cancellationToken)
    {
        var clientes = await _unitOfWork.Clientes.ObterTodosAsync(cancellationToken);

        var response = clientes.Select(c => new ClienteResponse
        {
            Id = c.Id,
            Nome = c.Nome,
            Cpf = c.Cpf,
            Telefone = c.Telefone,
            CriadoEm = c.CriadoEm,
            AtualizadoEm = c.AtualizadoEm
        }).ToList();

        return Result<List<ClienteResponse>>.Ok(response);
    }
}
