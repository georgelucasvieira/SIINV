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

        builder.Property(rt => rt.ExpiraEm)
            .IsRequired();

        builder.Property(rt => rt.Revogado)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(rt => rt.SubstituidoPor)
            .HasMaxLength(500);

        builder.Property(rt => rt.CriadoEm)
            .IsRequired();

        builder.HasIndex(rt => rt.UsuarioId);
        builder.HasIndex(rt => rt.ExpiraEm);
        builder.HasIndex(rt => new { rt.UsuarioId, rt.Revogado });
    }
}
