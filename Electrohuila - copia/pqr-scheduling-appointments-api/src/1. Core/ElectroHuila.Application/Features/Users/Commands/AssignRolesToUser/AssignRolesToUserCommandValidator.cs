using FluentValidation;

namespace ElectroHuila.Application.Features.Users.Commands.AssignRolesToUser;

public class AssignRolesToUserCommandValidator : AbstractValidator<AssignRolesToUserCommand>
{
    public AssignRolesToUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("User ID must be greater than 0");

        RuleFor(x => x.RoleIds)
            .NotEmpty()
            .WithMessage("At least one role must be assigned")
            .Must(roles => roles.All(id => id > 0))
            .WithMessage("All role IDs must be greater than 0");
    }
}