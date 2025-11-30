# MoneyMatters.Api.Tests

Integration tests for the Money Matters API endpoints.

## Test Structure

This project will contain integration tests for:
- **API Endpoints**: All REST API controllers
- **Middleware**: Error handling, authentication, authorization
- **Request/Response**: HTTP request/response validation
- **End-to-End Scenarios**: Complete workflows through the API

## Test Naming Convention

Tests follow the pattern: `Endpoint_Scenario_ExpectedResult`

Examples:
- `GetAccount_WithValidId_ReturnsAccount`
- `CreateBill_WithInvalidData_ReturnsBadRequest`
- `GetForecast_WithUnauthorizedUser_ReturnsUnauthorized`

## Test Organization

```
MoneyMatters.Api.Tests/
├── Controllers/
│   ├── AccountsControllerTests.cs
│   ├── BillsControllerTests.cs
│   ├── GoalsControllerTests.cs
│   ├── ForecastsControllerTests.cs
│   ├── AlertsControllerTests.cs
│   └── HealthControllerTests.cs
├── Helpers/
│   ├── CustomWebApplicationFactory.cs
│   └── TestDataBuilder.cs
└── README.md
```

## Running Tests

```bash
# Run all API integration tests
dotnet test MoneyMatters.Api.Tests

# Run specific test class
dotnet test --filter "FullyQualifiedName~AccountsControllerTests"

# Run with coverage
dotnet test /p:CollectCoverage=true
```

## Guidelines

- Use `WebApplicationFactory` for integration testing
- Tests should use an in-memory database
- Each test should be independent
- Clean up test data after each test
- Test happy paths and error scenarios
- Verify HTTP status codes and response bodies
- Test authentication and authorization
- Use FluentAssertions for readable assertions

## Current Status

**Integration tests will be implemented once the following are complete:**
1. Domain entities and models (Phase 2)
2. CQRS commands/queries (Phase 3-4)
3. API endpoints (Phase 5)

This infrastructure is ready and waiting for the API endpoints to be built.

---

🤖 Submitted by George with love ♥
