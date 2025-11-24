namespace ElectroHuila.Domain.Exceptions.Appointments;

/// <summary>
/// Excepción que se lanza cuando el horario solicitado no está disponible
/// </summary>
public sealed class TimeSlotNotAvailableException : DomainException
{
    /// <summary>
    /// Constructor para excepción de horario no disponible
    /// </summary>
    /// <param name="requestedDate">Fecha solicitada</param>
    /// <param name="requestedTime">Hora solicitada</param>
    /// <param name="branchId">ID de la sucursal</param>
    public TimeSlotNotAvailableException(DateTime requestedDate, string requestedTime, int branchId)
        : base("TIME_SLOT_NOT_AVAILABLE",
               $"The time slot {requestedTime} on {requestedDate:yyyy-MM-dd} is not available at branch {branchId}.",
               new { RequestedDate = requestedDate, RequestedTime = requestedTime, BranchId = branchId })
    {
    }

    /// <summary>
    /// Constructor para excepción de horario no disponible con motivo
    /// </summary>
    /// <param name="requestedDate">Fecha solicitada</param>
    /// <param name="requestedTime">Hora solicitada</param>
    /// <param name="branchId">ID de la sucursal</param>
    /// <param name="reason">Motivo por el cual no está disponible</param>
    public TimeSlotNotAvailableException(DateTime requestedDate, string requestedTime, int branchId, string reason)
        : base("TIME_SLOT_NOT_AVAILABLE",
               $"The time slot {requestedTime} on {requestedDate:yyyy-MM-dd} is not available at branch {branchId}. Reason: {reason}",
               new { RequestedDate = requestedDate, RequestedTime = requestedTime, BranchId = branchId, Reason = reason })
    {
    }
}