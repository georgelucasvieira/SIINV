using API_Investimentos.Domain.Common;

namespace API_Investimentos.Domain.Interfaces;

/// <summary>
/// Interface base para todos os repositórios
/// </summary>
public interface IRepositorioBase<T> where T : BaseEntity
{
    /// <summary>
    /// Obtém uma entidade por ID
    /// </summary>
    Task<T?> ObterPorIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém todas as entidades
    /// </summary>
    Task<IEnumerable<T>> ObterTodosAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adiciona uma nova entidade
    /// </summary>
    Task<T> AdicionarAsync(T entidade, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza uma entidade existente
    /// </summary>
    Task AtualizarAsync(T entidade, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove uma entidade (soft delete)
    /// </summary>
    Task RemoverAsync(T entidade, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove uma entidade permanentemente (hard delete)
    /// </summary>
    Task RemoverPermanentementeAsync(T entidade, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica se existe uma entidade com o ID especificado
    /// </summary>
    Task<bool> ExisteAsync(long id, CancellationToken cancellationToken = default);
}
