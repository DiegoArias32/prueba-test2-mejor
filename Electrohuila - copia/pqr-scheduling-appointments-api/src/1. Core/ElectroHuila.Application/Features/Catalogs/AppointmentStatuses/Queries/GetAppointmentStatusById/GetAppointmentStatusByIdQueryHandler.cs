using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Catalogs;
using MediatR;

namespace ElectroHuila.Application.Features.Catalogs.AppointmentStatuses.Queries.GetAppointmentStatusById;

/// <summary>
/// Handler para obtener un estado de cita por ID
/// </summary>
public class GetAppointmentStatusByIdQueryHandler
    : IRequestHandler<GetAppointmentStatusByIdQuery, Result<AppointmentStatusDto>>
{
    private readonly IAppointmentStatusRepository _repository;

    public GetAppointmentStatusByIdQueryHandler(IAppointmentStatusRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<AppointmentStatusDto>> Handle(
        GetAppointmentStatusByIdQuery request,
        CancellationToken cancellationToken)
    {
        var status = await _repository.GetByIdAsync(request.Id);

        if (status == null)
            return Result.Failure<AppointmentStatusDto>($"Estado con ID {request.Id} no encontrado");

        var statusDto = new AppointmentStatusDto
        {
            Id = status.Id,
            Code = status.Code,
            Name = status.Name,
            Description = status.Description,
            ColorPrimary = status.ColorPrimary,
            ColorSecondary = status.ColorSecondary,
            ColorText = status.ColorText,
            IconName = status.IconName,
            DisplayOrder = status.DisplayOrder,
            AllowCancellation = status.AllowCancellation,
            IsFinalState = status.IsFinalState,
            IsActive = status.IsActive
        };

        return Result.Success(statusDto);
    }
}
