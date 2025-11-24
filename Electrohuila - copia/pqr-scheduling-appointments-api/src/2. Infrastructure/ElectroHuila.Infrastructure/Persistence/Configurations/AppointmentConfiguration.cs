using ElectroHuila.Domain.Entities.Appointments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectroHuila.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework Core para la entidad Appointment (Cita).
/// Define el mapeo de propiedades, relaciones y restricciones en la base de datos Oracle.
/// </summary>
public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    /// <summary>
    /// Configura la entidad Appointment con Fluent API.
    /// </summary>
    /// <param name="builder">Constructor de configuración de la entidad.</param>
    /// <remarks>
    /// Configuraciones aplicadas:
    /// - Clave primaria: ID
    /// - Propiedades: Número de cita, fecha, hora, estado, notas
    /// - Relaciones: Cliente, Sucursal, Tipo de cita (todas con DeleteBehavior.Restrict)
    /// - Índices: AppointmentNumber único
    /// - Tabla: APPOINTMENTS
    /// </remarks>
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id).HasColumnName("ID");

        builder.Property(a => a.AppointmentNumber).HasColumnName("APPOINTMENT_NUMBER")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(a => a.AppointmentDate).HasColumnName("APPOINTMENT_DATE")
            .IsRequired();

        builder.Property(a => a.AppointmentTime).HasColumnName("APPOINTMENT_TIME")
            .HasMaxLength(20);

        builder.Property(a => a.StatusId).HasColumnName("STATUS_ID")
            .IsRequired();

        builder.Property(a => a.Notes).HasColumnName("NOTES")
            .HasMaxLength(1000);

        builder.Property(a => a.CancellationReason).HasColumnName("CANCELLATION_REASON")
            .HasMaxLength(500);

        builder.Property(a => a.CompletedDate).HasColumnName("COMPLETED_DATE");

        builder.Property(a => a.ClientId).HasColumnName("CLIENT_ID");
        builder.Property(a => a.BranchId).HasColumnName("BRANCH_ID");
        builder.Property(a => a.AppointmentTypeId).HasColumnName("APPOINTMENT_TYPE_ID");

        builder.Property(a => a.IsEnabled).HasColumnName("IS_ENABLED")
            .IsRequired()
            .HasDefaultValue(1);

        builder.HasOne(a => a.Client)
            .WithMany(c => c.Appointments)
            .HasForeignKey(a => a.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Branch)
            .WithMany(b => b.Appointments)
            .HasForeignKey(a => a.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.AppointmentType)
            .WithMany(at => at.Appointments)
            .HasForeignKey(a => a.AppointmentTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Status)
            .WithMany(s => s.Appointments)
            .HasForeignKey(a => a.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(a => a.AppointmentNumber)
            .IsUnique();

        builder.ToTable("APPOINTMENTS");
    }
}