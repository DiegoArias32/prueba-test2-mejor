using ElectroHuila.Domain.Entities.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectroHuila.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework Core para la entidad Module (Módulo).
/// Define los módulos del sistema que agrupan formularios relacionados.
/// </summary>
public class ModuleConfiguration : IEntityTypeConfiguration<Module>
{
    /// <summary>
    /// Configura la entidad Module con Fluent API.
    /// </summary>
    /// <param name="builder">Constructor de configuración de la entidad.</param>
    /// <remarks>
    /// Configuraciones aplicadas:
    /// - Clave primaria: ID
    /// - Propiedades: Nombre, código
    /// - Índices únicos: Name, Code
    /// - Tabla: MODULES
    /// </remarks>
    public void Configure(EntityTypeBuilder<Module> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id).HasColumnName("ID");

        builder.Property(m => m.Name).HasColumnName("NAME")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(m => m.Code).HasColumnName("CODE")
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(m => m.Name)
            .IsUnique();

        builder.HasIndex(m => m.Code)
            .IsUnique();

        builder.ToTable("MODULES");
    }
}