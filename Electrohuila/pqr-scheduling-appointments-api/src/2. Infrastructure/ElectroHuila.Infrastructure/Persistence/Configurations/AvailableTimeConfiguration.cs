using ElectroHuila.Domain.Entities.Appointments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectroHuila.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework Core para la entidad AvailableTime (Horario Disponible).
/// Define los horarios disponibles para agendamiento en cada sucursal y tipo de cita.
/// </summary>
public class AvailableTimeConfiguration : IEntityTypeConfiguration<AvailableTime>
{
    /// <summary>
    /// Configura la entidad AvailableTime con Fluent API.
    /// </summary>
    /// <param name="builder">Constructor de configuración de la entidad.</param>
    /// <remarks>
    /// Configuraciones aplicadas:
    /// - Clave primaria: ID
    /// - Propiedades: Hora, BranchId, AppointmentTypeId
    /// - Relaciones: Sucursal (Restrict), Tipo de cita (SetNull)
    /// - Índice único compuesto: BranchId + Time + AppointmentTypeId
    /// - Tabla: AVAILABLETIMES
    /// </remarks>
    public void Configure(EntityTypeBuilder<AvailableTime> builder)
    {
        builder.HasKey(at => at.Id);

        builder.Property(at => at.Id).HasColumnName("ID");

        builder.Property(at => at.Time).HasColumnName("TIME")
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(at => at.BranchId).HasColumnName("BRANCH_ID");
        builder.Property(at => at.AppointmentTypeId).HasColumnName("APPOINTMENT_TYPE_ID");

        builder.HasOne(at => at.Branch)
            .WithMany()
            .HasForeignKey(at => at.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(at => at.AppointmentType)
            .WithMany()
            .HasForeignKey(at => at.AppointmentTypeId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(at => new { at.BranchId, at.Time, at.AppointmentTypeId })
            .IsUnique()
            .HasDatabaseName("IX_AvailableTimes_Branch_Time_AppointmentType");

        builder.ToTable("AVAILABLETIMES");
    }
}