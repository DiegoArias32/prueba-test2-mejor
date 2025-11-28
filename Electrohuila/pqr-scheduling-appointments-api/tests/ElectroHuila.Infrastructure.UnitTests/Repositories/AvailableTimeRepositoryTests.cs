using ElectroHuila.Domain.Entities.Appointments;
using ElectroHuila.Domain.Entities.Locations;
using ElectroHuila.Infrastructure.Persistence;
using ElectroHuila.Infrastructure.Persistence.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Xunit;

namespace ElectroHuila.Infrastructure.UnitTests.Repositories;

/// <summary>
/// Pruebas unitarias completas para el repositorio de horarios disponibles (AvailableTimeRepository).
/// Cubre operaciones CRUD, consultas por sucursal, tipo de cita, validaciones de disponibilidad y soft delete.
/// </summary>
public class AvailableTimeRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly AvailableTimeRepository _repository;

    public AvailableTimeRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new AvailableTimeRepository(_context);
    }

    #region GetByBranchIdAsync Tests

    /// <summary>
    /// Verifica que GetByBranchIdAsync retorna todos los horarios activos de una sucursal.
    /// </summary>
    [Fact]
    public async Task GetByBranchIdAsync_ReturnsActiveTimesForBranch()
    {
        // Arrange
        var branch1 = CreateTestBranch();
        var branch2 = CreateTestBranch();
        var appointmentType = CreateTestAppointmentType();
        await _context.Branches.AddRangeAsync(branch1, branch2);
        await _context.AppointmentTypes.AddAsync(appointmentType);
        await _context.SaveChangesAsync();

        var time1 = CreateTestAvailableTime(branch1.Id, appointmentType.Id, "09:00");
        var time2 = CreateTestAvailableTime(branch1.Id, appointmentType.Id, "10:00");
        var time3 = CreateTestAvailableTime(branch2.Id, appointmentType.Id, "09:00");

        await _context.AvailableTimes.AddRangeAsync(time1, time2, time3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByBranchIdAsync(branch1.Id);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(t => t.BranchId == branch1.Id);
    }

    /// <summary>
    /// Verifica que GetByBranchIdAsync excluye horarios inactivos.
    /// </summary>
    [Fact]
    public async Task GetByBranchIdAsync_ExcludesInactiveTimes()
    {
        // Arrange
        var branch = CreateTestBranch();
        var appointmentType = CreateTestAppointmentType();
        await _context.Branches.AddAsync(branch);
        await _context.AppointmentTypes.AddAsync(appointmentType);
        await _context.SaveChangesAsync();

        var activeTime = CreateTestAvailableTime(branch.Id, appointmentType.Id, "09:00");
        var inactiveTime = CreateTestAvailableTime(branch.Id, appointmentType.Id, "10:00");
        inactiveTime.IsActive = false;

        await _context.AvailableTimes.AddRangeAsync(activeTime, inactiveTime);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByBranchIdAsync(branch.Id);

        // Assert
        result.Should().HaveCount(1);
        result.Should().OnlyContain(t => t.IsActive);
    }

    /// <summary>
    /// Verifica que GetByBranchIdAsync ordena los horarios por tiempo.
    /// </summary>
    [Fact]
    public async Task GetByBranchIdAsync_ReturnsTimesOrderedByTime()
    {
        // Arrange
        var branch = CreateTestBranch();
        var appointmentType = CreateTestAppointmentType();
        await _context.Branches.AddAsync(branch);
        await _context.AppointmentTypes.AddAsync(appointmentType);
        await _context.SaveChangesAsync();

        var time1 = CreateTestAvailableTime(branch.Id, appointmentType.Id, "14:00");
        var time2 = CreateTestAvailableTime(branch.Id, appointmentType.Id, "09:00");
        var time3 = CreateTestAvailableTime(branch.Id, appointmentType.Id, "11:00");

        await _context.AvailableTimes.AddRangeAsync(time1, time2, time3);
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetByBranchIdAsync(branch.Id)).ToList();

        // Assert
        result.Should().HaveCount(3);
        result[0].Time.Should().Be("09:00");
        result[1].Time.Should().Be("11:00");
        result[2].Time.Should().Be("14:00");
    }

    #endregion

    #region GetByDayOfWeekAsync Tests

    /// <summary>
    /// Verifica que GetByDayOfWeekAsync retorna todos los horarios activos (sin filtro de día implementado).
    /// </summary>
    [Fact]
    public async Task GetByDayOfWeekAsync_ReturnsActiveTimesWithBranchInfo()
    {
        // Arrange
        var branch = CreateTestBranch();
        var appointmentType = CreateTestAppointmentType();
        await _context.Branches.AddAsync(branch);
        await _context.AppointmentTypes.AddAsync(appointmentType);
        await _context.SaveChangesAsync();

        var time1 = CreateTestAvailableTime(branch.Id, appointmentType.Id, "09:00");
        var time2 = CreateTestAvailableTime(branch.Id, appointmentType.Id, "10:00");
        var inactiveTime = CreateTestAvailableTime(branch.Id, appointmentType.Id, "11:00");
        inactiveTime.IsActive = false;

        await _context.AvailableTimes.AddRangeAsync(time1, time2, inactiveTime);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByDayOfWeekAsync(DayOfWeek.Monday);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(t => t.IsActive);
        result.Should().OnlyContain(t => t.Branch != null);
    }

    #endregion

    #region GetByBranchAndDayAsync Tests

    /// <summary>
    /// Verifica que GetByBranchAndDayAsync retorna horarios de la sucursal especificada.
    /// </summary>
    [Fact]
    public async Task GetByBranchAndDayAsync_ReturnsTimesForSpecificBranch()
    {
        // Arrange
        var branch1 = CreateTestBranch();
        var branch2 = CreateTestBranch();
        var appointmentType = CreateTestAppointmentType();
        await _context.Branches.AddRangeAsync(branch1, branch2);
        await _context.AppointmentTypes.AddAsync(appointmentType);
        await _context.SaveChangesAsync();

        var time1 = CreateTestAvailableTime(branch1.Id, appointmentType.Id, "09:00");
        var time2 = CreateTestAvailableTime(branch2.Id, appointmentType.Id, "09:00");

        await _context.AvailableTimes.AddRangeAsync(time1, time2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByBranchAndDayAsync(branch1.Id, DayOfWeek.Monday);

        // Assert
        result.Should().HaveCount(1);
        result.Should().OnlyContain(t => t.BranchId == branch1.Id);
    }

    #endregion

    #region IsTimeAvailableAsync Tests

    /// <summary>
    /// Verifica que IsTimeAvailableAsync retorna true cuando hay horarios activos en la sucursal.
    /// </summary>
    [Fact]
    public async Task IsTimeAvailableAsync_WithActiveTimesInBranch_ReturnsTrue()
    {
        // Arrange
        var branch = CreateTestBranch();
        var appointmentType = CreateTestAppointmentType();
        await _context.Branches.AddAsync(branch);
        await _context.AppointmentTypes.AddAsync(appointmentType);
        await _context.SaveChangesAsync();

        var time = CreateTestAvailableTime(branch.Id, appointmentType.Id, "09:00");
        await _context.AvailableTimes.AddAsync(time);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.IsTimeAvailableAsync(branch.Id, DayOfWeek.Monday, TimeSpan.FromHours(9), TimeSpan.FromHours(10));

        // Assert
        result.Should().BeTrue();
    }

    /// <summary>
    /// Verifica que IsTimeAvailableAsync retorna false cuando no hay horarios activos.
    /// </summary>
    [Fact]
    public async Task IsTimeAvailableAsync_WithNoActiveTimesInBranch_ReturnsFalse()
    {
        // Arrange
        var branch = CreateTestBranch();
        await _context.Branches.AddAsync(branch);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.IsTimeAvailableAsync(branch.Id, DayOfWeek.Monday, TimeSpan.FromHours(9), TimeSpan.FromHours(10));

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region GetActiveAsync Tests

    /// <summary>
    /// Verifica que GetActiveAsync retorna todos los horarios activos del sistema.
    /// </summary>
    [Fact]
    public async Task GetActiveAsync_ReturnsAllActiveTimes()
    {
        // Arrange
        var branch1 = CreateTestBranch();
        var branch2 = CreateTestBranch();
        var appointmentType = CreateTestAppointmentType();
        await _context.Branches.AddRangeAsync(branch1, branch2);
        await _context.AppointmentTypes.AddAsync(appointmentType);
        await _context.SaveChangesAsync();

        var activeTime1 = CreateTestAvailableTime(branch1.Id, appointmentType.Id, "09:00");
        var activeTime2 = CreateTestAvailableTime(branch2.Id, appointmentType.Id, "10:00");
        var inactiveTime = CreateTestAvailableTime(branch1.Id, appointmentType.Id, "11:00");
        inactiveTime.IsActive = false;

        await _context.AvailableTimes.AddRangeAsync(activeTime1, activeTime2, inactiveTime);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetActiveAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(t => t.IsActive);
    }

    /// <summary>
    /// Verifica que GetActiveAsync ordena los resultados por sucursal y tiempo.
    /// </summary>
    [Fact]
    public async Task GetActiveAsync_ReturnsTimesOrderedByBranchAndTime()
    {
        // Arrange
        var branch1 = CreateTestBranch();
        var branch2 = CreateTestBranch();
        var appointmentType = CreateTestAppointmentType();
        await _context.Branches.AddRangeAsync(branch1, branch2);
        await _context.AppointmentTypes.AddAsync(appointmentType);
        await _context.SaveChangesAsync();

        var time1 = CreateTestAvailableTime(branch2.Id, appointmentType.Id, "14:00");
        var time2 = CreateTestAvailableTime(branch1.Id, appointmentType.Id, "09:00");
        var time3 = CreateTestAvailableTime(branch1.Id, appointmentType.Id, "11:00");

        await _context.AvailableTimes.AddRangeAsync(time1, time2, time3);
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetActiveAsync()).ToList();

        // Assert
        result.Should().HaveCount(3);
        result[0].BranchId.Should().Be(branch1.Id);
        result[1].BranchId.Should().Be(branch1.Id);
        result[2].BranchId.Should().Be(branch2.Id);
    }

    #endregion

    #region GetAllIncludingInactiveAsync Tests

    /// <summary>
    /// Verifica que GetAllIncludingInactiveAsync retorna todos los horarios incluyendo inactivos.
    /// </summary>
    [Fact]
    public async Task GetAllIncludingInactiveAsync_ReturnsAllTimes()
    {
        // Arrange
        var branch = CreateTestBranch();
        var appointmentType = CreateTestAppointmentType();
        await _context.Branches.AddAsync(branch);
        await _context.AppointmentTypes.AddAsync(appointmentType);
        await _context.SaveChangesAsync();

        var activeTime = CreateTestAvailableTime(branch.Id, appointmentType.Id, "09:00");
        var inactiveTime = CreateTestAvailableTime(branch.Id, appointmentType.Id, "10:00");
        inactiveTime.IsActive = false;

        await _context.AvailableTimes.AddRangeAsync(activeTime, inactiveTime);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllIncludingInactiveAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(t => t.IsActive);
        result.Should().Contain(t => !t.IsActive);
    }

    /// <summary>
    /// Verifica que GetAllIncludingInactiveAsync incluye Branch y AppointmentType.
    /// </summary>
    [Fact]
    public async Task GetAllIncludingInactiveAsync_IncludesBranchAndAppointmentType()
    {
        // Arrange
        var branch = CreateTestBranch();
        var appointmentType = CreateTestAppointmentType();
        await _context.Branches.AddAsync(branch);
        await _context.AppointmentTypes.AddAsync(appointmentType);
        await _context.SaveChangesAsync();

        var time = CreateTestAvailableTime(branch.Id, appointmentType.Id, "09:00");
        await _context.AvailableTimes.AddAsync(time);
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetAllIncludingInactiveAsync()).ToList();

        // Assert
        result.Should().HaveCount(1);
        result[0].Branch.Should().NotBeNull();
        result[0].AppointmentType.Should().NotBeNull();
    }

    #endregion

    #region GetByAppointmentTypeIdAsync Tests

    /// <summary>
    /// Verifica que GetByAppointmentTypeIdAsync retorna horarios del tipo de cita especificado.
    /// </summary>
    [Fact]
    public async Task GetByAppointmentTypeIdAsync_ReturnsTimesForAppointmentType()
    {
        // Arrange
        var branch = CreateTestBranch();
        var appointmentType1 = CreateTestAppointmentType();
        var appointmentType2 = CreateTestAppointmentType();
        await _context.Branches.AddAsync(branch);
        await _context.AppointmentTypes.AddRangeAsync(appointmentType1, appointmentType2);
        await _context.SaveChangesAsync();

        var time1 = CreateTestAvailableTime(branch.Id, appointmentType1.Id, "09:00");
        var time2 = CreateTestAvailableTime(branch.Id, appointmentType2.Id, "10:00");

        await _context.AvailableTimes.AddRangeAsync(time1, time2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByAppointmentTypeIdAsync(appointmentType1.Id);

        // Assert
        result.Should().HaveCount(1);
        result.Should().OnlyContain(t => t.AppointmentTypeId == appointmentType1.Id);
    }

    /// <summary>
    /// Verifica que GetByAppointmentTypeIdAsync excluye horarios inactivos.
    /// </summary>
    [Fact]
    public async Task GetByAppointmentTypeIdAsync_ExcludesInactiveTimes()
    {
        // Arrange
        var branch = CreateTestBranch();
        var appointmentType = CreateTestAppointmentType();
        await _context.Branches.AddAsync(branch);
        await _context.AppointmentTypes.AddAsync(appointmentType);
        await _context.SaveChangesAsync();

        var activeTime = CreateTestAvailableTime(branch.Id, appointmentType.Id, "09:00");
        var inactiveTime = CreateTestAvailableTime(branch.Id, appointmentType.Id, "10:00");
        inactiveTime.IsActive = false;

        await _context.AvailableTimes.AddRangeAsync(activeTime, inactiveTime);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByAppointmentTypeIdAsync(appointmentType.Id);

        // Assert
        result.Should().HaveCount(1);
        result.Should().OnlyContain(t => t.IsActive);
    }

    #endregion

    #region GetAvailableTimesAsync Tests

    /// <summary>
    /// Verifica que GetAvailableTimesAsync retorna horarios filtrados por sucursal.
    /// </summary>
    [Fact]
    public async Task GetAvailableTimesAsync_WithBranchIdOnly_ReturnsAllTimesForBranch()
    {
        // Arrange
        var branch1 = CreateTestBranch();
        var branch2 = CreateTestBranch();
        var appointmentType = CreateTestAppointmentType();
        await _context.Branches.AddRangeAsync(branch1, branch2);
        await _context.AppointmentTypes.AddAsync(appointmentType);
        await _context.SaveChangesAsync();

        var time1 = CreateTestAvailableTime(branch1.Id, appointmentType.Id, "09:00");
        var time2 = CreateTestAvailableTime(branch1.Id, appointmentType.Id, "10:00");
        var time3 = CreateTestAvailableTime(branch2.Id, appointmentType.Id, "09:00");

        await _context.AvailableTimes.AddRangeAsync(time1, time2, time3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAvailableTimesAsync(branch1.Id);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(t => t.BranchId == branch1.Id);
    }

    /// <summary>
    /// Verifica que GetAvailableTimesAsync filtra por sucursal y tipo de cita cuando ambos se proporcionan.
    /// </summary>
    [Fact]
    public async Task GetAvailableTimesAsync_WithBranchAndAppointmentType_ReturnsFilteredTimes()
    {
        // Arrange
        var branch = CreateTestBranch();
        var appointmentType1 = CreateTestAppointmentType();
        var appointmentType2 = CreateTestAppointmentType();
        await _context.Branches.AddAsync(branch);
        await _context.AppointmentTypes.AddRangeAsync(appointmentType1, appointmentType2);
        await _context.SaveChangesAsync();

        var time1 = CreateTestAvailableTime(branch.Id, appointmentType1.Id, "09:00");
        var time2 = CreateTestAvailableTime(branch.Id, appointmentType2.Id, "10:00");

        await _context.AvailableTimes.AddRangeAsync(time1, time2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAvailableTimesAsync(branch.Id, appointmentType1.Id);

        // Assert
        result.Should().HaveCount(1);
        result.Should().OnlyContain(t => t.AppointmentTypeId == appointmentType1.Id);
    }

    #endregion

    #region IsTimeSlotAvailableAsync Tests

    /// <summary>
    /// Verifica que IsTimeSlotAvailableAsync retorna true cuando NO existe el horario (disponible para crear).
    /// </summary>
    [Fact]
    public async Task IsTimeSlotAvailableAsync_WithNonExistingSlot_ReturnsTrue()
    {
        // Arrange
        var branch = CreateTestBranch();
        var appointmentType = CreateTestAppointmentType();
        await _context.Branches.AddAsync(branch);
        await _context.AppointmentTypes.AddAsync(appointmentType);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.IsTimeSlotAvailableAsync(branch.Id, "09:00", appointmentType.Id);

        // Assert
        result.Should().BeTrue();
    }

    /// <summary>
    /// Verifica que IsTimeSlotAvailableAsync retorna false cuando el horario ya existe.
    /// </summary>
    [Fact]
    public async Task IsTimeSlotAvailableAsync_WithExistingSlot_ReturnsFalse()
    {
        // Arrange
        var branch = CreateTestBranch();
        var appointmentType = CreateTestAppointmentType();
        await _context.Branches.AddAsync(branch);
        await _context.AppointmentTypes.AddAsync(appointmentType);
        await _context.SaveChangesAsync();

        var time = CreateTestAvailableTime(branch.Id, appointmentType.Id, "09:00");
        await _context.AvailableTimes.AddAsync(time);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.IsTimeSlotAvailableAsync(branch.Id, "09:00", appointmentType.Id);

        // Assert
        result.Should().BeFalse();
    }

    /// <summary>
    /// Verifica que IsTimeSlotAvailableAsync sin tipo de cita valida solo por sucursal y hora.
    /// </summary>
    [Fact]
    public async Task IsTimeSlotAvailableAsync_WithoutAppointmentType_ValidatesOnlyBranchAndTime()
    {
        // Arrange
        var branch = CreateTestBranch();
        var appointmentType = CreateTestAppointmentType();
        await _context.Branches.AddAsync(branch);
        await _context.AppointmentTypes.AddAsync(appointmentType);
        await _context.SaveChangesAsync();

        var time = CreateTestAvailableTime(branch.Id, appointmentType.Id, "09:00");
        await _context.AvailableTimes.AddAsync(time);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.IsTimeSlotAvailableAsync(branch.Id, "09:00");

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region DeleteAsync Tests

    /// <summary>
    /// Verifica que DeleteAsync realiza soft delete marcando IsActive como false.
    /// </summary>
    [Fact]
    public async Task DeleteAsync_PerformsSoftDelete()
    {
        // Arrange
        var branch = CreateTestBranch();
        var appointmentType = CreateTestAppointmentType();
        await _context.Branches.AddAsync(branch);
        await _context.AppointmentTypes.AddAsync(appointmentType);
        await _context.SaveChangesAsync();

        var time = CreateTestAvailableTime(branch.Id, appointmentType.Id, "09:00");
        await _context.AvailableTimes.AddAsync(time);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(time.Id);
        await _context.SaveChangesAsync();

        // Assert
        var deleted = await _context.AvailableTimes.FindAsync(time.Id);
        deleted.Should().NotBeNull();
        deleted!.IsActive.Should().BeFalse();
    }

    /// <summary>
    /// Verifica que DeleteAsync no falla cuando el ID no existe.
    /// </summary>
    [Fact]
    public async Task DeleteAsync_WithNonExistentId_DoesNotThrow()
    {
        // Act
        var act = async () => await _repository.DeleteAsync(999);

        // Assert
        await act.Should().NotThrowAsync();
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

    private AppointmentType CreateTestAppointmentType()
    {
        return AppointmentType.Create(
            $"TYPE{Guid.NewGuid().ToString().Substring(0, 8)}",
            $"Test Type {Guid.NewGuid()}",
            "Test Description");
    }

    private AvailableTime CreateTestAvailableTime(int branchId, int appointmentTypeId, string time)
    {
        return new AvailableTime
        {
            BranchId = branchId,
            AppointmentTypeId = appointmentTypeId,
            Time = time,
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
