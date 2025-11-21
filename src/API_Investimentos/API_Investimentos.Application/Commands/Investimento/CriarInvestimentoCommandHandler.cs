using API_Investimentos.Application.Common;
using API_Investimentos.Application.DTOs.Responses;
using API_Investimentos.Domain.Entities;
using API_Investimentos.Domain.Interfaces;
using MediatR;

namespace API_Investimentos.Application.Commands.Investimento;

/// <summary>
/// Handler para criar investimento
/// </summary>
public class CriarInvestimentoCommandHandler : IRequestHandler<CriarInvestimentoCommand, Result<InvestimentoResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CriarInvestimentoCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<InvestimentoResponse>> Handle(
        CriarInvestimentoCommand request,
        CancellationToken cancellationToken)
    {

        var simulacao = await _unitOfWork.Simulacoes.ObterPorIdAsync(request.SimulacaoId, cancellationToken);

        if (simulacao == null)
        {
            return Result<InvestimentoResponse>.Falha("Simulação não encontrada");
        }

        if (simulacao.ClienteId != request.ClienteId)
        {
            return Result<InvestimentoResponse>.Falha("Simulação não pertence ao cliente informado");
        }


        var produto = await _unitOfWork.Produtos.ObterPorIdAsync(simulacao.ProdutoId, cancellationToken);

        if (produto == null)
        {
            return Result<InvestimentoResponse>.Falha("Produto da simulação não encontrado");
        }


        var investimento = new HistoricoInvestimento(
            clienteId: request.ClienteId,
            tipoProduto: produto.Tipo,
            valor: simulacao.ValorInvestido,
            rentabilidade: simulacao.TaxaRentabilidadeEfetiva,
            dataInvestimento: DateTime.UtcNow,
            dataVencimento: simulacao.DataVencimento);

        await _unitOfWork.HistoricoInvestimentos.AdicionarAsync(investimento, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new InvestimentoResponse
        {
            Id = investimento.Id,
            Tipo = investimento.TipoProduto.ToString(),
            Valor = investimento.Valor.Valor,
            Rentabilidade = investimento.Rentabilidade.Valor,
            Data = investimento.DataInvestimento
        };

        return Result<InvestimentoResponse>.Ok(response, "Investimento criado com sucesso");
    }
}
