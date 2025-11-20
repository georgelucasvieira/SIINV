namespace API_Investimentos.Application.Messaging.Events;

/// <summary>
/// Evento publicado quando uma simulação é realizada com sucesso
/// </summary>
public record SimulacaoRealizadaEvent
{
    public long SimulacaoId { get; init; }
    public long ClienteId { get; init; }
    public long ProdutoId { get; init; }
    public string TipoProduto { get; init; } = string.Empty;
    public decimal ValorInvestido { get; init; }
    public decimal ValorFinalBruto { get; init; }
    public decimal ValorFinalLiquido { get; init; }
    public decimal RendimentoBruto { get; init; }
    public decimal RendimentoLiquido { get; init; }
    public int PrazoMeses { get; init; }
    public DateTime DataSimulacao { get; init; }
    public DateTime DataVencimento { get; init; }
}
