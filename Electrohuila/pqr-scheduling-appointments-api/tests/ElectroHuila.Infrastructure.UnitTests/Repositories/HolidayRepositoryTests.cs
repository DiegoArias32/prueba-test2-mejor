using ElectroHuila.Domain.Entities.Catalogs;
using ElectroHuila.Domain.Entities.Locations;
using ElectroHuila.Infrastructure.Persistence;
using ElectroHuila.Infrastructure.Persistence.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Xunit;

namespace ElectroHuila.Infrastructure.UnitTests.Repositories;

/// <summary>
/// Pruebas unitarias completas para el repositorio de festivos (HolidayRepository).
/// Cubre operaciones CRUD, consultas por fecha, tipo de festivo, sucursal y validaciones.
/// </summary>
public class HolidayRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly HolidayRepository _repository;

    public HolidayRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new HolidayRepository(_context);
    }

    #region IsHolidayAsync Tests

    /// <summary>
    /// Verifica que IsHolidayAsync retorna true cuando la fecha es festivo nacional.
    /// </summary>
    [Fact]
    public async Task IsHolidayAsync_WithNationalHoliday_ReturnsTrue()
    {
        // Arrange
        var holidayDate = new DateTime(2025, 12, 25);
        var holiday = Holiday.CreateNationalHoliday(holidayDate, "Navidad");
        await _context.Holidays.AddAsync(holiday);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.IsHolidayAsync(holidayDate);

        // Assert
        result.Should().BeTrue();
    }

    /// <summary>
    /// Verifica que IsHolidayAsync retorna true cuando la fecha es festivo local de la sucursal.
    /// </summary>
    [Fact]
    public async Task IsHolidayAsync_WithLocalHolidayForBranch_ReturnsTrue()
    {
        // Arrange
        var branch = CreateTestBranch();
        await _context.Branches.AddAsync(branch);
        await _context.SaveChangesAsync();

        var holidayDate = new DateTime(2025, 6, 29);
        var holiday = Holiday.CreateLocalHoliday(holidayDate, "Día de San Pedro", branch.Id);
        await _context.Holidays.AddAsync(holiday);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.IsHolidayAsync(holidayDate, branch.Id);

        // Assert
        result.Should().BeTrue();
    }

    /// <summary>
    /// Verifica que IsHolidayAsync retorna false cuando la fecha no es festivo.
    /// </summary>
    [Fact]
    public async Task IsHolidayAsync_WithNonHolidayDate_ReturnsFalse()
    {
        // Arrange
        var normalDate = new DateTime(2025, 3, 15);

        // Act
        var result = await _repository.IsHolidayAsync(normalDate);

        // Assert
        result.Should().BeFalse();
    }

    /// <summary>
    /// Verifica que IsHolidayAsync retorna false cuando el festivo está inactivo.
    /// </summary>
    [Fact]
    public async Task IsHolidayAsync_WithInactiveHoliday_ReturnsFalse()
    {
        // Arrange
        var holidayDate = new DateTime(2025, 12, 25);
        var holiday = Holiday.CreateNationalHoliday(holidayDate, "Navidad");
        holiday.IsActive = false;
        await _context.Holidays.AddAsync(holiday);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.IsHolidayAsync(holidayDate);

        // Assert
        result.Should().BeFalse();
    }

    /// <summary>
    /// Verifica que IsHolidayAsync ignora la hora y solo compara la fecha.
    /// </summary>
    [Fact]
    public async Task IsHolidayAsync_IgnoresTimeComponent()
    {
        // Arrange
        var holidayDate = new DateTime(2025, 12, 25);
        var holiday = Holiday.CreateNationalHoliday(holidayDate, "Navidad");
        await _context.Holidays.AddAsync(holiday);
        await _context.SaveChangesAsync();

        // Act
        var dateWithTime = new DateTime(2025, 12, 25, 14, 30, 0);
        var result = await _repository.IsHolidayAsync(dateWithTime);

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region GetByDateRangeAsync Tests

    /// <summary>
    /// Verifica que GetByDateRangeAsync retorna festivos dentro del rango especificado.
    /// </summary>
    [Fact]
    public async Task GetByDateRangeAsync_ReturnsHolidaysInRange()
    {
        // Arrange
        var holiday1 = Holiday.CreateNationalHoliday(new DateTime(2025, 1, 1), "Año Nuevo");
        var holiday2 = Holiday.CreateNationalHoliday(new DateTime(2025, 5, 1), "Día del Trabajo");
        var holiday3 = Holiday.CreateNationalHoliday(new DateTime(2025, 12, 25), "Navidad");

        await _context.Holidays.AddRangeAsync(holiday1, holiday2, holiday3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByDateRangeAsync(
            new DateTime(2025, 1, 1),
            new DateTime(2025, 6, 30)
        );

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(h => h.HolidayName == "Año Nuevo");
        result.Should().Contain(h => h.HolidayName == "Día del Trabajo");
    }

    /// <summary>
    /// Verifica que GetByDateRangeAsync excluye festivos inactivos.
    /// </summary>
    [Fact]
    public async Task GetByDateRangeAsync_ExcludesInactiveHolidays()
    {
        // Arrange
        var activeHoliday = Holiday.CreateNationalHoliday(new DateTime(2025, 1, 1), "Año Nuevo");
        var inactiveHoliday = Holiday.CreateNationalHoliday(new DateTime(2025, 5, 1), "Día del Trabajo");
        inactiveHoliday.IsActive = false;

        await _context.Holidays.AddRangeAsync(activeHoliday, inactiveHoliday);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByDateRangeAsync(
            new DateTime(2025, 1, 1),
            new DateTime(2025, 12, 31)
        );

        // Assert
        result.Should().HaveCount(1);
        result.Should().OnlyContain(h => h.IsActive);
    }

    /// <summary>
    /// Verifica que GetByDateRangeAsync filtra por sucursal cuando se especifica.
    /// </summary>
    [Fact]
    public async Task GetByDateRangeAsync_WithBranchId_FiltersCorrectly()
    {
        // Arrange
        var branch1 = CreateTestBranch();
        var branch2 = CreateTestBranch();
        await _context.Branches.AddRangeAsync(branch1, branch2);
        await _context.SaveChangesAsync();

        var nationalHoliday = Holiday.CreateNationalHoliday(new DateTime(2025, 1, 1), "Año Nuevo");
        var localHoliday1 = Holiday.CreateLocalHoliday(new DateTime(2025, 6, 29), "Festivo Local 1", branch1.Id);
        var localHoliday2 = Holiday.CreateLocalHoliday(new DateTime(2025, 8, 15), "Festivo Local 2", branch2.Id);

        await _context.Holidays.AddRangeAsync(nationalHoliday, localHoliday1, localHoliday2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByDateRangeAsync(
            new DateTime(2025, 1, 1),
            new DateTime(2025, 12, 31),
            branch1.Id
        );

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(h => h.HolidayName == "Año Nuevo"); // Nacional aplica a todos
        result.Should().Contain(h => h.HolidayName == "Festivo Local 1");
        result.Should().NotContain(h => h.HolidayName == "Festivo Local 2");
    }

    /// <summary>
    /// Verifica que GetByDateRangeAsync ordena los resultados por fecha.
    /// </summary>
    [Fact]
    public async Task GetByDateRangeAsync_ReturnsOrderedByDate()
    {
        // Arrange
        var holiday1 = Holiday.CreateNationalHoliday(new DateTime(2025, 12, 25), "Navidad");
        var holiday2 = Holiday.CreateNationalHoliday(new DateTime(2025, 1, 1), "Año Nuevo");
        var holiday3 = Holiday.CreateNationalHoliday(new DateTime(2025, 5, 1), "Día del Trabajo");

        await _context.Holidays.AddRangeAsync(holiday1, holiday2, holiday3);
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetByDateRangeAsync(
            new DateTime(2025, 1, 1),
            new DateTime(2025, 12, 31)
        )).ToList();

        // Assert
        result[0].HolidayName.Should().Be("Año Nuevo");
        result[1].HolidayName.Should().Be("Día del Trabajo");
        result[2].HolidayName.Should().Be("Navidad");
    }

    #endregion

    #region GetByYearAsync Tests

    /// <summary>
    /// Verifica que GetByYearAsync retorna todos los festivos del año especificado.
    /// </summary>
    [Fact]
    public async Task GetByYearAsync_ReturnsHolidaysForYear()
    {
        // Arrange
        var holiday2025_1 = Holiday.CreateNationalHoliday(new DateTime(2025, 1, 1), "Año Nuevo 2025");
        var holiday2025_2 = Holiday.CreateNationalHoliday(new DateTime(2025, 12, 25), "Navidad 2025");
        var holiday2024 = Holiday.CreateNationalHoliday(new DateTime(2024, 1, 1), "Año Nuevo 2024");

        await _context.Holidays.AddRangeAsync(holiday2025_1, holiday2025_2, holiday2024);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByYearAsync(2025);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(h => h.HolidayDate.Year == 2025);
    }

    #endregion

    #region GetByDateAsync Tests

    /// <summary>
    /// Verifica que GetByDateAsync retorna el festivo de una fecha específica.
    /// </summary>
    [Fact]
    public async Task GetByDateAsync_WithExistingDate_ReturnsHoliday()
    {
        // Arrange
        var holidayDate = new DateTime(2025, 12, 25);
        var holiday = Holiday.CreateNationalHoliday(holidayDate, "Navidad");
        await _context.Holidays.AddAsync(holiday);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByDateAsync(holidayDate);

        // Assert
        result.Should().NotBeNull();
        result!.HolidayName.Should().Be("Navidad");
    }

    /// <summary>
    /// Verifica que GetByDateAsync retorna null cuando no hay festivo en la fecha.
    /// </summary>
    [Fact]
    public async Task GetByDateAsync_WithNonExistingDate_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByDateAsync(new DateTime(2025, 3, 15));

        // Assert
        result.Should().BeNull();
    }

    /// <summary>
    /// Verifica que GetByDateAsync (interfaz) retorna múltiples festivos de la misma fecha.
    /// </summary>
    [Fact]
    public async Task GetByDateAsync_Interface_ReturnsMultipleHolidays()
    {
        // Arrange
        var branch = CreateTestBranch();
        await _context.Branches.AddAsync(branch);
        await _context.SaveChangesAsync();

        var holidayDate = new DateTime(2025, 12, 25);
        var nationalHoliday = Holiday.CreateNationalHoliday(holidayDate, "Navidad");
        var localHoliday = Holiday.CreateLocalHoliday(holidayDate, "Festivo Local", branch.Id);

        await _context.Holidays.AddRangeAsync(nationalHoliday, localHoliday);
        await _context.SaveChangesAsync();

        // Act
        var result = await ((ElectroHuila.Application.Contracts.Repositories.IHolidayRepository)_repository)
            .GetByDateAsync(holidayDate, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
    }

    #endregion

    #region GetNationalHolidaysAsync Tests

    /// <summary>
    /// Verifica que GetNationalHolidaysAsync retorna solo festivos nacionales.
    /// </summary>
    [Fact]
    public async Task GetNationalHolidaysAsync_ReturnsOnlyNationalHolidays()
    {
        // Arrange
        var branch = CreateTestBranch();
        await _context.Branches.AddAsync(branch);
        await _context.SaveChangesAsync();

        var nationalHoliday1 = Holiday.CreateNationalHoliday(new DateTime(2025, 1, 1), "Año Nuevo");
        var nationalHoliday2 = Holiday.CreateNationalHoliday(new DateTime(2025, 12, 25), "Navidad");
        var localHoliday = Holiday.CreateLocalHoliday(new DateTime(2025, 6, 29), "Festivo Local", branch.Id);
        var companyHoliday = Holiday.CreateCompanyHoliday(new DateTime(2025, 3, 15), "Día de la Empresa");

        await _context.Holidays.AddRangeAsync(nationalHoliday1, nationalHoliday2, localHoliday, companyHoliday);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetNationalHolidaysAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(h => h.HolidayType == "NATIONAL");
    }

    /// <summary>
    /// Verifica que GetNationalHolidaysAsync excluye festivos inactivos.
    /// </summary>
    [Fact]
    public async Task GetNationalHolidaysAsync_ExcludesInactiveHolidays()
    {
        // Arrange
        var activeHoliday = Holiday.CreateNationalHoliday(new DateTime(2025, 1, 1), "Año Nuevo");
        var inactiveHoliday = Holiday.CreateNationalHoliday(new DateTime(2025, 12, 25), "Navidad");
        inactiveHoliday.IsActive = false;

        await _context.Holidays.AddRangeAsync(activeHoliday, inactiveHoliday);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetNationalHolidaysAsync();

        // Assert
        result.Should().HaveCount(1);
        result.Should().OnlyContain(h => h.IsActive);
    }

    #endregion

    #region GetBranchHolidaysAsync Tests

    /// <summary>
    /// Verifica que GetBranchHolidaysAsync retorna solo festivos de la sucursal especificada.
    /// </summary>
    [Fact]
    public async Task GetBranchHolidaysAsync_ReturnsBranchSpecificHolidays()
    {
        // Arrange
        var branch1 = CreateTestBranch();
        var branch2 = CreateTestBranch();
        await _context.Branches.AddRangeAsync(branch1, branch2);
        await _context.SaveChangesAsync();

        var holiday1 = Holiday.CreateLocalHoliday(new DateTime(2025, 6, 29), "Festivo Branch 1", branch1.Id);
        var holiday2 = Holiday.CreateLocalHoliday(new DateTime(2025, 8, 15), "Festivo Branch 2", branch2.Id);
        var nationalHoliday = Holiday.CreateNationalHoliday(new DateTime(2025, 1, 1), "Año Nuevo");

        await _context.Holidays.AddRangeAsync(holiday1, holiday2, nationalHoliday);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetBranchHolidaysAsync(branch1.Id);

        // Assert
        result.Should().HaveCount(1);
        result.First().HolidayName.Should().Be("Festivo Branch 1");
    }

    /// <summary>
    /// Verifica que GetBranchHolidaysAsync excluye festivos inactivos.
    /// </summary>
    [Fact]
    public async Task GetBranchHolidaysAsync_ExcludesInactiveHolidays()
    {
        // Arrange
        var branch = CreateTestBranch();
        await _context.Branches.AddAsync(branch);
        await _context.SaveChangesAsync();

        var activeHoliday = Holiday.CreateLocalHoliday(new DateTime(2025, 6, 29), "Festivo Activo", branch.Id);
        var inactiveHoliday = Holiday.CreateLocalHoliday(new DateTime(2025, 8, 15), "Festivo Inactivo", branch.Id);
        inactiveHoliday.IsActive = false;

        await _context.Holidays.AddRangeAsync(activeHoliday, inactiveHoliday);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetBranchHolidaysAsync(branch.Id);

        // Assert
        result.Should().HaveCount(1);
        result.Should().OnlyContain(h => h.IsActive);
    }

    #endregion

    #region Helper Methods

    private Branch CreateTestBranch()
    {
        return new Branch
        {
            Name = $"Test Branch {Guid.NewGuid()}",
            Address = "Test Address",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    #endregion

    public void Dispose()
    {
        _context?.Dispose();
    }
}
