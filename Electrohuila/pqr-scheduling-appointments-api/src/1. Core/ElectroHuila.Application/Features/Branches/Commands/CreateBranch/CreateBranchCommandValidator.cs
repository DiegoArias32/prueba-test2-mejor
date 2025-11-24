using FluentValidation;

namespace ElectroHuila.Application.Features.Branches.Commands.CreateBranch;

public class CreateBranchCommandValidator : AbstractValidator<CreateBranchCommand>
{
    public CreateBranchCommandValidator()
    {
        RuleFor(x => x.BranchDto.Name)
            .NotEmpty()
            .WithMessage("Branch name is required")
            .MaximumLength(100)
            .WithMessage("Branch name must not exceed 100 characters");

        RuleFor(x => x.BranchDto.Code)
            .NotEmpty()
            .WithMessage("Branch code is required")
            .MaximumLength(20)
            .WithMessage("Branch code must not exceed 20 characters");

        RuleFor(x => x.BranchDto.Address)
            .NotEmpty()
            .WithMessage("Branch address is required")
            .MaximumLength(200)
            .WithMessage("Branch address must not exceed 200 characters");

        RuleFor(x => x.BranchDto.Phone)
            .NotEmpty()
            .WithMessage("Phone number is required")
            .MaximumLength(20)
            .WithMessage("Phone number must not exceed 20 characters");

        RuleFor(x => x.BranchDto.City)
            .NotEmpty()
            .WithMessage("City is required")
            .MaximumLength(100)
            .WithMessage("City must not exceed 100 characters");

        RuleFor(x => x.BranchDto.State)
            .NotEmpty()
            .WithMessage("State is required")
            .MaximumLength(100)
            .WithMessage("State must not exceed 100 characters");
    }
}