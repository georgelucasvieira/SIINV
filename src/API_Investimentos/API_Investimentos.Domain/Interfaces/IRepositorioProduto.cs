using API_Investimentos.Domain.Entities;
using API_Investimentos.Domain.Enums;
using API_Investimentos.Domain.ValueObjects;

namespace API_Investimentos.Domain.Interfaces;

/// <summary>
/// Repositório para operações com produtos de investimento
/// </summary>
public interface IRepositorioProduto : IRepositorioBase<Produto>
{
    /// <summary>
    /// Obtém produtos por tipo
    /// </summary>
    Task<IEnumerable<Produto>> ObterPorTipoAsync(TipoProduto tipo, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém apenas produtos ativos
    /// </summary>
    Task<IEnumerable<Produto>> ObterAtivosAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém produtos compatíveis com o perfil do investidor
    /// </summary>
    Task<IEnumerable<Produto>> ObterPorPerfilAsync(PerfilInvestidor perfil, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém produtos que podem ser investidos com o valor e prazo especificados
    /// </summary>
    Task<IEnumerable<Produto>> ObterDisponiveis(Dinheiro valor, int prazoMeses, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém produto por nome
    /// </summary>
    Task<Produto?> ObterPorNomeAsync(string nome, CancellationToken cancellationToken = default);
}
