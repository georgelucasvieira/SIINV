using API_Investimentos.Application.Common;
using API_Investimentos.Application.DTOs.Responses;
using API_Investimentos.Domain.Interfaces;
using MediatR;

namespace API_Investimentos.Application.Commands.PerfilRisco;

/// <summary>
/// Handler para cadastrar perfil de risco
/// </summary>
public class CadastrarPerfilRiscoCommandHandler : IRequestHandler<CadastrarPerfilRiscoCommand, Result<PerfilRiscoResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CadastrarPerfilRiscoCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PerfilRiscoResponse>> Handle(
        CadastrarPerfilRiscoCommand request,
        CancellationToken cancellationToken)
    {

        var perfilExistente = await _unitOfWork.PerfisRisco.ObterPorClienteAsync(
            request.ClienteId,
            cancellationToken);

        if (perfilExistente != null)
        {

            perfilExistente.AtualizarPontuacao(request.Pontuacao, request.FatoresCalculo);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var responseAtualizado = new PerfilRiscoResponse
            {
                ClienteId = perfilExistente.ClienteId,
                Perfil = perfilExistente.Perfil.ToString(),
                Pontuacao = perfilExistente.Pontuacao,
                Descricao = perfilExistente.Descricao,
                UltimaAtualizacao = perfilExistente.AtualizadoEm ?? perfilExistente.CriadoEm,
                ProximaAvaliacao = perfilExistente.DataProximaAvaliacao
            };

            return Result<PerfilRiscoResponse>.Ok(responseAtualizado, "Perfil de risco atualizado com sucesso");
        }


        var novoPerfil = new Domain.Entities.PerfilRisco(
            request.ClienteId,
            request.Pontuacao,
            request.FatoresCalculo);

        await _unitOfWork.PerfisRisco.AdicionarAsync(novoPerfil, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new PerfilRiscoResponse
        {
            ClienteId = novoPerfil.ClienteId,
            Perfil = novoPerfil.Perfil.ToString(),
            Pontuacao = novoPerfil.Pontuacao,
            Descricao = novoPerfil.Descricao,
            UltimaAtualizacao = novoPerfil.CriadoEm,
            ProximaAvaliacao = novoPerfil.DataProximaAvaliacao
        };

        return Result<PerfilRiscoResponse>.Ok(response, "Perfil de risco cadastrado com sucesso");
    }
}
