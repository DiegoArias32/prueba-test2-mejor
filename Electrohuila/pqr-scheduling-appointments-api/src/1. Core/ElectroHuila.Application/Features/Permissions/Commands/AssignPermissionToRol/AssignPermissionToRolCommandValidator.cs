using FluentValidation;

namespace ElectroHuila.Application.Features.Permissions.Commands.AssignPermissionToRol;

public class AssignPermissionToRolCommandValidator : AbstractValidator<AssignPermissionToRolCommand>
{
    public AssignPermissionToRolCommandValidator()
    {
        RuleFor(x => x.Dto.RolId)
            .GreaterThan(0)
            .WithMessage("Role ID must be greater than 0");

        RuleFor(x => x.Dto.FormId)
            .GreaterThan(0)
            .WithMessage("Form ID must be greater than 0");

        RuleFor(x => x.Dto)
            .Must(dto => dto.CanRead || dto.CanCreate || dto.CanUpdate || dto.CanDelete)
            .WithMessage("At least one permission flag must be set to true");
    }
}