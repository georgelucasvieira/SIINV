using API_Investimentos.Domain.Entities;
using API_Investimentos.Domain.Enums;

namespace API_Investimentos.Domain.Interfaces;

/// <summary>
/// Repositório para operações com histórico de investimentos
/// </summary>
public interface IRepositorioHistoricoInvestimento : IRepositorioBase<HistoricoInvestimento>
{
    /// <summary>
    /// Obtém histórico de investimentos de um cliente
    /// </summary>
    Task<IEnumerable<HistoricoInvestimento>> ObterPorClienteAsync(long clienteId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém investimentos ativos (não resgatados) de um cliente
    /// </summary>
    Task<IEnumerable<HistoricoInvestimento>> ObterAtivosDeClienteAsync(long clienteId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém histórico por tipo de produto
    /// </summary>
    Task<IEnumerable<HistoricoInvestimento>> ObterPorTipoProdutoAsync(long clienteId, TipoProduto tipo, CancellationToken cancellationToken = default);

    /// <summary>
    /// Calcula o volume total investido por um cliente
    /// </summary>
    Task<decimal> CalcularVolumeTotalAsync(long clienteId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Calcula a frequência de investimentos (quantidade nos últimos N dias)
    /// </summary>
    Task<int> CalcularFrequenciaAsync(long clienteId, int dias, CancellationToken cancellationToken = default);
}
