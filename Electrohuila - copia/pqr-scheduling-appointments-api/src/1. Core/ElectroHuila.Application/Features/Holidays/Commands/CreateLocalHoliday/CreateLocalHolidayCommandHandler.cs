using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Catalogs;
using ElectroHuila.Domain.Entities.Catalogs;
using MediatR;

namespace ElectroHuila.Application.Features.Holidays.Commands.CreateLocalHoliday;

/// <summary>
/// Manejador del comando para crear un festivo local
/// </summary>
public class CreateLocalHolidayCommandHandler : IRequestHandler<CreateLocalHolidayCommand, Result<HolidayDto>>
{
    private readonly IHolidayRepository _holidayRepository;
    private readonly IBranchRepository _branchRepository;
    private readonly IMapper _mapper;

    public CreateLocalHolidayCommandHandler(
        IHolidayRepository holidayRepository,
        IBranchRepository branchRepository,
        IMapper mapper)
    {
        _holidayRepository = holidayRepository;
        _branchRepository = branchRepository;
        _mapper = mapper;
    }

    public async Task<Result<HolidayDto>> Handle(CreateLocalHolidayCommand request, CancellationToken cancellationToken)
    {
        // Validar que la fecha no esté en el pasado
        if (request.Dto.HolidayDate.Date < DateTime.UtcNow.Date)
        {
            return Result.Failure<HolidayDto>("No se puede crear un festivo en el pasado");
        }

        // Verificar que la sucursal exista
        var branch = await _branchRepository.GetByIdAsync(request.Dto.BranchId);
        if (branch == null)
        {
            return Result.Failure<HolidayDto>($"La sucursal con ID {request.Dto.BranchId} no existe");
        }

        // Verificar si ya existe un festivo local en esa fecha para esa sucursal
        var existingHolidays = await _holidayRepository.GetByDateAsync(request.Dto.HolidayDate, cancellationToken);
        if (existingHolidays.Any(h => h.HolidayType == "LOCAL" && h.BranchId == request.Dto.BranchId))
        {
            return Result.Failure<HolidayDto>($"Ya existe un festivo local en esa fecha para la sucursal {branch.Name}");
        }

        // Crear el festivo local
        var holiday = Holiday.CreateLocalHoliday(
            request.Dto.HolidayDate,
            request.Dto.HolidayName,
            request.Dto.BranchId
        );

        await _holidayRepository.AddAsync(holiday);

        var dto = _mapper.Map<HolidayDto>(holiday);
        return Result.Success(dto);
    }
}
