using ElectroHuila.Domain.Entities.Catalogs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectroHuila.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework Core para ProjectType
/// </summary>
public class ProjectTypeConfiguration : IEntityTypeConfiguration<ProjectType>
{
    public void Configure(EntityTypeBuilder<ProjectType> builder)
    {
        builder.ToTable("PROJECTTYPES");

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

        builder.Property(x => x.IconName).HasColumnName("ICON_NAME")
            .HasMaxLength(100);

        builder.Property(x => x.ColorPrimary).HasColumnName("COLOR_PRIMARY")
            .HasMaxLength(20);

        builder.Property(x => x.DisplayOrder).HasColumnName("DISPLAY_ORDER")
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
