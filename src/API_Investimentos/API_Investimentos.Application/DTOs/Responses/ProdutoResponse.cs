namespace API_Investimentos.Application.DTOs.Responses;

/// <summary>
/// Response com informações de um produto
/// </summary>
public class ProdutoResponse
{
    public long Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public decimal Rentabilidade { get; set; }
    public string Risco { get; set; } = string.Empty;
    public decimal ValorMinimo { get; set; }
    public int PrazoMinimoMeses { get; set; }
    public int? PrazoMaximoMeses { get; set; }
    public bool LiquidezDiaria { get; set; }
    public bool Ativo { get; set; }
    public bool IsentoIR { get; set; }
    public decimal? TaxaAdministracao { get; set; }
    public decimal? TaxaPerformance { get; set; }
}
