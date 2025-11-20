using AuthService.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Api.Data.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.HasKey(rt => rt.Id);

        builder.Property(rt => rt.Id)
            .ValueGeneratedOnAdd();

        builder.Property(rt => rt.Token)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasIndex(rt => rt.Token)
            .IsUnique();

        builder.Property(rt => rt.UsuarioId)
            .IsRequired();

        // Índice único para garantir 1 refresh token por usuário
        builder.HasIndex(rt => rt.UsuarioId)
            .IsUnique();

        builder.Property(rt => rt.ExpiraEm)
            .IsRequired();

        builder.Property(rt => rt.CriadoEm)
            .IsRequired();
    }
}
