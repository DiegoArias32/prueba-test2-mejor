using ElectroHuila.Domain.Entities.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectroHuila.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework Core para la entidad Branch (Sucursal).
/// Define las sucursales/sedes de ElectroHuila donde se atienden clientes.
/// </summary>
public class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    /// <summary>
    /// Configura la entidad Branch con Fluent API.
    /// </summary>
    /// <param name="builder">Constructor de configuración de la entidad.</param>
    /// <remarks>
    /// Configuraciones aplicadas:
    /// - Clave primaria: ID
    /// - Propiedades: Nombre, código, dirección, teléfono, ciudad, estado, es principal, activo
    /// - Valor por defecto: IsMain=false
    /// - Índices: Code único, Name indexado
    /// - Tabla: BRANCHES
    /// </remarks>
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id).HasColumnName("ID");
        builder.Property(b => b.Name).HasColumnName("NAME").IsRequired().HasMaxLength(200);
        builder.Property(b => b.Code).HasColumnName("CODE").IsRequired().HasMaxLength(20);
        builder.Property(b => b.Address).HasColumnName("ADDRESS").IsRequired().HasMaxLength(500);
        builder.Property(b => b.Phone).HasColumnName("PHONE").IsRequired().HasMaxLength(20);
        builder.Property(b => b.City).HasColumnName("CITY").IsRequired().HasMaxLength(100);
        builder.Property(b => b.State).HasColumnName("STATE").IsRequired().HasMaxLength(100);
        builder.Property(b => b.IsMain).HasColumnName("IS_MAIN").IsRequired().HasDefaultValue(0);
        builder.Property(b => b.ColorPrimary).HasColumnName("COLOR_PRIMARY").HasMaxLength(20);
        builder.Property(b => b.IsActive).HasColumnName("IS_ACTIVE");
        builder.Property(b => b.CreatedAt).HasColumnName("CREATED_AT");
        builder.Property(b => b.UpdatedAt).HasColumnName("UPDATED_AT");

        builder.HasIndex(b => b.Code).IsUnique();
        builder.HasIndex(b => b.Name);

        builder.ToTable("BRANCHES");
    }
}