using API_Investimentos.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace API_Investimentos.Infrastructure.Data;

/// <summary>
/// DbContext para o domínio de investimentos
/// </summary>
public class InvestimentosDbContext : DbContext
{
    public InvestimentosDbContext(DbContextOptions<InvestimentosDbContext> options)
        : base(options)
    {
    }

    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<Simulacao> Simulacoes => Set<Simulacao>();
    public DbSet<PerfilRisco> PerfisRisco => Set<PerfilRisco>();
    public DbSet<HistoricoInvestimento> HistoricoInvestimentos => Set<HistoricoInvestimento>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplicar todas as configurações do assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InvestimentosDbContext).Assembly);

        // Query Filters para Soft Delete
        modelBuilder.Entity<Produto>().HasQueryFilter(p => !p.Excluido);
        modelBuilder.Entity<Simulacao>().HasQueryFilter(s => !s.Excluido);
        modelBuilder.Entity<PerfilRisco>().HasQueryFilter(pr => !pr.Excluido);
        modelBuilder.Entity<HistoricoInvestimento>().HasQueryFilter(hi => !hi.Excluido);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Atualizar timestamps automaticamente
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Modified)
            {
                var property = entry.Property("AtualizadoEm");
                if (property != null)
                {
                    property.CurrentValue = DateTime.UtcNow;
                }
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
