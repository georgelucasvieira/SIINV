using API_Investimentos.Domain.Entities;
using API_Investimentos.Domain.Interfaces;
using API_Investimentos.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace API_Investimentos.Infrastructure.Repositories;

/// <summary>
/// Implementação do repositório de clientes
/// </summary>
public class RepositorioCliente : RepositorioBase<Cliente>, IRepositorioCliente
{
    public RepositorioCliente(InvestimentosDbContext context) : base(context)
    {
    }

    public async Task<Cliente?> ObterPorCpfAsync(string cpf, CancellationToken cancellationToken = default)
    {
        var cpfLimpo = new string(cpf.Where(char.IsDigit).ToArray());
        return await _dbSet
            .FirstOrDefaultAsync(c => c.Cpf == cpfLimpo, cancellationToken);
    }

    public async Task<bool> ExisteCpfAsync(string cpf, CancellationToken cancellationToken = default)
    {
        var cpfLimpo = new string(cpf.Where(char.IsDigit).ToArray());
        return await _dbSet
            .AnyAsync(c => c.Cpf == cpfLimpo, cancellationToken);
    }
}
