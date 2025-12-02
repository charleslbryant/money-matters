# Money Matters Backend - Testing Guide

This document provides comprehensive guidance on testing the Money Matters backend.

## Test Project Structure

The solution includes four test projects following the clean architecture layers:

```
backend/
├── MoneyMatters.Core.Tests/          # Domain entity and value object tests
├── MoneyMatters.Application.Tests/   # Business logic, CQRS handlers, services
├── MoneyMatters.Infrastructure.Tests/ # Data access, repository tests
└── MoneyMatters.Api.Tests/           # API integration tests
```

## Test Frameworks and Libraries

All test projects use:
- **xUnit** - Test framework
- **FluentAssertions** - Readable assertions
- **Moq** - Mocking framework
- **Coverlet** - Code coverage collection

Additional packages by project:
- **Infrastructure.Tests** & **Api.Tests**: `Microsoft.EntityFrameworkCore.InMemory` for database testing
- **Api.Tests**: `Microsoft.AspNetCore.Mvc.Testing` for integration tests

## Running Tests

### Run All Tests

```bash
# From backend directory
dotnet test MoneyMatters.sln

# With detailed output
dotnet test MoneyMatters.sln --verbosity detailed

# Run specific test project
dotnet test MoneyMatters.Core.Tests/MoneyMatters.Core.Tests.csproj
```

### Run Specific Tests

```bash
# Run tests by class name
dotnet test --filter "FullyQualifiedName~AccountTests"

# Run tests by namespace
dotnet test --filter "FullyQualifiedName~MoneyMatters.Core.Tests"

# Run tests by display name
dotnet test --filter "DisplayName~CanConnectToDatabase"
```

### Run Tests with Code Coverage

```bash
# Using the provided script
./run-tests-with-coverage.sh

# Or manually
dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults
```

### Generate Coverage Reports

```bash
# Install ReportGenerator globally (one-time setup)
dotnet tool install -g dotnet-reportgenerator-globaltool

# Generate HTML coverage report
reportgenerator \
  -reports:./TestResults/**/coverage.cobertura.xml \
  -targetdir:./TestResults/CoverageReport \
  -reporttypes:Html

# Open the report
# Linux/WSL: xdg-open ./TestResults/CoverageReport/index.html
# macOS: open ./TestResults/CoverageReport/index.html
# Windows: start ./TestResults/CoverageReport/index.html
```

## Test Naming Conventions

All tests follow the pattern: `MethodName_StateUnderTest_ExpectedBehavior`

### Examples

**Good:**
```csharp
[Fact]
public void Constructor_WithValidData_CreatesAccount() { }

[Fact]
public void AddTransaction_WhenAccountIsClosed_ThrowsException() { }

[Fact]
public void CalculateBalance_WithMultipleTransactions_ReturnsCorrectSum() { }
```

**Avoid:**
```csharp
[Fact]
public void Test1() { }  // Not descriptive

[Fact]
public void AccountCreation() { }  // Missing state and expectation
```

## Test Organization

### Core.Tests

Tests for domain entities, value objects, and domain services.

```
MoneyMatters.Core.Tests/
├── Entities/
│   ├── AccountTests.cs
│   ├── BillTests.cs
│   ├── GoalTests.cs
│   └── TransactionTests.cs
├── ValueObjects/
│   └── MoneyTests.cs
└── README.md
```

### Application.Tests

Tests for CQRS handlers, validators, and application services (forecast/alert engines).

```
MoneyMatters.Application.Tests/
├── Commands/
│   └── Accounts/
│       ├── CreateAccountCommandHandlerTests.cs
│       └── CreateAccountCommandValidatorTests.cs
├── Queries/
│   └── Forecasts/
│       └── GetForecastQueryHandlerTests.cs
├── Services/
│   ├── ForecastEngineTests.cs
│   └── AlertEngineTests.cs
└── README.md
```

### Infrastructure.Tests

Tests for database access, repositories, and external service integrations.

```
MoneyMatters.Infrastructure.Tests/
├── Data/
│   └── ApplicationDbContextTests.cs
├── Repositories/
│   ├── AccountRepositoryTests.cs
│   └── BillRepositoryTests.cs
├── Helpers/
│   └── DatabaseTestBase.cs
└── README.md
```

### Api.Tests

Integration tests for API endpoints and middleware.

```
MoneyMatters.Api.Tests/
├── Controllers/
│   ├── AccountsControllerTests.cs
│   ├── BillsControllerTests.cs
│   └── HealthControllerTests.cs
├── Helpers/
│   └── CustomWebApplicationFactory.cs
└── README.md
```

## Writing Tests

### Unit Test Example (Core)

```csharp
using FluentAssertions;

namespace MoneyMatters.Core.Tests.Entities;

public class AccountTests
{
    [Fact]
    public void Constructor_WithValidData_CreatesAccount()
    {
        // Arrange
        var name = "Checking Account";
        var balance = 1000m;

        // Act
        var account = new Account(name, balance);

        // Assert
        account.Should().NotBeNull();
        account.Name.Should().Be(name);
        account.Balance.Should().Be(balance);
    }

    [Fact]
    public void AddTransaction_WithPositiveAmount_IncreasesBalance()
    {
        // Arrange
        var account = new Account("Checking", 1000m);
        var transaction = new Transaction(100m, "Deposit");

        // Act
        account.AddTransaction(transaction);

        // Assert
        account.Balance.Should().Be(1100m);
    }
}
```

### Application Service Test Example

```csharp
using FluentAssertions;
using Moq;

namespace MoneyMatters.Application.Tests.Services;

public class ForecastEngineTests
{
    [Fact]
    public async Task CalculateProjectedBalance_With30DayHorizon_ReturnsCorrectProjection()
    {
        // Arrange
        var mockRepository = new Mock<IAccountRepository>();
        mockRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new Account("Test", 1000m));

        var engine = new ForecastEngine(mockRepository.Object);

        // Act
        var forecast = await engine.CalculateProjectedBalance(1, 30);

        // Assert
        forecast.Should().NotBeNull();
        forecast.HorizonDays.Should().Be(30);
        forecast.Projections.Should().HaveCount(30);
    }
}
```

### Database Test Example (Infrastructure)

```csharp
using FluentAssertions;
using MoneyMatters.Infrastructure.Tests.Helpers;

namespace MoneyMatters.Infrastructure.Tests.Data;

public class AccountRepositoryTests : DatabaseTestBase
{
    [Fact]
    public async Task AddAsync_WithValidAccount_SavesToDatabase()
    {
        // Arrange
        var repository = new AccountRepository(Context);
        var account = new Account("Savings", 5000m);

        // Act
        await repository.AddAsync(account);
        await Context.SaveChangesAsync();

        // Assert
        var saved = await repository.GetByIdAsync(account.Id);
        saved.Should().NotBeNull();
        saved.Name.Should().Be("Savings");
        saved.Balance.Should().Be(5000m);
    }
}
```

### Integration Test Example (API)

```csharp
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MoneyMatters.Api.Tests.Helpers;

namespace MoneyMatters.Api.Tests.Controllers;

public class AccountsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AccountsControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAccounts_ReturnsOkWithAccountList()
    {
        // Act
        var response = await _client.GetAsync("/api/accounts");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var accounts = await response.Content.ReadFromJsonAsync<List<AccountDto>>();
        accounts.Should().NotBeNull();
    }
}
```

## Test Helpers

### DatabaseTestBase

Base class for tests requiring database access:

```csharp
public abstract class DatabaseTestBase : IDisposable
{
    protected ApplicationDbContext Context { get; private set; }

    protected DatabaseTestBase()
    {
        // Creates isolated in-memory database per test
    }

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        Context.Dispose();
    }
}
```

Usage:

```csharp
public class MyRepositoryTests : DatabaseTestBase
{
    [Fact]
    public async Task MyTest()
    {
        // Context is ready to use
        var repository = new MyRepository(Context);
        // ... test code
    }
}
```

### CustomWebApplicationFactory

Factory for integration tests:

```csharp
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    // Configures app to use in-memory database for testing
}
```

## Best Practices

### General Guidelines

1. **AAA Pattern**: Arrange, Act, Assert
2. **One assertion per test** (when possible)
3. **Descriptive test names** following the convention
4. **Independent tests**: No test should depend on another
5. **Fast tests**: Unit tests should run in milliseconds
6. **Use FluentAssertions** for readable assertions

### Mocking

```csharp
// Good: Mock interfaces, not concrete classes
var mockRepository = new Mock<IAccountRepository>();

// Good: Setup only what you need
mockRepository
    .Setup(r => r.GetByIdAsync(1))
    .ReturnsAsync(testAccount);

// Good: Verify important interactions
mockRepository.Verify(r => r.AddAsync(It.IsAny<Account>()), Times.Once);
```

### Assertions

```csharp
// Good: Use FluentAssertions
result.Should().NotBeNull();
result.Balance.Should().Be(1000m);
result.Transactions.Should().HaveCount(5);

// Avoid: Traditional assertions
Assert.NotNull(result);
Assert.Equal(1000m, result.Balance);
Assert.Equal(5, result.Transactions.Count);
```

### Test Data

```csharp
// Good: Use test builders for complex objects
var account = new AccountBuilder()
    .WithName("Test Account")
    .WithBalance(1000m)
    .WithTransactions(5)
    .Build();

// Good: Use descriptive test data
var validEmail = "user@example.com";
var invalidEmail = "not-an-email";

// Avoid: Magic numbers and strings
var account = new Account("test", 123);  // What is 123?
```

## Coverage Goals

- **Minimum**: 80% code coverage across all projects
- **Core Domain**: 90%+ coverage (critical business logic)
- **Application Services**: 85%+ coverage (forecast/alert engines)
- **Infrastructure**: 70%+ coverage (data access)
- **API**: 75%+ coverage (endpoints)

### Viewing Coverage

After running tests with coverage:

```bash
# Generate and view coverage report
reportgenerator -reports:./TestResults/**/coverage.cobertura.xml -targetdir:./TestResults/CoverageReport -reporttypes:Html
open ./TestResults/CoverageReport/index.html
```

## CI/CD Integration

Tests run automatically on:
- **Pull Requests**: All tests must pass
- **Main Branch**: Tests + coverage reports generated
- **Release**: Full test suite + coverage validation

### GitHub Actions Workflow

```yaml
- name: Run tests with coverage
  run: dotnet test --collect:"XPlat Code Coverage"

- name: Generate coverage report
  run: reportgenerator -reports:**/coverage.cobertura.xml -targetdir:coverage -reporttypes:Html

- name: Upload coverage
  uses: codecov/codecov-action@v3
```

## Troubleshooting

### Tests Not Discovered

If xUnit isn't finding your tests:
1. Ensure test class is `public`
2. Ensure test method has `[Fact]` or `[Theory]` attribute
3. Ensure xUnit packages are installed
4. Clean and rebuild: `dotnet clean && dotnet build`

### In-Memory Database Issues

If database tests fail:
1. Each test should use isolated database
2. Use `DatabaseTestBase` for automatic cleanup
3. Ensure `EnsureDeleted()` is called in Dispose

### Slow Tests

If tests are slow:
1. Check for unnecessary database operations
2. Avoid real HTTP calls (use mocks)
3. Minimize test data size
4. Run unit tests separately from integration tests

## Resources

- [xUnit Documentation](https://xunit.net/)
- [FluentAssertions Documentation](https://fluentassertions.com/)
- [Moq Documentation](https://github.com/moq/moq4)
- [EF Core Testing](https://learn.microsoft.com/en-us/ef/core/testing/)

---

🤖 Submitted by George with love ♥
