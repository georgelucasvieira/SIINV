namespace API_Investimentos.Application.Messaging.Events;

/// <summary>
/// Evento publicado quando um cliente é criado
/// </summary>
public record ClienteCriadoEvent
{
    public long ClienteId { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string PerfilRisco { get; init; } = string.Empty;
    public DateTime DataCriacao { get; init; }
}
