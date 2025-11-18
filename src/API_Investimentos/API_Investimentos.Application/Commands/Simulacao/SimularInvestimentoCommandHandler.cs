using API_Investimentos.Application.Common;
using API_Investimentos.Application.DTOs.Responses;
using API_Investimentos.Application.Services;
using API_Investimentos.Domain.Enums;
using API_Investimentos.Domain.Interfaces;
using API_Investimentos.Domain.ValueObjects;
using MediatR;

namespace API_Investimentos.Application.Commands.Simulacao;

/// <summary>
/// Handler para processar simulação de investimento
/// </summary>
public class SimularInvestimentoCommandHandler : IRequestHandler<SimularInvestimentoCommand, Result<SimulacaoResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICalculadoraInvestimentos _calculadora;

    public SimularInvestimentoCommandHandler(
        IUnitOfWork unitOfWork,
        ICalculadoraInvestimentos calculadora)
    {
        _unitOfWork = unitOfWork;
        _calculadora = calculadora;
    }

    public async Task<Result<SimulacaoResponse>> Handle(
        SimularInvestimentoCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Validar e converter tipo de produto
        if (!Enum.TryParse<TipoProduto>(request.TipoProduto, true, out var tipoProduto))
        {
            return Result<SimulacaoResponse>.Falha($"Tipo de produto inválido: {request.TipoProduto}");
        }

        // 2. Buscar produtos do tipo solicitado que estão ativos
        var produtos = await _unitOfWork.Produtos.ObterPorTipoAsync(tipoProduto, cancellationToken);
        var produtosAtivos = produtos.Where(p => p.Ativo).ToList();

        if (!produtosAtivos.Any())
        {
            return Result<SimulacaoResponse>.Falha($"Nenhum produto do tipo {request.TipoProduto} disponível");
        }

        // 3. Criar value objects
        var valorInvestido = Dinheiro.Criar(request.Valor);

        // 4. Filtrar produtos adequados ao valor e prazo
        var produtoAdequado = produtosAtivos.FirstOrDefault(p =>
            p.PodeInvestir(valorInvestido, request.PrazoMeses));

        if (produtoAdequado == null)
        {
            return Result<SimulacaoResponse>.Falha(
                $"Nenhum produto adequado encontrado. Valor mínimo ou prazo não atendem aos requisitos.");
        }

        // 5. Calcular simulação
        var resultadoCalculo = _calculadora.Calcular(
            produtoAdequado,
            valorInvestido,
            request.PrazoMeses);

        // 6. Criar entidade de simulação
        var simulacao = new Domain.Entities.Simulacao(
            request.ClienteId,
            produtoAdequado.Id,
            valorInvestido,
            request.PrazoMeses,
            resultadoCalculo.DataVencimento,
            resultadoCalculo.ValorFinalBruto,
            resultadoCalculo.ValorIR,
            resultadoCalculo.ValorFinalLiquido,
            resultadoCalculo.TaxaRentabilidadeEfetiva,
            resultadoCalculo.AliquotaIR);

        // 7. Persistir simulação
        await _unitOfWork.Simulacoes.AdicionarAsync(simulacao, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 8. Montar response
        var response = new SimulacaoResponse
        {
            Id = simulacao.Id,
            ProdutoValidado = true,
            Produto = new ProdutoResponse
            {
                Id = produtoAdequado.Id,
                Nome = produtoAdequado.Nome,
                Tipo = produtoAdequado.Tipo.ToString(),
                Rentabilidade = produtoAdequado.TaxaRentabilidade.Valor,
                Risco = produtoAdequado.NivelRisco.ToString()
            },
            ResultadoSimulacao = new ResultadoSimulacaoResponse
            {
                ValorInvestido = simulacao.ValorInvestido.Valor,
                ValorFinalBruto = simulacao.ValorFinalBruto.Valor,
                ValorFinalLiquido = simulacao.ValorFinalLiquido.Valor,
                RendimentoBruto = simulacao.CalcularRendimentoBruto().Valor,
                RendimentoLiquido = simulacao.CalcularRendimentoLiquido().Valor,
                ValorIR = simulacao.ValorIR.Valor,
                AliquotaIR = simulacao.AliquotaIR.Valor,
                RentabilidadeEfetiva = simulacao.TaxaRentabilidadeEfetiva.Valor,
                RentabilidadeLiquida = simulacao.CalcularRentabilidadeLiquida().Valor,
                PrazoMeses = simulacao.PrazoMeses,
                DataVencimento = simulacao.DataVencimento,
                DataSimulacao = simulacao.CriadoEm
            }
        };

        return Result<SimulacaoResponse>.Ok(response, "Simulação realizada com sucesso");
    }
}
