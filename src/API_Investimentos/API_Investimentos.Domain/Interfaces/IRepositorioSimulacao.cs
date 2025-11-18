using API_Investimentos.Domain.Entities;

namespace API_Investimentos.Domain.Interfaces;

/// <summary>
/// Repositório para operações com simulações
/// </summary>
public interface IRepositorioSimulacao : IRepositorioBase<Simulacao>
{
    /// <summary>
    /// Obtém simulações de um cliente
    /// </summary>
    Task<IEnumerable<Simulacao>> ObterPorClienteAsync(long clienteId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém simulações de um produto
    /// </summary>
    Task<IEnumerable<Simulacao>> ObterPorProdutoAsync(long produtoId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém simulações por período
    /// </summary>
    Task<IEnumerable<Simulacao>> ObterPorPeriodoAsync(DateTime dataInicio, DateTime dataFim, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém estatísticas de simulações por produto e dia
    /// </summary>
    Task<IEnumerable<(long ProdutoId, string ProdutoNome, DateTime Data, int Quantidade, decimal MediaValorFinal)>>
        ObterEstatisticasPorProdutoDiaAsync(DateTime? dataInicio = null, DateTime? dataFim = null, CancellationToken cancellationToken = default);
}
