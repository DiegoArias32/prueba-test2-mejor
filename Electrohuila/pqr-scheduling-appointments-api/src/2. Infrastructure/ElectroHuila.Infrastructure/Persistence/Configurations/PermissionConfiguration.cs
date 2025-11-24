using ElectroHuila.Domain.Entities.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectroHuila.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework Core para la entidad Permission (Permiso).
/// Define los permisos CRUD (Create, Read, Update, Delete) del sistema.
/// </summary>
public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    /// <summary>
    /// Configura la entidad Permission con Fluent API.
    /// </summary>
    /// <param name="builder">Constructor de configuración de la entidad.</param>
    /// <remarks>
    /// Configuraciones aplicadas:
    /// - Clave primaria: ID
    /// - Propiedades booleanas: CanRead, CanCreate, CanUpdate, CanDelete
    /// - Valores por defecto: Todos los permisos en false
    /// - Tabla: PERMISSIONS
    /// </remarks>
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id).HasColumnName("ID");

        builder.Property(p => p.CanRead).HasColumnName("CAN_READ")
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(p => p.CanCreate).HasColumnName("CAN_CREATE")
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(p => p.CanUpdate).HasColumnName("CAN_UPDATE")
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(p => p.CanDelete).HasColumnName("CAN_DELETE")
            .IsRequired()
            .HasDefaultValue(0);

        builder.ToTable("PERMISSIONS");
    }
}