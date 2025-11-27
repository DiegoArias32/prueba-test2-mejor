using ElectroHuila.Domain.Entities.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectroHuila.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework Core para la entidad ThemeSettings.
/// Define la configuración de temas y colores de la aplicación.
/// </summary>
public class ThemeSettingsConfiguration : IEntityTypeConfiguration<ThemeSettings>
{
    /// <summary>
    /// Configura la entidad ThemeSettings con Fluent API.
    /// </summary>
    /// <param name="builder">Constructor de configuración de la entidad.</param>
    public void Configure(EntityTypeBuilder<ThemeSettings> builder)
    {
        builder.HasKey(ts => ts.Id);

        builder.Property(ts => ts.Id).HasColumnName("ID");

        builder.Property(ts => ts.Name).HasColumnName("NAME")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(ts => ts.Description).HasColumnName("DESCRIPTION")
            .HasMaxLength(500);

        // Colores principales
        builder.Property(ts => ts.ColorPrimary).HasColumnName("COLOR_PRIMARY")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ts => ts.ColorSecondary).HasColumnName("COLOR_SECONDARY")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ts => ts.ColorAccent).HasColumnName("COLOR_ACCENT")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ts => ts.ColorIntermediate).HasColumnName("COLOR_INTERMEDIATE")
            .IsRequired()
            .HasMaxLength(50);

        // Colores de estado
        builder.Property(ts => ts.ColorSuccess).HasColumnName("COLOR_SUCCESS")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ts => ts.ColorError).HasColumnName("COLOR_ERROR")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ts => ts.ColorWarning).HasColumnName("COLOR_WARNING")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ts => ts.ColorInfo).HasColumnName("COLOR_INFO")
            .IsRequired()
            .HasMaxLength(50);

        // Colores de fondo y texto
        builder.Property(ts => ts.BackgroundPrimary).HasColumnName("BG_PRIMARY")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ts => ts.BackgroundSecondary).HasColumnName("BG_SECONDARY")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ts => ts.TextPrimary).HasColumnName("TEXT_PRIMARY")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ts => ts.TextSecondary).HasColumnName("TEXT_SECONDARY")
            .IsRequired()
            .HasMaxLength(50);

        // Configuración de scrollbar
        builder.Property(ts => ts.ScrollbarGradientStart).HasColumnName("SCROLLBAR_GRADIENT_START")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ts => ts.ScrollbarGradientEnd).HasColumnName("SCROLLBAR_GRADIENT_END")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ts => ts.ScrollbarHoverStart).HasColumnName("SCROLLBAR_HOVER_START")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ts => ts.ScrollbarHoverEnd).HasColumnName("SCROLLBAR_HOVER_END")
            .IsRequired()
            .HasMaxLength(50);

        // Configuración
        builder.Property(ts => ts.IsDefaultTheme).HasColumnName("IS_DEFAULT_THEME")
            .IsRequired();

        // Índice para tema por defecto
        builder.HasIndex(ts => ts.IsDefaultTheme);

        builder.ToTable("THEMESETTINGS");
    }
}
