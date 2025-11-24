using FluentValidation;

namespace ElectroHuila.Application.Features.Appointments.Commands.CreateAppointment;

public class CreateAppointmentCommandValidator : AbstractValidator<CreateAppointmentCommand>
{
    public CreateAppointmentCommandValidator()
    {
        RuleFor(x => x.AppointmentDto.AppointmentDate)
            .NotEmpty()
            .GreaterThan(DateTime.Today)
            .WithMessage("Appointment date must be in the future");

        RuleFor(x => x.AppointmentDto.ClientId)
            .GreaterThan(0)
            .WithMessage("Client ID is required");

        RuleFor(x => x.AppointmentDto.BranchId)
            .GreaterThan(0)
            .WithMessage("Branch ID is required");

        RuleFor(x => x.AppointmentDto.AppointmentTypeId)
            .GreaterThan(0)
            .WithMessage("Appointment Type ID is required");
    }
}