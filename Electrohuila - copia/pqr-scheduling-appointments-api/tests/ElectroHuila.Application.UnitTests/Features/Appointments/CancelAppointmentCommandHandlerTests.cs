using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.Features.Appointments.Commands.CancelAppointment;
using ElectroHuila.Domain.Entities.Appointments;
using FluentAssertions;
using Moq;
using Xunit;

namespace ElectroHuila.Application.UnitTests.Features.Appointments;

/// <summary>
/// Pruebas unitarias para el manejador de comando CancelAppointmentCommandHandler.
/// Verifica el comportamiento de cancelación de citas y manejo de errores.
/// </summary>
public class CancelAppointmentCommandHandlerTests
{
    private readonly Mock<IAppointmentRepository> _appointmentRepositoryMock;
    private readonly CancelAppointmentCommandHandler _handler;

    // StatusIds constants
    private const int PENDING_STATUS_ID = 1;
    private const int CONFIRMED_STATUS_ID = 2;
    private const int IN_PROGRESS_STATUS_ID = 3;
    private const int COMPLETED_STATUS_ID = 4;
    private const int CANCELLED_STATUS_ID = 5;

    /// <summary>
    /// Inicializa una nueva instancia de las pruebas del manejador de cancelación de citas.
    /// Configura los mocks necesarios para las dependencias.
    /// </summary>
    public CancelAppointmentCommandHandlerTests()
    {
        _appointmentRepositoryMock = new Mock<IAppointmentRepository>();
        _handler = new CancelAppointmentCommandHandler(_appointmentRepositoryMock.Object);
    }

    /// <summary>
    /// Verifica que el manejador cancele una cita pendiente exitosamente.
    /// Debe actualizar el estado a cancelado y establecer la razón de cancelación.
    /// </summary>
    [Fact]
    public async Task Handle_Should_Cancel_Pending_Appointment_Successfully()
    {
        // Arrange
        var appointmentId = 1;
        var cancellationReason = "El cliente solicitó cambio de fecha";
        var command = new CancelAppointmentCommand(appointmentId, cancellationReason);

        var appointment = new Appointment
        {
            Id = appointmentId,
            AppointmentNumber = "APT-001",
            StatusId = PENDING_STATUS_ID,
            ClientId = 1,
            BranchId = 1,
            AppointmentTypeId = 1,
            AppointmentDate = DateTime.UtcNow.AddDays(1),
            AppointmentTime = "10:00 AM"
        };

        _appointmentRepositoryMock
            .Setup(x => x.GetByIdAsync(appointmentId))
            .ReturnsAsync(appointment);

        _appointmentRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Appointment>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        appointment.StatusId.Should().Be(CANCELLED_STATUS_ID);
        appointment.CancellationReason.Should().Be(cancellationReason);
        appointment.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        _appointmentRepositoryMock.Verify(
            x => x.GetByIdAsync(appointmentId),
            Times.Once);

        _appointmentRepositoryMock.Verify(
            x => x.UpdateAsync(It.Is<Appointment>(a =>
                a.StatusId == CANCELLED_STATUS_ID &&
                a.CancellationReason == cancellationReason)),
            Times.Once);
    }

    /// <summary>
    /// Verifica que el manejador cancele una cita confirmada exitosamente.
    /// Las citas confirmadas también deben poder ser canceladas.
    /// </summary>
    [Fact]
    public async Task Handle_Should_Cancel_Confirmed_Appointment_Successfully()
    {
        // Arrange
        var appointmentId = 2;
        var cancellationReason = "Emergencia médica";
        var command = new CancelAppointmentCommand(appointmentId, cancellationReason);

        var appointment = new Appointment
        {
            Id = appointmentId,
            AppointmentNumber = "APT-002",
            StatusId = CONFIRMED_STATUS_ID,
            ClientId = 1,
            BranchId = 1,
            AppointmentTypeId = 1,
            AppointmentDate = DateTime.UtcNow.AddDays(2),
            AppointmentTime = "02:00 PM"
        };

        _appointmentRepositoryMock
            .Setup(x => x.GetByIdAsync(appointmentId))
            .ReturnsAsync(appointment);

        _appointmentRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Appointment>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        appointment.StatusId.Should().Be(CANCELLED_STATUS_ID);
        appointment.CancellationReason.Should().Be(cancellationReason);
    }

    /// <summary>
    /// Verifica que el manejador cancele una cita en progreso exitosamente.
    /// Incluso las citas que ya comenzaron pueden ser canceladas si es necesario.
    /// </summary>
    [Fact]
    public async Task Handle_Should_Cancel_InProgress_Appointment_Successfully()
    {
        // Arrange
        var appointmentId = 3;
        var cancellationReason = "Cliente no pudo continuar con el servicio";
        var command = new CancelAppointmentCommand(appointmentId, cancellationReason);

        var appointment = new Appointment
        {
            Id = appointmentId,
            AppointmentNumber = "APT-003",
            StatusId = IN_PROGRESS_STATUS_ID,
            ClientId = 1,
            BranchId = 1,
            AppointmentTypeId = 1,
            AppointmentDate = DateTime.UtcNow,
            AppointmentTime = "09:00 AM"
        };

        _appointmentRepositoryMock
            .Setup(x => x.GetByIdAsync(appointmentId))
            .ReturnsAsync(appointment);

        _appointmentRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Appointment>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        appointment.StatusId.Should().Be(CANCELLED_STATUS_ID);
    }

    /// <summary>
    /// Verifica que el manejador retorne un error cuando la cita no existe.
    /// Debe retornar un resultado de falla con mensaje descriptivo.
    /// </summary>
        [Fact]
        public async Task Handle_Should_Return_Failure_When_Appointment_Not_Found()
        {
        // Arrange
        var appointmentId = 999;
        var cancellationReason = "No importa";
        var command = new CancelAppointmentCommand(appointmentId, cancellationReason);

        _appointmentRepositoryMock
            .Setup(x => x.GetByIdAsync(appointmentId))
            .ReturnsAsync((Appointment?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Appointment not found");

        _appointmentRepositoryMock.Verify(
            x => x.GetByIdAsync(appointmentId),
            Times.Once);

        _appointmentRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<Appointment>()),
            Times.Never);
    }

    /// <summary>
    /// Verifica que el manejador retorne un error cuando se intenta cancelar una cita ya cancelada.
    /// No debe permitir cancelar una cita que ya tiene estado cancelado.
    /// </summary>
    [Fact]
    public async Task Handle_Should_Return_Failure_When_Appointment_Already_Cancelled()
    {
        // Arrange
        var appointmentId = 4;
        var cancellationReason = "Intentando cancelar de nuevo";
        var command = new CancelAppointmentCommand(appointmentId, cancellationReason);

        var appointment = new Appointment
        {
            Id = appointmentId,
            AppointmentNumber = "APT-004",
            StatusId = CANCELLED_STATUS_ID,
            CancellationReason = "Ya fue cancelada anteriormente",
            ClientId = 1,
            BranchId = 1,
            AppointmentTypeId = 1,
            AppointmentDate = DateTime.UtcNow.AddDays(1),
            AppointmentTime = "11:00 AM"
        };

        _appointmentRepositoryMock
            .Setup(x => x.GetByIdAsync(appointmentId))
            .ReturnsAsync(appointment);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Appointment is already cancelled");

        _appointmentRepositoryMock.Verify(
            x => x.GetByIdAsync(appointmentId),
            Times.Once);

        _appointmentRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<Appointment>()),
            Times.Never);
    }

    /// <summary>
    /// Verifica que el manejador retorne un error cuando se intenta cancelar una cita completada.
    /// Las citas completadas no pueden ser canceladas ya que el servicio fue prestado.
    /// </summary>
    [Fact]
    public async Task Handle_Should_Return_Failure_When_Appointment_Already_Completed()
    {
        // Arrange
        var appointmentId = 5;
        var cancellationReason = "Intentando cancelar completada";
        var command = new CancelAppointmentCommand(appointmentId, cancellationReason);

        var appointment = new Appointment
        {
            Id = appointmentId,
            AppointmentNumber = "APT-005",
            StatusId = COMPLETED_STATUS_ID,
            CompletedDate = DateTime.UtcNow.AddHours(-2),
            ClientId = 1,
            BranchId = 1,
            AppointmentTypeId = 1,
            AppointmentDate = DateTime.UtcNow,
            AppointmentTime = "08:00 AM"
        };

        _appointmentRepositoryMock
            .Setup(x => x.GetByIdAsync(appointmentId))
            .ReturnsAsync(appointment);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Cannot cancel a completed appointment");

        _appointmentRepositoryMock.Verify(
            x => x.GetByIdAsync(appointmentId),
            Times.Once);

        _appointmentRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<Appointment>()),
            Times.Never);
    }

    /// <summary>
    /// Verifica que el manejador maneje correctamente excepciones de base de datos al buscar la cita.
    /// Debe capturar la excepción y retornar un resultado de falla con mensaje de error.
    /// </summary>
    [Fact]
    public async Task Handle_Should_Return_Failure_When_Repository_GetById_Throws_Exception()
    {
        // Arrange
        var appointmentId = 6;
        var cancellationReason = "Razón válida";
        var command = new CancelAppointmentCommand(appointmentId, cancellationReason);

        _appointmentRepositoryMock
            .Setup(x => x.GetByIdAsync(appointmentId))
            .ThrowsAsync(new Exception("Database connection error"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Error cancelling appointment");
        result.Error.Should().Contain("Database connection error");

        _appointmentRepositoryMock.Verify(
            x => x.GetByIdAsync(appointmentId),
            Times.Once);
    }

    /// <summary>
    /// Verifica que el manejador maneje correctamente excepciones de base de datos al actualizar la cita.
    /// Debe capturar la excepción y retornar un resultado de falla.
    /// </summary>
    [Fact]
    public async Task Handle_Should_Return_Failure_When_Repository_Update_Throws_Exception()
    {
        // Arrange
        var appointmentId = 7;
        var cancellationReason = "Razón válida";
        var command = new CancelAppointmentCommand(appointmentId, cancellationReason);

        var appointment = new Appointment
        {
            Id = appointmentId,
            AppointmentNumber = "APT-007",
            StatusId = PENDING_STATUS_ID,
            ClientId = 1,
            BranchId = 1,
            AppointmentTypeId = 1,
            AppointmentDate = DateTime.UtcNow.AddDays(1),
            AppointmentTime = "03:00 PM"
        };

        _appointmentRepositoryMock
            .Setup(x => x.GetByIdAsync(appointmentId))
            .ReturnsAsync(appointment);

        _appointmentRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Appointment>()))
            .ThrowsAsync(new Exception("Failed to update appointment in database"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Error cancelling appointment");
        result.Error.Should().Contain("Failed to update appointment");

        _appointmentRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<Appointment>()),
            Times.Once);
    }

    /// <summary>
    /// Verifica que el manejador respete el token de cancelación.
    /// Debe pasar el CancellationToken a los métodos del repositorio.
    /// </summary>
    [Fact]
    public async Task Handle_Should_Pass_CancellationToken_To_Repository()
    {
        // Arrange
        var appointmentId = 8;
        var cancellationReason = "Razón válida";
        var command = new CancelAppointmentCommand(appointmentId, cancellationReason);
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var appointment = new Appointment
        {
            Id = appointmentId,
            AppointmentNumber = "APT-008",
            StatusId = PENDING_STATUS_ID,
            ClientId = 1,
            BranchId = 1,
            AppointmentTypeId = 1,
            AppointmentDate = DateTime.UtcNow.AddDays(1),
            AppointmentTime = "04:00 PM"
        };

        _appointmentRepositoryMock
            .Setup(x => x.GetByIdAsync(appointmentId))
            .ReturnsAsync(appointment);

        _appointmentRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Appointment>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
    }

    /// <summary>
    /// Verifica que se actualice correctamente el campo UpdatedAt al cancelar una cita.
    /// El timestamp debe reflejar el momento de la cancelación.
    /// </summary>
    [Fact]
    public async Task Handle_Should_Update_Timestamp_When_Cancelling_Appointment()
    {
        // Arrange
        var appointmentId = 9;
        var cancellationReason = "Verificando timestamp";
        var command = new CancelAppointmentCommand(appointmentId, cancellationReason);
        var beforeCancellation = DateTime.UtcNow;

        var appointment = new Appointment
        {
            Id = appointmentId,
            AppointmentNumber = "APT-009",
            StatusId = PENDING_STATUS_ID,
            ClientId = 1,
            BranchId = 1,
            AppointmentTypeId = 1,
            AppointmentDate = DateTime.UtcNow.AddDays(1),
            AppointmentTime = "05:00 PM",
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        _appointmentRepositoryMock
            .Setup(x => x.GetByIdAsync(appointmentId))
            .ReturnsAsync(appointment);

        _appointmentRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Appointment>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        appointment.UpdatedAt.Should().BeOnOrAfter(beforeCancellation);
        appointment.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    /// <summary>
    /// Verifica que el manejador preserve otros campos de la cita al cancelarla.
    /// Solo debe modificar StatusId, CancellationReason y UpdatedAt.
    /// </summary>
    [Theory]
    [InlineData(PENDING_STATUS_ID)]
    [InlineData(CONFIRMED_STATUS_ID)]
    [InlineData(IN_PROGRESS_STATUS_ID)]
    public async Task Handle_Should_Preserve_Other_Fields_When_Cancelling(int initialStatusId)
    {
        // Arrange
        var appointmentId = 10;
        var cancellationReason = "Preservando campos";
        var command = new CancelAppointmentCommand(appointmentId, cancellationReason);

        var originalNumber = "APT-010";
        var originalDate = DateTime.UtcNow.AddDays(3);
        var originalTime = "06:00 PM";
        var originalNotes = "Notas importantes";

        var appointment = new Appointment
        {
            Id = appointmentId,
            AppointmentNumber = originalNumber,
            StatusId = initialStatusId,
            AppointmentDate = originalDate,
            AppointmentTime = originalTime,
            Notes = originalNotes,
            ClientId = 1,
            BranchId = 2,
            AppointmentTypeId = 3
        };

        _appointmentRepositoryMock
            .Setup(x => x.GetByIdAsync(appointmentId))
            .ReturnsAsync(appointment);

        _appointmentRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Appointment>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        // Verificar que los campos originales no cambiaron
        appointment.AppointmentNumber.Should().Be(originalNumber);
        appointment.AppointmentDate.Should().Be(originalDate);
        appointment.AppointmentTime.Should().Be(originalTime);
        appointment.Notes.Should().Be(originalNotes);
        appointment.ClientId.Should().Be(1);
        appointment.BranchId.Should().Be(2);
        appointment.AppointmentTypeId.Should().Be(3);

        // Verificar que solo se actualizaron los campos esperados
        appointment.StatusId.Should().Be(CANCELLED_STATUS_ID);
        appointment.CancellationReason.Should().Be(cancellationReason);
    }
}
