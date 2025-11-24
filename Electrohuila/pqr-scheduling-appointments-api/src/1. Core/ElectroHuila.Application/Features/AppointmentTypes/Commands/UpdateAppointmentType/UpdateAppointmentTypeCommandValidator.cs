using FluentValidation;

namespace ElectroHuila.Application.Features.AppointmentTypes.Commands.UpdateAppointmentType;

public class UpdateAppointmentTypeCommandValidator : AbstractValidator<UpdateAppointmentTypeCommand>
{
    public UpdateAppointmentTypeCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id must be greater than 0");

        RuleFor(x => x.AppointmentTypeDto.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");

        RuleFor(x => x.AppointmentTypeDto.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters");

        RuleFor(x => x.AppointmentTypeDto.Icon)
            .NotEmpty().WithMessage("Icon is required")
            .MaximumLength(50).WithMessage("Icon must not exceed 50 characters");

        RuleFor(x => x.AppointmentTypeDto.EstimatedTimeMinutes)
            .GreaterThan(0).WithMessage("Estimated time must be greater than 0")
            .LessThanOrEqualTo(480).WithMessage("Estimated time must not exceed 480 minutes (8 hours)");
    }
}