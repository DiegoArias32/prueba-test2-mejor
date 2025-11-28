using ElectroHuila.Domain.Entities.Appointments;
using ElectroHuila.Domain.Entities.Catalogs;
using ElectroHuila.Domain.Entities.Clients;
using ElectroHuila.Domain.Entities.Locations;
using ElectroHuila.Domain.Enums;
using ElectroHuila.Infrastructure.Persistence;
using ElectroHuila.Infrastructure.Persistence.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ElectroHuila.Infrastructure.UnitTests.Repositories;

/// <summary>
/// Pruebas unitarias completas para el repositorio de citas (AppointmentRepository).
/// Cubre operaciones CRUD, consultas especializadas, validaciones y manejo de errores.
/// </summary>
public class AppointmentRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<ILogger<AppointmentRepository>> _loggerMock;
    private readonly AppointmentRepository _repository;

    public AppointmentRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _loggerMock = new Mock<ILogger<AppointmentRepository>>();
        _repository = new AppointmentRepository(_context, _loggerMock.Object);
    }

    #region GetByIdAsync Tests

    /// <summary>
    /// Verifica que GetByIdAsync retorna una cita existente y activa con sus relaciones cargadas.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsAppointmentWithRelations()
    {
        // Arrange
        var client = CreateTestClient();
        var branch = CreateTestBranch();
        var appointmentType = CreateTestAppointmentType();
        await _context.Clients.AddAsync(client);
        await _context.Branches.AddAsync(branch);
        await _context.AppointmentTypes.AddAsync(appointmentType);
        await _context.SaveChangesAsync();

        var appointment = CreateTestAppointment(client.Id, branch.Id, appointmentType.Id);
        await _context.Appointments.AddAsync(appointment);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(appointment.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(appointment.Id);
        result.Client.Should().NotBeNull();
        result.Branch.Should().NotBeNull();
        result.AppointmentType.Should().NotBeNull();
    }

    /// <summary>
    /// Verifica que GetByIdAsync retorna null cuando la cita no existe.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_WithNonExistentId_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    /// <summary>
    /// Verifica que GetByIdAsync retorna null cuando la cita está inactiva.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_WithInactiveAppointment_ReturnsNull()
    {
        // Arrange
        var client = CreateTestClient();
        var branch = CreateTestBranch();
        var appointmentType = CreateTestAppointmentType();
        await _context.Clients.AddAsync(client);
        await _context.Branches.AddAsync(branch);
        await _context.AppointmentTypes.AddAsync(appointmentType);
        await _context.SaveChangesAsync();

        var appointment = CreateTestAppointment(client.Id, branch.Id, appointmentType.Id);
        appointment.IsActive = false;
        await _context.Appointments.AddAsync(appointment);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(appointment.Id);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region GetAllAsync Tests

    /// <summary>
    /// Verifica que GetAllAsync retorna todas las citas activas con sus relaciones.
    /// </summary>
    [Fact]
    public async Task GetAllAsync_ReturnsOnlyActiveAppointments()
    {
        // Arrange
        var client = CreateTestClient();
        var branch = CreateTestBranch();
        var appointmentType = CreateTestAppointmentType();
        await _context.Clients.AddAsync(client);
        await _context.Branches.AddAsync(branch);
        await _context.AppointmentTypes.AddAsync(appointmentType);
        await _context.SaveChangesAsync();

        var activeAppointment1 = CreateTestAppointment(client.Id, branch.Id, appointmentType.Id);
        var activeAppointment2 = CreateTestAppointment(client.Id, branch.Id, appointmentType.Id);
        var inactiveAppointment = CreateTestAppointment(client.Id, branch.Id, appointmentType.Id);
        inactiveAppointment.IsActive = false;

        await _context.Appointments.AddRangeAsync(activeAppointment1, activeAppointment2, inactiveAppointment);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(a => a.IsActive);
    }

    /// <summary>
    /// Verifica que GetAllAsync retorna una colección vacía cuando no hay citas.
    /// </summary>
    [Fact]
    public async Task GetAllAsync_WithNoAppointments_ReturnsEmptyCollection()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    #endregion

    #region GetAllIncludingInactiveAsync Tests

    /// <summary>
    /// Verifica que GetAllIncludingInactiveAsync retorna todas las citas incluyendo inactivas.
    /// </summary>
    [Fact]
    public async Task GetAllIncludingInactiveAsync_ReturnsAllAppointments()
    {
        // Arrange
        var client = CreateTestClient();
        var branch = CreateTestBranch();
        var appointmentType = CreateTestAppointmentType();
        var status = CreateTestAppointmentStatus();
        await _context.Clients.AddAsync(client);
        await _context.Branches.AddAsync(branch);
        await _context.AppointmentTypes.AddAsync(appointmentType);
        await _context.AppointmentStatuses.AddAsync(status);
        await _context.SaveChangesAsync();

        var activeAppointment = CreateTestAppointment(client.Id, branch.Id, appointmentType.Id);
        var inactiveAppointment = CreateTestAppointment(client.Id, branch.Id, appointmentType.Id);
        inactiveAppointment.IsActive = false;

        await _context.Appointments.AddRangeAsync(activeAppointment, inactiveAppointment);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllIncludingInactiveAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(a => a.IsActive);
        result.Should().Contain(a => !a.IsActive);
    }

    #endregion

    #region GetByClientIdAsync Tests

    /// <summary>
    /// Verifica que GetByClientIdAsync retorna solo las citas del cliente especificado.
    /// </summary>
    [Fact]
    public async Task GetByClientIdAsync_ReturnsClientAppointments()
    {
        // Arrange
        var client1 = CreateTestClient();
        var client2 = CreateTestClient();
        var branch = CreateTestBranch();
        var appointmentType = CreateTestAppointmentType();
        var status = CreateTestAppointmentStatus();

        await _context.Clients.AddRangeAsync(client1, client2);
        await _context.Branches.AddAsync(branch);
        await _context.AppointmentTypes.AddAsync(appointmentType);
        await _context.AppointmentStatuses.AddAsync(status);
        await _context.SaveChangesAsync();

        var appointment1 = CreateTestAppointment(client1.Id, branch.Id, appointmentType.Id);
        var appointment2 = CreateTestAppointment(client1.Id, branch.Id, appointmentType.Id);
        var appointment3 = CreateTestAppointment(client2.Id, branch.Id, appointmentType.Id);

        await _context.Appointments.AddRangeAsync(appointment1, appointment2, appointment3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByClientIdAsync(client1.Id);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(a => a.ClientId == client1.Id);
    }

    /// <summary>
    /// Verifica que GetByClientIdAsync no retorna citas inactivas.
    /// </summary>
    [Fact]
    public async Task GetByClientIdAsync_ExcludesInactiveAppointments()
    {
        // Arrange
        var client = CreateTestClient();
        var branch = CreateTestBranch();
        var appointmentType = CreateTestAppointmentType();
        var status = CreateTestAppointmentStatus();

        await _context.Clients.AddAsync(client);
        await _context.Branches.AddAsync(branch);
        await _context.AppointmentTypes.AddAsync(appointmentType);
        await _context.AppointmentStatuses.AddAsync(status);
        await _context.SaveChangesAsync();

        var activeAppointment = CreateTestAppointment(client.Id, branch.Id, appointmentType.Id);
        var inactiveAppointment = CreateTestAppointment(client.Id, branch.Id, appointmentType.Id);
        inactiveAppointment.IsActive = false;

        await _context.Appointments.AddRangeAsync(activeAppointment, inactiveAppointment);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByClientIdAsync(client.Id);

        // Assert
        result.Should().HaveCount(1);
        result.Should().OnlyContain(a => a.IsActive);
    }

    #endregion

    #region GetByBranchIdAsync Tests

    /// <summary>
    /// Verifica que GetByBranchIdAsync retorna solo las citas de la sucursal especificada.
    /// </summary>
    [Fact]
    public async Task GetByBranchIdAsync_ReturnsBranchAppointments()
    {
        // Arrange
        var client = CreateTestClient();
        var branch1 = CreateTestBranch();
        var branch2 = CreateTestBranch();
        var appointmentType = CreateTestAppointmentType();

        await _context.Clients.AddAsync(client);
        await _context.Branches.AddRangeAsync(branch1, branch2);
        await _context.AppointmentTypes.AddAsync(appointmentType);
        await _context.SaveChangesAsync();

        var appointment1 = CreateTestAppointment(client.Id, branch1.Id, appointmentType.Id);
        var appointment2 = CreateTestAppointment(client.Id, branch2.Id, appointmentType.Id);

        await _context.Appointments.AddRangeAsync(appointment1, appointment2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByBranchIdAsync(branch1.Id);

        // Assert
        result.Should().HaveCount(1);
        result.Should().OnlyContain(a => a.BranchId == branch1.Id);
    }

    #endregion

    #region AddAsync Tests

    /// <summary>
    /// Verifica que AddAsync agrega una nueva cita correctamente y retorna con ID asignado.
    /// </summary>
    [Fact]
    public async Task AddAsync_AddsAppointmentSuccessfully()
    {
        // Arrange
        var client = CreateTestClient();
        var branch = CreateTestBranch();
        var appointmentType = CreateTestAppointmentType();
        await _context.Clients.AddAsync(client);
        await _context.Branches.AddAsync(branch);
        await _context.AppointmentTypes.AddAsync(appointmentType);
        await _context.SaveChangesAsync();

        var appointment = CreateTestAppointment(client.Id, branch.Id, appointmentType.Id);

        // Act
        var result = await _repository.AddAsync(appointment);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        var saved = await _context.Appointments.FindAsync(result.Id);
        saved.Should().NotBeNull();
    }

    #endregion

    #region UpdateAsync Tests

    /// <summary>
    /// Verifica que UpdateAsync actualiza una cita existente correctamente.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_UpdatesAppointmentSuccessfully()
    {
        // Arrange
        var client = CreateTestClient();
        var branch = CreateTestBranch();
        var appointmentType = CreateTestAppointmentType();
        await _context.Clients.AddAsync(client);
        await _context.Branches.AddAsync(branch);
        await _context.AppointmentTypes.AddAsync(appointmentType);
        await _context.SaveChangesAsync();

        var appointment = CreateTestAppointment(client.Id, branch.Id, appointmentType.Id);
        await _context.Appointments.AddAsync(appointment);
        await _context.SaveChangesAsync();

        // Detach the entity to simulate getting it from another context
        _context.Entry(appointment).State = EntityState.Detached;

        // Act
        appointment.Notes = "Updated notes";
        await _repository.UpdateAsync(appointment);

        // Assert
        var updated = await _context.Appointments.FindAsync(appointment.Id);
        updated.Should().NotBeNull();
        updated!.Notes.Should().Be("Updated notes");
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
        var client = CreateTestClient();
        var branch = CreateTestBranch();
        var appointmentType = CreateTestAppointmentType();
        await _context.Clients.AddAsync(client);
        await _context.Branches.AddAsync(branch);
        await _context.AppointmentTypes.AddAsync(appointmentType);
        await _context.SaveChangesAsync();

        var appointment = CreateTestAppointment(client.Id, branch.Id, appointmentType.Id);
        await _context.Appointments.AddAsync(appointment);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(appointment.Id);

        // Assert
        var deleted = await _context.Appointments.FindAsync(appointment.Id);
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

    #region ExistsAsync Tests

    /// <summary>
    /// Verifica que ExistsAsync retorna true para una cita activa existente.
    /// </summary>
    [Fact]
    public async Task ExistsAsync_WithExistingActiveAppointment_ReturnsTrue()
    {
        // Arrange
        var client = CreateTestClient();
        var branch = CreateTestBranch();
        var appointmentType = CreateTestAppointmentType();
        await _context.Clients.AddAsync(client);
        await _context.Branches.AddAsync(branch);
        await _context.AppointmentTypes.AddAsync(appointmentType);
        await _context.SaveChangesAsync();

        var appointment = CreateTestAppointment(client.Id, branch.Id, appointmentType.Id);
        await _context.Appointments.AddAsync(appointment);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsAsync(appointment.Id);

        // Assert
        result.Should().BeTrue();
    }

    /// <summary>
    /// Verifica que ExistsAsync retorna false para una cita inactiva.
    /// </summary>
    [Fact]
    public async Task ExistsAsync_WithInactiveAppointment_ReturnsFalse()
    {
        // Arrange
        var client = CreateTestClient();
        var branch = CreateTestBranch();
        var appointmentType = CreateTestAppointmentType();
        await _context.Clients.AddAsync(client);
        await _context.Branches.AddAsync(branch);
        await _context.AppointmentTypes.AddAsync(appointmentType);
        await _context.SaveChangesAsync();

        var appointment = CreateTestAppointment(client.Id, branch.Id, appointmentType.Id);
        appointment.IsActive = false;
        await _context.Appointments.AddAsync(appointment);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsAsync(appointment.Id);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region GetPendingOrNoShowAppointmentsByDocumentNumberAsync Tests

    /// <summary>
    /// Verifica que retorna citas pendientes, confirmadas y no asistidas por número de documento.
    /// </summary>
    [Fact]
    public async Task GetPendingOrNoShowAppointmentsByDocumentNumberAsync_ReturnsPendingStatuses()
    {
        // Arrange
        var client = CreateTestClient();
        client.DocumentNumber = "12345678";
        var branch = CreateTestBranch();
        var appointmentType = CreateTestAppointmentType();

        await _context.Clients.AddAsync(client);
        await _context.Branches.AddAsync(branch);
        await _context.AppointmentTypes.AddAsync(appointmentType);

        // Create statuses
        var pendingStatus = AppointmentStatus.Create("PENDING", "Pending", "Pending Status");
        pendingStatus.GetType().GetProperty("Id")!.SetValue(pendingStatus, 1);
        var confirmedStatus = AppointmentStatus.Create("CONFIRMED", "Confirmed", "Confirmed Status");
        confirmedStatus.GetType().GetProperty("Id")!.SetValue(confirmedStatus, 2);
        var noShowStatus = AppointmentStatus.Create("NO_SHOW", "No Show", "No Show Status");
        noShowStatus.GetType().GetProperty("Id")!.SetValue(noShowStatus, 3);
        var completedStatus = AppointmentStatus.Create("COMPLETED", "Completed", "Completed Status");
        completedStatus.GetType().GetProperty("Id")!.SetValue(completedStatus, 4);

        await _context.AppointmentStatuses.AddRangeAsync(pendingStatus, confirmedStatus, noShowStatus, completedStatus);
        await _context.SaveChangesAsync();

        var appointment1 = CreateTestAppointment(client.Id, branch.Id, appointmentType.Id);
        appointment1.StatusId = 1; // PENDING

        var appointment2 = CreateTestAppointment(client.Id, branch.Id, appointmentType.Id);
        appointment2.StatusId = 2; // CONFIRMED

        var appointment3 = CreateTestAppointment(client.Id, branch.Id, appointmentType.Id);
        appointment3.StatusId = 4; // COMPLETED (should not be returned)

        await _context.Appointments.AddRangeAsync(appointment1, appointment2, appointment3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetPendingOrNoShowAppointmentsByDocumentNumberAsync("12345678");

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(a => new[] { 1, 2, 3 }.Contains(a.StatusId));
    }

    #endregion

    #region HasPendingOrNoShowAppointmentsAsync Tests

    /// <summary>
    /// Verifica que HasPendingOrNoShowAppointmentsAsync retorna true cuando hay citas pendientes.
    /// </summary>
    [Fact]
    public async Task HasPendingOrNoShowAppointmentsAsync_WithPendingAppointments_ReturnsTrue()
    {
        // Arrange
        var client = CreateTestClient();
        client.DocumentNumber = "12345678";
        var branch = CreateTestBranch();
        var appointmentType = CreateTestAppointmentType();

        await _context.Clients.AddAsync(client);
        await _context.Branches.AddAsync(branch);
        await _context.AppointmentTypes.AddAsync(appointmentType);

        var pendingStatus = AppointmentStatus.Create("PENDING", "Pending", "Pending Status");
        pendingStatus.GetType().GetProperty("Id")!.SetValue(pendingStatus, 1);
        await _context.AppointmentStatuses.AddAsync(pendingStatus);
        await _context.SaveChangesAsync();

        var appointment = CreateTestAppointment(client.Id, branch.Id, appointmentType.Id);
        appointment.StatusId = 1;
        await _context.Appointments.AddAsync(appointment);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.HasPendingOrNoShowAppointmentsAsync("12345678");

        // Assert
        result.Should().BeTrue();
    }

    /// <summary>
    /// Verifica que HasPendingOrNoShowAppointmentsAsync retorna false cuando no hay citas pendientes.
    /// </summary>
    [Fact]
    public async Task HasPendingOrNoShowAppointmentsAsync_WithoutPendingAppointments_ReturnsFalse()
    {
        // Act
        var result = await _repository.HasPendingOrNoShowAppointmentsAsync("12345678");

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region GetAppointmentsWithDetailsAsync Tests

    /// <summary>
    /// Verifica que GetAppointmentsWithDetailsAsync retorna citas filtradas por tipos con todas sus relaciones.
    /// </summary>
    [Fact]
    public async Task GetAppointmentsWithDetailsAsync_ReturnsFilteredAppointmentsWithDetails()
    {
        // Arrange
        var client = CreateTestClient();
        var branch = CreateTestBranch();
        var appointmentType1 = CreateTestAppointmentType();
        var appointmentType2 = CreateTestAppointmentType();
        var status = CreateTestAppointmentStatus();

        await _context.Clients.AddAsync(client);
        await _context.Branches.AddAsync(branch);
        await _context.AppointmentTypes.AddRangeAsync(appointmentType1, appointmentType2);
        await _context.AppointmentStatuses.AddAsync(status);
        await _context.SaveChangesAsync();

        var appointment1 = CreateTestAppointment(client.Id, branch.Id, appointmentType1.Id);
        var appointment2 = CreateTestAppointment(client.Id, branch.Id, appointmentType2.Id);

        await _context.Appointments.AddRangeAsync(appointment1, appointment2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAppointmentsWithDetailsAsync(new[] { appointmentType1.Id });

        // Assert
        result.Should().HaveCount(1);
        result.First().AppointmentTypeId.Should().Be(appointmentType1.Id);
    }

    #endregion

    #region Helper Methods

    private Client CreateTestClient()
    {
        return new Client
        {
            DocumentNumber = Guid.NewGuid().ToString().Substring(0, 10),
            DocumentType = DocumentType.CC,
            FullName = "Test Client",
            Email = $"test{Guid.NewGuid()}@example.com",
            Phone = "1234567890",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

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

    private AppointmentStatus CreateTestAppointmentStatus()
    {
        return AppointmentStatus.Create(
            "PENDING",
            "Pending",
            "Pending Status");
    }

    private Appointment CreateTestAppointment(int clientId, int branchId, int appointmentTypeId)
    {
        return new Appointment
        {
            AppointmentNumber = $"APT-{Guid.NewGuid().ToString().Substring(0, 8)}",
            AppointmentDate = DateTime.UtcNow.AddDays(1),
            AppointmentTime = "09:00",
            StatusId = 1,
            ClientId = clientId,
            BranchId = branchId,
            AppointmentTypeId = appointmentTypeId,
            IsActive = true,
            IsEnabled = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    #endregion

    public void Dispose()
    {
        _context?.Dispose();
    }
}
