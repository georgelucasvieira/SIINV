namespace API_Investimentos.Domain.Common;

/// <summary>
/// Interface para entidades que precisam de auditoria
/// </summary>
public interface IAuditavel
{
    /// <summary>
    /// ID do usuário que criou o registro
    /// </summary>
    long? CriadoPorId { get; }

    /// <summary>
    /// ID do usuário que atualizou o registro
    /// </summary>
    long? AtualizadoPorId { get; }

    /// <summary>
    /// Data de criação
    /// </summary>
    DateTime CriadoEm { get; }

    /// <summary>
    /// Data da última atualização
    /// </summary>
    DateTime? AtualizadoEm { get; }
}
