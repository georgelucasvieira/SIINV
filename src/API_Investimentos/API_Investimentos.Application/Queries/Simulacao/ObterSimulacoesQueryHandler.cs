using API_Investimentos.Application.Common;
using API_Investimentos.Application.DTOs.Responses;
using API_Investimentos.Domain.Interfaces;
using MediatR;

namespace API_Investimentos.Application.Queries.Simulacao;

/// <summary>
/// Handler para obter simulações
/// </summary>
public class ObterSimulacoesQueryHandler : IRequestHandler<ObterSimulacoesQuery, Result<List<SimulacaoResponse>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ObterSimulacoesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<SimulacaoResponse>>> Handle(
        ObterSimulacoesQuery request,
        CancellationToken cancellationToken)
    {
        IEnumerable<Domain.Entities.Simulacao> simulacoes;

        if (request.ClienteId.HasValue)
        {
            simulacoes = await _unitOfWork.Simulacoes.ObterPorClienteAsync(
                request.ClienteId.Value,
                cancellationToken);
        }
        else if (request.DataInicio.HasValue && request.DataFim.HasValue)
        {
            simulacoes = await _unitOfWork.Simulacoes.ObterPorPeriodoAsync(
                request.DataInicio.Value,
                request.DataFim.Value,
                cancellationToken);
        }
        else
        {
            simulacoes = await _unitOfWork.Simulacoes.ObterTodosAsync(cancellationToken);
        }

        var response = simulacoes.Select(s => new SimulacaoResponse
        {
            Id = s.Id,
            ProdutoValidado = true,
            Produto = s.Produto != null ? new ProdutoResponse
            {
                Id = s.Produto.Id,
                Nome = s.Produto.Nome,
                Tipo = s.Produto.Tipo.ToString(),
                Rentabilidade = s.Produto.TaxaRentabilidade.Valor,
                Risco = s.Produto.NivelRisco.ToString()
            } : null,
            ResultadoSimulacao = new ResultadoSimulacaoResponse
            {
                ValorInvestido = s.ValorInvestido.Valor,
                ValorFinalBruto = s.ValorFinalBruto.Valor,
                ValorFinalLiquido = s.ValorFinalLiquido.Valor,
                RendimentoBruto = s.CalcularRendimentoBruto().Valor,
                RendimentoLiquido = s.CalcularRendimentoLiquido().Valor,
                ValorIR = s.ValorIR.Valor,
                AliquotaIR = s.AliquotaIR.Valor,
                RentabilidadeEfetiva = s.TaxaRentabilidadeEfetiva.Valor,
                RentabilidadeLiquida = s.CalcularRentabilidadeLiquida().Valor,
                PrazoMeses = s.PrazoMeses,
                DataVencimento = s.DataVencimento,
                DataSimulacao = s.CriadoEm
            }
        }).ToList();

        return Result<List<SimulacaoResponse>>.Ok(response);
    }
}
