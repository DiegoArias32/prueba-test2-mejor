using ElectroHuila.Domain.Entities.Appointments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectroHuila.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework Core para la entidad AppointmentDocument.
/// </summary>
public class AppointmentDocumentConfiguration : IEntityTypeConfiguration<AppointmentDocument>
{
    public void Configure(EntityTypeBuilder<AppointmentDocument> builder)
    {
        builder.HasKey(ad => ad.Id);

        builder.Property(ad => ad.Id).HasColumnName("ID");

        builder.Property(ad => ad.AppointmentId).HasColumnName("APPOINTMENT_ID")
            .IsRequired();

        builder.Property(ad => ad.DocumentName).HasColumnName("DOCUMENT_NAME")
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(ad => ad.DocumentType).HasColumnName("DOCUMENT_TYPE")
            .HasMaxLength(100);

        builder.Property(ad => ad.FilePath).HasColumnName("FILE_PATH")
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(ad => ad.FileSize).HasColumnName("FILE_SIZE");

        builder.Property(ad => ad.UploadedBy).HasColumnName("UPLOADED_BY");

        builder.Property(ad => ad.Description).HasColumnName("DESCRIPTION")
            .HasMaxLength(1000);

        builder.Property(ad => ad.CreatedAt).HasColumnName("CREATED_AT")
            .IsRequired();

        builder.Property(ad => ad.UpdatedAt).HasColumnName("UPDATED_AT");

        builder.Property(ad => ad.IsActive).HasColumnName("IS_ACTIVE")
            .IsRequired();

        // Relación con Appointment (CASCADE DELETE)
        builder.HasOne(ad => ad.Appointment)
            .WithMany()
            .HasForeignKey(ad => ad.AppointmentId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_APPTDOCS_APPOINTMENT");

        // Relación con User (opcional)
        builder.HasOne(ad => ad.UploadedByUser)
            .WithMany()
            .HasForeignKey(ad => ad.UploadedBy)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_APPTDOCS_USER");

        // Índice en AppointmentId para búsquedas rápidas
        builder.HasIndex(ad => ad.AppointmentId)
            .HasDatabaseName("IDX_APPTDOCS_APPOINTMENT");

        builder.ToTable("APPOINTMENTDOCUMENTS");
    }
}
