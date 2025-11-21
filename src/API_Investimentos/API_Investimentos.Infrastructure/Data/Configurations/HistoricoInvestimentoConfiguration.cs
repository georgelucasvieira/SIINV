using API_Investimentos.Domain.Entities;
using API_Investimentos.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API_Investimentos.Infrastructure.Data.Configurations;

/// <summary>
/// Configuração do Entity Framework para a entidade HistoricoInvestimento
/// </summary>
public class HistoricoInvestimentoConfiguration : IEntityTypeConfiguration<HistoricoInvestimento>
{
    public void Configure(EntityTypeBuilder<HistoricoInvestimento> builder)
    {
        builder.ToTable("HistoricoInvestimentos");

        builder.HasKey(hi => hi.Id);

        builder.Property(hi => hi.Id)
            .ValueGeneratedOnAdd();

        builder.Property(hi => hi.ClienteId)
            .IsRequired();

        builder.Property(hi => hi.TipoProduto)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(hi => hi.Valor)
            .HasConversion(
                v => v.Valor,
                v => Dinheiro.CriarSemValidacao(v))
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(hi => hi.Rentabilidade)
            .HasConversion(
                v => v.Valor,
                v => Percentual.CriarSemValidacao(v))
            .HasColumnType("decimal(10,6)")
            .IsRequired();

        builder.Property(hi => hi.DataInvestimento)
            .IsRequired();

        builder.Property(hi => hi.DataVencimento);

        builder.Property(hi => hi.Resgatado)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(hi => hi.DataResgate);


        builder.Property(hi => hi.CriadoEm)
            .IsRequired();

        builder.Property(hi => hi.AtualizadoEm);

        builder.Property(hi => hi.Excluido)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(hi => hi.ExcluidoEm);


        builder.HasIndex(hi => hi.ClienteId)
            .HasDatabaseName("IX_HistoricoInvestimentos_ClienteId");

        builder.HasIndex(hi => hi.TipoProduto)
            .HasDatabaseName("IX_HistoricoInvestimentos_TipoProduto");

        builder.HasIndex(hi => hi.DataInvestimento)
            .HasDatabaseName("IX_HistoricoInvestimentos_DataInvestimento");

        builder.HasIndex(hi => new { hi.ClienteId, hi.Resgatado })
            .HasDatabaseName("IX_HistoricoInvestimentos_ClienteId_Resgatado");
    }
}
