namespace API_Investimentos.Application.DTOs.Requests;

/// <summary>
/// Request para simulação de investimento
/// </summary>
public class SimularInvestimentoRequest
{
    /// <summary>
    /// ID do cliente
    /// </summary>
    public long ClienteId { get; set; }

    /// <summary>
    /// Valor a ser investido
    /// </summary>
    public decimal Valor { get; set; }

    /// <summary>
    /// Prazo do investimento em meses
    /// </summary>
    public int PrazoMeses { get; set; }

    /// <summary>
    /// Tipo do produto (CDB, Tesouro, LCI, etc.)
    /// </summary>
    public string TipoProduto { get; set; } = string.Empty;
}
