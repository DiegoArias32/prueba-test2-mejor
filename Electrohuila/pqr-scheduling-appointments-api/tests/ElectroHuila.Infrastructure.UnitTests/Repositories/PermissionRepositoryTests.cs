using ElectroHuila.Domain.Entities.Security;
using ElectroHuila.Infrastructure.Persistence;
using ElectroHuila.Infrastructure.Persistence.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Xunit;

namespace ElectroHuila.Infrastructure.UnitTests.Repositories;

/// <summary>
/// Pruebas unitarias para el repositorio de permisos (PermissionRepository).
/// Verifica el comportamiento de las operaciones CRUD y consultas especializadas para permisos.
/// </summary>
public class PermissionRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly PermissionRepository _repository;

    /// <summary>
    /// Inicializa una nueva instancia de las pruebas del repositorio de permisos.
    /// Configura un contexto de base de datos en memoria para pruebas.
    /// </summary>
    public PermissionRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new PermissionRepository(_context);
    }

    /// <summary>
    /// Limpia los recursos después de cada prueba.
    /// </summary>
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    /// <summary>
    /// Verifica que GetByNameAsync retorne el permiso correcto cuando existe y está activo.
    /// Nota: Actualmente busca por Id.ToString() == name según la implementación actual.
    /// </summary>
    [Fact]
    public async Task GetByNameAsync_Should_Return_Permission_When_Name_Exists_And_IsActive()
    {
        // Arrange
        var permission = new Permission
        {
            CanRead = true,
            CanCreate = true,
            CanUpdate = false,
            CanDelete = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Permissions.AddAsync(permission);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByNameAsync(permission.Id.ToString());

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(permission.Id);
        result.CanRead.Should().BeTrue();
        result.CanCreate.Should().BeTrue();
        result.IsActive.Should().BeTrue();
    }

    /// <summary>
    /// Verifica que GetByNameAsync retorne null cuando el permiso está inactivo.
    /// </summary>
    [Fact]
    public async Task GetByNameAsync_Should_Return_Null_When_Permission_Is_Inactive()
    {
        // Arrange
        var permission = new Permission
        {
            CanRead = true,
            CanCreate = false,
            CanUpdate = false,
            CanDelete = false,
            IsActive = false,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Permissions.AddAsync(permission);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByNameAsync(permission.Id.ToString());

        // Assert
        result.Should().BeNull();
    }

    /// <summary>
    /// Verifica que GetByNameAsync retorne null cuando no existe el permiso.
    /// </summary>
    [Fact]
    public async Task GetByNameAsync_Should_Return_Null_When_Name_Does_Not_Exist()
    {
        // Act
        var result = await _repository.GetByNameAsync("999");

        // Assert
        result.Should().BeNull();
    }

    /// <summary>
    /// Verifica que GetByActionAsync retorne el permiso correcto cuando existe y está activo.
    /// </summary>
    [Fact]
    public async Task GetByActionAsync_Should_Return_Permission_When_Action_Exists_And_IsActive()
    {
        // Arrange
        var permission = new Permission
        {
            CanRead = false,
            CanCreate = true,
            CanUpdate = true,
            CanDelete = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Permissions.AddAsync(permission);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByActionAsync(permission.Id.ToString());

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(permission.Id);
        result.CanCreate.Should().BeTrue();
        result.CanUpdate.Should().BeTrue();
    }

    /// <summary>
    /// Verifica que GetByActionAsync retorne null cuando el permiso está inactivo.
    /// </summary>
    [Fact]
    public async Task GetByActionAsync_Should_Return_Null_When_Permission_Is_Inactive()
    {
        // Arrange
        var permission = new Permission
        {
            CanRead = true,
            CanCreate = true,
            CanUpdate = true,
            CanDelete = true,
            IsActive = false,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Permissions.AddAsync(permission);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByActionAsync(permission.Id.ToString());

        // Assert
        result.Should().BeNull();
    }

    /// <summary>
    /// Verifica que GetActiveAsync retorne solo los permisos activos ordenados por Id.
    /// </summary>
    [Fact]
    public async Task GetActiveAsync_Should_Return_Only_Active_Permissions_Ordered_By_Id()
    {
        // Arrange
        var permissions = new List<Permission>
        {
            new Permission { CanRead = true, CanCreate = false, CanUpdate = false, CanDelete = false, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Permission { CanRead = false, CanCreate = true, CanUpdate = false, CanDelete = false, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Permission { CanRead = false, CanCreate = false, CanUpdate = true, CanDelete = false, IsActive = false, CreatedAt = DateTime.UtcNow },
            new Permission { CanRead = false, CanCreate = false, CanUpdate = false, CanDelete = true, IsActive = true, CreatedAt = DateTime.UtcNow }
        };

        await _context.Permissions.AddRangeAsync(permissions);
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetActiveAsync()).ToList();

        // Assert
        result.Should().HaveCount(3);
        result.Should().NotContain(p => p.CanUpdate && !p.CanRead && !p.CanCreate && !p.CanDelete);
        result.Should().BeInAscendingOrder(p => p.Id);
    }

    /// <summary>
    /// Verifica que GetActiveAsync retorne lista vacía cuando no hay permisos activos.
    /// </summary>
    [Fact]
    public async Task GetActiveAsync_Should_Return_Empty_List_When_No_Active_Permissions()
    {
        // Arrange
        var permission = new Permission
        {
            CanRead = true,
            CanCreate = true,
            CanUpdate = true,
            CanDelete = true,
            IsActive = false,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Permissions.AddAsync(permission);
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetActiveAsync()).ToList();

        // Assert
        result.Should().BeEmpty();
    }

    /// <summary>
    /// Verifica que GetByRoleIdAsync retorne solo permisos activos asociados al rol.
    /// </summary>
    [Fact]
    public async Task GetByRoleIdAsync_Should_Return_Active_Permissions_For_Specific_Role()
    {
        // Arrange
        var rol = new Rol { Name = "Admin", Code = "ADMIN", IsActive = true, CreatedAt = DateTime.UtcNow };
        var form = new Form { Name = "Users", Code = "USERS", IsActive = true, CreatedAt = DateTime.UtcNow };

        var permission1 = new Permission { CanRead = true, CanCreate = true, CanUpdate = true, CanDelete = true, IsActive = true, CreatedAt = DateTime.UtcNow };
        var permission2 = new Permission { CanRead = true, CanCreate = false, CanUpdate = false, CanDelete = false, IsActive = true, CreatedAt = DateTime.UtcNow };
        var permission3 = new Permission { CanRead = false, CanCreate = true, CanUpdate = false, CanDelete = false, IsActive = false, CreatedAt = DateTime.UtcNow };

        await _context.Roles.AddAsync(rol);
        await _context.Forms.AddAsync(form);
        await _context.Permissions.AddRangeAsync(new[] { permission1, permission2, permission3 });
        await _context.SaveChangesAsync();

        var rolFormPermis1 = new RolFormPermi { RolId = rol.Id, FormId = form.Id, PermissionId = permission1.Id };
        var rolFormPermis2 = new RolFormPermi { RolId = rol.Id, FormId = form.Id, PermissionId = permission3.Id };

        await _context.RolFormPermis.AddRangeAsync(new[] { rolFormPermis1, rolFormPermis2 });
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetByRoleIdAsync(rol.Id)).ToList();

        // Assert
        result.Should().HaveCount(1);
        result[0].Id.Should().Be(permission1.Id);
        result.Should().NotContain(p => p.Id == permission3.Id);
    }

    /// <summary>
    /// Verifica que GetByRoleIdAsync retorne lista vacía cuando el rol no tiene permisos.
    /// </summary>
    [Fact]
    public async Task GetByRoleIdAsync_Should_Return_Empty_List_When_Role_Has_No_Permissions()
    {
        // Arrange
        var rol = new Rol { Name = "Empty Role", Code = "EMPTY", IsActive = true, CreatedAt = DateTime.UtcNow };
        await _context.Roles.AddAsync(rol);
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetByRoleIdAsync(rol.Id)).ToList();

        // Assert
        result.Should().BeEmpty();
    }

    /// <summary>
    /// Verifica que GetByRolIdAsync (método duplicado) funcione correctamente.
    /// </summary>
    [Fact]
    public async Task GetByRolIdAsync_Should_Return_Active_Permissions_For_Specific_Role()
    {
        // Arrange
        var rol = new Rol { Name = "User", Code = "USER", IsActive = true, CreatedAt = DateTime.UtcNow };
        var form = new Form { Name = "Dashboard", Code = "DASH", IsActive = true, CreatedAt = DateTime.UtcNow };

        var permission = new Permission { CanRead = true, CanCreate = false, CanUpdate = false, CanDelete = false, IsActive = true, CreatedAt = DateTime.UtcNow };

        await _context.Roles.AddAsync(rol);
        await _context.Forms.AddAsync(form);
        await _context.Permissions.AddAsync(permission);
        await _context.SaveChangesAsync();

        var rolFormPermi = new RolFormPermi { RolId = rol.Id, FormId = form.Id, PermissionId = permission.Id };
        await _context.RolFormPermis.AddAsync(rolFormPermi);
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetByRolIdAsync(rol.Id)).ToList();

        // Assert
        result.Should().HaveCount(1);
        result[0].Id.Should().Be(permission.Id);
        result[0].CanRead.Should().BeTrue();
    }

    /// <summary>
    /// Verifica que GetByFormIdAsync retorne solo permisos activos asociados al formulario.
    /// </summary>
    [Fact]
    public async Task GetByFormIdAsync_Should_Return_Active_Permissions_For_Specific_Form()
    {
        // Arrange
        var rol = new Rol { Name = "Admin", Code = "ADMIN", IsActive = true, CreatedAt = DateTime.UtcNow };
        var form = new Form { Name = "Products", Code = "PROD", IsActive = true, CreatedAt = DateTime.UtcNow };

        var permission1 = new Permission { CanRead = true, CanCreate = true, CanUpdate = false, CanDelete = false, IsActive = true, CreatedAt = DateTime.UtcNow };
        var permission2 = new Permission { CanRead = true, CanCreate = true, CanUpdate = true, CanDelete = true, IsActive = true, CreatedAt = DateTime.UtcNow };
        var permission3 = new Permission { CanRead = false, CanCreate = false, CanUpdate = true, CanDelete = false, IsActive = false, CreatedAt = DateTime.UtcNow };

        await _context.Roles.AddAsync(rol);
        await _context.Forms.AddAsync(form);
        await _context.Permissions.AddRangeAsync(new[] { permission1, permission2, permission3 });
        await _context.SaveChangesAsync();

        var rolFormPermis1 = new RolFormPermi { RolId = rol.Id, FormId = form.Id, PermissionId = permission1.Id };
        var rolFormPermis2 = new RolFormPermi { RolId = rol.Id, FormId = form.Id, PermissionId = permission2.Id };
        var rolFormPermis3 = new RolFormPermi { RolId = rol.Id, FormId = form.Id, PermissionId = permission3.Id };

        await _context.RolFormPermis.AddRangeAsync(new[] { rolFormPermis1, rolFormPermis2, rolFormPermis3 });
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetByFormIdAsync(form.Id)).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(p => p.Id == permission1.Id);
        result.Should().Contain(p => p.Id == permission2.Id);
        result.Should().NotContain(p => p.Id == permission3.Id);
    }

    /// <summary>
    /// Verifica que GetByFormIdAsync retorne lista vacía cuando el formulario no tiene permisos.
    /// </summary>
    [Fact]
    public async Task GetByFormIdAsync_Should_Return_Empty_List_When_Form_Has_No_Permissions()
    {
        // Arrange
        var form = new Form { Name = "Empty Form", Code = "EMPTY", IsActive = true, CreatedAt = DateTime.UtcNow };
        await _context.Forms.AddAsync(form);
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetByFormIdAsync(form.Id)).ToList();

        // Assert
        result.Should().BeEmpty();
    }

    /// <summary>
    /// Verifica que ExistsByNameAsync retorne true cuando existe un permiso con ese Id.
    /// </summary>
    [Fact]
    public async Task ExistsByNameAsync_Should_Return_True_When_Permission_Exists()
    {
        // Arrange
        var permission = new Permission
        {
            CanRead = true,
            CanCreate = true,
            CanUpdate = true,
            CanDelete = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Permissions.AddAsync(permission);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsByNameAsync(permission.Id.ToString());

        // Assert
        result.Should().BeTrue();
    }

    /// <summary>
    /// Verifica que ExistsByNameAsync retorne true incluso cuando el permiso está inactivo.
    /// </summary>
    [Fact]
    public async Task ExistsByNameAsync_Should_Return_True_Even_When_Permission_Is_Inactive()
    {
        // Arrange
        var permission = new Permission
        {
            CanRead = true,
            CanCreate = false,
            CanUpdate = false,
            CanDelete = false,
            IsActive = false,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Permissions.AddAsync(permission);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsByNameAsync(permission.Id.ToString());

        // Assert
        result.Should().BeTrue();
    }

    /// <summary>
    /// Verifica que ExistsByNameAsync retorne false cuando no existe el permiso.
    /// </summary>
    [Fact]
    public async Task ExistsByNameAsync_Should_Return_False_When_Permission_Does_Not_Exist()
    {
        // Act
        var result = await _repository.ExistsByNameAsync("999");

        // Assert
        result.Should().BeFalse();
    }

    /// <summary>
    /// Verifica que ExistsByActionAsync retorne true cuando existen permisos en la base de datos.
    /// Nota: La implementación actual verifica si existe algún permiso con Id > 0.
    /// </summary>
    [Fact]
    public async Task ExistsByActionAsync_Should_Return_True_When_Permissions_Exist()
    {
        // Arrange
        var permission = new Permission
        {
            CanRead = true,
            CanCreate = false,
            CanUpdate = false,
            CanDelete = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Permissions.AddAsync(permission);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsByActionAsync("anyaction");

        // Assert
        result.Should().BeTrue();
    }

    /// <summary>
    /// Verifica que ExistsByActionAsync retorne false cuando no hay permisos en la base de datos.
    /// </summary>
    [Fact]
    public async Task ExistsByActionAsync_Should_Return_False_When_No_Permissions_Exist()
    {
        // Act
        var result = await _repository.ExistsByActionAsync("anyaction");

        // Assert
        result.Should().BeFalse();
    }

    /// <summary>
    /// Verifica que DeleteAsync realice soft delete marcando IsActive como false.
    /// </summary>
    [Fact]
    public async Task DeleteAsync_Should_Perform_Soft_Delete_By_Setting_IsActive_False()
    {
        // Arrange
        var permission = new Permission
        {
            CanRead = true,
            CanCreate = true,
            CanUpdate = true,
            CanDelete = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Permissions.AddAsync(permission);
        await _context.SaveChangesAsync();

        var permissionId = permission.Id;

        // Act
        await _repository.DeleteAsync(permissionId);
        await _context.SaveChangesAsync();

        // Assert
        var deletedPermission = await _context.Permissions.FindAsync(permissionId);
        deletedPermission.Should().NotBeNull();
        deletedPermission!.IsActive.Should().BeFalse();
    }

    /// <summary>
    /// Verifica que DeleteAsync no haga nada cuando el permiso no existe.
    /// </summary>
    [Fact]
    public async Task DeleteAsync_Should_Do_Nothing_When_Permission_Does_Not_Exist()
    {
        // Act
        await _repository.DeleteAsync(999);
        await _context.SaveChangesAsync();

        // Assert
        var permissions = await _context.Permissions.ToListAsync();
        permissions.Should().BeEmpty();
    }

    /// <summary>
    /// Verifica que GetByRoleIdAsync elimine duplicados con Distinct.
    /// </summary>
    [Fact]
    public async Task GetByRoleIdAsync_Should_Return_Distinct_Permissions()
    {
        // Arrange
        var rol = new Rol { Name = "TestRole", Code = "TEST", IsActive = true, CreatedAt = DateTime.UtcNow };
        var form1 = new Form { Name = "Form1", Code = "FORM1", IsActive = true, CreatedAt = DateTime.UtcNow };
        var form2 = new Form { Name = "Form2", Code = "FORM2", IsActive = true, CreatedAt = DateTime.UtcNow };

        var permission = new Permission { CanRead = true, CanCreate = true, CanUpdate = false, CanDelete = false, IsActive = true, CreatedAt = DateTime.UtcNow };

        await _context.Roles.AddAsync(rol);
        await _context.Forms.AddRangeAsync(new[] { form1, form2 });
        await _context.Permissions.AddAsync(permission);
        await _context.SaveChangesAsync();

        // Mismo permiso asociado a dos formularios diferentes
        var rolFormPermis1 = new RolFormPermi { RolId = rol.Id, FormId = form1.Id, PermissionId = permission.Id };
        var rolFormPermis2 = new RolFormPermi { RolId = rol.Id, FormId = form2.Id, PermissionId = permission.Id };

        await _context.RolFormPermis.AddRangeAsync(new[] { rolFormPermis1, rolFormPermis2 });
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetByRoleIdAsync(rol.Id)).ToList();

        // Assert
        result.Should().HaveCount(1);
        result[0].Id.Should().Be(permission.Id);
    }
}
