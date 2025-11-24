using ElectroHuila.Domain.Entities.Catalogs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectroHuila.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework Core para la entidad Holiday.
/// </summary>
public class HolidayConfiguration : IEntityTypeConfiguration<Holiday>
{
    public void Configure(EntityTypeBuilder<Holiday> builder)
    {
        builder.HasKey(h => h.Id);

        builder.Property(h => h.Id).HasColumnName("ID");

        builder.Property(h => h.HolidayDate).HasColumnName("HOLIDAY_DATE")
            .IsRequired()
            .HasColumnType("DATE");

        builder.Property(h => h.HolidayName).HasColumnName("HOLIDAY_NAME")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(h => h.HolidayType).HasColumnName("HOLIDAY_TYPE")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(h => h.BranchId).HasColumnName("BRANCH_ID");

        builder.Property(h => h.CreatedAt).HasColumnName("CREATED_AT")
            .IsRequired();

        builder.Property(h => h.UpdatedAt).HasColumnName("UPDATED_AT");

        builder.Property(h => h.IsActive).HasColumnName("IS_ACTIVE")
            .IsRequired();

        // Relación con Branch (opcional)
        builder.HasOne(h => h.Branch)
            .WithMany()
            .HasForeignKey(h => h.BranchId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_HOLIDAYS_BRANCH");

        // Índice en fecha para búsquedas rápidas
        builder.HasIndex(h => h.HolidayDate)
            .HasDatabaseName("IDX_HOLIDAYS_DATE");

        // Índice en BranchId
        builder.HasIndex(h => h.BranchId)
            .HasDatabaseName("IDX_HOLIDAYS_BRANCH");

        builder.ToTable("HOLIDAYS");
    }
}
