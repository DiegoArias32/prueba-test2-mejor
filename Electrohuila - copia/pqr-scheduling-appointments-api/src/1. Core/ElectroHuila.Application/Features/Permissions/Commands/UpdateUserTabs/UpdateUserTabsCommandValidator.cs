using FluentValidation;

namespace ElectroHuila.Application.Features.Permissions.Commands.UpdateUserTabs;

public class UpdateUserTabsCommandValidator : AbstractValidator<UpdateUserTabsCommand>
{
    public UpdateUserTabsCommandValidator()
    {
        RuleFor(x => x.Dto.UserId)
            .GreaterThan(0)
            .WithMessage("User ID must be greater than 0");

        RuleFor(x => x.Dto.AllowedTabs)
            .NotEmpty()
            .WithMessage("Allowed tabs cannot be empty")
            .MaximumLength(500)
            .WithMessage("Allowed tabs must not exceed 500 characters");
    }
}