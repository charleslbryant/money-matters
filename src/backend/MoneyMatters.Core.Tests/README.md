# MoneyMatters.Core.Tests

Unit tests for the Core domain layer.

## Test Structure

This project contains unit tests for:
- **Domain Entities**: Account, Transaction, Bill, IncomeStream, Goal, Alert, Forecast
- **Value Objects**: Money, DateRange, etc.
- **Domain Services**: Business logic that doesn't fit in entities
- **Enums and Constants**: Domain enumerations

## Test Naming Convention

Tests follow the pattern: `MethodName_StateUnderTest_ExpectedBehavior`

Examples:
- `Constructor_WithValidData_CreatesAccount`
- `AddTransaction_WhenAccountIsClosed_ThrowsException`
- `CalculateBalance_WithMultipleTransactions_ReturnsCorrectSum`

## Test Organization

```
MoneyMatters.Core.Tests/
├── Entities/
│   ├── AccountTests.cs
│   ├── BillTests.cs
│   ├── GoalTests.cs
│   ├── TransactionTests.cs
│   ├── IncomeStreamTests.cs
│   ├── AlertTests.cs
│   └── ForecastTests.cs
├── ValueObjects/
│   └── (value object tests)
└── README.md
```

## Running Tests

```bash
# Run all core tests
dotnet test MoneyMatters.Core.Tests

# Run specific test class
dotnet test --filter "FullyQualifiedName~AccountTests"

# Run with coverage
dotnet test /p:CollectCoverage=true
```

## Guidelines

- Each domain entity should have comprehensive test coverage
- Test both happy paths and edge cases
- Use FluentAssertions for readable assertions
- Keep tests isolated and independent
- Mock external dependencies using Moq

---

🤖 Submitted by George with love ♥
