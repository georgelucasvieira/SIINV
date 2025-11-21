using API_Investimentos.Domain.Entities;
using API_Investimentos.Domain.Enums;
using API_Investimentos.Domain.Interfaces;
using API_Investimentos.Domain.ValueObjects;
using API_Investimentos.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace API_Investimentos.Infrastructure.Repositories;

/// <summary>
/// Implementação do repositório de produtos
/// </summary>
public class RepositorioProduto : RepositorioBase<Produto>, IRepositorioProduto
{
    public RepositorioProduto(InvestimentosDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Produto>> ObterPorTipoAsync(TipoProduto tipo, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.Tipo == tipo)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Produto>> ObterAtivosAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.Ativo)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Produto>> ObterPorPerfilAsync(PerfilInvestidor perfil, CancellationToken cancellationToken = default)
    {

        var niveisRiscoAceitos = perfil switch
        {
            PerfilInvestidor.Conservador => new[] { NivelRisco.MuitoBaixo, NivelRisco.Baixo },
            PerfilInvestidor.Moderado => new[] { NivelRisco.MuitoBaixo, NivelRisco.Baixo, NivelRisco.Medio },
            PerfilInvestidor.Agressivo => Enum.GetValues<NivelRisco>(),
            _ => Array.Empty<NivelRisco>()
        };

        return await _dbSet
            .Where(p => p.Ativo && niveisRiscoAceitos.Contains(p.NivelRisco))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Produto>> ObterDisponiveis(Dinheiro valor, int prazoMeses, CancellationToken cancellationToken = default)
    {
        var valorDecimal = valor.Valor;

        return await _dbSet
            .Where(p => p.Ativo &&
                       p.ValorMinimo.Valor <= valorDecimal &&
                       p.PrazoMinimoMeses <= prazoMeses &&
                       (p.PrazoMaximoMeses == null || p.PrazoMaximoMeses >= prazoMeses))
            .ToListAsync(cancellationToken);
    }

    public async Task<Produto?> ObterPorNomeAsync(string nome, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(p => p.Nome == nome, cancellationToken);
    }
}
