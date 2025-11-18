namespace API_Investimentos.Application.DTOs.Responses;

/// <summary>
/// Response com o resultado de uma simulação
/// </summary>
public class SimulacaoResponse
{
    public long Id { get; set; }
    public bool ProdutoValidado { get; set; }
    public ProdutoResponse? Produto { get; set; }
    public ResultadoSimulacaoResponse ResultadoSimulacao { get; set; } = new();
}

/// <summary>
/// Detalhes do resultado da simulação
/// </summary>
public class ResultadoSimulacaoResponse
{
    public decimal ValorInvestido { get; set; }
    public decimal ValorFinalBruto { get; set; }
    public decimal ValorFinalLiquido { get; set; }
    public decimal RendimentoBruto { get; set; }
    public decimal RendimentoLiquido { get; set; }
    public decimal ValorIR { get; set; }
    public decimal AliquotaIR { get; set; }
    public decimal RentabilidadeEfetiva { get; set; }
    public decimal RentabilidadeLiquida { get; set; }
    public int PrazoMeses { get; set; }
    public DateTime DataVencimento { get; set; }
    public DateTime DataSimulacao { get; set; }
}
