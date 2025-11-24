using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Catalogs;
using MediatR;

namespace ElectroHuila.Application.Features.Catalogs.AppointmentStatuses.Queries.GetAllAppointmentStatuses;

/// <summary>
/// Handler para obtener todos los estados de citas
/// </summary>
public class GetAllAppointmentStatusesQueryHandler
    : IRequestHandler<GetAllAppointmentStatusesQuery, Result<IEnumerable<AppointmentStatusDto>>>
{
    private readonly IAppointmentStatusRepository _repository;

    public GetAllAppointmentStatusesQueryHandler(IAppointmentStatusRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IEnumerable<AppointmentStatusDto>>> Handle(
        GetAllAppointmentStatusesQuery request,
        CancellationToken cancellationToken)
    {
        var statuses = await _repository.GetAllActiveOrderedAsync();

        var statusDtos = statuses.Select(s => new AppointmentStatusDto
        {
            Id = s.Id,
            Code = s.Code,
            Name = s.Name,
            Description = s.Description,
            ColorPrimary = s.ColorPrimary,
            ColorSecondary = s.ColorSecondary,
            ColorText = s.ColorText,
            IconName = s.IconName,
            DisplayOrder = s.DisplayOrder,
            AllowCancellation = s.AllowCancellation,
            IsFinalState = s.IsFinalState,
            IsActive = s.IsActive
        });

        return Result<IEnumerable<AppointmentStatusDto>>.Success(statusDtos);
    }
}
