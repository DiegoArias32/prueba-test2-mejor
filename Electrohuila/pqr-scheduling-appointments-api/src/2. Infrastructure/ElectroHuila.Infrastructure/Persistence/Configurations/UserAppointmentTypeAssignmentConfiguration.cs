using ElectroHuila.Domain.Entities.Assignments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectroHuila.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework Core para la entidad UserAppointmentTypeAssignment.
/// Define el mapeo de la entidad a la tabla de Oracle y sus relaciones.
/// </summary>
public class UserAppointmentTypeAssignmentConfiguration : IEntityTypeConfiguration<UserAppointmentTypeAssignment>
{
    public void Configure(EntityTypeBuilder<UserAppointmentTypeAssignment> builder)
    {
        // Configuración de tabla
        builder.ToTable("USERAPPOINTMENTTYPEASSIGNMENTS", "ADMIN");

        // Clave primaria
        builder.HasKey(ua => ua.Id);
        builder.Property(ua => ua.Id)
            .HasColumnName("ID")
            .ValueGeneratedOnAdd();

        // Propiedades
        builder.Property(ua => ua.UserId)
            .HasColumnName("USER_ID")
            .IsRequired();

        builder.Property(ua => ua.AppointmentTypeId)
            .HasColumnName("APPOINTMENT_TYPE_ID")
            .IsRequired();

        // Propiedades heredadas de BaseEntity
        builder.Property(ua => ua.CreatedAt)
            .HasColumnName("CREATED_AT")
            .IsRequired();

        builder.Property(ua => ua.UpdatedAt)
            .HasColumnName("UPDATED_AT");

        builder.Property(ua => ua.IsActive)
            .HasColumnName("IS_ACTIVE")
            .HasColumnType("NUMBER(1)")
            .IsRequired()
            .HasDefaultValue(1);

        // Relaciones
        builder.HasOne(ua => ua.User)
            .WithMany()
            .HasForeignKey(ua => ua.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ua => ua.AppointmentType)
            .WithMany()
            .HasForeignKey(ua => ua.AppointmentTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índices
        builder.HasIndex(ua => ua.UserId)
            .HasDatabaseName("IDX_USER_ASSIGN_USER_ID");

        builder.HasIndex(ua => ua.AppointmentTypeId)
            .HasDatabaseName("IDX_USER_ASSIGN_APPT_TYPE_ID");

        builder.HasIndex(ua => ua.IsActive)
            .HasDatabaseName("IDX_USER_ASSIGN_ACTIVE");

        // Constraint único para evitar duplicados
        builder.HasIndex(ua => new { ua.UserId, ua.AppointmentTypeId })
            .HasDatabaseName("UK_USER_APPT_TYPE_ASSIGN")
            .IsUnique();
    }
}
