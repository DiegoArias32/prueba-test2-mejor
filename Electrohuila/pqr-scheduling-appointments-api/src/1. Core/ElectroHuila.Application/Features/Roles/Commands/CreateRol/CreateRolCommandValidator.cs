using FluentValidation;

namespace ElectroHuila.Application.Features.Roles.Commands.CreateRol;

public class CreateRolCommandValidator : AbstractValidator<CreateRolCommand>
{
    public CreateRolCommandValidator()
    {
        RuleFor(x => x.RolDto.Name)
            .NotEmpty()
            .WithMessage("Role name is required")
            .MaximumLength(100)
            .WithMessage("Role name must not exceed 100 characters");

        RuleFor(x => x.RolDto.Code)
            .NotEmpty()
            .WithMessage("Role code is required")
            .MaximumLength(50)
            .WithMessage("Role code must not exceed 50 characters");
    }
}