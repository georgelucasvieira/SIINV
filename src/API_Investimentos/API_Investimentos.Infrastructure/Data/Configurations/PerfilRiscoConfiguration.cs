using API_Investimentos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API_Investimentos.Infrastructure.Data.Configurations;

/// <summary>
/// Configuração do Entity Framework para a entidade PerfilRisco
/// </summary>
public class PerfilRiscoConfiguration : IEntityTypeConfiguration<PerfilRisco>
{
    public void Configure(EntityTypeBuilder<PerfilRisco> builder)
    {
        builder.ToTable("PerfisRisco");

        builder.HasKey(pr => pr.Id);

        builder.Property(pr => pr.Id)
            .ValueGeneratedOnAdd();

        builder.Property(pr => pr.ClienteId)
            .IsRequired();

        builder.Property(pr => pr.Perfil)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(pr => pr.Pontuacao)
            .IsRequired();

        builder.Property(pr => pr.Descricao)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(pr => pr.FatoresCalculo)
            .HasColumnType("nvarchar(max)");

        builder.Property(pr => pr.DataProximaAvaliacao)
            .IsRequired();


        builder.Property(pr => pr.CriadoEm)
            .IsRequired();

        builder.Property(pr => pr.AtualizadoEm);

        builder.Property(pr => pr.Excluido)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(pr => pr.ExcluidoEm);


        builder.HasIndex(pr => pr.ClienteId)
            .IsUnique()
            .HasDatabaseName("IX_PerfisRisco_ClienteId");

        builder.HasIndex(pr => pr.DataProximaAvaliacao)
            .HasDatabaseName("IX_PerfisRisco_DataProximaAvaliacao");
    }
}
