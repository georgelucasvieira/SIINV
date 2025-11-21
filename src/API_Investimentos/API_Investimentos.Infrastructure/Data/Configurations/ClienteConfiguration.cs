using API_Investimentos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API_Investimentos.Infrastructure.Data.Configurations;

/// <summary>
/// Configuração do Entity Framework para a entidade Cliente
/// </summary>
public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("Clientes");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedOnAdd();

        builder.Property(c => c.Nome)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Cpf)
            .IsRequired()
            .HasMaxLength(11);

        builder.Property(c => c.Telefone)
            .IsRequired()
            .HasMaxLength(20);


        builder.Property(c => c.CriadoEm)
            .IsRequired();

        builder.Property(c => c.AtualizadoEm);

        builder.Property(c => c.Excluido)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(c => c.ExcluidoEm);


        builder.HasIndex(c => c.Cpf)
            .IsUnique()
            .HasDatabaseName("IX_Clientes_Cpf");

        builder.HasIndex(c => c.Nome)
            .HasDatabaseName("IX_Clientes_Nome");
    }
}
