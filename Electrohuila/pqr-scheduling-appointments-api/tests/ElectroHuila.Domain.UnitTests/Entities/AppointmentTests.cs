using ElectroHuila.Domain.Entities.Appointments;
using ElectroHuila.Domain.Entities.Clients;
using ElectroHuila.Domain.Entities.Locations;
using ElectroHuila.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace ElectroHuila.Domain.UnitTests.Entities;

/// <summary>
/// Pruebas unitarias para la entidad Appointment del dominio.
/// Verifica el comportamiento, propiedades y reglas de negocio de las citas.
/// </summary>
public class AppointmentTests
{
    /// <summary>
    /// Verifica que una cita se puede crear correctamente con datos válidos.
    /// Debe establecer todas las propiedades necesarias y mantener integridad referencial.
    /// </summary>
    [Fact]
    public void Appointment_Should_Be_Created_With_Valid_Data()
    {
        // Arrange
        var client = new Client
        {
            Id = 1,
            FullName = "John Doe",
            DocumentNumber = "123456789"
        };

        var branch = new Branch
        {
            Id = 1,
            Name = "Main Branch",
            Code = "MAIN001"
        };

        // Act
        var appointment = new Appointment
        {
            ClientId = client.Id,
            Client = client,
            BranchId = branch.Id,
            Branch = branch,
            AppointmentDate = DateTime.UtcNow.AddDays(1),
            AppointmentTime = "10:00 AM",
            Status = AppointmentStatus.Confirmed,
            AppointmentNumber = "APT-001"
        };

        // Assert
        appointment.Should().NotBeNull();
        appointment.ClientId.Should().Be(1);
        appointment.BranchId.Should().Be(1);
        appointment.Status.Should().Be(AppointmentStatus.Confirmed);
        appointment.AppointmentNumber.Should().Be("APT-001");
    }

    /// <summary>
    /// Verifica que el estado de una cita se puede actualizar correctamente.
    /// Debe permitir cambios de estado según el flujo de vida de la cita.
    /// </summary>
    [Fact]
    public void Appointment_Status_Should_Be_Updatable()
    {
        // Arrange
        var appointment = new Appointment
        {
            Status = AppointmentStatus.Pending,
            AppointmentNumber = "APT-002"
        };

        // Act
        appointment.Status = AppointmentStatus.Confirmed;

        // Assert
        appointment.Status.Should().Be(AppointmentStatus.Confirmed);
    }

    /// <summary>
    /// Verifica que una cita tiene las propiedades de auditoría correctamente configuradas.
    /// Debe incluir fecha de creación y estado de activación para control de datos.
    /// </summary>
    [Fact]
    public void Appointment_Should_Have_CreatedAt_And_IsActive()
    {
        // Arrange & Act
        var appointment = new Appointment
        {
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        // Assert
        appointment.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        appointment.IsActive.Should().BeTrue();
    }
}