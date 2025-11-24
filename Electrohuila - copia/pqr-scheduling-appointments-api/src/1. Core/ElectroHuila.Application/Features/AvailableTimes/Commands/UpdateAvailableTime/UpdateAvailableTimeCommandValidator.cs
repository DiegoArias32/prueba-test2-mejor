using FluentValidation;

namespace ElectroHuila.Application.Features.AvailableTimes.Commands.UpdateAvailableTime;

public class UpdateAvailableTimeCommandValidator : AbstractValidator<UpdateAvailableTimeCommand>
{
    public UpdateAvailableTimeCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id must be greater than 0");

        RuleFor(x => x.AvailableTimeDto.Time)
            .NotEmpty().WithMessage("Time is required")
            .Matches(@"^([01]?[0-9]|2[0-3]):[0-5][0-9]$").WithMessage("Time must be in HH:mm format");

        RuleFor(x => x.AvailableTimeDto.BranchId)
            .GreaterThan(0).WithMessage("Branch ID must be greater than 0");

        RuleFor(x => x.AvailableTimeDto.AppointmentTypeId)
            .GreaterThan(0).When(x => x.AvailableTimeDto.AppointmentTypeId.HasValue)
            .WithMessage("Appointment Type ID must be greater than 0");
    }
}