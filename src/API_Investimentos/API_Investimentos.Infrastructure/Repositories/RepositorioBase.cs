using API_Investimentos.Domain.Common;
using API_Investimentos.Domain.Interfaces;
using API_Investimentos.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace API_Investimentos.Infrastructure.Repositories;

/// <summary>
/// Implementação base do repositório genérico
/// </summary>
public class RepositorioBase<T> : IRepositorioBase<T> where T : BaseEntity
{
    protected readonly InvestimentosDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public RepositorioBase(InvestimentosDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> ObterPorIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> ObterTodosAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    public virtual async Task<T> AdicionarAsync(T entidade, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entidade, cancellationToken);
        return entidade;
    }

    public virtual Task AtualizarAsync(T entidade, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entidade);
        return Task.CompletedTask;
    }

    public virtual Task RemoverAsync(T entidade, CancellationToken cancellationToken = default)
    {
        entidade.MarcarComoExcluido();
        return Task.CompletedTask;
    }

    public virtual Task RemoverPermanentementeAsync(T entidade, CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(entidade);
        return Task.CompletedTask;
    }

    public virtual async Task<bool> ExisteAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(e => e.Id == id, cancellationToken);
    }
}
