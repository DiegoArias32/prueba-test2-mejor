using FluentValidation;

namespace ElectroHuila.Application.Features.Clients.Commands.CreateClient;

public class CreateClientCommandValidator : AbstractValidator<CreateClientCommand>
{
    public CreateClientCommandValidator()
    {
        RuleFor(x => x.ClientDto.DocumentType)
            .NotEmpty()
            .WithMessage("Document type is required");

        RuleFor(x => x.ClientDto.DocumentNumber)
            .NotEmpty()
            .WithMessage("Document number is required")
            .MinimumLength(6)
            .WithMessage("Document number must be at least 6 characters");

        RuleFor(x => x.ClientDto.FullName)
            .NotEmpty()
            .WithMessage("Full name is required")
            .MinimumLength(2)
            .WithMessage("Full name must be at least 2 characters");

        RuleFor(x => x.ClientDto.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Valid email address is required");

        RuleFor(x => x.ClientDto.Mobile)
            .NotEmpty()
            .WithMessage("Mobile phone is required")
            .Matches(@"^3\d{9}$")
            .WithMessage("Mobile phone must be a valid Colombian mobile number");

        RuleFor(x => x.ClientDto.Address)
            .NotEmpty()
            .WithMessage("Address is required")
            .MinimumLength(10)
            .WithMessage("Address must be at least 10 characters");
    }
}