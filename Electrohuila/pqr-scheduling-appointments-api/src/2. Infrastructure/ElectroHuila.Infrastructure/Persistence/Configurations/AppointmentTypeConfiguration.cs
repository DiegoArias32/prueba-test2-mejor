using ElectroHuila.Domain.Entities.Appointments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectroHuila.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework Core para la entidad AppointmentType (Tipo de Cita).
/// Define los tipos de servicios que se pueden agendar (PQR, Solicitudes, etc).
/// </summary>
public class AppointmentTypeConfiguration : IEntityTypeConfiguration<AppointmentType>
{
    /// <summary>
    /// Configura la entidad AppointmentType con Fluent API.
    /// </summary>
    /// <param name="builder">Constructor de configuración de la entidad.</param>
    /// <remarks>
    /// Configuraciones aplicadas:
    /// - Clave primaria: ID
    /// - Propiedades: Nombre, descripción, icono, tiempo estimado, requiere documentación
    /// - Valores por defecto: EstimatedTimeMinutes=120, RequiresDocumentation=true
    /// - Índices: Name único
    /// - Tabla: APPOINTMENTTYPES
    /// </remarks>
    public void Configure(EntityTypeBuilder<AppointmentType> builder)
    {
        builder.HasKey(at => at.Id);

        builder.Property(at => at.Id).HasColumnName("ID");

        builder.Property(at => at.Code).HasColumnName("CODE")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(at => at.Name).HasColumnName("NAME")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(at => at.Description).HasColumnName("DESCRIPTION")
            .HasMaxLength(500);

        builder.Property(at => at.IconName).HasColumnName("ICON_NAME")
            .HasMaxLength(100);

        builder.Property(at => at.ColorPrimary).HasColumnName("COLOR_PRIMARY")
            .HasMaxLength(20);

        builder.Property(at => at.ColorSecondary).HasColumnName("COLOR_SECONDARY")
            .HasMaxLength(20);

        builder.Property(at => at.EstimatedTimeMinutes).HasColumnName("ESTIMATED_TIME_MINUTES")
            .IsRequired()
            .HasDefaultValue(120);

        builder.Property(at => at.RequiresDocumentation).HasColumnName("REQUIRES_DOCUMENTATION")
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(at => at.DisplayOrder).HasColumnName("DISPLAY_ORDER")
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(at => at.IsActive).HasColumnName("IS_ACTIVE");
        builder.Property(at => at.CreatedAt).HasColumnName("CREATED_AT");
        builder.Property(at => at.UpdatedAt).HasColumnName("UPDATED_AT");

        builder.HasIndex(at => at.Code)
            .IsUnique();

        builder.HasIndex(at => at.Name)
            .IsUnique();

        builder.ToTable("APPOINTMENTTYPES");
    }
}