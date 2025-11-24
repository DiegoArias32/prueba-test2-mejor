using ElectroHuila.Domain.Entities.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectroHuila.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework Core para la entidad FormModule (Formulario-Módulo).
/// Define la relación muchos a muchos entre formularios y módulos.
/// </summary>
public class FormModuleConfiguration : IEntityTypeConfiguration<FormModule>
{
    /// <summary>
    /// Configura la entidad FormModule con Fluent API.
    /// </summary>
    /// <param name="builder">Constructor de configuración de la entidad.</param>
    /// <remarks>
    /// Configuraciones aplicadas:
    /// - Clave primaria: ID
    /// - Relaciones: Form (Cascade), Module (Cascade)
    /// - Índice único compuesto: FormId + ModuleId
    /// - Tabla: FORMMODULES
    /// </remarks>
    public void Configure(EntityTypeBuilder<FormModule> builder)
    {
        builder.HasKey(fm => fm.Id);

        builder.Property(fm => fm.Id).HasColumnName("ID");
        builder.Property(fm => fm.FormId).HasColumnName("FORM_ID");
        builder.Property(fm => fm.ModuleId).HasColumnName("MODULE_ID");

        builder.HasOne(fm => fm.Form)
            .WithMany(f => f.FormModules)
            .HasForeignKey(fm => fm.FormId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(fm => fm.Module)
            .WithMany(m => m.FormModules)
            .HasForeignKey(fm => fm.ModuleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(fm => new { fm.FormId, fm.ModuleId })
            .IsUnique()
            .HasDatabaseName("IX_FormModules_Form_Module");

        builder.ToTable("FORMMODULES");
    }
}