using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Domain.Entities.Locations;
using ElectroHuila.Domain.Entities.Appointments;
using ElectroHuila.Domain.Entities.Clients;
using MediatR;

namespace ElectroHuila.Application.Features.Setup.Commands.ConfigureInitialData;

public class ConfigureInitialDataCommandHandler : IRequestHandler<ConfigureInitialDataCommand, Result<object>>
{
    private readonly IBranchRepository _branchRepository;
    private readonly IAppointmentTypeRepository _appointmentTypeRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IMapper _mapper;

    public ConfigureInitialDataCommandHandler(
        IBranchRepository branchRepository,
        IAppointmentTypeRepository appointmentTypeRepository,
        IClientRepository clientRepository,
        IMapper mapper)
    {
        _branchRepository = branchRepository;
        _appointmentTypeRepository = appointmentTypeRepository;
        _clientRepository = clientRepository;
        _mapper = mapper;
    }

    public async Task<Result<object>> Handle(ConfigureInitialDataCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var results = new List<string>();
            var errors = new List<string>();

            // Create branches
            if (request.Dto.Branches?.Any() == true)
            {
                foreach (var branchDto in request.Dto.Branches)
                {
                    try
                    {
                        var branch = _mapper.Map<Branch>(branchDto);
                        await _branchRepository.AddAsync(branch);
                        results.Add($"Branch '{branchDto.Name}' created successfully");
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Error creating branch '{branchDto.Name}': {ex.Message}");
                    }
                }
            }

            // Create appointment types
            if (request.Dto.AppointmentTypes?.Any() == true)
            {
                foreach (var typeDto in request.Dto.AppointmentTypes)
                {
                    try
                    {
                        var appointmentType = _mapper.Map<AppointmentType>(typeDto);
                        await _appointmentTypeRepository.AddAsync(appointmentType);
                        results.Add($"Appointment type '{typeDto.Name}' created successfully");
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Error creating appointment type '{typeDto.Name}': {ex.Message}");
                    }
                }
            }

            // Create clients
            if (request.Dto.Clients?.Any() == true)
            {
                foreach (var clientDto in request.Dto.Clients)
                {
                    try
                    {
                        var client = _mapper.Map<Client>(clientDto);
                        await _clientRepository.AddAsync(client);
                        results.Add($"Client '{clientDto.FullName}' created successfully");
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Error creating client '{clientDto.FullName}': {ex.Message}");
                    }
                }
            }

            var result = new
            {
                message = "Initial data configuration completed",
                results,
                errors,
                totalSuccess = results.Count,
                totalErrors = errors.Count
            };

            return Result.Success<object>(result);
        }
        catch (Exception ex)
        {
            return Result.Failure<object>($"Error configuring initial data: {ex.Message}");
        }
    }
}
