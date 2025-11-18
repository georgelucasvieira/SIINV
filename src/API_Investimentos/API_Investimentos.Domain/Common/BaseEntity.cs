namespace API_Investimentos.Domain.Common;

/// <summary>
/// Classe base para todas as entidades do domínio
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Identificador único da entidade
    /// </summary>
    public long Id { get; protected set; }

    /// <summary>
    /// Data de criação do registro
    /// </summary>
    public DateTime CriadoEm { get; protected set; } = DateTime.UtcNow;

    /// <summary>
    /// Data da última atualização do registro
    /// </summary>
    public DateTime? AtualizadoEm { get; protected set; }

    /// <summary>
    /// Indica se o registro foi excluído logicamente
    /// </summary>
    public bool Excluido { get; protected set; }

    /// <summary>
    /// Data de exclusão lógica
    /// </summary>
    public DateTime? ExcluidoEm { get; protected set; }

    /// <summary>
    /// Marca o registro como atualizado
    /// </summary>
    protected void MarcarComoAtualizado()
    {
        AtualizadoEm = DateTime.UtcNow;
    }

    /// <summary>
    /// Marca o registro como excluído (soft delete)
    /// </summary>
    public void MarcarComoExcluido()
    {
        Excluido = true;
        ExcluidoEm = DateTime.UtcNow;
        AtualizadoEm = DateTime.UtcNow;
    }

    /// <summary>
    /// Restaura o registro excluído
    /// </summary>
    public void Restaurar()
    {
        Excluido = false;
        ExcluidoEm = null;
        AtualizadoEm = DateTime.UtcNow;
    }
}
