using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace MoneyMatters.Api.Tests.Controllers;

public class HealthControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public HealthControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetHealth_ReturnsOkStatus()
    {
        // Act
        var response = await _client.GetAsync("/api/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetHealth_ReturnsHealthyStatus()
    {
        // Act
        var response = await _client.GetAsync("/api/health");
        var content = await response.Content.ReadFromJsonAsync<HealthResponse>();

        // Assert
        content.Should().NotBeNull();
        content!.Status.Should().Be("Healthy");
        content.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        content.Version.Should().Be("1.0.0");
    }

    // Note: /health endpoint requires database connection and is tested separately in integration tests

    private record HealthResponse(
        string Status,
        DateTime Timestamp,
        string Version,
        string Environment);
}
