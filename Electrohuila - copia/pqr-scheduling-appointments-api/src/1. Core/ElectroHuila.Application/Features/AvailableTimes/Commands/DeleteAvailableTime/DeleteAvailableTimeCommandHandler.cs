using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using MediatR;

namespace ElectroHuila.Application.Features.AvailableTimes.Commands.DeleteAvailableTime;

public class DeleteAvailableTimeCommandHandler : IRequestHandler<DeleteAvailableTimeCommand, Result<bool>>
{
    private readonly IAvailableTimeRepository _availableTimeRepository;

    public DeleteAvailableTimeCommandHandler(IAvailableTimeRepository availableTimeRepository)
    {
        _availableTimeRepository = availableTimeRepository;
    }

    public async Task<Result<bool>> Handle(DeleteAvailableTimeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var availableTime = await _availableTimeRepository.GetByIdAsync(request.Id);
            if (availableTime == null)
            {
                return Result.Failure<bool>($"Available time with ID {request.Id} not found");
            }

            // Soft delete
            availableTime.IsActive = false;
            await _availableTimeRepository.UpdateAsync(availableTime);

            return Result.Success(true);
        }
        catch (Exception ex)
        {
            return Result.Failure<bool>($"Error deleting available time: {ex.Message}");
        }
    }
}