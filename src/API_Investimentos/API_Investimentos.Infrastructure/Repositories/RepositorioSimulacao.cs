using API_Investimentos.Domain.Entities;
using API_Investimentos.Domain.Interfaces;
using API_Investimentos.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace API_Investimentos.Infrastructure.Repositories;

/// <summary>
/// Implementação do repositório de simulações
/// </summary>
public class RepositorioSimulacao : RepositorioBase<Simulacao>, IRepositorioSimulacao
{
    public RepositorioSimulacao(InvestimentosDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Simulacao>> ObterPorClienteAsync(long clienteId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.Produto)
            .Where(s => s.ClienteId == clienteId)
            .OrderByDescending(s => s.CriadoEm)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Simulacao>> ObterPorProdutoAsync(long produtoId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.Produto)
            .Where(s => s.ProdutoId == produtoId)
            .OrderByDescending(s => s.CriadoEm)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Simulacao>> ObterPorPeriodoAsync(DateTime dataInicio, DateTime dataFim, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.Produto)
            .Where(s => s.CriadoEm >= dataInicio && s.CriadoEm <= dataFim)
            .OrderByDescending(s => s.CriadoEm)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<(long ProdutoId, string ProdutoNome, DateTime Data, int Quantidade, decimal MediaValorFinal)>>
        ObterEstatisticasPorProdutoDiaAsync(DateTime? dataInicio = null, DateTime? dataFim = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Include(s => s.Produto).AsQueryable();

        if (dataInicio.HasValue)
            query = query.Where(s => s.CriadoEm >= dataInicio.Value);

        if (dataFim.HasValue)
            query = query.Where(s => s.CriadoEm <= dataFim.Value);

        // Carregar dados para memória e fazer agregação no cliente
        // devido à limitação do EF com Value Objects em funções de agregação
        var dados = await query
            .Select(s => new
            {
                s.ProdutoId,
                ProdutoNome = s.Produto!.Nome,
                Data = s.CriadoEm.Date,
                ValorFinalLiquido = s.ValorFinalLiquido.Valor
            })
            .ToListAsync(cancellationToken);

        return dados
            .GroupBy(s => new { s.ProdutoId, s.ProdutoNome, s.Data })
            .Select(g => (
                g.Key.ProdutoId,
                g.Key.ProdutoNome,
                g.Key.Data,
                g.Count(),
                g.Average(s => s.ValorFinalLiquido)
            ))
            .ToList();
    }
}
