namespace API_Investimentos.Application.DTOs.Responses;

/// <summary>
/// DTO de resposta para Investimento
/// </summary>
public class InvestimentoResponse
{
    public long Id { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public decimal Rentabilidade { get; set; }
    public DateTime Data { get; set; }
}
