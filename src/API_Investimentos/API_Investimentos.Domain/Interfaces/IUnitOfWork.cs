namespace API_Investimentos.Domain.Interfaces;

/// <summary>
/// Interface para Unit of Work - gerencia transações
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Repositório de produtos
    /// </summary>
    IRepositorioProduto Produtos { get; }

    /// <summary>
    /// Repositório de simulações
    /// </summary>
    IRepositorioSimulacao Simulacoes { get; }

    /// <summary>
    /// Repositório de perfis de risco
    /// </summary>
    IRepositorioPerfilRisco PerfisRisco { get; }

    /// <summary>
    /// Repositório de histórico de investimentos
    /// </summary>
    IRepositorioHistoricoInvestimento HistoricoInvestimentos { get; }

    /// <summary>
    /// Repositório de clientes
    /// </summary>
    IRepositorioCliente Clientes { get; }

    /// <summary>
    /// Salva todas as alterações no banco de dados
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Inicia uma transação
    /// </summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Confirma a transação
    /// </summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Reverte a transação
    /// </summary>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
