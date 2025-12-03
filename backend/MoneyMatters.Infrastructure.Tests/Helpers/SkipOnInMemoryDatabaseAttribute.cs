using Xunit;

namespace MoneyMatters.Infrastructure.Tests.Helpers;

/// <summary>
/// Custom xUnit attribute to skip tests when using in-memory database.
/// Use this for tests that require real database constraint enforcement (foreign keys, unique constraints).
/// </summary>
public sealed class SkipOnInMemoryDatabaseAttribute : FactAttribute
{
    public SkipOnInMemoryDatabaseAttribute()
    {
        // In-memory SQLite doesn't enforce foreign key constraints and some unique constraints
        // like PostgreSQL does, so we skip these tests in the in-memory environment
        Skip = "Skipped on in-memory database - requires PostgreSQL for proper constraint enforcement";
    }
}
