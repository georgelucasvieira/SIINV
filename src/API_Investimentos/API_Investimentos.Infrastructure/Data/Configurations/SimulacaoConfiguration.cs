using API_Investimentos.Domain.Entities;
using API_Investimentos.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API_Investimentos.Infrastructure.Data.Configurations;

/// <summary>
/// Configuração do Entity Framework para a entidade Simulacao
/// </summary>
public class SimulacaoConfiguration : IEntityTypeConfiguration<Simulacao>
{
    public void Configure(EntityTypeBuilder<Simulacao> builder)
    {
        builder.ToTable("Simulacoes");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .ValueGeneratedOnAdd();

        builder.Property(s => s.ClienteId)
            .IsRequired();

        builder.Property(s => s.ProdutoId)
            .IsRequired();


        builder.HasOne(s => s.Produto)
            .WithMany()
            .HasForeignKey(s => s.ProdutoId)
            .OnDelete(DeleteBehavior.Restrict);


        builder.Property(s => s.ValorInvestido)
            .HasConversion(
                v => v.Valor,
                v => Dinheiro.CriarSemValidacao(v))
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(s => s.PrazoMeses)
            .IsRequired();

        builder.Property(s => s.DataVencimento)
            .IsRequired();

        builder.Property(s => s.ValorFinalBruto)
            .HasConversion(
                v => v.Valor,
                v => Dinheiro.CriarSemValidacao(v))
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(s => s.ValorIR)
            .HasConversion(
                v => v.Valor,
                v => Dinheiro.CriarSemValidacao(v))
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(s => s.ValorFinalLiquido)
            .HasConversion(
                v => v.Valor,
                v => Dinheiro.CriarSemValidacao(v))
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(s => s.TaxaRentabilidadeEfetiva)
            .HasConversion(
                v => v.Valor,
                v => Percentual.CriarSemValidacao(v))
            .HasColumnType("decimal(10,6)")
            .IsRequired();

        builder.Property(s => s.AliquotaIR)
            .HasConversion(
                v => v.Valor,
                v => Percentual.CriarSemValidacao(v))
            .HasColumnType("decimal(10,6)")
            .IsRequired();

        builder.Property(s => s.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(s => s.Observacoes)
            .HasMaxLength(500);


        builder.Property(s => s.CriadoPorId);

        builder.Property(s => s.AtualizadoPorId);

        builder.Property(s => s.CriadoEm)
            .IsRequired();

        builder.Property(s => s.AtualizadoEm);

        builder.Property(s => s.Excluido)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(s => s.ExcluidoEm);


        builder.HasIndex(s => s.ClienteId)
            .HasDatabaseName("IX_Simulacoes_ClienteId");

        builder.HasIndex(s => s.ProdutoId)
            .HasDatabaseName("IX_Simulacoes_ProdutoId");

        builder.HasIndex(s => s.CriadoEm)
            .HasDatabaseName("IX_Simulacoes_CriadoEm");

        builder.HasIndex(s => s.Status)
            .HasDatabaseName("IX_Simulacoes_Status");

        builder.HasIndex(s => new { s.ClienteId, s.CriadoEm })
            .HasDatabaseName("IX_Simulacoes_ClienteId_CriadoEm");
    }
}
