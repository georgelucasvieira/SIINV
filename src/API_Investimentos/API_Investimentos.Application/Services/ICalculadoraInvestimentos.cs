using API_Investimentos.Domain.Entities;
using API_Investimentos.Domain.ValueObjects;

namespace API_Investimentos.Application.Services;

/// <summary>
/// Interface para cálculo de investimentos
/// </summary>
public interface ICalculadoraInvestimentos
{
    /// <summary>
    /// Calcula o resultado de um investimento
    /// </summary>
    ResultadoCalculo Calcular(Produto produto, Dinheiro valorInvestido, int prazoMeses);
}

/// <summary>
/// Resultado do cálculo de investimento
/// </summary>
public class ResultadoCalculo
{
    public Dinheiro ValorFinalBruto { get; set; } = Dinheiro.Zero;
    public Dinheiro ValorIR { get; set; } = Dinheiro.Zero;
    public Dinheiro ValorFinalLiquido { get; set; } = Dinheiro.Zero;
    public Percentual TaxaRentabilidadeEfetiva { get; set; } = Percentual.Zero;
    public Percentual AliquotaIR { get; set; } = Percentual.Zero;
    public DateTime DataVencimento { get; set; }
}
