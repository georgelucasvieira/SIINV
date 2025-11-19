using API_Investimentos.Domain.Entities;
using API_Investimentos.Domain.Enums;
using API_Investimentos.Domain.Interfaces;
using API_Investimentos.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace API_Investimentos.Infrastructure.Repositories;

/// <summary>
/// Implementação do repositório de histórico de investimentos
/// </summary>
public class RepositorioHistoricoInvestimento : RepositorioBase<HistoricoInvestimento>, IRepositorioHistoricoInvestimento
{
    public RepositorioHistoricoInvestimento(InvestimentosDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<HistoricoInvestimento>> ObterPorClienteAsync(long clienteId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(h => h.ClienteId == clienteId)
            .OrderByDescending(h => h.DataInvestimento)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<HistoricoInvestimento>> ObterAtivosDeClienteAsync(long clienteId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(h => h.ClienteId == clienteId && !h.Resgatado)
            .OrderByDescending(h => h.DataInvestimento)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<HistoricoInvestimento>> ObterPorTipoProdutoAsync(long clienteId, TipoProduto tipo, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(h => h.ClienteId == clienteId && h.TipoProduto == tipo)
            .OrderByDescending(h => h.DataInvestimento)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> CalcularVolumeTotalAsync(long clienteId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(h => h.ClienteId == clienteId && !h.Resgatado)
            .SumAsync(h => h.Valor.Valor, cancellationToken);
    }

    public async Task<int> CalcularFrequenciaAsync(long clienteId, int dias, CancellationToken cancellationToken = default)
    {
        var dataLimite = DateTime.UtcNow.AddDays(-dias);

        return await _dbSet
            .Where(h => h.ClienteId == clienteId && h.DataInvestimento >= dataLimite)
            .CountAsync(cancellationToken);
    }
}
