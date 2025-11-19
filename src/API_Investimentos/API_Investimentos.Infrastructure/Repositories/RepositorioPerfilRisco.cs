using API_Investimentos.Domain.Entities;
using API_Investimentos.Domain.Interfaces;
using API_Investimentos.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace API_Investimentos.Infrastructure.Repositories;

/// <summary>
/// Implementação do repositório de perfis de risco
/// </summary>
public class RepositorioPerfilRisco : RepositorioBase<PerfilRisco>, IRepositorioPerfilRisco
{
    public RepositorioPerfilRisco(InvestimentosDbContext context) : base(context)
    {
    }

    public async Task<PerfilRisco?> ObterPorClienteAsync(long clienteId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(p => p.ClienteId == clienteId, cancellationToken);
    }

    public async Task<IEnumerable<PerfilRisco>> ObterPerfisParaReavaliacaoAsync(CancellationToken cancellationToken = default)
    {
        var dataAtual = DateTime.UtcNow;

        return await _dbSet
            .Where(p => p.DataProximaAvaliacao <= dataAtual)
            .ToListAsync(cancellationToken);
    }
}
