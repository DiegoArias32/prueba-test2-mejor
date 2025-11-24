using FluentValidation;

namespace ElectroHuila.Application.Features.Appointments.Commands.ScheduleSimpleAppointment;

/// <summary>
/// Validador para el comando de agendamiento simple de cita.
/// Valida los datos del cliente y de la cita antes de procesarlos.
/// </summary>
public class ScheduleSimpleAppointmentCommandValidator : AbstractValidator<ScheduleSimpleAppointmentCommand>
{
    public ScheduleSimpleAppointmentCommandValidator()
    {
        // ========== VALIDACIONES DE CLIENTE ==========

        RuleFor(x => x.DocumentType)
            .NotEmpty()
            .WithMessage("El tipo de documento es requerido")
            .Must(BeValidDocumentType)
            .WithMessage("El tipo de documento debe ser CC, TI, CE o RC");

        RuleFor(x => x.DocumentNumber)
            .NotEmpty()
            .WithMessage("El número de documento es requerido")
            .MaximumLength(20)
            .WithMessage("El número de documento no puede exceder 20 caracteres")
            .Matches(@"^[a-zA-Z0-9]+$")
            .WithMessage("El número de documento solo puede contener letras y números");

        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage("El nombre completo es requerido")
            .MaximumLength(200)
            .WithMessage("El nombre completo no puede exceder 200 caracteres")
            .MinimumLength(3)
            .WithMessage("El nombre completo debe tener al menos 3 caracteres");

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("El formato del email es inválido")
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("El email no puede exceder 100 caracteres");

        RuleFor(x => x.Mobile)
            .NotEmpty()
            .WithMessage("El número de celular es requerido")
            .Length(10)
            .WithMessage("El número de celular debe tener exactamente 10 dígitos")
            .Matches(@"^[0-9]+$")
            .WithMessage("El número de celular solo puede contener dígitos");

        RuleFor(x => x.Phone)
            .MaximumLength(20)
            .When(x => !string.IsNullOrEmpty(x.Phone))
            .WithMessage("El teléfono no puede exceder 20 caracteres")
            .Matches(@"^[0-9]+$")
            .When(x => !string.IsNullOrEmpty(x.Phone))
            .WithMessage("El teléfono solo puede contener dígitos");

        RuleFor(x => x.Address)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Address))
            .WithMessage("La dirección no puede exceder 500 caracteres");

        // ========== VALIDACIONES DE CITA ==========

        RuleFor(x => x.BranchId)
            .GreaterThan(0)
            .WithMessage("El ID de la sucursal es requerido y debe ser mayor a 0");

        RuleFor(x => x.AppointmentTypeId)
            .GreaterThan(0)
            .WithMessage("El ID del tipo de cita es requerido y debe ser mayor a 0");

        RuleFor(x => x.AppointmentDate)
            .NotEmpty()
            .WithMessage("La fecha de la cita es requerida")
            .Must(BeValidDate)
            .WithMessage("La fecha debe estar en formato ISO válido (YYYY-MM-DD)")
            .Must(BeFutureDate)
            .When(x => BeValidDate(x.AppointmentDate))
            .WithMessage("La fecha de la cita debe ser futura");

        RuleFor(x => x.AppointmentTime)
            .NotEmpty()
            .WithMessage("La hora de la cita es requerida")
            .Matches(@"^([0-1]?[0-9]|2[0-3]):[0-5][0-9]$")
            .WithMessage("El formato de hora debe ser HH:mm (ejemplo: 09:30)");

        RuleFor(x => x.Observations)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrEmpty(x.Observations))
            .WithMessage("Las observaciones no pueden exceder 1000 caracteres");
    }

    /// <summary>
    /// Valida que el tipo de documento sea uno de los permitidos
    /// </summary>
    private static bool BeValidDocumentType(string documentType)
    {
        if (string.IsNullOrWhiteSpace(documentType))
            return false;

        var validTypes = new[] { "CC", "TI", "CE", "RC" };
        return validTypes.Contains(documentType.ToUpper());
    }

    /// <summary>
    /// Valida que la fecha tenga un formato ISO válido
    /// </summary>
    private static bool BeValidDate(string dateString)
    {
        if (string.IsNullOrWhiteSpace(dateString))
            return false;

        return DateTime.TryParse(dateString, out _);
    }

    /// <summary>
    /// Valida que la fecha sea futura (mayor a hoy)
    /// </summary>
    private static bool BeFutureDate(string dateString)
    {
        if (!DateTime.TryParse(dateString, out var date))
            return false;

        return date.Date >= DateTime.Today;
    }
}
