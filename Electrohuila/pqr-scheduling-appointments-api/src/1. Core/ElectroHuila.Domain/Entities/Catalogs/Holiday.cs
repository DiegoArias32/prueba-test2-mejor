using ElectroHuila.Domain.Entities.Common;
using ElectroHuila.Domain.Entities.Locations;

namespace ElectroHuila.Domain.Entities.Catalogs;

/// <summary>
/// Festivo o día no laborable
/// Previene que clientes agenden citas en días festivos
/// </summary>
public class Holiday : BaseEntity
{
    /// <summary>
    /// Fecha del festivo
    /// </summary>
    public DateTime HolidayDate { get; private set; }

    /// <summary>
    /// Nombre del festivo
    /// Ejemplo: "Día de la Independencia", "Navidad"
    /// </summary>
    public string HolidayName { get; private set; } = string.Empty;

    /// <summary>
    /// Tipo de festivo
    /// Valores: NATIONAL, LOCAL, COMPANY
    /// </summary>
    public string HolidayType { get; private set; } = "NATIONAL";

    /// <summary>
    /// ID de la sucursal (NULL = aplica a todas las sucursales)
    /// Solo para festivos locales
    /// </summary>
    public int? BranchId { get; private set; }

    // Navigation property
    public virtual Branch? Branch { get; private set; }

    private Holiday() { } // Para EF Core

    public static Holiday CreateNationalHoliday(DateTime holidayDate, string holidayName)
    {
        if (string.IsNullOrWhiteSpace(holidayName))
            throw new ArgumentException("Holiday name cannot be null or empty", nameof(holidayName));

        return new Holiday
        {
            HolidayDate = holidayDate.Date, // Solo la fecha, sin hora
            HolidayName = holidayName,
            HolidayType = "NATIONAL",
            BranchId = null,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static Holiday CreateLocalHoliday(DateTime holidayDate, string holidayName, int branchId)
    {
        if (string.IsNullOrWhiteSpace(holidayName))
            throw new ArgumentException("Holiday name cannot be null or empty", nameof(holidayName));

        if (branchId <= 0)
            throw new ArgumentException("Branch ID must be greater than zero", nameof(branchId));

        return new Holiday
        {
            HolidayDate = holidayDate.Date,
            HolidayName = holidayName,
            HolidayType = "LOCAL",
            BranchId = branchId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static Holiday CreateCompanyHoliday(DateTime holidayDate, string holidayName)
    {
        if (string.IsNullOrWhiteSpace(holidayName))
            throw new ArgumentException("Holiday name cannot be null or empty", nameof(holidayName));

        return new Holiday
        {
            HolidayDate = holidayDate.Date,
            HolidayName = holidayName,
            HolidayType = "COMPANY",
            BranchId = null,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdateDetails(string holidayName, string? holidayType = null)
    {
        if (!string.IsNullOrWhiteSpace(holidayName))
            HolidayName = holidayName;

        if (!string.IsNullOrWhiteSpace(holidayType))
        {
            if (!IsValidHolidayType(holidayType))
                throw new ArgumentException($"Invalid holiday type: {holidayType}", nameof(holidayType));

            HolidayType = holidayType.ToUpperInvariant();
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDate(DateTime newDate)
    {
        HolidayDate = newDate.Date;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Verifica si una fecha es este festivo
    /// </summary>
    public bool IsHolidayDate(DateTime date)
    {
        return HolidayDate.Date == date.Date;
    }

    /// <summary>
    /// Verifica si este festivo aplica a una sucursal específica
    /// </summary>
    public bool AppliesToBranch(int? branchId)
    {
        // Festivos nacionales o de empresa aplican a todas las sucursales
        if (BranchId == null)
            return true;

        // Festivos locales solo aplican a su sucursal específica
        return BranchId == branchId;
    }

    private static bool IsValidHolidayType(string type)
    {
        var validTypes = new[] { "NATIONAL", "LOCAL", "COMPANY" };
        return validTypes.Contains(type.ToUpperInvariant());
    }
}
