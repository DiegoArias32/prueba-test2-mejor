using AutoMapper;
using ElectroHuila.Domain.Entities.Appointments;
using ElectroHuila.Domain.Entities.Catalogs;
using ElectroHuila.Domain.Entities.Clients;
using ElectroHuila.Domain.Entities.Locations;
using ElectroHuila.Domain.Entities.Security;
using ElectroHuila.Domain.Entities.Settings;
using ElectroHuila.Domain.Entities.Notifications;
using ElectroHuila.Domain.Entities.Assignments;
using ElectroHuila.Application.DTOs.Appointments;
using ElectroHuila.Application.DTOs.AppointmentTypes;
using ElectroHuila.Application.DTOs.Branches;
using ElectroHuila.Application.DTOs.Catalogs;
using ElectroHuila.Application.DTOs.Clients;
using ElectroHuila.Application.DTOs.Users;
using ElectroHuila.Application.DTOs.Settings;
using ElectroHuila.Application.DTOs.Notifications;
using ElectroHuila.Application.DTOs.Assignments;

namespace ElectroHuila.Application.Mappings;

/// <summary>
/// Perfil de AutoMapper que define los mapeos entre entidades del dominio y DTOs
/// </summary>
public class MappingProfile : Profile
{
    /// <summary>
    /// Constructor que configura todos los mapeos de la aplicación
    /// </summary>
    public MappingProfile()
    {
        // Mapeos de Citas (Appointments)
        CreateMap<Appointment, AppointmentDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status != null ? src.Status.Name : string.Empty))
            .ReverseMap();
        CreateMap<Appointment, CreateAppointmentDto>().ReverseMap();
        CreateMap<Appointment, UpdateAppointmentDto>().ReverseMap();

        // Mapeo de Appointment a AppointmentDetailDto con datos completos
        CreateMap<Appointment, AppointmentDetailDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status != null ? src.Status.Name : string.Empty))
            .ForMember(dest => dest.Client, opt => opt.MapFrom(src => new ClientSummaryDto
            {
                Id = src.Client.Id,
                ClientNumber = src.Client.ClientNumber,
                FullName = src.Client.FullName,
                Email = src.Client.Email,
                PhoneNumber = src.Client.Phone,
                DocumentNumber = src.Client.DocumentNumber
            }))
            .ForMember(dest => dest.Branch, opt => opt.MapFrom(src => new BranchSummaryDto
            {
                Id = src.Branch.Id,
                Code = src.Branch.Code,
                Name = src.Branch.Name,
                Address = src.Branch.Address,
                City = src.Branch.City
            }))
            .ForMember(dest => dest.AppointmentType, opt => opt.MapFrom(src => new AppointmentTypeSummaryDto
            {
                Id = src.AppointmentType.Id,
                Code = src.AppointmentType.Code,
                Name = src.AppointmentType.Name,
                Description = src.AppointmentType.Description,
                IconName = src.AppointmentType.IconName,
                ColorPrimary = src.AppointmentType.ColorPrimary,
                ColorSecondary = src.AppointmentType.ColorSecondary
            }));

        // Mapeos de Tipos de Cita (AppointmentTypes)
        CreateMap<AppointmentType, AppointmentTypeDto>().ReverseMap();
        CreateMap<CreateAppointmentTypeDto, AppointmentType>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
        CreateMap<UpdateAppointmentTypeDto, AppointmentType>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        // Mapeos de Clientes (Clients)
        CreateMap<Client, ClientDto>()
            .ForMember(dest => dest.DocumentType, opt => opt.MapFrom(src => src.DocumentType.ToString()))
            .ReverseMap()
            .ForMember(dest => dest.DocumentType, opt => opt.MapFrom(src => Enum.Parse<ElectroHuila.Domain.Enums.DocumentType>(src.DocumentType)));

        CreateMap<Client, CreateClientDto>()
            .ForMember(dest => dest.DocumentType, opt => opt.MapFrom(src => src.DocumentType.ToString()))
            .ReverseMap()
            .ForMember(dest => dest.DocumentType, opt => opt.MapFrom(src => Enum.Parse<ElectroHuila.Domain.Enums.DocumentType>(src.DocumentType)));

        CreateMap<Client, UpdateClientDto>()
            .ForMember(dest => dest.DocumentType, opt => opt.MapFrom(src => src.DocumentType.ToString()))
            .ReverseMap()
            .ForMember(dest => dest.DocumentType, opt => opt.MapFrom(src => Enum.Parse<ElectroHuila.Domain.Enums.DocumentType>(src.DocumentType)));

        // Mapeos de Sucursales (Branches)
        CreateMap<Branch, BranchDto>().ReverseMap();
        CreateMap<Branch, CreateBranchDto>().ReverseMap();
        CreateMap<Branch, UpdateBranchDto>().ReverseMap();

        // Mapeos de Usuarios (Users)
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src =>
                src.RolUsers.Select(ru => ru.Rol.Name)))
            .ReverseMap()
            .ForMember(dest => dest.RolUsers, opt => opt.Ignore());
        CreateMap<CreateUserDto, User>()
            .ForMember(dest => dest.RolUsers, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
        CreateMap<UpdateUserDto, User>()
            .ForMember(dest => dest.RolUsers, opt => opt.Ignore())
            .ForMember(dest => dest.Password, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        // Mapeo de User a UserDetailsDto con roles
        CreateMap<User, ElectroHuila.Application.DTOs.Users.UserDetailsDto>()
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src =>
                src.RolUsers.Select(ru => new ElectroHuila.Application.DTOs.Users.UserRoleDto
                {
                    RoleId = ru.RolId,
                    RoleName = ru.Rol.Name,
                    RoleCode = ru.Rol.Code
                })));

        // Mapeos de Permisos (Permissions)
        CreateMap<Permission, ElectroHuila.Application.DTOs.Permissions.PermissionDto>().ReverseMap();

        // Mapeos de Roles (Roles)
        CreateMap<Rol, ElectroHuila.Application.DTOs.Roles.RolDto>().ReverseMap();
        CreateMap<ElectroHuila.Application.DTOs.Roles.CreateRolDto, Rol>();
        CreateMap<ElectroHuila.Application.DTOs.Roles.UpdateRolDto, Rol>();

        // Mapeos de Catálogos
        CreateMap<AppointmentStatus, AppointmentStatusDto>().ReverseMap();
        CreateMap<ProjectType, ProjectTypeDto>().ReverseMap();
        CreateMap<PropertyType, PropertyTypeDto>().ReverseMap();
        CreateMap<ServiceUseType, ServiceUseTypeDto>().ReverseMap();

        // Mapeos de Configuración
        CreateMap<ThemeSettings, ThemeSettingsDto>().ReverseMap();
        CreateMap<ThemeSettings, UpdateThemeSettingsDto>().ReverseMap();

        // Mapeos de Horarios Disponibles (AvailableTimes)
        CreateMap<AvailableTime, ElectroHuila.Application.DTOs.AvailableTimes.AvailableTimeDto>()
            .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Branch != null ? src.Branch.Name : null))
            .ForMember(dest => dest.AppointmentTypeName, opt => opt.MapFrom(src => src.AppointmentType != null ? src.AppointmentType.Name : null));
        CreateMap<ElectroHuila.Application.DTOs.AvailableTimes.CreateAvailableTimeDto, AvailableTime>()
            .ForMember(dest => dest.Branch, opt => opt.Ignore())
            .ForMember(dest => dest.AppointmentType, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
        CreateMap<ElectroHuila.Application.DTOs.AvailableTimes.UpdateAvailableTimeDto, AvailableTime>()
            .ForMember(dest => dest.Branch, opt => opt.Ignore())
            .ForMember(dest => dest.AppointmentType, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        // Mapeos de Configuración del Sistema (SystemSettings)
        CreateMap<SystemSetting, SystemSettingDto>().ReverseMap();
        CreateMap<SystemSetting, CreateSystemSettingDto>().ReverseMap();
        CreateMap<SystemSetting, UpdateSystemSettingDto>().ReverseMap();

        // Mapeos de Notificaciones (Notifications)
        CreateMap<Notification, NotificationDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.Username : null))
            .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Client != null ? src.Client.FullName : null))
            .ForMember(dest => dest.AppointmentNumber, opt => opt.MapFrom(src => src.Appointment != null ? src.Appointment.AppointmentNumber : null));

        CreateMap<Notification, NotificationListDto>()
            .ForMember(dest => dest.MessagePreview, opt => opt.MapFrom(src =>
                src.Message.Length > 100 ? src.Message.Substring(0, 100) + "..." : src.Message))
            .ForMember(dest => dest.AppointmentNumber, opt => opt.MapFrom(src => src.Appointment != null ? src.Appointment.AppointmentNumber : null))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.Username : null))
            .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Client != null ? src.Client.FullName : null));

        // Mapeos de Festivos (Holidays)
        CreateMap<Holiday, HolidayDto>()
            .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Branch != null ? src.Branch.Name : null));
        CreateMap<HolidayDto, Holiday>()
            .ForMember(dest => dest.Branch, opt => opt.Ignore());
        CreateMap<CreateNationalHolidayDto, Holiday>()
            .ForMember(dest => dest.HolidayType, opt => opt.MapFrom(src => "NATIONAL"))
            .ForMember(dest => dest.BranchId, opt => opt.MapFrom(src => (int?)null))
            .ForMember(dest => dest.Branch, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
        CreateMap<CreateLocalHolidayDto, Holiday>()
            .ForMember(dest => dest.HolidayType, opt => opt.MapFrom(src => "LOCAL"))
            .ForMember(dest => dest.Branch, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
        CreateMap<CreateCompanyHolidayDto, Holiday>()
            .ForMember(dest => dest.HolidayType, opt => opt.MapFrom(src => "COMPANY"))
            .ForMember(dest => dest.BranchId, opt => opt.MapFrom(src => (int?)null))
            .ForMember(dest => dest.Branch, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
        CreateMap<UpdateHolidayDto, Holiday>()
            .ForMember(dest => dest.Branch, opt => opt.Ignore())
            .ForMember(dest => dest.HolidayType, opt => opt.Ignore())
            .ForMember(dest => dest.BranchId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        // Mapeos de Documentos de Citas (AppointmentDocuments)
        CreateMap<AppointmentDocument, AppointmentDocumentDto>()
            .ForMember(dest => dest.FileSizeFormatted, opt => opt.MapFrom(src => src.GetFileSizeFormatted()))
            .ForMember(dest => dest.IsImage, opt => opt.MapFrom(src => src.IsImage()))
            .ForMember(dest => dest.IsPdf, opt => opt.MapFrom(src => src.IsPdf()))
            .ForMember(dest => dest.UploadedByName, opt => opt.MapFrom(src => src.UploadedByUser != null ? src.UploadedByUser.Username : null));
        CreateMap<CreateAppointmentDocumentDto, AppointmentDocument>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Appointment, opt => opt.Ignore())
            .ForMember(dest => dest.UploadedByUser, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        // Mapeos de Asignaciones de Usuarios a Tipos de Cita (UserAppointmentTypeAssignments)
        CreateMap<UserAppointmentTypeAssignment, UserAssignmentDto>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User != null ? src.User.Username : string.Empty))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User != null ? src.User.Email : string.Empty))
            .ForMember(dest => dest.AppointmentTypeCode, opt => opt.MapFrom(src => src.AppointmentType != null ? src.AppointmentType.Code : string.Empty))
            .ForMember(dest => dest.AppointmentTypeName, opt => opt.MapFrom(src => src.AppointmentType != null ? src.AppointmentType.Name : string.Empty))
            .ForMember(dest => dest.AppointmentTypeDescription, opt => opt.MapFrom(src => src.AppointmentType != null ? src.AppointmentType.Description : null))
            .ForMember(dest => dest.AppointmentTypeIcon, opt => opt.MapFrom(src => src.AppointmentType != null ? src.AppointmentType.IconName : null))
            .ForMember(dest => dest.AppointmentTypeColor, opt => opt.MapFrom(src => src.AppointmentType != null ? src.AppointmentType.ColorPrimary : null));
    }
}