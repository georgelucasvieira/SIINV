using API_Investimentos.Application.Common;
using API_Investimentos.Application.DTOs.Responses;
using MediatR;

namespace API_Investimentos.Application.Commands.PerfilRisco;

/// <summary>
/// Command para cadastrar um perfil de risco
/// </summary>
public class CadastrarPerfilRiscoCommand : IRequest<Result<PerfilRiscoResponse>>
{
    public long ClienteId { get; set; }
    public int Pontuacao { get; set; }
    public string? FatoresCalculo { get; set; }
}
