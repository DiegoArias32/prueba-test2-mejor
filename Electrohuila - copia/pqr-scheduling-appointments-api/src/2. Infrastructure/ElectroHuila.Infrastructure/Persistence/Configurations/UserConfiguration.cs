using ElectroHuila.Domain.Entities.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectroHuila.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework Core para la entidad User (Usuario).
/// Define los usuarios del sistema que pueden autenticarse y usar la aplicación.
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    /// <summary>
    /// Configura la entidad User con Fluent API.
    /// </summary>
    /// <param name="builder">Constructor de configuración de la entidad.</param>
    /// <remarks>
    /// Configuraciones aplicadas:
    /// - Clave primaria: ID
    /// - Propiedades: Username, email, password (hasheado), tabs permitidas
    /// - Índices únicos: Username, Email
    /// - Tabla: USERS
    /// </remarks>
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id).HasColumnName("ID");

        builder.Property(u => u.Username).HasColumnName("USERNAME")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.Email).HasColumnName("EMAIL")
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.Password).HasColumnName("PASSWORD")
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(u => u.AllowedTabs).HasColumnName("ALLOWED_TABS")
            .HasMaxLength(1000);

        builder.HasIndex(u => u.Username)
            .IsUnique();

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.ToTable("USERS");
    }
}