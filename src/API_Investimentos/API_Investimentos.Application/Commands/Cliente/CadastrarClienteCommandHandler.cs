using API_Investimentos.Application.Common;
using API_Investimentos.Application.DTOs.Responses;
using API_Investimentos.Domain.Interfaces;
using MediatR;

namespace API_Investimentos.Application.Commands.Cliente;

/// <summary>
/// Handler para cadastrar cliente
/// </summary>
public class CadastrarClienteCommandHandler : IRequestHandler<CadastrarClienteCommand, Result<ClienteResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CadastrarClienteCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ClienteResponse>> Handle(
        CadastrarClienteCommand request,
        CancellationToken cancellationToken)
    {
        // Verificar se já existe um cliente com o mesmo CPF
        if (await _unitOfWork.Clientes.ExisteCpfAsync(request.Cpf, cancellationToken))
        {
            return Result<ClienteResponse>.Falha("Já existe um cliente cadastrado com este CPF");
        }

        // Criar novo cliente
        var cliente = new Domain.Entities.Cliente(
            request.Nome,
            request.Cpf,
            request.Telefone);

        await _unitOfWork.Clientes.AdicionarAsync(cliente, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new ClienteResponse
        {
            Id = cliente.Id,
            Nome = cliente.Nome,
            Cpf = cliente.Cpf,
            Telefone = cliente.Telefone,
            CriadoEm = cliente.CriadoEm,
            AtualizadoEm = cliente.AtualizadoEm
        };

        return Result<ClienteResponse>.Ok(response, "Cliente cadastrado com sucesso");
    }
}
