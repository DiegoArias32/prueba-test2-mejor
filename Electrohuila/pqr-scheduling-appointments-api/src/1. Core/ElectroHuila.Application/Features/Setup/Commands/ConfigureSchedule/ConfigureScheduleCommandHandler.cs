using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Domain.Entities.Appointments;
using MediatR;

namespace ElectroHuila.Application.Features.Setup.Commands.ConfigureSchedule;

public class ConfigureScheduleCommandHandler : IRequestHandler<ConfigureScheduleCommand, Result<object>>
{
    private readonly IAvailableTimeRepository _availableTimeRepository;

    public ConfigureScheduleCommandHandler(IAvailableTimeRepository availableTimeRepository)
    {
        _availableTimeRepository = availableTimeRepository;
    }

    public async Task<Result<object>> Handle(ConfigureScheduleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var dto = request.Dto;

            if (dto.BranchId <= 0 || dto.AppointmentTypeId <= 0 || dto.Times == null || !dto.Times.Any())
            {
                return Result.Failure<object>("Invalid configuration data");
            }

            var createdTimes = 0;
            var errors = new List<string>();

            foreach (var timeString in dto.Times)
            {
                try
                {
                    if (TimeSpan.TryParse(timeString, out var time))
                    {
                        var availableTime = new AvailableTime
                        {
                            Time = timeString,
                            BranchId = dto.BranchId,
                            AppointmentTypeId = dto.AppointmentTypeId,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        };

                        await _availableTimeRepository.AddAsync(availableTime);
                        createdTimes++;
                    }
                    else
                    {
                        errors.Add($"Invalid time format: {timeString}");
                    }
                }
                catch (Exception ex)
                {
                    errors.Add($"Error creating time {timeString}: {ex.Message}");
                }
            }

            var result = new
            {
                message = "Schedule configuration completed",
                createdTimes,
                totalTimes = dto.Times.Count,
                errors
            };

            return Result.Success<object>(result);
        }
        catch (Exception ex)
        {
            return Result.Failure<object>($"Error configuring schedule: {ex.Message}");
        }
    }
}
