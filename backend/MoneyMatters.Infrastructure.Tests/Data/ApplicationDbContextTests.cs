using FluentAssertions;
using MoneyMatters.Infrastructure.Tests.Helpers;

namespace MoneyMatters.Infrastructure.Tests.Data;

/// <summary>
/// Tests for ApplicationDbContext configuration and behavior.
/// </summary>
public class ApplicationDbContextTests : DatabaseTestBase
{
    [Fact]
    public void Context_CanBeCreated()
    {
        // Assert
        Context.Should().NotBeNull();
        Context.Database.Should().NotBeNull();
    }

    [Fact]
    public void Context_CanConnectToDatabase()
    {
        // Act
        var canConnect = Context.Database.CanConnect();

        // Assert
        canConnect.Should().BeTrue();
    }
}
