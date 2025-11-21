namespace API_Investimentos.Application.DTOs.Responses;

/// <summary>
/// DTO de resposta para Cliente
/// </summary>
public class ClienteResponse
{
    public long Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; }
    public DateTime? AtualizadoEm { get; set; }
}
