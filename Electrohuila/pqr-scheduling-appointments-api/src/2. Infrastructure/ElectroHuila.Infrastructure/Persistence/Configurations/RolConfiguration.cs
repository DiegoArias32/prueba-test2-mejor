using ElectroHuila.Domain.Entities.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectroHuila.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework Core para la entidad Rol (Rol de Usuario).
/// Define los roles del sistema (Admin, Operador, Supervisor, etc).
/// </summary>
public class RolConfiguration : IEntityTypeConfiguration<Rol>
{
    /// <summary>
    /// Configura la entidad Rol con Fluent API.
    /// </summary>
    /// <param name="builder">Constructor de configuración de la entidad.</param>
    /// <remarks>
    /// Configuraciones aplicadas:
    /// - Clave primaria: ID
    /// - Propiedades: Nombre, código
    /// - Índices únicos: Name, Code
    /// - Tabla: ROLES
    /// </remarks>
    public void Configure(EntityTypeBuilder<Rol> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id).HasColumnName("ID");

        builder.Property(r => r.Name).HasColumnName("NAME")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.Code).HasColumnName("CODE")
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(r => r.Name)
            .IsUnique();

        builder.HasIndex(r => r.Code)
            .IsUnique();

        builder.ToTable("ROLES");
    }
}