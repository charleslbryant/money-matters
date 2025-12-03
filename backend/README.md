# Money Matters - Backend API

.NET 10 Web API with clean architecture, Entity Framework Core, and PostgreSQL.

## Architecture

The solution follows clean architecture principles with clear separation of concerns:

```
backend/
├── MoneyMatters.Api/                   # Web API layer
│   ├── Controllers/                    # API controllers
│   ├── Middleware/                     # Custom middleware
│   ├── Program.cs                      # Application entry point
│   └── appsettings.json                # Configuration
│
├── MoneyMatters.Api.Tests/             # API integration tests
│   ├── Controllers/                    # Controller tests
│   └── Helpers/                        # Test helpers
│
├── MoneyMatters.Application/           # Business logic layer
│   └── (CQRS handlers, DTOs, services)
│
├── MoneyMatters.Application.Tests/     # Application layer tests
│   └── (Handler tests, service tests)
│
├── MoneyMatters.Core/                  # Domain layer
│   ├── Entities/                       # Domain entities
│   ├── Enums/                          # Domain enumerations
│   └── (Interfaces, value objects)
│
├── MoneyMatters.Core.Tests/            # Domain/core tests
│   └── (Entity tests, business logic tests)
│
├── MoneyMatters.Infrastructure/        # Infrastructure layer
│   ├── Data/                           # EF Core DbContext
│   ├── Migrations/                     # EF Core migrations
│   └── DependencyInjection.cs          # DI configuration
│
├── MoneyMatters.Infrastructure.Tests/  # Infrastructure tests
│   ├── Data/                           # Repository tests
│   └── Helpers/                        # Test helpers
│
├── MoneyMatters.sln                    # Solution file
├── README.md                           # This file
└── TESTING.md                          # Testing guide
```

## Prerequisites

- .NET 10 SDK
- PostgreSQL 15+
- (Optional) Docker for PostgreSQL

## Getting Started

### 1. Database Setup

#### Recommended: Docker Compose (Recommended)

From the project root:

```bash
# Create environment file
cp .env.example .env

# Start PostgreSQL container
./backend/scripts/db-start.sh
```

This starts a PostgreSQL 15 container with persistent storage using Docker Compose.

**Database Management Scripts:**

- **Start**: `./backend/scripts/db-start.sh`
- **Stop**: `./backend/scripts/db-stop.sh`
- **Reset** (destroys all data): `./backend/scripts/db-reset.sh`
- **Logs**: `./backend/scripts/db-logs.sh`
- **Verify Setup**: `./backend/scripts/verify-db-setup.sh` (runs 36+ tests to verify database setup)
- **Run All Verification Tests**: `./backend/scripts/run-verification-tests.sh` (runs bash + xUnit tests for comprehensive validation)

See [backend/scripts/README.md](scripts/README.md) for detailed documentation.

#### Database Verification

After setting up the database and running migrations, verify everything works correctly:

```bash
# Run comprehensive verification (recommended)
./backend/scripts/run-verification-tests.sh
```

This runs **95+ verification tests** including:
- ✅ Docker container health
- ✅ Database connectivity
- ✅ Migrations applied
- ✅ All 10 tables created
- ✅ Indexes and constraints
- ✅ Seed data populated (1 user, 5 accounts, 6 bills, etc.)
- ✅ CRUD operations functional
- ✅ Foreign key relationships
- ✅ Constraint enforcement
- ✅ Security checks

**Quick verification options:**

```bash
# Bash tests only (infrastructure + seed data)
./backend/scripts/verify-db-setup.sh

# xUnit tests only (schema + CRUD + constraints)
cd backend
dotnet test MoneyMatters.Infrastructure.Tests --filter "FullyQualifiedName~SchemaValidationTests|FullyQualifiedName~SeedDataValidationTests|FullyQualifiedName~CrudOperationTests|FullyQualifiedName~ConstraintValidationTests"
```

See [docs/database-verification.md](../docs/database-verification.md) for complete verification guide.

#### Alternative: Local PostgreSQL Installation

If you prefer a local installation instead of Docker:

```sql
CREATE DATABASE moneymatters_dev;
```

### 2. Configuration

**IMPORTANT: Use User Secrets for local development**

The connection string is **NOT** stored in `appsettings.json` files. Use .NET user secrets:

```bash
# Initialize user secrets (already done)
dotnet user-secrets init --project MoneyMatters.Api

# Set connection string
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=moneymatters_dev;Username=postgres;Password=postgres" --project MoneyMatters.Api
```

**Note:** For production, use Azure Key Vault. Never commit real credentials.

### 3. Run Migrations

```bash
# From the backend directory
export PATH="$PATH:$HOME/.dotnet/tools"  # Add dotnet tools to PATH
dotnet ef database update --project MoneyMatters.Infrastructure --startup-project MoneyMatters.Api
```

The complete schema migration has already been created (`CompleteSchemaInitial`).

### 4. Run the API

```bash
cd MoneyMatters.Api
dotnet run
```

The API will be available at:
- HTTPS: `https://localhost:7001`
- HTTP: `http://localhost:5000`

## API Endpoints

### Health Checks

- `GET /health` - Basic health check (includes database connectivity check)
- `GET /api/health` - Detailed health check endpoint

### OpenAPI / Swagger

In development mode, Swagger UI is available at:
- Swagger UI: `https://localhost:7001/swagger`
- OpenAPI spec: `https://localhost:7001/swagger/v1/swagger.json`

## Key Features

✅ Clean Architecture with CQRS pattern (MediatR ready)
✅ FluentValidation for request validation
✅ Entity Framework Core 10 with PostgreSQL
✅ Serilog for structured logging (console + file)
✅ Health checks with database connectivity
✅ CORS configured for frontend
✅ Swagger/OpenAPI documentation with XML comments
✅ Global exception handling middleware
✅ Secure configuration management (User Secrets)
✅ Comprehensive test infrastructure (xUnit, FluentAssertions, Moq)

## Development

### Building

```bash
dotnet build MoneyMatters.sln
```

### Running Tests

```bash
# Run all tests
dotnet test MoneyMatters.sln

# Run tests with coverage
dotnet test /p:CollectCoverage=true

# Run specific test project
dotnet test MoneyMatters.Api.Tests
dotnet test MoneyMatters.Application.Tests
dotnet test MoneyMatters.Core.Tests
dotnet test MoneyMatters.Infrastructure.Tests

# Run tests with detailed output
dotnet test --verbosity detailed

# Run with coverage report (using script)
./run-tests-with-coverage.sh
```

Current test coverage:
- ✅ Health check API endpoint tests (2 passing)
- Test infrastructure ready for all layers
- See [TESTING.md](./TESTING.md) for comprehensive testing guide

### Database Migrations

```bash
# Add a new migration
dotnet ef migrations add <MigrationName> --project MoneyMatters.Infrastructure --startup-project MoneyMatters.Api

# Update database
dotnet ef database update --project MoneyMatters.Infrastructure --startup-project MoneyMatters.Api

# Remove last migration
dotnet ef migrations remove --project MoneyMatters.Infrastructure --startup-project MoneyMatters.Api
```

## Logging

Logs are written to:
- Console (all environments)
- File: `logs/moneymatters-{Date}.log` (rolling daily)

Log levels can be configured in `appsettings.json` and `appsettings.Development.json`.

## Security

🔒 **Important Security Notes:**

- Never commit `appsettings.Production.json` or `appsettings.Staging.json`
- Use `appsettings.example.json` as a template
- Store production secrets in Azure Key Vault
- Connection strings in development should use non-production credentials
- Review [SECURITY.md](../../SECURITY.md) for complete guidelines

## Dependencies

### Core Packages
- `Microsoft.AspNetCore.OpenApi` (10.0.0) - OpenAPI support
- `Swashbuckle.AspNetCore` (10.0.1) - Swagger UI
- `Microsoft.EntityFrameworkCore` (10.0.0) - ORM
- `Npgsql.EntityFrameworkCore.PostgreSQL` (10.0.0) - PostgreSQL provider
- `Serilog.AspNetCore` (10.0.0) - Logging
- `AspNetCore.HealthChecks.NpgSql` (9.0.0) - Database health checks
- `MediatR` (13.1.0) - CQRS pattern implementation
- `FluentValidation` (12.1.0) - Request validation

### Test Packages
- `xUnit` (2.9.3) - Testing framework
- `FluentAssertions` (7.0.0) - Fluent assertion library
- `Moq` (4.20.72) - Mocking framework
- `Microsoft.AspNetCore.Mvc.Testing` (10.0.0) - Integration testing

## Project Status

Current implementation: **Phase 1 - Foundation** ✅ COMPLETE

- [x] Solution structure with clean architecture
- [x] Entity Framework Core with PostgreSQL
- [x] MediatR for CQRS pattern
- [x] FluentValidation for request validation
- [x] Dependency injection configured
- [x] Swagger/OpenAPI with XML documentation
- [x] Health check endpoints
- [x] CORS configuration
- [x] Serilog logging (console + file)
- [x] Global exception handling middleware
- [x] User Secrets for local development
- [x] Initial EF Core migration created
- [x] Test infrastructure with xUnit, FluentAssertions, Moq
- [x] Health check endpoint tests

Next up: **Phase 2 - Database Schema & Domain Models**

---

🤖 Submitted by George with love ♥
