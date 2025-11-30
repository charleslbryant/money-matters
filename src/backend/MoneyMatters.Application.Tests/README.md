# MoneyMatters.Application.Tests

Unit tests for the Application layer (business logic, CQRS handlers).

## Test Structure

This project contains unit tests for:
- **Command Handlers**: CreateAccount, UpdateAccount, CreateBill, etc.
- **Query Handlers**: GetAccountById, GetForecast, GetAlerts, etc.
- **Validators**: FluentValidation validators for commands/queries
- **Services**: Forecast engine, alert engine, notification services
- **DTOs**: Data transfer objects and mappings

## Test Naming Convention

Tests follow the pattern: `MethodName_StateUnderTest_ExpectedBehavior`

Examples:
- `Handle_WithValidCommand_CreatesAccount`
- `Handle_WithInvalidData_ReturnsValidationError`
- `Validate_WhenAmountIsNegative_ReturnsError`

## Test Organization

```
MoneyMatters.Application.Tests/
├── Commands/
│   ├── Accounts/
│   │   ├── CreateAccountCommandHandlerTests.cs
│   │   └── UpdateAccountCommandHandlerTests.cs
│   ├── Bills/
│   └── Goals/
├── Queries/
│   ├── Accounts/
│   ├── Forecasts/
│   └── Alerts/
├── Services/
│   ├── ForecastEngineTests.cs
│   ├── AlertEngineTests.cs
│   └── NotificationServiceTests.cs
├── Validators/
│   └── (validator tests)
└── README.md
```

## Running Tests

```bash
# Run all application tests
dotnet test MoneyMatters.Application.Tests

# Run specific test class
dotnet test --filter "FullyQualifiedName~ForecastEngineTests"

# Run with coverage
dotnet test /p:CollectCoverage=true
```

## Guidelines

- Mock repositories and external dependencies
- Test business logic thoroughly
- Verify validation rules
- Test both success and failure scenarios
- Use FluentAssertions for clear assertions
- Test command/query handlers in isolation

## Forecast Engine Test Coverage

The forecast engine should have tests for:
- Cash flow projection calculations
- Days of runway calculations
- Bill coverage determination
- Goal completion projections
- Personal vs Business vs Combined scope handling
- Different time horizons (30, 60, 90 days)

## Alert Engine Test Coverage

The alert engine should have tests for:
- Cash shortfall detection
- Bill risk identification
- Income delay warnings
- Goal risk calculations
- Alert severity (green/yellow/red) determination

---

🤖 Submitted by George with love ♥
