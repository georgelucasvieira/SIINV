using API_Investimentos.Application.Common;
using API_Investimentos.Application.DTOs.Responses;
using API_Investimentos.Domain.Interfaces;
using MediatR;

namespace API_Investimentos.Application.Queries.Cliente;

/// <summary>
/// Handler para obter cliente por ID
/// </summary>
public class ObterClientePorIdQueryHandler : IRequestHandler<ObterClientePorIdQuery, Result<ClienteResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ObterClientePorIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ClienteResponse>> Handle(
        ObterClientePorIdQuery request,
        CancellationToken cancellationToken)
    {
        var cliente = await _unitOfWork.Clientes.ObterPorIdAsync(request.Id, cancellationToken);

        if (cliente == null)
        {
            return Result<ClienteResponse>.Falha("Cliente não encontrado");
        }

        var response = new ClienteResponse
        {
            Id = cliente.Id,
            Nome = cliente.Nome,
            Cpf = cliente.Cpf,
            Telefone = cliente.Telefone,
            CriadoEm = cliente.CriadoEm,
            AtualizadoEm = cliente.AtualizadoEm
        };

        return Result<ClienteResponse>.Ok(response);
    }
}
