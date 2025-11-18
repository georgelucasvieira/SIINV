using API_Investimentos.Application.Common;
using API_Investimentos.Application.DTOs.Responses;
using API_Investimentos.Domain.Interfaces;
using MediatR;

namespace API_Investimentos.Application.Queries.PerfilRisco;

/// <summary>
/// Handler para obter perfil de risco de um cliente
/// </summary>
public class ObterPerfilRiscoQueryHandler : IRequestHandler<ObterPerfilRiscoQuery, Result<PerfilRiscoResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ObterPerfilRiscoQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PerfilRiscoResponse>> Handle(
        ObterPerfilRiscoQuery request,
        CancellationToken cancellationToken)
    {
        var perfil = await _unitOfWork.PerfisRisco.ObterPorClienteAsync(
            request.ClienteId,
            cancellationToken);

        if (perfil == null)
        {
            return Result<PerfilRiscoResponse>.Falha("Perfil de risco não encontrado para este cliente");
        }

        var response = new PerfilRiscoResponse
        {
            ClienteId = perfil.ClienteId,
            Perfil = perfil.Perfil.ToString(),
            Pontuacao = perfil.Pontuacao,
            Descricao = perfil.Descricao,
            UltimaAtualizacao = perfil.AtualizadoEm ?? perfil.CriadoEm,
            ProximaAvaliacao = perfil.DataProximaAvaliacao
        };

        return Result<PerfilRiscoResponse>.Ok(response);
    }
}
