using Microsoft.EntityFrameworkCore;
using MoneyMatters.Infrastructure.Data;

namespace MoneyMatters.Infrastructure.Tests.Helpers;

/// <summary>
/// Base class for tests that require a database context.
/// Uses in-memory database for fast, isolated tests.
/// </summary>
public abstract class DatabaseTestBase : IDisposable
{
    protected ApplicationDbContext Context { get; private set; }

    protected DatabaseTestBase()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        Context = new ApplicationDbContext(options);
        Context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        Context.Dispose();
        GC.SuppressFinalize(this);
    }
}
