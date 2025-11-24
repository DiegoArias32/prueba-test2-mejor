using FluentValidation;

namespace ElectroHuila.Application.Features.Permissions.Commands.CreatePermission;

public class CreatePermissionCommandValidator : AbstractValidator<CreatePermissionCommand>
{
    public CreatePermissionCommandValidator()
    {
        RuleFor(x => x.Dto)
            .Must(dto => dto.CanRead || dto.CanCreate || dto.CanUpdate || dto.CanDelete)
            .WithMessage("At least one permission flag must be set to true");
    }
}