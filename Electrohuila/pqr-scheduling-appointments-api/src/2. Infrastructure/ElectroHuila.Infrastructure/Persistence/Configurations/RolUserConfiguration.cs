using ElectroHuila.Domain.Entities.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectroHuila.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework Core para la entidad RolUser (Rol-Usuario).
/// Define la relación muchos a muchos entre roles y usuarios.
/// Un usuario puede tener múltiples roles asignados.
/// </summary>
public class RolUserConfiguration : IEntityTypeConfiguration<RolUser>
{
    /// <summary>
    /// Configura la entidad RolUser con Fluent API.
    /// </summary>
    /// <param name="builder">Constructor de configuración de la entidad.</param>
    /// <remarks>
    /// Configuraciones aplicadas:
    /// - Clave primaria: ID
    /// - Relaciones: Rol (Cascade), User (Cascade)
    /// - Índice único compuesto: RolId + UserId
    /// - Tabla: ROLUSERS
    /// </remarks>
    public void Configure(EntityTypeBuilder<RolUser> builder)
    {
        builder.HasKey(ru => ru.Id);

        builder.Property(ru => ru.Id).HasColumnName("ID");
        builder.Property(ru => ru.RolId).HasColumnName("ROLE_ID");
        builder.Property(ru => ru.UserId).HasColumnName("USER_ID");

        builder.HasOne(ru => ru.Rol)
            .WithMany(r => r.RolUsers)
            .HasForeignKey(ru => ru.RolId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ru => ru.User)
            .WithMany(u => u.RolUsers)
            .HasForeignKey(ru => ru.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(ru => new { ru.RolId, ru.UserId })
            .IsUnique()
            .HasDatabaseName("IX_RolUsers_Rol_User");

        builder.ToTable("ROLUSERS");
    }
}