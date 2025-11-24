using ElectroHuila.Domain.Entities.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectroHuila.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework Core para la entidad SystemSetting.
/// </summary>
public class SystemSettingConfiguration : IEntityTypeConfiguration<SystemSetting>
{
    public void Configure(EntityTypeBuilder<SystemSetting> builder)
    {
        builder.HasKey(ss => ss.Id);

        builder.Property(ss => ss.Id).HasColumnName("ID");

        builder.Property(ss => ss.SettingKey).HasColumnName("SETTING_KEY")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(ss => ss.SettingValue).HasColumnName("SETTING_VALUE")
            .HasMaxLength(1000);

        builder.Property(ss => ss.SettingType).HasColumnName("SETTING_TYPE")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ss => ss.Description).HasColumnName("DESCRIPTION")
            .HasMaxLength(500);

        builder.Property(ss => ss.IsEncrypted).HasColumnName("IS_ENCRYPTED")
            .IsRequired();

        builder.Property(ss => ss.CreatedAt).HasColumnName("CREATED_AT")
            .IsRequired();

        builder.Property(ss => ss.UpdatedAt).HasColumnName("UPDATED_AT");

        builder.Property(ss => ss.IsActive).HasColumnName("IS_ACTIVE")
            .IsRequired();

        // Índice único en SettingKey
        builder.HasIndex(ss => ss.SettingKey)
            .IsUnique()
            .HasDatabaseName("UQ_SYSTEMSETTINGS_SETTING_KEY");

        builder.ToTable("SYSTEMSETTINGS");
    }
}
