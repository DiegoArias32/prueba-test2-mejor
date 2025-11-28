using AutoMapper;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.Contracts.Notifications;
using ElectroHuila.Application.Contracts.Services;
using ElectroHuila.Application.DTOs.Appointments;
using ElectroHuila.Application.Features.Appointments.Commands.CreateAppointment;
using ElectroHuila.Domain.Entities.Appointments;
using ElectroHuila.Domain.Enums;
using FluentAssertions;
using Moq;
using Xunit;

namespace ElectroHuila.Application.UnitTests.Features.Appointments;

/// <summary>
/// Pruebas unitarias para el manejador de comando CreateAppointmentCommandHandler.
/// Verifica el comportamiento de creación de citas y manejo de errores.
/// </summary>
public class CreateAppointmentCommandHandlerTests
{
    private readonly Mock<IAppointmentRepository> _appointmentRepositoryMock;
    private readonly Mock<IHolidayRepository> _holidayRepositoryMock;
    private readonly Mock<IBranchRepository> _branchRepositoryMock;
    private readonly Mock<IUserAssignmentRepository> _userAssignmentRepositoryMock;
    private readonly Mock<ISystemSettingRepository> _systemSettingRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ISignalRNotificationService> _signalRServiceMock;
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly CreateAppointmentCommandHandler _handler;

    /// <summary>
    /// Inicializa una nueva instancia de las pruebas del manejador de creación de citas.
    /// Configura los mocks necesarios para las dependencias.
    /// </summary>
    public CreateAppointmentCommandHandlerTests()
    {
        _appointmentRepositoryMock = new Mock<IAppointmentRepository>();
        _holidayRepositoryMock = new Mock<IHolidayRepository>();
        _branchRepositoryMock = new Mock<IBranchRepository>();
        _userAssignmentRepositoryMock = new Mock<IUserAssignmentRepository>();
        _systemSettingRepositoryMock = new Mock<ISystemSettingRepository>();
        _mapperMock = new Mock<IMapper>();
        _signalRServiceMock = new Mock<ISignalRNotificationService>();
        _notificationServiceMock = new Mock<INotificationService>();
        _handler = new CreateAppointmentCommandHandler(
            _appointmentRepositoryMock.Object,
            _holidayRepositoryMock.Object,
            _branchRepositoryMock.Object,
            _userAssignmentRepositoryMock.Object,
            _systemSettingRepositoryMock.Object,
            _mapperMock.Object,
            _signalRServiceMock.Object,
            _notificationServiceMock.Object);
    }

    /// <summary>
    /// Verifica que el manejador cree una cita exitosamente cuando se proporcionan datos válidos.
    /// Debe retornar un resultado exitoso con los datos de la cita creada.
    /// </summary>
    [Fact]
    public async Task Handle_Should_Create_Appointment_Successfully()
    {
        // Arrange
        var dto = new CreateAppointmentDto
        {
            ClientId = 1,
            BranchId = 1,
            AppointmentTypeId = 1,
            AppointmentDate = DateTime.UtcNow.AddDays(1),
            AppointmentTime = "10:00 AM"
        };

        var command = new CreateAppointmentCommand(dto);

        var appointment = new Appointment
        {
            Id = 1,
            ClientId = dto.ClientId,
            BranchId = dto.BranchId,
            AppointmentTypeId = dto.AppointmentTypeId,
            AppointmentDate = dto.AppointmentDate,
            AppointmentTime = dto.AppointmentTime,
            StatusId = 1, // Pending
            AppointmentNumber = "APT-001"
        };

        var appointmentDto = new AppointmentDto
        {
            Id = 1,
            AppointmentNumber = "APT-001"
        };

        _mapperMock
            .Setup(x => x.Map<Appointment>(It.IsAny<CreateAppointmentDto>()))
            .Returns(appointment);

        _holidayRepositoryMock
            .Setup(x => x.IsHolidayAsync(It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _appointmentRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Appointment>()))
            .ReturnsAsync(appointment);

        _mapperMock
            .Setup(x => x.Map<AppointmentDto>(It.IsAny<Appointment>()))
            .Returns(appointmentDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.AppointmentNumber.Should().Be("APT-001");

        _appointmentRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Appointment>()),
            Times.Once);
    }

    /// <summary>
    /// Verifica que el manejador retorne un resultado de falla cuando ocurre una excepción.
    /// Debe manejar errores de base de datos y otros errores inesperados correctamente.
    /// </summary>
    [Fact]
    public async Task Handle_Should_Return_Failure_When_Exception_Occurs()
    {
        // Arrange
        var dto = new CreateAppointmentDto
        {
            ClientId = 1,
            BranchId = 1,
            AppointmentTypeId = 1
        };

        var command = new CreateAppointmentCommand(dto);

        _mapperMock
            .Setup(x => x.Map<Appointment>(It.IsAny<CreateAppointmentDto>()))
            .Returns(new Appointment
            {
                ClientId = dto.ClientId,
                BranchId = dto.BranchId,
                AppointmentTypeId = dto.AppointmentTypeId,
                AppointmentDate = DateTime.UtcNow.AddDays(1),
                StatusId = 1
            });

        _holidayRepositoryMock
            .Setup(x => x.IsHolidayAsync(It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _appointmentRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Appointment>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("error");
    }
}