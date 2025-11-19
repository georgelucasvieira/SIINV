using API_Investimentos.Domain.Interfaces;
using API_Investimentos.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace API_Investimentos.Infrastructure.Repositories;

/// <summary>
/// Implementação do Unit of Work
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly InvestimentosDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(
        InvestimentosDbContext context,
        IRepositorioProduto produtos,
        IRepositorioSimulacao simulacoes,
        IRepositorioPerfilRisco perfisRisco,
        IRepositorioHistoricoInvestimento historicoInvestimentos)
    {
        _context = context;
        Produtos = produtos;
        Simulacoes = simulacoes;
        PerfisRisco = perfisRisco;
        HistoricoInvestimentos = historicoInvestimentos;
    }

    public IRepositorioProduto Produtos { get; }
    public IRepositorioSimulacao Simulacoes { get; }
    public IRepositorioPerfilRisco PerfisRisco { get; }
    public IRepositorioHistoricoInvestimento HistoricoInvestimentos { get; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);

            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
            }
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
