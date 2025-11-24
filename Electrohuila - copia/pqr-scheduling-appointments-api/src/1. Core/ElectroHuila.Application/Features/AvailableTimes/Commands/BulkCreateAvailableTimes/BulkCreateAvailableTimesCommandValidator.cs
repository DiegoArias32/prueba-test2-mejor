using FluentValidation;

namespace ElectroHuila.Application.Features.AvailableTimes.Commands.BulkCreateAvailableTimes;

public class BulkCreateAvailableTimesCommandValidator : AbstractValidator<BulkCreateAvailableTimesCommand>
{
    public BulkCreateAvailableTimesCommandValidator()
    {
        RuleFor(x => x.Dto.BranchId)
            .GreaterThan(0).WithMessage("Branch ID must be greater than 0");

        RuleFor(x => x.Dto.AppointmentTypeId)
            .GreaterThan(0).When(x => x.Dto.AppointmentTypeId.HasValue)
            .WithMessage("Appointment Type ID must be greater than 0");

        RuleFor(x => x.Dto.TimeSlots)
            .NotEmpty().WithMessage("Time slots list cannot be empty")
            .Must(x => x != null && x.Count > 0).WithMessage("At least one time slot is required");

        RuleForEach(x => x.Dto.TimeSlots)
            .ChildRules(timeSlot =>
            {
                timeSlot.RuleFor(x => x.Time)
                    .NotEmpty().WithMessage("Time is required")
                    .Matches(@"^([01]?[0-9]|2[0-3]):[0-5][0-9]$").WithMessage("Time must be in HH:mm format");
            });
    }
}