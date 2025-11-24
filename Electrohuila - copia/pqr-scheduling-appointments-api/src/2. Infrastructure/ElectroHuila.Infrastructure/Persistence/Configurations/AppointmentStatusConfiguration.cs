using ElectroHuila.Domain.Entities.Catalogs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectroHuila.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework Core para AppointmentStatus
/// </summary>
public class AppointmentStatusConfiguration : IEntityTypeConfiguration<AppointmentStatus>
{
    public void Configure(EntityTypeBuilder<AppointmentStatus> builder)
    {
        builder.ToTable("APPOINTMENTSTATUSES");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("ID");

        builder.Property(x => x.Code).HasColumnName("CODE")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Name).HasColumnName("NAME")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Description).HasColumnName("DESCRIPTION")
            .HasMaxLength(500);

        builder.Property(x => x.ColorPrimary).HasColumnName("COLOR_PRIMARY")
            .HasMaxLength(20);

        builder.Property(x => x.ColorSecondary).HasColumnName("COLOR_SECONDARY")
            .HasMaxLength(20);

        builder.Property(x => x.ColorText).HasColumnName("COLOR_TEXT")
            .HasMaxLength(20);

        builder.Property(x => x.IconName).HasColumnName("ICON_NAME")
            .HasMaxLength(100);

        builder.Property(x => x.DisplayOrder).HasColumnName("DISPLAY_ORDER")
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.AllowCancellation).HasColumnName("ALLOW_CANCELLATION")
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(x => x.IsFinalState).HasColumnName("IS_FINAL_STATE")
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.IsActive)
            .HasColumnName("IS_ACTIVE")
            .IsRequired();
        builder.Property(x => x.CreatedAt)
            .HasColumnName("CREATED_AT")
            .IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("UPDATED_AT");

        builder.HasIndex(x => x.Code).IsUnique();
    }
}
