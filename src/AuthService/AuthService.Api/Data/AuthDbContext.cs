using AuthService.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Api.Data;

/// <summary>
/// DbContext para o Auth Service com schema dedicado
/// </summary>
public class AuthDbContext : DbContext
{
    public const string SchemaName = "Auth";

    public AuthDbContext(DbContextOptions<AuthDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Definir schema padrão para todas as tabelas
        modelBuilder.HasDefaultSchema(SchemaName);

        // Aplicar todas as configurações do assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuthDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
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
