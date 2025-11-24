using ElectroHuila.Domain.Entities.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectroHuila.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework Core para la entidad RolFormPermi (Rol-Formulario-Permiso).
/// Define los permisos que tiene cada rol sobre cada formulario del sistema.
/// Esta es la tabla central del sistema de autorización basado en permisos.
/// </summary>
public class RolFormPermiConfiguration : IEntityTypeConfiguration<RolFormPermi>
{
    /// <summary>
    /// Configura la entidad RolFormPermi con Fluent API.
    /// </summary>
    /// <param name="builder">Constructor de configuración de la entidad.</param>
    /// <remarks>
    /// Configuraciones aplicadas:
    /// - Clave primaria: ID
    /// - Relaciones: Rol (Cascade), Form (Cascade), Permission (Cascade)
    /// - Índice único compuesto: RolId + FormId + PermissionId
    /// - Tabla: ROLFORMPERMIS
    /// </remarks>
    public void Configure(EntityTypeBuilder<RolFormPermi> builder)
    {
        builder.HasKey(rfp => rfp.Id);

        builder.Property(rfp => rfp.Id).HasColumnName("ID");
        builder.Property(rfp => rfp.RolId).HasColumnName("ROLE_ID");
        builder.Property(rfp => rfp.FormId).HasColumnName("FORM_ID");
        builder.Property(rfp => rfp.PermissionId).HasColumnName("PERMISSION_ID");

        builder.HasOne(rfp => rfp.Rol)
            .WithMany(r => r.RolFormPermis)
            .HasForeignKey(rfp => rfp.RolId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(rfp => rfp.Form)
            .WithMany(f => f.RolFormPermis)
            .HasForeignKey(rfp => rfp.FormId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(rfp => rfp.Permission)
            .WithMany(p => p.RolFormPermis)
            .HasForeignKey(rfp => rfp.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(rfp => new { rfp.RolId, rfp.FormId, rfp.PermissionId })
            .IsUnique()
            .HasDatabaseName("IX_RolFormPermis_Rol_Form_Permission");

        builder.ToTable("ROLFORMPERMIS");
    }
}