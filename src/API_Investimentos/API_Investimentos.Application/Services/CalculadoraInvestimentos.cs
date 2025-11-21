using API_Investimentos.Domain.Entities;
using API_Investimentos.Domain.Enums;
using API_Investimentos.Domain.ValueObjects;

namespace API_Investimentos.Application.Services;

/// <summary>
/// Implementação do serviço de cálculo de investimentos
/// Baseado nas fórmulas definidas em docs/design-tecnico/CALCULOS.md
/// </summary>
public class CalculadoraInvestimentos : ICalculadoraInvestimentos
{
    public ResultadoCalculo Calcular(Produto produto, Dinheiro valorInvestido, int prazoMeses)
    {
        var dataVencimento = DateTime.UtcNow.AddMonths(prazoMeses);
        var prazoAnos = prazoMeses / 12.0m;


        var valorFinalBruto = produto.Tipo switch
        {
            TipoProduto.CDB => CalcularCDB(valorInvestido, produto.TaxaRentabilidade, prazoAnos),
            TipoProduto.TesouroSelic => CalcularTesouroSelic(valorInvestido, produto.TaxaRentabilidade, prazoAnos),
            TipoProduto.TesouroPrefixado => CalcularTesouroPrefixado(valorInvestido, produto.TaxaRentabilidade, prazoAnos),
            TipoProduto.TesouroIPCA => CalcularTesouroIPCA(valorInvestido, produto.TaxaRentabilidade, prazoAnos),
            TipoProduto.LCI => CalcularLCI(valorInvestido, produto.TaxaRentabilidade, prazoAnos),
            TipoProduto.LCA => CalcularLCA(valorInvestido, produto.TaxaRentabilidade, prazoAnos),
            TipoProduto.Fundo => CalcularFundo(valorInvestido, produto.TaxaRentabilidade, produto.TaxaAdministracao, prazoAnos),
            _ => throw new InvalidOperationException($"Tipo de produto não suportado: {produto.Tipo}")
        };


        if (produto.Tipo == TipoProduto.Fundo && produto.TaxaPerformance != null)
        {
            var rendimento = valorFinalBruto - valorInvestido;
            var taxaPerformance = produto.TaxaPerformance.AplicarA(rendimento);
            valorFinalBruto = valorFinalBruto - taxaPerformance;
        }


        Dinheiro valorIR;
        Percentual aliquotaIR;

        if (produto.IsentoIR)
        {
            valorIR = Dinheiro.Zero;
            aliquotaIR = Percentual.Zero;
        }
        else
        {
            aliquotaIR = CalcularAliquotaIR(prazoMeses);
            var rendimento = valorFinalBruto - valorInvestido;
            valorIR = aliquotaIR.AplicarA(rendimento);
        }

        var valorFinalLiquido = valorFinalBruto - valorIR;

        return new ResultadoCalculo
        {
            ValorFinalBruto = valorFinalBruto,
            ValorIR = valorIR,
            ValorFinalLiquido = valorFinalLiquido,
            TaxaRentabilidadeEfetiva = produto.TaxaRentabilidade,
            AliquotaIR = aliquotaIR,
            DataVencimento = dataVencimento
        };
    }

    /// <summary>
    /// CDB: VF = VP × (1 + Taxa_Anual)^Anos
    /// </summary>
    private Dinheiro CalcularCDB(Dinheiro valorInvestido, Percentual taxa, decimal prazoAnos)
    {
        var montante = valorInvestido.Valor * (decimal)Math.Pow((double)(1 + taxa.Valor), (double)prazoAnos);
        return Dinheiro.Criar(montante);
    }

    /// <summary>
    /// Tesouro Selic: VF = VP × (1 + Taxa_Selic)^Anos
    /// </summary>
    private Dinheiro CalcularTesouroSelic(Dinheiro valorInvestido, Percentual taxa, decimal prazoAnos)
    {
        var montante = valorInvestido.Valor * (decimal)Math.Pow((double)(1 + taxa.Valor), (double)prazoAnos);
        return Dinheiro.Criar(montante);
    }

    /// <summary>
    /// Tesouro Prefixado: VF = VP × (1 + Taxa_Anual)^Anos
    /// </summary>
    private Dinheiro CalcularTesouroPrefixado(Dinheiro valorInvestido, Percentual taxa, decimal prazoAnos)
    {
        var montante = valorInvestido.Valor * (decimal)Math.Pow((double)(1 + taxa.Valor), (double)prazoAnos);
        return Dinheiro.Criar(montante);
    }

    /// <summary>
    /// Tesouro IPCA+: VF = VP × (1 + Taxa_Real + IPCA)^Anos
    /// Simplificado: assumindo IPCA médio de 4% ao ano
    /// </summary>
    private Dinheiro CalcularTesouroIPCA(Dinheiro valorInvestido, Percentual taxa, decimal prazoAnos)
    {
        var ipcaMedio = 0.04m;
        var taxaTotal = taxa.Valor + ipcaMedio;
        var montante = valorInvestido.Valor * (decimal)Math.Pow((double)(1 + taxaTotal), (double)prazoAnos);
        return Dinheiro.Criar(montante);
    }

    /// <summary>
    /// LCI: VF = VP × (1 + Taxa_Anual)^Anos (isento de IR)
    /// </summary>
    private Dinheiro CalcularLCI(Dinheiro valorInvestido, Percentual taxa, decimal prazoAnos)
    {
        var montante = valorInvestido.Valor * (decimal)Math.Pow((double)(1 + taxa.Valor), (double)prazoAnos);
        return Dinheiro.Criar(montante);
    }

    /// <summary>
    /// LCA: VF = VP × (1 + Taxa_Anual)^Anos (isento de IR)
    /// </summary>
    private Dinheiro CalcularLCA(Dinheiro valorInvestido, Percentual taxa, decimal prazoAnos)
    {
        var montante = valorInvestido.Valor * (decimal)Math.Pow((double)(1 + taxa.Valor), (double)prazoAnos);
        return Dinheiro.Criar(montante);
    }

    /// <summary>
    /// Fundo: VF = [VP × (1 + Rent)^Anos] × (1 - Taxa_Admin)^Anos
    /// </summary>
    private Dinheiro CalcularFundo(Dinheiro valorInvestido, Percentual rentabilidade, Percentual? taxaAdmin, decimal prazoAnos)
    {
        var montanteComRentabilidade = valorInvestido.Valor * (decimal)Math.Pow((double)(1 + rentabilidade.Valor), (double)prazoAnos);

        if (taxaAdmin != null && taxaAdmin.Valor > 0)
        {
            var fatorTaxaAdmin = (decimal)Math.Pow((double)(1 - taxaAdmin.Valor), (double)prazoAnos);
            montanteComRentabilidade *= fatorTaxaAdmin;
        }

        return Dinheiro.Criar(montanteComRentabilidade);
    }

    /// <summary>
    /// Calcula a alíquota de IR regressiva
    /// Até 180 dias: 22.5%
    /// 181-360 dias: 20%
    /// 361-720 dias: 17.5%
    /// Acima de 720 dias: 15%
    /// </summary>
    private Percentual CalcularAliquotaIR(int prazoMeses)
    {
        var prazoDias = prazoMeses * 30;

        return prazoDias switch
        {
            <= 180 => Percentual.CriarDePercentual(22.5m),
            <= 360 => Percentual.CriarDePercentual(20.0m),
            <= 720 => Percentual.CriarDePercentual(17.5m),
            _ => Percentual.CriarDePercentual(15.0m)
        };
    }
}
