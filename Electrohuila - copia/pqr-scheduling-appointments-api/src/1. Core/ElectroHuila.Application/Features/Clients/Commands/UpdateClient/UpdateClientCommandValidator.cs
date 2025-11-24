using FluentValidation;

namespace ElectroHuila.Application.Features.Clients.Commands.UpdateClient;

public class UpdateClientCommandValidator : AbstractValidator<UpdateClientCommand>
{
    public UpdateClientCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Client ID must be greater than 0");

        RuleFor(x => x.ClientDto.DocumentType)
            .NotEmpty()
            .WithMessage("Document type is required");

        RuleFor(x => x.ClientDto.DocumentNumber)
            .NotEmpty()
            .WithMessage("Document number is required");

        RuleFor(x => x.ClientDto.FullName)
            .NotEmpty()
            .WithMessage("Full name is required");

        RuleFor(x => x.ClientDto.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrEmpty(x.ClientDto.Email))
            .WithMessage("Invalid email format");
    }
}
