using ElectroHuila.Domain.Entities.Clients;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectroHuila.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework Core para la entidad Client (Cliente).
/// Define los clientes que agendan citas en el sistema.
/// </summary>
public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    /// <summary>
    /// Configura la entidad Client con Fluent API.
    /// </summary>
    /// <param name="builder">Constructor de configuración de la entidad.</param>
    /// <remarks>
    /// Configuraciones aplicadas:
    /// - Clave primaria: ID
    /// - Propiedades: Número de cliente, tipo/número documento, nombre, email, teléfonos, dirección
    /// - Índices únicos: ClientNumber, DocumentNumber, Email
    /// - Tabla: CLIENTS
    /// </remarks>
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id).HasColumnName("ID");

        builder.Property(c => c.ClientNumber).HasColumnName("CLIENT_NUMBER")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.DocumentType).HasColumnName("DOCUMENT_TYPE")
            .IsRequired()
            .HasConversion<int>();

        builder.Property(c => c.DocumentNumber).HasColumnName("DOCUMENT_NUMBER")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.FullName).HasColumnName("FULL_NAME")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Email).HasColumnName("EMAIL")
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(c => c.Phone).HasColumnName("PHONE")
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(c => c.Mobile).HasColumnName("MOBILE")
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(c => c.Address).HasColumnName("ADDRESS")
            .IsRequired()
            .HasMaxLength(500);

        builder.HasIndex(c => c.ClientNumber)
            .IsUnique();

        builder.HasIndex(c => c.DocumentNumber)
            .IsUnique();

        builder.HasIndex(c => c.Email)
            .IsUnique();

        builder.ToTable("CLIENTS");
    }
}