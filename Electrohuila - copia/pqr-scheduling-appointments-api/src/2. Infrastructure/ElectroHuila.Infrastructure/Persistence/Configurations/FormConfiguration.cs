using ElectroHuila.Domain.Entities.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectroHuila.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework Core para la entidad Form (Formulario).
/// Define los formularios/pantallas del sistema para control de permisos.
/// </summary>
public class FormConfiguration : IEntityTypeConfiguration<Form>
{
    /// <summary>
    /// Configura la entidad Form con Fluent API.
    /// </summary>
    /// <param name="builder">Constructor de configuración de la entidad.</param>
    /// <remarks>
    /// Configuraciones aplicadas:
    /// - Clave primaria: ID
    /// - Propiedades: Nombre, código
    /// - Índices únicos: Name, Code
    /// - Tabla: FORMS
    /// </remarks>
    public void Configure(EntityTypeBuilder<Form> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Id).HasColumnName("ID");

        builder.Property(f => f.Name).HasColumnName("NAME")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(f => f.Code).HasColumnName("CODE")
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(f => f.Name)
            .IsUnique();

        builder.HasIndex(f => f.Code)
            .IsUnique();

        builder.ToTable("FORMS");
    }
}