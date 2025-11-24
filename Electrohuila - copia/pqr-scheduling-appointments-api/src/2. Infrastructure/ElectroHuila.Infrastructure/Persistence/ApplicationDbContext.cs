using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.Common.Interfaces.Persistence;
using ElectroHuila.Domain.Entities.Common;
using ElectroHuila.Domain.Entities.Appointments;
using ElectroHuila.Domain.Entities.Clients;
using ElectroHuila.Domain.Entities.Locations;
using ElectroHuila.Domain.Entities.Security;
using ElectroHuila.Domain.Entities.Catalogs;
using ElectroHuila.Domain.Entities.Settings;
using ElectroHuila.Domain.Entities.Notifications;
using ElectroHuila.Domain.Entities.Assignments;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ElectroHuila.Infrastructure.Persistence;

/// <summary>
/// Contexto de base de datos principal de la aplicación ElectroHuila.
/// Implementa IUnitOfWork para transacciones y manejo de cambios.
/// Configura Entity Framework Core para trabajar con Oracle Database.
/// </summary>
public class ApplicationDbContext : DbContext, IUnitOfWork
{
    /// <summary>
    /// Constructor del contexto de base de datos.
    /// </summary>
    /// <param name="options">Opciones de configuración del DbContext (cadena de conexión, provider, etc).</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Configura las convenciones del modelo antes de que se construya.
    /// Este es el lugar apropiado para configurar conversiones de tipos globales.
    /// </summary>
    /// <param name="configurationBuilder">Constructor de configuración de convenciones.</param>
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        // Convertir todos los bool a NUMBER(1) para Oracle (0=false, 1=true)
        // Esto se aplica ANTES de que se construya el modelo, asegurando que funcione en todas las queries
        configurationBuilder
            .Properties<bool>()
            .HaveConversion<Microsoft.EntityFrameworkCore.Storage.ValueConversion.BoolToZeroOneConverter<int>>();
    }

    // ========== DBSETS DE AGENDAMIENTO ==========
    /// <summary>
    /// Tabla de citas agendadas por los clientes.
    /// </summary>
    public DbSet<Appointment> Appointments { get; set; }

    /// <summary>
    /// Tabla de tipos de cita (PQR, Solicitudes, etc).
    /// </summary>
    public DbSet<AppointmentType> AppointmentTypes { get; set; }

    /// <summary>
    /// Tabla de horarios disponibles para agendamiento.
    /// </summary>
    public DbSet<AvailableTime> AvailableTimes { get; set; }

    // ========== DBSETS DE UBICACIÓN ==========
    /// <summary>
    /// Tabla de sucursales/sedes de ElectroHuila.
    /// </summary>
    public DbSet<Branch> Branches { get; set; }

    // ========== DBSETS DE CLIENTES ==========
    /// <summary>
    /// Tabla de clientes que agendan citas.
    /// </summary>
    public DbSet<Client> Clients { get; set; }

    // ========== DBSETS DE SEGURIDAD ==========
    /// <summary>
    /// Tabla de usuarios del sistema (administradores, operadores, etc).
    /// </summary>
    public DbSet<User> Users { get; set; }

    /// <summary>
    /// Tabla de roles de usuario (Admin, Operador, Supervisor, etc).
    /// </summary>
    public DbSet<Rol> Roles { get; set; }

    /// <summary>
    /// Tabla de permisos del sistema (Crear, Leer, Actualizar, Eliminar).
    /// </summary>
    public DbSet<Permission> Permissions { get; set; }

    /// <summary>
    /// Tabla de formularios/pantallas del sistema.
    /// </summary>
    public DbSet<Form> Forms { get; set; }

    /// <summary>
    /// Tabla de módulos del sistema que agrupan formularios.
    /// </summary>
    public DbSet<ElectroHuila.Domain.Entities.Security.Module> Modules { get; set; }

    // ========== DBSETS DE RELACIONES MUCHOS A MUCHOS ==========
    /// <summary>
    /// Tabla de relación entre roles y usuarios (un usuario puede tener múltiples roles).
    /// </summary>
    public DbSet<RolUser> RolUsers { get; set; }

    /// <summary>
    /// Tabla de relación entre formularios y módulos.
    /// </summary>
    public DbSet<FormModule> FormModules { get; set; }

    /// <summary>
    /// Tabla de permisos asignados a roles sobre formularios específicos.
    /// </summary>
    public DbSet<RolFormPermi> RolFormPermis { get; set; }

    // ========== DBSETS DE CATÁLOGOS ==========
    /// <summary>
    /// Tabla de estados de citas (Pendiente, Confirmada, Completada, Cancelada, etc).
    /// </summary>
    public DbSet<AppointmentStatus> AppointmentStatuses { get; set; }

    /// <summary>
    /// Tabla de tipos de proyecto (Urbanización, Centro Comercial, etc).
    /// </summary>
    public DbSet<ProjectType> ProjectTypes { get; set; }

    /// <summary>
    /// Tabla de tipos de propiedad (Casa, Apartamento, Local Comercial, etc).
    /// </summary>
    public DbSet<PropertyType> PropertyTypes { get; set; }

    /// <summary>
    /// Tabla de tipos de uso de servicio (Residencial, Comercial, Industrial, etc).
    /// </summary>
    public DbSet<ServiceUseType> ServiceUseTypes { get; set; }

    // ========== DBSETS DE CONFIGURACIÓN ==========
    /// <summary>
    /// Tabla de configuración de temas y colores de la aplicación.
    /// </summary>
    public DbSet<ThemeSettings> ThemeSettings { get; set; }

    /// <summary>
    /// Tabla de configuración del sistema en runtime.
    /// </summary>
    public DbSet<SystemSetting> SystemSettings { get; set; }

    /// <summary>
    /// Tabla de plantillas de notificación (Email, SMS, Push).
    /// </summary>
    public DbSet<NotificationTemplate> NotificationTemplates { get; set; }

    /// <summary>
    /// Tabla de notificaciones enviadas a usuarios del sistema.
    /// </summary>
    public DbSet<Notification> Notifications { get; set; }

    /// <summary>
    /// Tabla de festivos y días no laborables.
    /// </summary>
    public DbSet<Holiday> Holidays { get; set; }

    /// <summary>
    /// Tabla de documentos adjuntos a citas.
    /// </summary>
    public DbSet<AppointmentDocument> AppointmentDocuments { get; set; }

    // ========== DBSETS DE ASIGNACIONES ==========
    /// <summary>
    /// Tabla de asignaciones de usuarios a tipos de cita.
    /// Permite filtrar qué tipos de cita puede ver y gestionar cada empleado.
    /// </summary>
    public DbSet<UserAppointmentTypeAssignment> UserAppointmentTypeAssignments { get; set; }

    /// <summary>
    /// Configura el modelo de base de datos usando Fluent API.
    /// Establece convenciones globales para Oracle y aplica configuraciones específicas de entidades.
    /// </summary>
    /// <param name="modelBuilder">Constructor del modelo de Entity Framework.</param>
    /// <remarks>
    /// Configuraciones aplicadas:
    /// 1. Esquema por defecto: ADMIN
    /// 2. Mapeo de propiedades BaseEntity a nombres de columnas Oracle (ID, CREATED_AT, UPDATED_AT, IS_ACTIVE)
    /// 3. Conversión de bool a NUMBER(1) para compatibilidad con Oracle
    /// 4. Aplicación de configuraciones individuales de entidades desde el ensamblado
    /// </remarks>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Establecer esquema por defecto para Oracle. Comentado para SQL Server.
        // modelBuilder.HasDefaultSchema("ADMIN");

        // Aplicar configuraciones específicas de entidades desde IEntityTypeConfiguration<T>
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // CRÍTICO: Configuración global para Oracle DESPUÉS de aplicar configuraciones individuales
        // Oracle no soporta el tipo BOOLEAN nativo hasta Oracle 23c
        // Por lo tanto, mapeamos bool (C#) a NUMBER(1) (Oracle) donde 0=false, 1=true
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // Configurar las propiedades de BaseEntity con nombres de columna en mayúsculas para Oracle
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                // Mapear propiedades de BaseEntity a nombres de columna Oracle en mayúsculas
                var isActiveProperty = entityType.FindProperty(nameof(BaseEntity.IsActive));
                if (isActiveProperty != null)
                {
                    isActiveProperty.SetColumnName("IS_ACTIVE");
                    isActiveProperty.SetColumnType("NUMBER(1)");
                    isActiveProperty.SetDefaultValue(1);
                }

                var createdAtProperty = entityType.FindProperty(nameof(BaseEntity.CreatedAt));
                if (createdAtProperty != null)
                {
                    createdAtProperty.SetColumnName("CREATED_AT");
                }

                var updatedAtProperty = entityType.FindProperty(nameof(BaseEntity.UpdatedAt));
                if (updatedAtProperty != null)
                {
                    updatedAtProperty.SetColumnName("UPDATED_AT");
                }
            }

            // Establecer tipo de columna para booleanos (el converter ya se aplicó en ConfigureConventions)
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(bool))
                {
                    property.SetColumnType("NUMBER(1)");
                }
            }
        }
    }

    /// <summary>
    /// Sobrescribe SaveChangesAsync para establecer automáticamente CreatedAt y UpdatedAt.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación para operaciones asíncronas.</param>
    /// <returns>Número de entidades afectadas en la base de datos.</returns>
    /// <remarks>
    /// Este método intercepta el guardado de cambios para:
    /// - Entidades nuevas (Added): Establece CreatedAt y UpdatedAt con la fecha/hora actual UTC
    /// - Entidades modificadas (Modified): Actualiza UpdatedAt con la fecha/hora actual UTC
    /// </remarks>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Interceptar entidades que heredan de BaseEntity para actualizar timestamps
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Guarda las entidades en la base de datos e indica si la operación fue exitosa.
    /// Implementa el método de IUnitOfWork.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación para operaciones asíncronas.</param>
    /// <returns>True si el guardado fue exitoso, False si ocurrió algún error.</returns>
    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);
            return true;
        }
        catch
        {
            return false;
        }
    }
}