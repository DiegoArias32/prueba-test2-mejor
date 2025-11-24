using ElectroHuila.Application.Common.Interfaces.Services.Common;
using ElectroHuila.Application.Common.Interfaces.Services.Security;
using ElectroHuila.Domain.Entities.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ElectroHuila.Infrastructure.Persistence.Seeds;

/// <summary>
/// Clase responsable de la inicialización de datos maestros en la base de datos.
/// Proporciona un conjunto completo de datos de prueba y configuración inicial
/// para el funcionamiento del sistema de agendamiento de citas.
/// </summary>
/// <remarks>
/// PROCESO DE SEEDING:
/// 1. Permisos base (CRUD combinations)
/// 2. Módulos del sistema (agrupación funcional)
/// 3. Formularios (pantallas del sistema)
/// 4. Roles de usuario
/// 5. Relaciones Form-Module
/// 6. Asignaciones Rol-Form-Permission
/// 7. Usuario administrador inicial
/// 
/// CARACTERÍSTICAS:
/// - Idempotente: Verifica existencia antes de crear
/// - Transaccional: Cada método maneja su propia transacción
/// - Logging detallado para auditoría del proceso
/// - Gestión robusta de errores con propagación
/// 
/// DEPENDENCIAS:
/// - Requiere IPasswordHasher para creación de usuarios
/// - Utiliza ILogger para trazabilidad del proceso
/// - Depende del ApplicationDbContext para acceso a datos
/// 
/// USO TÍPICO:
/// Ejecutado durante el startup de la aplicación o mediante
/// migraciones para establecer el estado inicial del sistema.
/// </remarks>
public class DatabaseSeeder : IDatabaseSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<DatabaseSeeder> _logger;

    /// <summary>
    /// Inicializa una nueva instancia del DatabaseSeeder.
    /// </summary>
    /// <param name="context">El contexto de la base de datos de la aplicación.</param>
    /// <param name="passwordHasher">El servicio de hashing de contraseñas para usuarios.</param>
    /// <param name="logger">El logger para registrar el proceso de seeding.</param>
    public DatabaseSeeder(
        ApplicationDbContext context,
        IPasswordHasher passwordHasher,
        ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    /// <summary>
    /// Ejecuta el proceso completo de inicialización de la base de datos.
    /// </summary>
    /// <returns>Una tarea que representa la operación asíncrona de seeding.</returns>
    /// <remarks>
    /// ORDEN DE EJECUCIÓN (CRÍTICO):
    /// 1. SeedPermissionsAsync() - Permisos base del sistema
    /// 2. SeedModulesAsync() - Módulos funcionales
    /// 3. SeedFormsAsync() - Formularios/pantallas
    /// 4. SeedRolesAsync() - Roles de usuario
    /// 5. SeedFormModulesAsync() - Asociaciones Form-Module
    /// 6. SeedRolFormPermisAsync() - Matriz de permisos
    /// 7. SeedUsersAsync() - Usuario administrador inicial
    /// 
    /// IMPORTANTE: El orden es fundamental debido a las dependencias
    /// de claves foráneas entre las entidades.
    /// 
    /// MANEJO DE ERRORES:
    /// - Captura cualquier excepción durante el proceso
    /// - Registra errores detallados en logs
    /// - Re-lanza la excepción para manejo upstream
    /// 
    /// IDEMPOTENCIA:
    /// Cada método individual verifica si los datos ya existen
    /// antes de proceder con la inserción.
    /// </remarks>
    public async Task SeedAsync()
    {
        try
        {
            _logger.LogInformation("Starting database seeding...");

            await SeedPermissionsAsync();
            await SeedModulesAsync();
            await SeedFormsAsync();
            await SeedRolesAsync();
            await SeedFormModulesAsync();
            await SeedRolFormPermisAsync();
            await SeedUsersAsync();

            _logger.LogInformation("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }

    /// <summary>
    /// Inicializa los permisos base del sistema.
    /// </summary>
    /// <remarks>
    /// PERMISOS CREADOS:
    /// 1. Full Access (CRUD completo) - Para administradores
    /// 2. Create & Update (Sin delete) - Para supervisores
    /// 3. Read Only (Solo lectura) - Para consultores
    /// 
    /// DISEÑO DE PERMISOS:
    /// Utiliza un enfoque de combinaciones de flags CRUD para
    /// máxima flexibilidad en la asignación de permisos.
    /// 
    /// IDEMPOTENCIA: Verifica existencia antes de crear.
    /// No utiliza códigos únicos, confía en la ausencia total de registros.
    /// </remarks>
    private async Task SeedPermissionsAsync()
    {
        if (await _context.Permissions.AnyAsync())
        {
            _logger.LogInformation("Permissions already seeded");
            return;
        }

        var permissions = new[]
        {
            new Permission { CanRead = true, CanCreate = true, CanUpdate = true, CanDelete = true }, // Full Access
            new Permission { CanRead = true, CanCreate = true, CanUpdate = true, CanDelete = false }, // Create & Update
            new Permission { CanRead = true, CanCreate = false, CanUpdate = false, CanDelete = false }, // Read Only
        };

        await _context.Permissions.AddRangeAsync(permissions);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Seeded {Count} permissions", permissions.Length);
    }

    /// <summary>
    /// Inicializa los módulos funcionales del sistema.
    /// </summary>
    /// <remarks>
    /// MÓDULOS DEL SISTEMA:
    /// - APPOINTMENTS: Gestión de citas y agendamiento
    /// - CLIENTS: Gestión de clientes y cuentas
    /// - PROJECTS: Gestión de proyectos nuevos
    /// - ADMIN: Funciones administrativas generales
    /// - SECURITY: Gestión de usuarios, roles y permisos
    /// 
    /// PROPÓSITO:
    /// Los módulos agrupan formularios relacionados funcionalmente
    /// y facilitan la organización de permisos por área de negocio.
    /// 
    /// CÓDIGOS ÚNICOS:
    /// Cada módulo tiene un código único para referencia programática.
    /// </remarks>
    private async Task SeedModulesAsync()
    {
        if (await _context.Modules.AnyAsync())
        {
            _logger.LogInformation("Modules already seeded");
            return;
        }

        var modules = new[]
        {
            new Module { Name = "Gestión de Citas", Code = "APPOINTMENTS" },
            new Module { Name = "Gestión de Clientes", Code = "CLIENTS" },
            new Module { Name = "Gestión de Proyectos", Code = "PROJECTS" },
            new Module { Name = "Administración", Code = "ADMIN" },
            new Module { Name = "Seguridad", Code = "SECURITY" }
        };

        await _context.Modules.AddRangeAsync(modules);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Seeded {Count} modules", modules.Length);
    }

    /// <summary>
    /// Inicializa los formularios (pantallas) del sistema organizados por módulo.
    /// </summary>
    /// <remarks>
    /// ESTRUCTURA DE FORMULARIOS POR MÓDULO:
    /// 
    /// APPOINTMENTS MODULE:
    /// - appointments: Gestión de citas
    /// - appointment-types: Tipos de citas disponibles
    /// - available-times: Horarios disponibles
    /// - branches: Sucursales de atención
    /// 
    /// CLIENTS MODULE:
    /// - clients: Gestión de clientes
    /// - new-accounts: Nuevas cuentas de servicio
    /// 
    /// PROJECTS MODULE:
    /// - project-news: Gestión de proyectos nuevos
    /// 
    /// SECURITY MODULE:
    /// - users: Gestión de usuarios
    /// - roles: Gestión de roles
    /// - permissions: Gestión de permisos
    /// - forms: Gestión de formularios
    /// - modules: Gestión de módulos
    /// 
    /// CÓDIGOS ÚNICOS:
    /// Cada formulario tiene un código único usado para:
    /// - Identificación programática
    /// - Configuración de rutas
    /// - Asignación de permisos
    /// 
    /// PROPÓSITO:
    /// Los formularios representan las pantallas/funcionalidades
    /// sobre las cuales se aplican los permisos de usuario.
    /// </remarks>
    private async Task SeedFormsAsync()
    {
        if (await _context.Forms.AnyAsync())
        {
            _logger.LogInformation("Forms already seeded");
            return;
        }

        var forms = new[]
        {
            // Appointments Module
            new Form { Name = "Citas", Code = "appointments" },
            new Form { Name = "Tipos de Citas", Code = "appointment-types" },
            new Form { Name = "Horarios Disponibles", Code = "available-times" },
            new Form { Name = "Sucursales", Code = "branches" },

            // Clients Module
            new Form { Name = "Clientes", Code = "clients" },
            new Form { Name = "Nuevas Cuentas", Code = "new-accounts" },

            // Projects Module
            new Form { Name = "Proyectos Nuevos", Code = "project-news" },

            // Security Module
            new Form { Name = "Usuarios", Code = "users" },
            new Form { Name = "Roles", Code = "roles" },
            new Form { Name = "Permisos", Code = "permissions" },
            new Form { Name = "Formularios", Code = "forms" },
            new Form { Name = "Módulos", Code = "modules" }
        };

        await _context.Forms.AddRangeAsync(forms);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Seeded {Count} forms", forms.Length);
    }

    /// <summary>
    /// Inicializa los roles base del sistema de autorización.
    /// </summary>
    /// <remarks>
    /// ROLES DEL SISTEMA:
    /// - ADMIN: Administrador con acceso completo al sistema
    /// - SUPERVISOR: Supervisor con permisos de gestión limitados
    /// - OPERATOR: Operador con permisos de trabajo diario
    /// - CLIENT: Cliente con acceso limitado a sus datos
    /// 
    /// JERARQUÍA IMPLÍCITA:
    /// Aunque no hay jerarquía explícita en el modelo,
    /// los roles están diseñados con niveles de acceso decrecientes.
    /// 
    /// CÓDIGOS ÚNICOS:
    /// Cada rol tiene un código único para referencia programática
    /// y asignación en el sistema de autorización.
    /// </remarks>
    private async Task SeedRolesAsync()
    {
        if (await _context.Roles.AnyAsync())
        {
            _logger.LogInformation("Roles already seeded");
            return;
        }

        var roles = new[]
        {
            new Rol { Name = "Administrador", Code = "ADMIN" },
            new Rol { Name = "Supervisor", Code = "SUPERVISOR" },
            new Rol { Name = "Operador", Code = "OPERATOR" },
            new Rol { Name = "Cliente", Code = "CLIENT" }
        };

        await _context.Roles.AddRangeAsync(roles);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Seeded {Count} roles", roles.Length);
    }

    /// <summary>
    /// Establece las relaciones entre formularios y módulos.
    /// </summary>
    /// <remarks>
    /// PROPÓSITO:
    /// Asocia cada formulario con su módulo correspondiente para:
    /// - Organización lógica de la aplicación
    /// - Navegación por módulos
    /// - Agrupación de permisos
    /// 
    /// PROCESO:
    /// 1. Busca módulos por código único
    /// 2. Busca formularios por código único
    /// 3. Crea relaciones FormModule
    /// 
    /// DEPENDENCIAS:
    /// Requiere que SeedModulesAsync() y SeedFormsAsync()
    /// hayan sido ejecutados previamente.
    /// 
    /// ORGANIZACIÓN:
    /// - Appointments: 4 formularios (citas, tipos, horarios, sucursales)
    /// - Clients: 2 formularios (clientes, nuevas cuentas)
    /// - Projects: 1 formulario (proyectos nuevos)
    /// - Security: 5 formularios (usuarios, roles, permisos, forms, módulos)
    /// </remarks>
    private async Task SeedFormModulesAsync()
    {
        if (await _context.FormModules.AnyAsync())
        {
            _logger.LogInformation("FormModules already seeded");
            return;
        }

        var appointmentsModule = await _context.Modules.FirstAsync(m => m.Code == "APPOINTMENTS");
        var clientsModule = await _context.Modules.FirstAsync(m => m.Code == "CLIENTS");
        var projectsModule = await _context.Modules.FirstAsync(m => m.Code == "PROJECTS");
        var securityModule = await _context.Modules.FirstAsync(m => m.Code == "SECURITY");

        var formModules = new List<FormModule>
        {
            // Appointments Forms
            new FormModule { FormId = (await _context.Forms.FirstAsync(f => f.Code == "appointments")).Id, ModuleId = appointmentsModule.Id },
            new FormModule { FormId = (await _context.Forms.FirstAsync(f => f.Code == "appointment-types")).Id, ModuleId = appointmentsModule.Id },
            new FormModule { FormId = (await _context.Forms.FirstAsync(f => f.Code == "available-times")).Id, ModuleId = appointmentsModule.Id },
            new FormModule { FormId = (await _context.Forms.FirstAsync(f => f.Code == "branches")).Id, ModuleId = appointmentsModule.Id },

            // Clients Forms
            new FormModule { FormId = (await _context.Forms.FirstAsync(f => f.Code == "clients")).Id, ModuleId = clientsModule.Id },
            new FormModule { FormId = (await _context.Forms.FirstAsync(f => f.Code == "new-accounts")).Id, ModuleId = clientsModule.Id },

            // Projects Forms
            new FormModule { FormId = (await _context.Forms.FirstAsync(f => f.Code == "project-news")).Id, ModuleId = projectsModule.Id },

            // Security Forms
            new FormModule { FormId = (await _context.Forms.FirstAsync(f => f.Code == "users")).Id, ModuleId = securityModule.Id },
            new FormModule { FormId = (await _context.Forms.FirstAsync(f => f.Code == "roles")).Id, ModuleId = securityModule.Id },
            new FormModule { FormId = (await _context.Forms.FirstAsync(f => f.Code == "permissions")).Id, ModuleId = securityModule.Id },
            new FormModule { FormId = (await _context.Forms.FirstAsync(f => f.Code == "forms")).Id, ModuleId = securityModule.Id },
            new FormModule { FormId = (await _context.Forms.FirstAsync(f => f.Code == "modules")).Id, ModuleId = securityModule.Id }
        };

        await _context.FormModules.AddRangeAsync(formModules);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Seeded {Count} form-module relationships", formModules.Count);
    }

    /// <summary>
    /// Establece la matriz de permisos inicial asignando acceso completo al rol administrador.
    /// </summary>
    /// <remarks>
    /// ESTRATEGIA DE PERMISOS INICIAL:
    /// - Rol ADMIN obtiene Full Access a TODOS los formularios
    /// - Otros roles requerirán configuración manual post-seeding
    /// 
    /// PROCESO:
    /// 1. Busca el rol ADMIN por código
    /// 2. Busca el permiso de Full Access (todos los flags CRUD en true)
    /// 3. Obtiene todos los formularios del sistema
    /// 4. Crea relación RolFormPermi para cada formulario
    /// 
    /// DEPENDENCIAS:
    /// Requiere ejecución previa de:
    /// - SeedRolesAsync()
    /// - SeedPermissionsAsync()
    /// - SeedFormsAsync()
    /// 
    /// RESULTADO:
    /// El administrador tendrá acceso CRUD completo a:
    /// - Todos los formularios de appointments (4)
    /// - Todos los formularios de clients (2)
    /// - Todos los formularios de projects (1)
    /// - Todos los formularios de security (5)
    /// 
    /// EXPANSIÓN FUTURA:
    /// Para roles adicionales, se requerirá lógica más sofisticada
    /// o configuración manual de permisos específicos.
    /// </remarks>
    private async Task SeedRolFormPermisAsync()
    {
        if (await _context.RolFormPermis.AnyAsync())
        {
            _logger.LogInformation("RolFormPermis already seeded");
            return;
        }

        var adminRole = await _context.Roles.FirstAsync(r => r.Code == "ADMIN");
        var fullAccessPermission = await _context.Permissions.FirstAsync(p => p.CanRead && p.CanCreate && p.CanUpdate && p.CanDelete);
        var forms = await _context.Forms.ToListAsync();

        var rolFormPermis = forms.Select(form => new RolFormPermi
        {
            RolId = adminRole.Id,
            FormId = form.Id,
            PermissionId = fullAccessPermission.Id
        }).ToList();

        await _context.RolFormPermis.AddRangeAsync(rolFormPermis);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Seeded {Count} role-form-permission relationships", rolFormPermis.Count);
    }

    /// <summary>
    /// Crea el usuario administrador inicial del sistema.
    /// </summary>
    /// <remarks>
    /// USUARIO ADMINISTRADOR:
    /// - Username: "admin"
    /// - Email: "admin@electrohuila.com"
    /// - Password: "Admin123!" (hasheada con PBKDF2)
    /// - AllowedTabs: "all" (acceso a todas las pestañas)
    /// - Rol: ADMIN (acceso completo al sistema)
    /// 
    /// PROCESO DE CREACIÓN:
    /// 1. Crea usuario con contraseña hasheada
    /// 2. Persiste usuario en base de datos
    /// 3. Crea relación RolUser con rol ADMIN
    /// 4. Persiste la asignación de rol
    /// 
    /// SEGURIDAD:
    /// - Contraseña hasheada usando IPasswordHasher (PBKDF2)
    /// - Email corporativo para identificación
    /// - Contraseña temporal que debe cambiarse en primer acceso
    /// 
    /// DEPENDENCIAS:
    /// Requiere SeedRolesAsync() ejecutado previamente.
    /// 
    /// CONSIDERACIONES:
    /// - La contraseña debería cambiarse inmediatamente en producción
    /// - El email debería configurarse según el dominio de la organización
    /// - AllowedTabs="all" otorga acceso máximo al sistema
    /// </remarks>
    private async Task SeedUsersAsync()
    {
        if (await _context.Users.AnyAsync())
        {
            _logger.LogInformation("Users already seeded");
            return;
        }

        var adminRole = await _context.Roles.FirstAsync(r => r.Code == "ADMIN");

        var adminUser = new User
        {
            Username = "admin",
            Email = "admin@electrohuila.com",
            Password = _passwordHasher.HashPassword("Admin123!"),
            AllowedTabs = "all"
        };

        await _context.Users.AddAsync(adminUser);
        await _context.SaveChangesAsync();

        var rolUser = new RolUser
        {
            UserId = adminUser.Id,
            RolId = adminRole.Id
        };

        await _context.RolUsers.AddAsync(rolUser);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Seeded admin user: {Username}", adminUser.Username);
    }
}