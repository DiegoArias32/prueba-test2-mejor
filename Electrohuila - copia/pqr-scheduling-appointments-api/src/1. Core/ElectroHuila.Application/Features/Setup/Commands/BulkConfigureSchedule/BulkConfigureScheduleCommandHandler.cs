using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Domain.Entities.Appointments;
using MediatR;

namespace ElectroHuila.Application.Features.Setup.Commands.BulkConfigureSchedule;

public class BulkConfigureScheduleCommandHandler : IRequestHandler<BulkConfigureScheduleCommand, Result<object>>
{
    private readonly IAvailableTimeRepository _availableTimeRepository;

    public BulkConfigureScheduleCommandHandler(IAvailableTimeRepository availableTimeRepository)
    {
        _availableTimeRepository = availableTimeRepository;
    }

    public async Task<Result<object>> Handle(BulkConfigureScheduleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var totalCreatedTimes = 0;
            var totalErrors = new List<string>();
            var successfulConfigurations = 0;

            foreach (var config in request.Configurations)
            {
                try
                {
                    if (config.BranchId <= 0 || config.AppointmentTypeId <= 0 || config.Times == null || !config.Times.Any())
                    {
                        totalErrors.Add($"Invalid configuration for branch {config.BranchId}, type {config.AppointmentTypeId}");
                        continue;
                    }

                    var createdTimes = 0;

                    foreach (var timeString in config.Times)
                    {
                        try
                        {
                            if (TimeSpan.TryParse(timeString, out var time))
                            {
                                var availableTime = new AvailableTime
                                {
                                    Time = timeString,
                                    BranchId = config.BranchId,
                                    AppointmentTypeId = config.AppointmentTypeId,
                                    IsActive = true,
                                    CreatedAt = DateTime.UtcNow
                                };

                                await _availableTimeRepository.AddAsync(availableTime);
                                createdTimes++;
                            }
                        }
                        catch (Exception ex)
                        {
                            totalErrors.Add($"Error creating time {timeString} for branch {config.BranchId}: {ex.Message}");
                        }
                    }

                    totalCreatedTimes += createdTimes;
                    if (createdTimes > 0) successfulConfigurations++;
                }
                catch (Exception ex)
                {
                    totalErrors.Add($"Error in configuration branch {config.BranchId}, type {config.AppointmentTypeId}: {ex.Message}");
                }
            }

            var result = new
            {
                message = "Bulk schedule configuration completed",
                processedConfigurations = request.Configurations.Count,
                successfulConfigurations,
                totalCreatedTimes,
                errors = totalErrors
            };

            return Result.Success<object>(result);
        }
        catch (Exception ex)
        {
            return Result.Failure<object>($"Error in bulk configuration: {ex.Message}");
        }
    }
}
