using ElectroHuila.Domain.Entities.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectroHuila.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework Core para la entidad NotificationTemplate.
/// </summary>
public class NotificationTemplateConfiguration : IEntityTypeConfiguration<NotificationTemplate>
{
    public void Configure(EntityTypeBuilder<NotificationTemplate> builder)
    {
        builder.HasKey(nt => nt.Id);

        builder.Property(nt => nt.Id).HasColumnName("ID");

        builder.Property(nt => nt.TemplateCode).HasColumnName("TEMPLATE_CODE")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(nt => nt.TemplateName).HasColumnName("TEMPLATE_NAME")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(nt => nt.Subject).HasColumnName("SUBJECT")
            .HasMaxLength(500);

        builder.Property(nt => nt.BodyTemplate).HasColumnName("BODY_TEMPLATE")
            .IsRequired()
            .HasColumnType("CLOB"); // Oracle CLOB para textos largos

        builder.Property(nt => nt.TemplateType).HasColumnName("TEMPLATE_TYPE")
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(nt => nt.Placeholders).HasColumnName("PLACEHOLDERS")
            .HasMaxLength(1000);

        builder.Property(nt => nt.CreatedAt).HasColumnName("CREATED_AT")
            .IsRequired();

        builder.Property(nt => nt.UpdatedAt).HasColumnName("UPDATED_AT");

        builder.Property(nt => nt.IsActive).HasColumnName("IS_ACTIVE")
            .IsRequired();

        // Índice único en TemplateCode
        builder.HasIndex(nt => nt.TemplateCode)
            .IsUnique()
            .HasDatabaseName("UQ_NOTIFTEMPLATES_TEMPLATE_CODE");

        builder.ToTable("NOTIFICATIONTEMPLATES");
    }
}
