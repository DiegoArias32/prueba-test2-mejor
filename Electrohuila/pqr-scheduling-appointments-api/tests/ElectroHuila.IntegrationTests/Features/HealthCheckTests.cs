using ElectroHuila.IntegrationTests.Base;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Xunit;

namespace ElectroHuila.IntegrationTests.Features;

public class HealthCheckTests : IntegrationTestBase
{
    public HealthCheckTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task Setup_Health_Endpoint_Should_Return_Success()
    {
        // Act
        var response = await Client.GetAsync("/api/v1/setup/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Setup_Ping_Endpoint_Should_Return_Success()
    {
        // Act
        var response = await Client.GetAsync("/api/v1/setup/ping");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("pong");
    }
}