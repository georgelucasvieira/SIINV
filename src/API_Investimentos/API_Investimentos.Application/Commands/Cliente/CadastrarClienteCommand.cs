using API_Investimentos.Application.Common;
using API_Investimentos.Application.DTOs.Responses;
using MediatR;

namespace API_Investimentos.Application.Commands.Cliente;

/// <summary>
/// Command para cadastrar um cliente
/// </summary>
public class CadastrarClienteCommand : IRequest<Result<ClienteResponse>>
{
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
}
