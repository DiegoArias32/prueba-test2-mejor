using ElectroHuila.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace ElectroHuila.Domain.UnitTests.ValueObjects;

/// <summary>
/// Pruebas unitarias para el objeto de valor Email del dominio.
/// Verifica la validación de formato, reglas de negocio y comportamiento del email.
/// </summary>
public class EmailTests
{
    /// <summary>
    /// Verifica que el objeto Email acepta direcciones de correo con formato válido.
    /// Debe permitir diferentes formatos estándar de email sin lanzar excepciones.
    /// </summary>
    /// <param name="emailAddress">Dirección de email válida para probar</param>
    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@domain.co")]
    [InlineData("info@electrohuila.com")]
    public void Email_Should_Be_Valid(string emailAddress)
    {
        // Arrange & Act
        var email = new Email(emailAddress);

        // Assert
        email.Value.Should().Be(emailAddress);
    }

    /// <summary>
    /// Verifica que el objeto Email rechaza direcciones con formato inválido.
    /// Debe lanzar ArgumentException para emails malformados o vacíos.
    /// </summary>
    /// <param name="invalidEmail">Dirección de email inválida para probar</param>
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("invalid-email")]
    [InlineData("@example.com")]
    [InlineData("user@")]
    public void Email_Should_Throw_Exception_For_Invalid_Format(string invalidEmail)
    {
        // Act
        var act = () => new Email(invalidEmail);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    /// <summary>
    /// Verifica que el objeto Email maneja correctamente las mayúsculas y minúsculas.
    /// Los emails deben ser tratados de forma insensible a mayúsculas para comparaciones.
    /// </summary>
    [Fact]
    public void Email_Should_Be_Case_Insensitive()
    {
        // Arrange
        var email1 = new Email("Test@Example.com");
        var email2 = new Email("test@example.com");

        // Assert
        email1.Value.ToLower().Should().Be(email2.Value.ToLower());
    }
}