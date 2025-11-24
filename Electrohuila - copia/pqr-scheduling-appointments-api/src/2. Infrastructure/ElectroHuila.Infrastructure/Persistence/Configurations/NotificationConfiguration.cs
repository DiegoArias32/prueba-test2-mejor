using ElectroHuila.Domain.Entities.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectroHuila.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework Core configuration for the Notification entity.
/// Defines property mappings, relationships, and constraints for the NOTIFICATIONS table.
/// </summary>
public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    /// <summary>
    /// Configures the Notification entity using Fluent API.
    /// </summary>
    /// <param name="builder">Entity type builder for configuration.</param>
    /// <remarks>
    /// Configurations applied:
    /// - Primary key: ID
    /// - Properties: Type, Title, Message, Status, etc.
    /// - Relationships: User (required), Appointment (optional) with DeleteBehavior.Restrict
    /// - Indexes: UserId, Status, IsRead, SentAt for query optimization
    /// - Table name: NOTIFICATIONS
    /// </remarks>
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        // Primary key
        builder.HasKey(n => n.Id);

        builder.Property(n => n.Id)
            .HasColumnName("ID");

        // User relationship (optional - nullable porque puede ser para Client)
        builder.Property(n => n.UserId)
            .HasColumnName("USER_ID")
            .IsRequired(false);

        // Client relationship (optional - nullable porque puede ser para User)
        builder.Property(n => n.ClientId)
            .HasColumnName("CLIENT_ID")
            .IsRequired(false);

        // Appointment relationship (optional)
        builder.Property(n => n.AppointmentId)
            .HasColumnName("APPOINTMENT_ID");

        // Notification type: EMAIL, SMS, WHATSAPP, IN_APP
        builder.Property(n => n.Type)
            .HasColumnName("TYPE")
            .IsRequired()
            .HasMaxLength(20);

        // Title/Subject
        builder.Property(n => n.Title)
            .HasColumnName("TITLE")
            .IsRequired()
            .HasMaxLength(200);

        // Message content
        builder.Property(n => n.Message)
            .HasColumnName("MESSAGE")
            .IsRequired()
            .HasMaxLength(2000);

        // Status: PENDING, SENT, FAILED
        builder.Property(n => n.Status)
            .HasColumnName("STATUS")
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue("PENDING");

        // Sent timestamp
        builder.Property(n => n.SentAt)
            .HasColumnName("SENT_AT");

        // Read timestamp (for IN_APP notifications)
        builder.Property(n => n.ReadAt)
            .HasColumnName("READ_AT");

        // Read flag (for IN_APP notifications)
        // Oracle stores booleans as NUMBER(1): 0 = false, 1 = true
        builder.Property(n => n.IsRead)
            .HasColumnName("IS_READ")
            .IsRequired()
            .HasDefaultValue(0);

        // Error message if sending failed
        builder.Property(n => n.ErrorMessage)
            .HasColumnName("ERROR_MESSAGE")
            .HasMaxLength(1000);

        // Additional metadata in JSON format (CLOB in Oracle)
        builder.Property(n => n.Metadata)
            .HasColumnName("METADATA");
            // No MaxLength para CLOB - soporta JSON grandes

        // Navigation properties and relationships
        builder.HasOne(n => n.User)
            .WithMany()
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.HasOne(n => n.Client)
            .WithMany()
            .HasForeignKey(n => n.ClientId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.HasOne(n => n.Appointment)
            .WithMany()
            .HasForeignKey(n => n.AppointmentId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        // CHECK constraint: debe tener USER_ID O CLIENT_ID, pero no ambos ni ninguno
        // NOTA: El constraint se crea directamente en la BD con el script SQL
        // NO usar HasCheckConstraint porque Oracle EF tiene problemas con constraints complejos
        // builder.HasCheckConstraint(
        //     "CHK_NOTIFICATION_RECIPIENT",
        //     "((USER_ID IS NOT NULL AND CLIENT_ID IS NULL) OR (USER_ID IS NULL AND CLIENT_ID IS NOT NULL))");

        // Indexes for query optimization
        builder.HasIndex(n => n.UserId)
            .HasDatabaseName("IX_NOTIFICATIONS_USER_ID");

        builder.HasIndex(n => n.ClientId)
            .HasDatabaseName("IX_NOTIFICATIONS_CLIENT_ID");

        builder.HasIndex(n => n.AppointmentId)
            .HasDatabaseName("IX_NOTIFICATIONS_APPOINTMENT_ID");

        builder.HasIndex(n => n.Status)
            .HasDatabaseName("IX_NOTIFICATIONS_STATUS");

        builder.HasIndex(n => new { n.UserId, n.IsRead })
            .HasDatabaseName("IX_NOTIFICATIONS_USER_ISREAD");

        builder.HasIndex(n => new { n.ClientId, n.CreatedAt })
            .HasDatabaseName("IX_NOTIFICATIONS_CLIENT_CREATED");

        builder.HasIndex(n => n.SentAt)
            .HasDatabaseName("IX_NOTIFICATIONS_SENT_AT");

        builder.HasIndex(n => new { n.UserId, n.CreatedAt })
            .HasDatabaseName("IX_NOTIFICATIONS_USER_CREATED");

        // Table name
        builder.ToTable("NOTIFICATIONS");
    }
}
