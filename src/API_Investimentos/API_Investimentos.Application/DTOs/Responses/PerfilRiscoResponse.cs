namespace API_Investimentos.Application.DTOs.Responses;

/// <summary>
/// Response com informações do perfil de risco do cliente
/// </summary>
public class PerfilRiscoResponse
{
    public long ClienteId { get; set; }
    public string Perfil { get; set; } = string.Empty;
    public int Pontuacao { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public DateTime UltimaAtualizacao { get; set; }
    public DateTime ProximaAvaliacao { get; set; }
}
