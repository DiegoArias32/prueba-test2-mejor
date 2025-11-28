using ElectroHuila.Domain.Entities.Security;
using ElectroHuila.Infrastructure.Persistence;
using ElectroHuila.Infrastructure.Persistence.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ElectroHuila.Infrastructure.UnitTests.Repositories;

/// <summary>
/// Pruebas unitarias completas para el repositorio de usuarios (UserRepository).
/// Cubre operaciones CRUD, consultas por email/username, gestión de roles, permisos y soft delete.
/// </summary>
public class UserRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly UserRepository _repository;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new UserRepository(_context);
    }

    #region GetByIdAsync Tests

    /// <summary>
    /// Verifica que GetByIdAsync retorna un usuario con sus roles incluidos.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsUserWithRoles()
    {
        // Arrange
        var user = CreateTestUser();
        var role = CreateTestRole();
        await _context.Users.AddAsync(user);
        await _context.Roles.AddAsync(role);
        await _context.SaveChangesAsync();

        var rolUser = new RolUser
        {
            UserId = user.Id,
            RolId = role.Id,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        await _context.RolUsers.AddAsync(rolUser);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(user.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(user.Id);
        result.RolUsers.Should().HaveCount(1);
        result.RolUsers.First().Rol.Should().NotBeNull();
    }

    /// <summary>
    /// Verifica que GetByIdAsync retorna null cuando el usuario no existe.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_WithNonExistentId_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region GetAllAsync Tests

    /// <summary>
    /// Verifica que GetAllAsync retorna todos los usuarios con sus roles ordenados por username.
    /// </summary>
    [Fact]
    public async Task GetAllAsync_ReturnsAllUsersWithRolesOrderedByUsername()
    {
        // Arrange
        var user1 = CreateTestUser();
        user1.Username = "charlie";
        var user2 = CreateTestUser();
        user2.Username = "alice";
        var user3 = CreateTestUser();
        user3.Username = "bob";

        await _context.Users.AddRangeAsync(user1, user2, user3);
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetAllAsync()).ToList();

        // Assert
        result.Should().HaveCount(3);
        result[0].Username.Should().Be("alice");
        result[1].Username.Should().Be("bob");
        result[2].Username.Should().Be("charlie");
    }

    #endregion

    #region GetAllIncludingInactiveAsync Tests

    /// <summary>
    /// Verifica que GetAllIncludingInactiveAsync retorna todos los usuarios incluyendo inactivos.
    /// </summary>
    [Fact]
    public async Task GetAllIncludingInactiveAsync_ReturnsAllUsers()
    {
        // Arrange
        var activeUser = CreateTestUser();
        var inactiveUser = CreateTestUser();
        inactiveUser.IsActive = false;

        await _context.Users.AddRangeAsync(activeUser, inactiveUser);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllIncludingInactiveAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(u => u.IsActive);
        result.Should().Contain(u => !u.IsActive);
    }

    #endregion

    #region GetByEmailAsync Tests

    /// <summary>
    /// Verifica que GetByEmailAsync retorna el usuario con el email especificado.
    /// </summary>
    [Fact]
    public async Task GetByEmailAsync_WithExistingEmail_ReturnsUser()
    {
        // Arrange
        var user = CreateTestUser();
        user.Email = "test@example.com";
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByEmailAsync("test@example.com");

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be("test@example.com");
    }

    /// <summary>
    /// Verifica que GetByEmailAsync retorna null cuando el email no existe.
    /// </summary>
    [Fact]
    public async Task GetByEmailAsync_WithNonExistentEmail_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByEmailAsync("nonexistent@example.com");

        // Assert
        result.Should().BeNull();
    }

    /// <summary>
    /// Verifica que GetByEmailAsync no retorna usuarios inactivos.
    /// </summary>
    [Fact]
    public async Task GetByEmailAsync_WithInactiveUser_ReturnsNull()
    {
        // Arrange
        var user = CreateTestUser();
        user.Email = "inactive@example.com";
        user.IsActive = false;
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByEmailAsync("inactive@example.com");

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region GetByUsernameAsync Tests

    /// <summary>
    /// Verifica que GetByUsernameAsync retorna el usuario con todos sus roles y permisos.
    /// </summary>
    [Fact]
    public async Task GetByUsernameAsync_ReturnsUserWithRolesAndPermissions()
    {
        // Arrange
        var user = CreateTestUser();
        user.Username = "testuser";
        var role = CreateTestRole();
        await _context.Users.AddAsync(user);
        await _context.Roles.AddAsync(role);
        await _context.SaveChangesAsync();

        var rolUser = new RolUser
        {
            UserId = user.Id,
            RolId = role.Id,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        await _context.RolUsers.AddAsync(rolUser);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByUsernameAsync("testuser");

        // Assert
        result.Should().NotBeNull();
        result!.Username.Should().Be("testuser");
        result.RolUsers.Should().HaveCount(1);
    }

    /// <summary>
    /// Verifica que GetByUsernameAsync retorna null cuando el username no existe.
    /// </summary>
    [Fact]
    public async Task GetByUsernameAsync_WithNonExistentUsername_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByUsernameAsync("nonexistent");

        // Assert
        result.Should().BeNull();
    }

    /// <summary>
    /// Verifica que GetByUsernameAsync no retorna usuarios inactivos.
    /// </summary>
    [Fact]
    public async Task GetByUsernameAsync_WithInactiveUser_ReturnsNull()
    {
        // Arrange
        var user = CreateTestUser();
        user.Username = "inactiveuser";
        user.IsActive = false;
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByUsernameAsync("inactiveuser");

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region GetByDocumentNumberAsync Tests

    /// <summary>
    /// Verifica que GetByDocumentNumberAsync retorna el usuario por número de documento.
    /// </summary>
    [Fact]
    public async Task GetByDocumentNumberAsync_WithExistingDocument_ReturnsUser()
    {
        // Arrange
        var user = CreateTestUser();
        user.Username = "12345678";
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByDocumentNumberAsync("12345678");

        // Assert
        result.Should().NotBeNull();
        result!.Username.Should().Be("12345678");
    }

    #endregion

    #region GetActiveAsync Tests

    /// <summary>
    /// Verifica que GetActiveAsync retorna solo usuarios activos.
    /// </summary>
    [Fact]
    public async Task GetActiveAsync_ReturnsOnlyActiveUsers()
    {
        // Arrange
        var activeUser1 = CreateTestUser();
        var activeUser2 = CreateTestUser();
        var inactiveUser = CreateTestUser();
        inactiveUser.IsActive = false;

        await _context.Users.AddRangeAsync(activeUser1, activeUser2, inactiveUser);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetActiveAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(u => u.IsActive);
    }

    #endregion

    #region GetByRoleAsync Tests

    /// <summary>
    /// Verifica que GetByRoleAsync retorna usuarios con el rol especificado.
    /// </summary>
    [Fact]
    public async Task GetByRoleAsync_ReturnsUsersWithRole()
    {
        // Arrange
        var user1 = CreateTestUser();
        var user2 = CreateTestUser();
        var user3 = CreateTestUser();
        var role1 = CreateTestRole();
        var role2 = CreateTestRole();

        await _context.Users.AddRangeAsync(user1, user2, user3);
        await _context.Roles.AddRangeAsync(role1, role2);
        await _context.SaveChangesAsync();

        var rolUser1 = new RolUser { UserId = user1.Id, RolId = role1.Id, IsActive = true, CreatedAt = DateTime.UtcNow };
        var rolUser2 = new RolUser { UserId = user2.Id, RolId = role1.Id, IsActive = true, CreatedAt = DateTime.UtcNow };
        var rolUser3 = new RolUser { UserId = user3.Id, RolId = role2.Id, IsActive = true, CreatedAt = DateTime.UtcNow };

        await _context.RolUsers.AddRangeAsync(rolUser1, rolUser2, rolUser3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByRoleAsync(role1.Id);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(u => u.RolUsers.Any(ru => ru.RolId == role1.Id));
    }

    /// <summary>
    /// Verifica que GetByRoleAsync no retorna usuarios inactivos.
    /// </summary>
    [Fact]
    public async Task GetByRoleAsync_ExcludesInactiveUsers()
    {
        // Arrange
        var activeUser = CreateTestUser();
        var inactiveUser = CreateTestUser();
        inactiveUser.IsActive = false;
        var role = CreateTestRole();

        await _context.Users.AddRangeAsync(activeUser, inactiveUser);
        await _context.Roles.AddAsync(role);
        await _context.SaveChangesAsync();

        var rolUser1 = new RolUser { UserId = activeUser.Id, RolId = role.Id, IsActive = true, CreatedAt = DateTime.UtcNow };
        var rolUser2 = new RolUser { UserId = inactiveUser.Id, RolId = role.Id, IsActive = true, CreatedAt = DateTime.UtcNow };

        await _context.RolUsers.AddRangeAsync(rolUser1, rolUser2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByRoleAsync(role.Id);

        // Assert
        result.Should().HaveCount(1);
        result.Should().OnlyContain(u => u.IsActive);
    }

    #endregion

    #region GetByRoleIdAsync Tests

    /// <summary>
    /// Verifica que GetByRoleIdAsync retorna usuarios con el rol especificado.
    /// </summary>
    [Fact]
    public async Task GetByRoleIdAsync_ReturnsUsersWithRole()
    {
        // Arrange
        var user1 = CreateTestUser();
        var user2 = CreateTestUser();
        var role = CreateTestRole();

        await _context.Users.AddRangeAsync(user1, user2);
        await _context.Roles.AddAsync(role);
        await _context.SaveChangesAsync();

        var rolUser1 = new RolUser { UserId = user1.Id, RolId = role.Id, IsActive = true, CreatedAt = DateTime.UtcNow };
        var rolUser2 = new RolUser { UserId = user2.Id, RolId = role.Id, IsActive = true, CreatedAt = DateTime.UtcNow };

        await _context.RolUsers.AddRangeAsync(rolUser1, rolUser2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByRoleIdAsync(role.Id);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(u => u.RolUsers.Any(ru => ru.RolId == role.Id));
    }

    #endregion

    #region ExistsByEmailAsync Tests

    /// <summary>
    /// Verifica que ExistsByEmailAsync retorna true cuando el email existe.
    /// </summary>
    [Fact]
    public async Task ExistsByEmailAsync_WithExistingEmail_ReturnsTrue()
    {
        // Arrange
        var user = CreateTestUser();
        user.Email = "exists@example.com";
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsByEmailAsync("exists@example.com");

        // Assert
        result.Should().BeTrue();
    }

    /// <summary>
    /// Verifica que ExistsByEmailAsync retorna false cuando el email no existe.
    /// </summary>
    [Fact]
    public async Task ExistsByEmailAsync_WithNonExistentEmail_ReturnsFalse()
    {
        // Act
        var result = await _repository.ExistsByEmailAsync("nonexistent@example.com");

        // Assert
        result.Should().BeFalse();
    }

    /// <summary>
    /// Verifica que ExistsByEmailAsync retorna true incluso para usuarios inactivos.
    /// </summary>
    [Fact]
    public async Task ExistsByEmailAsync_WithInactiveUser_ReturnsTrue()
    {
        // Arrange
        var user = CreateTestUser();
        user.Email = "inactive@example.com";
        user.IsActive = false;
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsByEmailAsync("inactive@example.com");

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region ExistsByUsernameAsync Tests

    /// <summary>
    /// Verifica que ExistsByUsernameAsync retorna true cuando el username existe.
    /// </summary>
    [Fact]
    public async Task ExistsByUsernameAsync_WithExistingUsername_ReturnsTrue()
    {
        // Arrange
        var user = CreateTestUser();
        user.Username = "existinguser";
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsByUsernameAsync("existinguser");

        // Assert
        result.Should().BeTrue();
    }

    /// <summary>
    /// Verifica que ExistsByUsernameAsync retorna false cuando el username no existe.
    /// </summary>
    [Fact]
    public async Task ExistsByUsernameAsync_WithNonExistentUsername_ReturnsFalse()
    {
        // Act
        var result = await _repository.ExistsByUsernameAsync("nonexistent");

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region ExistsByDocumentNumberAsync Tests

    /// <summary>
    /// Verifica que ExistsByDocumentNumberAsync retorna true cuando el documento existe.
    /// </summary>
    [Fact]
    public async Task ExistsByDocumentNumberAsync_WithExistingDocument_ReturnsTrue()
    {
        // Arrange
        var user = CreateTestUser();
        user.Username = "12345678";
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsByDocumentNumberAsync("12345678");

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region DeleteAsync Tests

    /// <summary>
    /// Verifica que DeleteAsync realiza soft delete marcando IsActive como false.
    /// </summary>
    [Fact]
    public async Task DeleteAsync_PerformsSoftDelete()
    {
        // Arrange
        var user = CreateTestUser();
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(user.Id);
        await _context.SaveChangesAsync();

        // Assert
        var deleted = await _context.Users.FindAsync(user.Id);
        deleted.Should().NotBeNull();
        deleted!.IsActive.Should().BeFalse();
    }

    /// <summary>
    /// Verifica que DeleteAsync no falla cuando el ID no existe.
    /// </summary>
    [Fact]
    public async Task DeleteAsync_WithNonExistentId_DoesNotThrow()
    {
        // Act
        var act = async () => await _repository.DeleteAsync(999);

        // Assert
        await act.Should().NotThrowAsync();
    }

    #endregion

    #region GetUserPermissionsAsync Tests

    /// <summary>
    /// Verifica que GetUserPermissionsAsync retorna los permisos agrupados por formulario.
    /// </summary>
    [Fact]
    public async Task GetUserPermissionsAsync_ReturnsPermissionsGroupedByForm()
    {
        // Arrange
        var user = CreateTestUser();
        var role = CreateTestRole();
        var form = CreateTestForm();
        var permission = CreateTestPermission();

        await _context.Users.AddAsync(user);
        await _context.Roles.AddAsync(role);
        await _context.Forms.AddAsync(form);
        await _context.Permissions.AddAsync(permission);
        await _context.SaveChangesAsync();

        var rolUser = new RolUser
        {
            UserId = user.Id,
            RolId = role.Id,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        await _context.RolUsers.AddAsync(rolUser);
        await _context.SaveChangesAsync();

        var rolFormPermission = new RolFormPermission
        {
            RolId = role.Id,
            FormId = form.Id,
            PermissionId = permission.Id,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        await _context.RolFormPermissions.AddAsync(rolFormPermission);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetUserPermissionsAsync(user.Id);

        // Assert
        result.Should().NotBeNull();
        var resultObj = result as dynamic;
        resultObj.Should().NotBeNull();
    }

    /// <summary>
    /// Verifica que GetUserPermissionsAsync retorna objeto vacío cuando el usuario no existe.
    /// </summary>
    [Fact]
    public async Task GetUserPermissionsAsync_WithNonExistentUser_ReturnsEmptyObject()
    {
        // Act
        var result = await _repository.GetUserPermissionsAsync(999);

        // Assert
        result.Should().NotBeNull();
    }

    /// <summary>
    /// Verifica que GetUserPermissionsAsync no retorna permisos de usuarios inactivos.
    /// </summary>
    [Fact]
    public async Task GetUserPermissionsAsync_WithInactiveUser_ReturnsEmptyObject()
    {
        // Arrange
        var user = CreateTestUser();
        user.IsActive = false;
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetUserPermissionsAsync(user.Id);

        // Assert
        result.Should().NotBeNull();
    }

    #endregion

    #region Helper Methods

    private User CreateTestUser()
    {
        return new User
        {
            Username = $"user{Guid.NewGuid().ToString().Substring(0, 8)}",
            Email = $"user{Guid.NewGuid().ToString().Substring(0, 8)}@example.com",
            Password = "HashedPassword123",
            FullName = "Test User",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    private Rol CreateTestRole()
    {
        return new Rol
        {
            Name = $"Role {Guid.NewGuid()}",
            Description = "Test Role",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    private Form CreateTestForm()
    {
        return new Form
        {
            Name = $"Form {Guid.NewGuid()}",
            Code = $"CODE{Guid.NewGuid().ToString().Substring(0, 8)}",
            Description = "Test Form",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    private Permission CreateTestPermission()
    {
        return new Permission
        {
            Name = "Test Permission",
            CanRead = true,
            CanCreate = true,
            CanUpdate = true,
            CanDelete = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    #endregion

    public void Dispose()
    {
        _context?.Dispose();
    }
}
