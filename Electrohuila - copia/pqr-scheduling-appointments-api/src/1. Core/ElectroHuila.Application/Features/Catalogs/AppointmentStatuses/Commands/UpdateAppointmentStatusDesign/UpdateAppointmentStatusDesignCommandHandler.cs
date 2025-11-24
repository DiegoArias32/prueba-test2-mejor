using ElectroHuila.Application.Common.Interfaces.Persistence;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Catalogs;
using MediatR;

namespace ElectroHuila.Application.Features.Catalogs.AppointmentStatuses.Commands.UpdateAppointmentStatusDesign;

/// <summary>
/// Handler para actualizar el diseño de un estado de cita
/// </summary>
public class UpdateAppointmentStatusDesignCommandHandler
    : IRequestHandler<UpdateAppointmentStatusDesignCommand, Result<AppointmentStatusDto>>
{
    private readonly IAppointmentStatusRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAppointmentStatusDesignCommandHandler(
        IAppointmentStatusRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AppointmentStatusDto>> Handle(
        UpdateAppointmentStatusDesignCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Obtener el estado
        var status = await _repository.GetByIdAsync(request.Id);

        if (status == null)
            return Result.Failure<AppointmentStatusDto>($"Estado con ID {request.Id} no encontrado");

        // 2. Actualizar colores e icono usando método del dominio
        status.UpdateColors(request.ColorPrimary ?? status.ColorPrimary ?? string.Empty, request.ColorSecondary, request.ColorText);

        if (!string.IsNullOrWhiteSpace(request.IconName))
            status.UpdateIcon(request.IconName);

        // 3. Guardar cambios
        await _repository.UpdateAsync(status);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 4. Retornar DTO
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
