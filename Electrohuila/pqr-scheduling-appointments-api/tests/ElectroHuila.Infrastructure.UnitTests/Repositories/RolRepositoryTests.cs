using ElectroHuila.Domain.Entities.Security;
using ElectroHuila.Infrastructure.Persistence;
using ElectroHuila.Infrastructure.Persistence.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Xunit;

namespace ElectroHuila.Infrastructure.UnitTests.Repositories;

/// <summary>
/// Pruebas unitarias para el repositorio de roles (RolRepository).
/// Verifica el comportamiento de las operaciones CRUD y consultas especializadas para roles.
/// </summary>
public class RolRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly RolRepository _repository;

    /// <summary>
    /// Inicializa una nueva instancia de las pruebas del repositorio de roles.
    /// Configura un contexto de base de datos en memoria para pruebas.
    /// </summary>
    public RolRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new RolRepository(_context);
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
    /// Verifica que GetByCodeAsync retorne el rol correcto cuando existe un rol activo con ese código.
    /// </summary>
    [Fact]
    public async Task GetByCodeAsync_Should_Return_Role_When_Code_Exists_And_IsActive()
    {
        // Arrange
        var rol = new Rol
        {
            Name = "Administrador",
            Code = "ADMIN",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Roles.AddAsync(rol);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByCodeAsync("ADMIN");

        // Assert
        result.Should().NotBeNull();
        result!.Code.Should().Be("ADMIN");
        result.Name.Should().Be("Administrador");
        result.IsActive.Should().BeTrue();
    }

    /// <summary>
    /// Verifica que GetByCodeAsync retorne null cuando el rol está inactivo.
    /// </summary>
    [Fact]
    public async Task GetByCodeAsync_Should_Return_Null_When_Role_Is_Inactive()
    {
        // Arrange
        var rol = new Rol
        {
            Name = "Usuario Eliminado",
            Code = "DELETED",
            IsActive = false,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Roles.AddAsync(rol);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByCodeAsync("DELETED");

        // Assert
        result.Should().BeNull();
    }

    /// <summary>
    /// Verifica que GetByCodeAsync retorne null cuando no existe el código.
    /// </summary>
    [Fact]
    public async Task GetByCodeAsync_Should_Return_Null_When_Code_Does_Not_Exist()
    {
        // Act
        var result = await _repository.GetByCodeAsync("NONEXISTENT");

        // Assert
        result.Should().BeNull();
    }

    /// <summary>
    /// Verifica que GetByNameAsync retorne el rol correcto cuando existe un rol activo con ese nombre.
    /// </summary>
    [Fact]
    public async Task GetByNameAsync_Should_Return_Role_When_Name_Exists_And_IsActive()
    {
        // Arrange
        var rol = new Rol
        {
            Name = "Supervisor",
            Code = "SUPER",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Roles.AddAsync(rol);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByNameAsync("Supervisor");

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Supervisor");
        result.Code.Should().Be("SUPER");
        result.IsActive.Should().BeTrue();
    }

    /// <summary>
    /// Verifica que GetByNameAsync retorne null cuando el rol está inactivo.
    /// </summary>
    [Fact]
    public async Task GetByNameAsync_Should_Return_Null_When_Role_Is_Inactive()
    {
        // Arrange
        var rol = new Rol
        {
            Name = "Rol Inactivo",
            Code = "INACTIVE",
            IsActive = false,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Roles.AddAsync(rol);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByNameAsync("Rol Inactivo");

        // Assert
        result.Should().BeNull();
    }

    /// <summary>
    /// Verifica que GetActiveAsync retorne solo los roles activos ordenados alfabéticamente.
    /// </summary>
    [Fact]
    public async Task GetActiveAsync_Should_Return_Only_Active_Roles_Ordered_By_Name()
    {
        // Arrange
        var roles = new List<Rol>
        {
            new Rol { Name = "Zebra Role", Code = "ZEBRA", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Rol { Name = "Alpha Role", Code = "ALPHA", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Rol { Name = "Inactive Role", Code = "INACTIVE", IsActive = false, CreatedAt = DateTime.UtcNow },
            new Rol { Name = "Beta Role", Code = "BETA", IsActive = true, CreatedAt = DateTime.UtcNow }
        };

        await _context.Roles.AddRangeAsync(roles);
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetActiveAsync()).ToList();

        // Assert
        result.Should().HaveCount(3);
        result.Should().NotContain(r => r.Code == "INACTIVE");
        result[0].Name.Should().Be("Alpha Role");
        result[1].Name.Should().Be("Beta Role");
        result[2].Name.Should().Be("Zebra Role");
    }

    /// <summary>
    /// Verifica que GetActiveAsync retorne lista vacía cuando no hay roles activos.
    /// </summary>
    [Fact]
    public async Task GetActiveAsync_Should_Return_Empty_List_When_No_Active_Roles()
    {
        // Arrange
        var rol = new Rol
        {
            Name = "Inactive Role",
            Code = "INACTIVE",
            IsActive = false,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Roles.AddAsync(rol);
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetActiveAsync()).ToList();

        // Assert
        result.Should().BeEmpty();
    }

    /// <summary>
    /// Verifica que GetAllIncludingInactiveAsync retorne todos los roles (activos e inactivos).
    /// </summary>
    [Fact]
    public async Task GetAllIncludingInactiveAsync_Should_Return_All_Roles_Including_Inactive()
    {
        // Arrange
        var roles = new List<Rol>
        {
            new Rol { Name = "Active Role 1", Code = "ACT1", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Rol { Name = "Inactive Role 1", Code = "INACT1", IsActive = false, CreatedAt = DateTime.UtcNow },
            new Rol { Name = "Active Role 2", Code = "ACT2", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Rol { Name = "Inactive Role 2", Code = "INACT2", IsActive = false, CreatedAt = DateTime.UtcNow }
        };

        await _context.Roles.AddRangeAsync(roles);
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetAllIncludingInactiveAsync()).ToList();

        // Assert
        result.Should().HaveCount(4);
        result.Should().Contain(r => r.Code == "ACT1");
        result.Should().Contain(r => r.Code == "INACT1");
        result.Should().Contain(r => r.Code == "ACT2");
        result.Should().Contain(r => r.Code == "INACT2");
    }

    /// <summary>
    /// Verifica que GetByUserIdAsync retorne solo los roles activos asignados a un usuario específico.
    /// </summary>
    [Fact]
    public async Task GetByUserIdAsync_Should_Return_Active_Roles_For_Specific_User()
    {
        // Arrange
        var rol1 = new Rol { Name = "Admin", Code = "ADMIN", IsActive = true, CreatedAt = DateTime.UtcNow };
        var rol2 = new Rol { Name = "User", Code = "USER", IsActive = true, CreatedAt = DateTime.UtcNow };
        var rol3 = new Rol { Name = "Inactive", Code = "INACTIVE", IsActive = false, CreatedAt = DateTime.UtcNow };

        await _context.Roles.AddRangeAsync(new[] { rol1, rol2, rol3 });
        await _context.SaveChangesAsync();

        var userId = 1;
        var rolUser1 = new RolUser { RolId = rol1.Id, UserId = userId };
        var rolUser2 = new RolUser { RolId = rol3.Id, UserId = userId };

        await _context.RolUsers.AddRangeAsync(new[] { rolUser1, rolUser2 });
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetByUserIdAsync(userId)).ToList();

        // Assert
        result.Should().HaveCount(1);
        result[0].Code.Should().Be("ADMIN");
        result.Should().NotContain(r => r.Code == "INACTIVE");
    }

    /// <summary>
    /// Verifica que GetByUserIdAsync retorne lista vacía cuando el usuario no tiene roles.
    /// </summary>
    [Fact]
    public async Task GetByUserIdAsync_Should_Return_Empty_List_When_User_Has_No_Roles()
    {
        // Arrange
        var rol = new Rol { Name = "Admin", Code = "ADMIN", IsActive = true, CreatedAt = DateTime.UtcNow };
        await _context.Roles.AddAsync(rol);
        await _context.SaveChangesAsync();

        var userId = 999;

        // Act
        var result = (await _repository.GetByUserIdAsync(userId)).ToList();

        // Assert
        result.Should().BeEmpty();
    }

    /// <summary>
    /// Verifica que ExistsByCodeAsync retorne true cuando existe un rol con el código especificado.
    /// </summary>
    [Fact]
    public async Task ExistsByCodeAsync_Should_Return_True_When_Code_Exists()
    {
        // Arrange
        var rol = new Rol
        {
            Name = "Test Role",
            Code = "TEST",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Roles.AddAsync(rol);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsByCodeAsync("TEST");

        // Assert
        result.Should().BeTrue();
    }

    /// <summary>
    /// Verifica que ExistsByCodeAsync retorne true incluso cuando el rol está inactivo.
    /// </summary>
    [Fact]
    public async Task ExistsByCodeAsync_Should_Return_True_Even_When_Role_Is_Inactive()
    {
        // Arrange
        var rol = new Rol
        {
            Name = "Inactive Role",
            Code = "INACTIVE",
            IsActive = false,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Roles.AddAsync(rol);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsByCodeAsync("INACTIVE");

        // Assert
        result.Should().BeTrue();
    }

    /// <summary>
    /// Verifica que ExistsByCodeAsync retorne false cuando el código no existe.
    /// </summary>
    [Fact]
    public async Task ExistsByCodeAsync_Should_Return_False_When_Code_Does_Not_Exist()
    {
        // Act
        var result = await _repository.ExistsByCodeAsync("NONEXISTENT");

        // Assert
        result.Should().BeFalse();
    }

    /// <summary>
    /// Verifica que ExistsByNameAsync retorne true cuando existe un rol con el nombre especificado.
    /// </summary>
    [Fact]
    public async Task ExistsByNameAsync_Should_Return_True_When_Name_Exists()
    {
        // Arrange
        var rol = new Rol
        {
            Name = "Test Role",
            Code = "TEST",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Roles.AddAsync(rol);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsByNameAsync("Test Role");

        // Assert
        result.Should().BeTrue();
    }

    /// <summary>
    /// Verifica que ExistsByNameAsync retorne false cuando el nombre no existe.
    /// </summary>
    [Fact]
    public async Task ExistsByNameAsync_Should_Return_False_When_Name_Does_Not_Exist()
    {
        // Act
        var result = await _repository.ExistsByNameAsync("Nonexistent Role");

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
        var rol = new Rol
        {
            Name = "To Delete",
            Code = "DELETE",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Roles.AddAsync(rol);
        await _context.SaveChangesAsync();

        var rolId = rol.Id;

        // Act
        await _repository.DeleteAsync(rolId);
        await _context.SaveChangesAsync();

        // Assert
        var deletedRol = await _context.Roles.FindAsync(rolId);
        deletedRol.Should().NotBeNull();
        deletedRol!.IsActive.Should().BeFalse();
    }

    /// <summary>
    /// Verifica que DeleteAsync no haga nada cuando el rol no existe.
    /// </summary>
    [Fact]
    public async Task DeleteAsync_Should_Do_Nothing_When_Role_Does_Not_Exist()
    {
        // Act
        await _repository.DeleteAsync(999);
        await _context.SaveChangesAsync();

        // Assert
        // No exception should be thrown
        var roles = await _context.Roles.ToListAsync();
        roles.Should().BeEmpty();
    }

    /// <summary>
    /// Verifica que DeleteLogicalAsync realice soft delete y persista cambios automáticamente.
    /// </summary>
    [Fact]
    public async Task DeleteLogicalAsync_Should_Perform_Soft_Delete_And_Persist_Changes()
    {
        // Arrange
        var rol = new Rol
        {
            Name = "To Delete",
            Code = "DELETE",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Roles.AddAsync(rol);
        await _context.SaveChangesAsync();

        var rolId = rol.Id;

        // Act
        await _repository.DeleteLogicalAsync(rolId);

        // Assert
        var deletedRol = await _context.Roles.FindAsync(rolId);
        deletedRol.Should().NotBeNull();
        deletedRol!.IsActive.Should().BeFalse();
    }

    /// <summary>
    /// Verifica que DeleteLogicalAsync no lance excepción cuando el rol no existe.
    /// </summary>
    [Fact]
    public async Task DeleteLogicalAsync_Should_Not_Throw_When_Role_Does_Not_Exist()
    {
        // Act
        var act = async () => await _repository.DeleteLogicalAsync(999);

        // Assert
        await act.Should().NotThrowAsync();
    }
}
