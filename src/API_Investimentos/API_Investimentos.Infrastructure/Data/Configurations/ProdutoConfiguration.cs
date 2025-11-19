using API_Investimentos.Domain.Entities;
using API_Investimentos.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API_Investimentos.Infrastructure.Data.Configurations;

/// <summary>
/// Configuração do Entity Framework para a entidade Produto
/// </summary>
public class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.ToTable("Produtos");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .ValueGeneratedOnAdd();

        builder.Property(p => p.Nome)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Descricao)
            .HasMaxLength(1000);

        builder.Property(p => p.Tipo)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(p => p.NivelRisco)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        // Configuração do Value Object TaxaRentabilidade
        builder.Property(p => p.TaxaRentabilidade)
            .HasConversion(
                v => v.Valor,
                v => Percentual.CriarSemValidacao(v))
            .HasColumnType("decimal(10,6)")
            .HasColumnName("TaxaRentabilidade")
            .IsRequired();

        // Configuração do Value Object ValorMinimo
        builder.Property(p => p.ValorMinimo)
            .HasConversion(
                v => v.Valor,
                v => Dinheiro.CriarSemValidacao(v))
            .HasColumnType("decimal(18,2)")
            .HasColumnName("ValorMinimo")
            .IsRequired();

        builder.Property(p => p.PrazoMinimoMeses)
            .IsRequired();

        builder.Property(p => p.PrazoMaximoMeses);

        builder.Property(p => p.LiquidezDiaria)
            .IsRequired();

        builder.Property(p => p.Ativo)
            .IsRequired()
            .HasDefaultValue(true);

        // Configuração do Value Object TaxaAdministracao (nullable)
        builder.Property(p => p.TaxaAdministracao)
            .HasConversion(
                v => v != null ? v.Valor : (decimal?)null,
                v => v.HasValue ? Percentual.CriarSemValidacao(v.Value) : null)
            .HasColumnType("decimal(10,6)")
            .HasColumnName("TaxaAdministracao");

        // Configuração do Value Object TaxaPerformance (nullable)
        builder.Property(p => p.TaxaPerformance)
            .HasConversion(
                v => v != null ? v.Valor : (decimal?)null,
                v => v.HasValue ? Percentual.CriarSemValidacao(v.Value) : null)
            .HasColumnType("decimal(10,6)")
            .HasColumnName("TaxaPerformance");

        builder.Property(p => p.IsentoIR)
            .IsRequired()
            .HasDefaultValue(false);

        // Propriedades de auditoria
        builder.Property(p => p.CriadoEm)
            .IsRequired();

        builder.Property(p => p.AtualizadoEm);

        builder.Property(p => p.Excluido)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(p => p.ExcluidoEm);

        // Índices
        builder.HasIndex(p => p.Nome)
            .HasDatabaseName("IX_Produtos_Nome");

        builder.HasIndex(p => p.Tipo)
            .HasDatabaseName("IX_Produtos_Tipo");

        builder.HasIndex(p => p.Ativo)
            .HasDatabaseName("IX_Produtos_Ativo");

        builder.HasIndex(p => new { p.Tipo, p.Ativo })
            .HasDatabaseName("IX_Produtos_Tipo_Ativo");
    }
}
