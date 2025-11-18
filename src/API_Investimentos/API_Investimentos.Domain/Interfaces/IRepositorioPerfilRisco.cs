using API_Investimentos.Domain.Entities;

namespace API_Investimentos.Domain.Interfaces;

/// <summary>
/// Repositório para operações com perfis de risco
/// </summary>
public interface IRepositorioPerfilRisco : IRepositorioBase<PerfilRisco>
{
    /// <summary>
    /// Obtém o perfil de risco de um cliente
    /// </summary>
    Task<PerfilRisco?> ObterPorClienteAsync(long clienteId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém perfis que precisam de reavaliação
    /// </summary>
    Task<IEnumerable<PerfilRisco>> ObterPerfisParaReavaliacaoAsync(CancellationToken cancellationToken = default);
}
