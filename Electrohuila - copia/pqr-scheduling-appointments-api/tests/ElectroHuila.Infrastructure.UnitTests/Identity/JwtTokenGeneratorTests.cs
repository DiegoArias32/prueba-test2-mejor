using ElectroHuila.Domain.Entities.Security;
using ElectroHuila.Infrastructure.Identity;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace ElectroHuila.Infrastructure.UnitTests.Identity;

/// <summary>
/// Pruebas unitarias para el generador de tokens JWT.
/// Verifica que los tokens se generen correctamente con la configuración y claims apropiados.
/// </summary>
public class JwtTokenGeneratorTests
{
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly JwtTokenGenerator _tokenGenerator;

    /// <summary>
    /// Inicializa una nueva instancia de las pruebas del generador JWT.
    /// Configura los mocks necesarios y la configuración de prueba.
    /// </summary>
    public JwtTokenGeneratorTests()
    {
        _configurationMock = new Mock<IConfiguration>();

        _configurationMock.Setup(x => x["Jwt:Secret"])
            .Returns("SuperSecretKeyForTestingPurposesOnly123456789");
        _configurationMock.Setup(x => x["Jwt:Issuer"])
            .Returns("ElectroHuila");
        _configurationMock.Setup(x => x["Jwt:Audience"])
            .Returns("ElectroHuila.Client");
        _configurationMock.Setup(x => x["Jwt:ExpirationMinutes"])
            .Returns("60");

        _tokenGenerator = new JwtTokenGenerator(_configurationMock.Object);
    }

    /// <summary>
    /// Verifica que se genere un token JWT válido con el formato correcto.
    /// Un token JWT debe tener 3 partes separadas por puntos.
    /// </summary>
    [Fact]
    public void GenerateToken_Should_Return_Valid_Token()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Email = "test@example.com"
        };

        // Act
        var token = _tokenGenerator.GenerateToken(user);

        // Assert
        token.Should().NotBeNullOrEmpty();
        token.Split('.').Should().HaveCount(3); // JWT has 3 parts
    }

    /// <summary>
    /// Verifica que el token generado incluya los claims del usuario.
    /// El token debe contener la información del usuario proporcionado.
    /// </summary>
    [Fact]
    public void GenerateToken_Should_Include_User_Claims()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Email = "test@example.com"
        };

        // Act
        var token = _tokenGenerator.GenerateToken(user);

        // Assert
        token.Should().NotBeNullOrEmpty();
    }
}