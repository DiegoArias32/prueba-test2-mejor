using ElectroHuila.Domain.Entities.Locations;
using ElectroHuila.Infrastructure.Persistence;
using ElectroHuila.Infrastructure.Persistence.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Xunit;

namespace ElectroHuila.Infrastructure.UnitTests.Repositories;

/// <summary>
/// Pruebas unitarias para el repositorio de sucursales (BranchRepository).
/// Verifica el comportamiento de las operaciones CRUD y consultas especializadas para sucursales.
/// </summary>
public class BranchRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly BranchRepository _repository;

    /// <summary>
    /// Inicializa una nueva instancia de las pruebas del repositorio de sucursales.
    /// Configura un contexto de base de datos en memoria para pruebas.
    /// </summary>
    public BranchRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new BranchRepository(_context);
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
    /// Verifica que GetByNameAsync retorne la sucursal correcta cuando existe y está activa.
    /// </summary>
    [Fact]
    public async Task GetByNameAsync_Should_Return_Branch_When_Name_Exists_And_IsActive()
    {
        // Arrange
        var branch = new Branch
        {
            Name = "Sucursal Centro",
            Code = "SC01",
            Address = "Calle Principal 123",
            Phone = "3001234567",
            City = "Neiva",
            State = "Huila",
            IsMain = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Branches.AddAsync(branch);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByNameAsync("Sucursal Centro");

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Sucursal Centro");
        result.Code.Should().Be("SC01");
        result.City.Should().Be("Neiva");
        result.IsActive.Should().BeTrue();
    }

    /// <summary>
    /// Verifica que GetByNameAsync retorne null cuando la sucursal está inactiva.
    /// </summary>
    [Fact]
    public async Task GetByNameAsync_Should_Return_Null_When_Branch_Is_Inactive()
    {
        // Arrange
        var branch = new Branch
        {
            Name = "Sucursal Eliminada",
            Code = "SD01",
            Address = "Calle Secundaria 456",
            Phone = "3009876543",
            City = "Neiva",
            State = "Huila",
            IsMain = false,
            IsActive = false,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Branches.AddAsync(branch);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByNameAsync("Sucursal Eliminada");

        // Assert
        result.Should().BeNull();
    }

    /// <summary>
    /// Verifica que GetByNameAsync retorne null cuando no existe la sucursal.
    /// </summary>
    [Fact]
    public async Task GetByNameAsync_Should_Return_Null_When_Name_Does_Not_Exist()
    {
        // Act
        var result = await _repository.GetByNameAsync("Sucursal Inexistente");

        // Assert
        result.Should().BeNull();
    }

    /// <summary>
    /// Verifica que GetByCodeAsync retorne la sucursal correcta cuando existe el código y está activa.
    /// </summary>
    [Fact]
    public async Task GetByCodeAsync_Should_Return_Branch_When_Code_Exists_And_IsActive()
    {
        // Arrange
        var branch = new Branch
        {
            Name = "Sucursal Norte",
            Code = "SN01",
            Address = "Avenida Norte 789",
            Phone = "3005551234",
            City = "Pitalito",
            State = "Huila",
            IsMain = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Branches.AddAsync(branch);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByCodeAsync("SN01");

        // Assert
        result.Should().NotBeNull();
        result!.Code.Should().Be("SN01");
        result.Name.Should().Be("Sucursal Norte");
        result.City.Should().Be("Pitalito");
    }

    /// <summary>
    /// Verifica que GetByCodeAsync retorne null cuando la sucursal está inactiva.
    /// </summary>
    [Fact]
    public async Task GetByCodeAsync_Should_Return_Null_When_Branch_Is_Inactive()
    {
        // Arrange
        var branch = new Branch
        {
            Name = "Sucursal Inactiva",
            Code = "SI01",
            Address = "Calle Inactiva 111",
            Phone = "3002223344",
            City = "Garzón",
            State = "Huila",
            IsMain = false,
            IsActive = false,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Branches.AddAsync(branch);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByCodeAsync("SI01");

        // Assert
        result.Should().BeNull();
    }

    /// <summary>
    /// Verifica que GetActiveAsync retorne solo sucursales activas ordenadas alfabéticamente.
    /// </summary>
    [Fact]
    public async Task GetActiveAsync_Should_Return_Only_Active_Branches_Ordered_By_Name()
    {
        // Arrange
        var branches = new List<Branch>
        {
            new Branch { Name = "Zeta Sucursal", Code = "ZS01", Address = "Dirección Z", Phone = "300111", City = "Neiva", State = "Huila", IsMain = false, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Branch { Name = "Alpha Sucursal", Code = "AS01", Address = "Dirección A", Phone = "300222", City = "Neiva", State = "Huila", IsMain = false, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Branch { Name = "Inactive Sucursal", Code = "IS01", Address = "Dirección I", Phone = "300333", City = "Neiva", State = "Huila", IsMain = false, IsActive = false, CreatedAt = DateTime.UtcNow },
            new Branch { Name = "Beta Sucursal", Code = "BS01", Address = "Dirección B", Phone = "300444", City = "Neiva", State = "Huila", IsMain = false, IsActive = true, CreatedAt = DateTime.UtcNow }
        };

        await _context.Branches.AddRangeAsync(branches);
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetActiveAsync()).ToList();

        // Assert
        result.Should().HaveCount(3);
        result.Should().NotContain(b => b.Code == "IS01");
        result[0].Name.Should().Be("Alpha Sucursal");
        result[1].Name.Should().Be("Beta Sucursal");
        result[2].Name.Should().Be("Zeta Sucursal");
    }

    /// <summary>
    /// Verifica que GetActiveAsync retorne lista vacía cuando no hay sucursales activas.
    /// </summary>
    [Fact]
    public async Task GetActiveAsync_Should_Return_Empty_List_When_No_Active_Branches()
    {
        // Arrange
        var branch = new Branch
        {
            Name = "Solo Inactiva",
            Code = "SOI01",
            Address = "Dirección Inactiva",
            Phone = "3001112233",
            City = "Neiva",
            State = "Huila",
            IsMain = false,
            IsActive = false,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Branches.AddAsync(branch);
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetActiveAsync()).ToList();

        // Assert
        result.Should().BeEmpty();
    }

    /// <summary>
    /// Verifica que GetAllIncludingInactiveAsync retorne todas las sucursales (activas e inactivas).
    /// </summary>
    [Fact]
    public async Task GetAllIncludingInactiveAsync_Should_Return_All_Branches_Including_Inactive()
    {
        // Arrange
        var branches = new List<Branch>
        {
            new Branch { Name = "Activa 1", Code = "A1", Address = "Dir A1", Phone = "3001", City = "Neiva", State = "Huila", IsMain = false, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Branch { Name = "Inactiva 1", Code = "I1", Address = "Dir I1", Phone = "3002", City = "Neiva", State = "Huila", IsMain = false, IsActive = false, CreatedAt = DateTime.UtcNow },
            new Branch { Name = "Activa 2", Code = "A2", Address = "Dir A2", Phone = "3003", City = "Neiva", State = "Huila", IsMain = false, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Branch { Name = "Inactiva 2", Code = "I2", Address = "Dir I2", Phone = "3004", City = "Neiva", State = "Huila", IsMain = false, IsActive = false, CreatedAt = DateTime.UtcNow }
        };

        await _context.Branches.AddRangeAsync(branches);
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetAllIncludingInactiveAsync()).ToList();

        // Assert
        result.Should().HaveCount(4);
        result.Should().Contain(b => b.Code == "A1");
        result.Should().Contain(b => b.Code == "I1");
        result.Should().Contain(b => b.Code == "A2");
        result.Should().Contain(b => b.Code == "I2");
    }

    /// <summary>
    /// Verifica que GetByCityAsync retorne solo sucursales activas de la ciudad especificada.
    /// </summary>
    [Fact]
    public async Task GetByCityAsync_Should_Return_Only_Active_Branches_In_Specified_City()
    {
        // Arrange
        var branches = new List<Branch>
        {
            new Branch { Name = "Neiva Central", Code = "NC01", Address = "Dir NC", Phone = "3001", City = "Neiva", State = "Huila", IsMain = true, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Branch { Name = "Neiva Norte", Code = "NN01", Address = "Dir NN", Phone = "3002", City = "Neiva", State = "Huila", IsMain = false, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Branch { Name = "Pitalito Central", Code = "PC01", Address = "Dir PC", Phone = "3003", City = "Pitalito", State = "Huila", IsMain = false, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Branch { Name = "Neiva Inactiva", Code = "NI01", Address = "Dir NI", Phone = "3004", City = "Neiva", State = "Huila", IsMain = false, IsActive = false, CreatedAt = DateTime.UtcNow }
        };

        await _context.Branches.AddRangeAsync(branches);
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetByCityAsync("Neiva")).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(b => b.Code == "NC01");
        result.Should().Contain(b => b.Code == "NN01");
        result.Should().NotContain(b => b.Code == "PC01");
        result.Should().NotContain(b => b.Code == "NI01");
    }

    /// <summary>
    /// Verifica que GetByCityAsync retorne lista vacía cuando no hay sucursales en la ciudad.
    /// </summary>
    [Fact]
    public async Task GetByCityAsync_Should_Return_Empty_List_When_No_Branches_In_City()
    {
        // Arrange
        var branch = new Branch
        {
            Name = "Pitalito Central",
            Code = "PC01",
            Address = "Dirección PC",
            Phone = "3005556789",
            City = "Pitalito",
            State = "Huila",
            IsMain = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Branches.AddAsync(branch);
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetByCityAsync("Bogotá")).ToList();

        // Assert
        result.Should().BeEmpty();
    }

    /// <summary>
    /// Verifica que GetByDepartmentAsync retorne solo sucursales activas del departamento especificado.
    /// </summary>
    [Fact]
    public async Task GetByDepartmentAsync_Should_Return_Only_Active_Branches_In_Specified_Department()
    {
        // Arrange
        var branches = new List<Branch>
        {
            new Branch { Name = "Neiva Central", Code = "NC01", Address = "Dir NC", Phone = "3001", City = "Neiva", State = "Huila", IsMain = true, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Branch { Name = "Pitalito Central", Code = "PC01", Address = "Dir PC", Phone = "3002", City = "Pitalito", State = "Huila", IsMain = false, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Branch { Name = "Bogotá Central", Code = "BC01", Address = "Dir BC", Phone = "3003", City = "Bogotá", State = "Cundinamarca", IsMain = false, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Branch { Name = "Garzón Inactiva", Code = "GI01", Address = "Dir GI", Phone = "3004", City = "Garzón", State = "Huila", IsMain = false, IsActive = false, CreatedAt = DateTime.UtcNow }
        };

        await _context.Branches.AddRangeAsync(branches);
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetByDepartmentAsync("Huila")).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(b => b.Code == "NC01");
        result.Should().Contain(b => b.Code == "PC01");
        result.Should().NotContain(b => b.Code == "BC01");
        result.Should().NotContain(b => b.Code == "GI01");
    }

    /// <summary>
    /// Verifica que GetByDepartmentAsync retorne lista vacía cuando no hay sucursales en el departamento.
    /// </summary>
    [Fact]
    public async Task GetByDepartmentAsync_Should_Return_Empty_List_When_No_Branches_In_Department()
    {
        // Arrange
        var branch = new Branch
        {
            Name = "Neiva Central",
            Code = "NC01",
            Address = "Dirección NC",
            Phone = "3001112233",
            City = "Neiva",
            State = "Huila",
            IsMain = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Branches.AddAsync(branch);
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetByDepartmentAsync("Antioquia")).ToList();

        // Assert
        result.Should().BeEmpty();
    }

    /// <summary>
    /// Verifica que ExistsByNameAsync retorne true cuando existe una sucursal activa con el nombre.
    /// </summary>
    [Fact]
    public async Task ExistsByNameAsync_Should_Return_True_When_Active_Branch_With_Name_Exists()
    {
        // Arrange
        var branch = new Branch
        {
            Name = "Sucursal Test",
            Code = "ST01",
            Address = "Dirección ST",
            Phone = "3001234567",
            City = "Neiva",
            State = "Huila",
            IsMain = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Branches.AddAsync(branch);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsByNameAsync("Sucursal Test");

        // Assert
        result.Should().BeTrue();
    }

    /// <summary>
    /// Verifica que ExistsByNameAsync retorne false cuando la sucursal está inactiva.
    /// </summary>
    [Fact]
    public async Task ExistsByNameAsync_Should_Return_False_When_Branch_Is_Inactive()
    {
        // Arrange
        var branch = new Branch
        {
            Name = "Sucursal Inactiva",
            Code = "SI01",
            Address = "Dirección SI",
            Phone = "3009876543",
            City = "Neiva",
            State = "Huila",
            IsMain = false,
            IsActive = false,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Branches.AddAsync(branch);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsByNameAsync("Sucursal Inactiva");

        // Assert
        result.Should().BeFalse();
    }

    /// <summary>
    /// Verifica que ExistsByCodeAsync retorne true cuando existe una sucursal activa con el código.
    /// </summary>
    [Fact]
    public async Task ExistsByCodeAsync_Should_Return_True_When_Active_Branch_With_Code_Exists()
    {
        // Arrange
        var branch = new Branch
        {
            Name = "Sucursal Test",
            Code = "TEST01",
            Address = "Dirección Test",
            Phone = "3001234567",
            City = "Neiva",
            State = "Huila",
            IsMain = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Branches.AddAsync(branch);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsByCodeAsync("TEST01");

        // Assert
        result.Should().BeTrue();
    }

    /// <summary>
    /// Verifica que ExistsByCodeAsync retorne false cuando la sucursal está inactiva.
    /// </summary>
    [Fact]
    public async Task ExistsByCodeAsync_Should_Return_False_When_Branch_Is_Inactive()
    {
        // Arrange
        var branch = new Branch
        {
            Name = "Sucursal Inactiva",
            Code = "INACT01",
            Address = "Dirección Inactiva",
            Phone = "3009876543",
            City = "Neiva",
            State = "Huila",
            IsMain = false,
            IsActive = false,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Branches.AddAsync(branch);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsByCodeAsync("INACT01");

        // Assert
        result.Should().BeFalse();
    }

    /// <summary>
    /// Verifica que GetMainBranchAsync retorne la sucursal principal cuando existe y está activa.
    /// </summary>
    [Fact]
    public async Task GetMainBranchAsync_Should_Return_Main_Branch_When_Exists_And_IsActive()
    {
        // Arrange
        var branches = new List<Branch>
        {
            new Branch { Name = "Sucursal Secundaria", Code = "SS01", Address = "Dir SS", Phone = "3001", City = "Pitalito", State = "Huila", IsMain = false, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Branch { Name = "Sucursal Principal", Code = "SP01", Address = "Dir SP", Phone = "3002", City = "Neiva", State = "Huila", IsMain = true, IsActive = true, CreatedAt = DateTime.UtcNow }
        };

        await _context.Branches.AddRangeAsync(branches);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetMainBranchAsync();

        // Assert
        result.Should().NotBeNull();
        result!.IsMain.Should().BeTrue();
        result.Code.Should().Be("SP01");
        result.Name.Should().Be("Sucursal Principal");
    }

    /// <summary>
    /// Verifica que GetMainBranchAsync retorne null cuando no hay sucursal principal activa.
    /// </summary>
    [Fact]
    public async Task GetMainBranchAsync_Should_Return_Null_When_No_Main_Branch_Is_Active()
    {
        // Arrange
        var branch = new Branch
        {
            Name = "Sucursal Principal Inactiva",
            Code = "SPI01",
            Address = "Dirección SPI",
            Phone = "3001234567",
            City = "Neiva",
            State = "Huila",
            IsMain = true,
            IsActive = false,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Branches.AddAsync(branch);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetMainBranchAsync();

        // Assert
        result.Should().BeNull();
    }

    /// <summary>
    /// Verifica que DeleteAsync realice soft delete marcando IsActive como false.
    /// </summary>
    [Fact]
    public async Task DeleteAsync_Should_Perform_Soft_Delete_By_Setting_IsActive_False()
    {
        // Arrange
        var branch = new Branch
        {
            Name = "Sucursal a Eliminar",
            Code = "SAE01",
            Address = "Dirección SAE",
            Phone = "3001234567",
            City = "Neiva",
            State = "Huila",
            IsMain = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Branches.AddAsync(branch);
        await _context.SaveChangesAsync();

        var branchId = branch.Id;

        // Act
        await _repository.DeleteAsync(branchId);
        await _context.SaveChangesAsync();

        // Assert
        var deletedBranch = await _context.Branches.FindAsync(branchId);
        deletedBranch.Should().NotBeNull();
        deletedBranch!.IsActive.Should().BeFalse();
    }

    /// <summary>
    /// Verifica que DeleteAsync no haga nada cuando la sucursal no existe.
    /// </summary>
    [Fact]
    public async Task DeleteAsync_Should_Do_Nothing_When_Branch_Does_Not_Exist()
    {
        // Act
        await _repository.DeleteAsync(999);
        await _context.SaveChangesAsync();

        // Assert
        var branches = await _context.Branches.ToListAsync();
        branches.Should().BeEmpty();
    }

    /// <summary>
    /// Verifica que UpdateColor actualice correctamente el color de la sucursal.
    /// </summary>
    [Fact]
    public async Task UpdateColor_Should_Update_Branch_Color_Successfully()
    {
        // Arrange
        var branch = new Branch
        {
            Name = "Sucursal con Color",
            Code = "SCC01",
            Address = "Dirección SCC",
            Phone = "3001234567",
            City = "Neiva",
            State = "Huila",
            IsMain = false,
            ColorPrimary = "#FF0000",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Branches.AddAsync(branch);
        await _context.SaveChangesAsync();

        var newColor = "#00FF00";

        // Act
        branch.UpdateColor(newColor);
        await _context.SaveChangesAsync();

        // Assert
        var updatedBranch = await _context.Branches.FindAsync(branch.Id);
        updatedBranch.Should().NotBeNull();
        updatedBranch!.ColorPrimary.Should().Be(newColor);
    }
}
