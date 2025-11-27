using FluentValidation;

namespace ElectroHuila.Application.Features.AppointmentTypes.Commands.UpdateAppointmentType;

public class UpdateAppointmentTypeCommandValidator : AbstractValidator<UpdateAppointmentTypeCommand>
{
    public UpdateAppointmentTypeCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id must be greater than 0");

        // Allow partial updates - only validate fields when they are provided (not empty/default)
        RuleFor(x => x.AppointmentTypeDto.Name)
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.AppointmentTypeDto.Name));

        RuleFor(x => x.AppointmentTypeDto.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.AppointmentTypeDto.Description));

        RuleFor(x => x.AppointmentTypeDto.Icon)
            .MaximumLength(50).WithMessage("Icon must not exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.AppointmentTypeDto.Icon));

        RuleFor(x => x.AppointmentTypeDto.EstimatedTimeMinutes)
            .GreaterThan(0).WithMessage("Estimated time must be greater than 0")
            .LessThanOrEqualTo(480).WithMessage("Estimated time must not exceed 480 minutes (8 hours)")
            .When(x => x.AppointmentTypeDto.EstimatedTimeMinutes > 0);
    }
}