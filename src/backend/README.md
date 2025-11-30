# Money Matters - Backend API

.NET 10 Web API with clean architecture, Entity Framework Core, and PostgreSQL.

## Architecture

The solution follows clean architecture principles with clear separation of concerns:

```
MoneyMatters.Api/          # Web API layer (Controllers, Middleware, Configuration)
MoneyMatters.Application/  # Business logic, CQRS handlers, DTOs
MoneyMatters.Core/         # Domain entities, interfaces, enums
MoneyMatters.Infrastructure/ # Data access, external services, repositories
```

## Prerequisites

- .NET 10 SDK
- PostgreSQL 15+
- (Optional) Docker for PostgreSQL

## Getting Started

### 1. Database Setup

#### Option A: Local PostgreSQL Installation

Create a new database:

```sql
CREATE DATABASE moneymatters_dev;
```

#### Option B: Docker PostgreSQL

```bash
docker run --name moneymatters-postgres \
  -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_DB=moneymatters_dev \
  -p 5432:5432 \
  -d postgres:15
```

### 2. Configuration

The connection string is configured in `appsettings.Development.json`. Update it if needed:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=moneymatters_dev;Username=postgres;Password=postgres"
  }
}
```

**Note:** For production, use Azure Key Vault or environment variables. Never commit real credentials.

### 3. Run Migrations

```bash
# From the src/backend directory
dotnet ef migrations add InitialCreate --project MoneyMatters.Infrastructure --startup-project MoneyMatters.Api
dotnet ef database update --project MoneyMatters.Infrastructure --startup-project MoneyMatters.Api
```

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

In development mode, OpenAPI documentation is available at:
- OpenAPI spec: `https://localhost:7001/openapi/v1.json`
- Swagger UI will be configured in a future update

## Key Features

✅ Clean Architecture with CQRS pattern ready
✅ Entity Framework Core 10 with PostgreSQL
✅ Serilog for structured logging
✅ Health checks with database connectivity
✅ CORS configured for frontend
✅ OpenAPI documentation
✅ Secure configuration management

## Development

### Building

```bash
dotnet build MoneyMatters.sln
```

### Running Tests

```bash
dotnet test
```

(Tests will be added in Phase 7)

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
- `Microsoft.AspNetCore.OpenApi` - OpenAPI support
- `Microsoft.EntityFrameworkCore` (10.0.0) - ORM
- `Npgsql.EntityFrameworkCore.PostgreSQL` (10.0.0) - PostgreSQL provider
- `Serilog.AspNetCore` (10.0.0) - Logging
- `AspNetCore.HealthChecks.NpgSql` (9.0.0) - Database health checks

## Project Status

Current implementation: **Phase 1 - Foundation** ✅

- [x] Solution structure with clean architecture
- [x] Entity Framework Core with PostgreSQL
- [x] Dependency injection configured
- [x] Swagger/OpenAPI setup
- [x] Health check endpoints
- [x] CORS configuration
- [x] Serilog logging
- [x] Secure configuration management

Next up: **Phase 2 - Database Schema & Domain Models**

---

🤖 Submitted by George with love ♥
