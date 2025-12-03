# Database Verification Guide

This document explains the comprehensive database verification testing approach for Money Matters. The verification suite validates that database migrations, schema, seed data, and constraints all work correctly.

## Overview

The database verification system uses a **two-tier testing approach**:

1. **Bash Script Tests** (`verify-db-setup.sh`) - Infrastructure, connectivity, and seed data validation
2. **xUnit Tests** (C# test files) - Schema validation, CRUD operations, and constraint testing

Together, these tests provide over **50 comprehensive tests** covering all aspects of the database implementation.

## Quick Start

### Run All Verification Tests

```bash
# From project root
./backend/scripts/run-verification-tests.sh
```

This master script runs both bash and xUnit tests in sequence and provides a comprehensive summary.

### Run Individual Test Suites

```bash
# Bash tests only (infrastructure + seed data)
./backend/scripts/verify-db-setup.sh

# xUnit tests only (schema + CRUD + constraints)
cd backend
dotnet test MoneyMatters.Infrastructure.Tests --filter "FullyQualifiedName~SchemaValidationTests|FullyQualifiedName~SeedDataValidationTests|FullyQualifiedName~CrudOperationTests|FullyQualifiedName~ConstraintValidationTests"
```

## Test Categories

### Tier 1: Bash Script Tests (verify-db-setup.sh)

The bash script performs **36+ tests** across 11 categories:

#### 1. Docker Compose Configuration
- `docker-compose.yml` exists
- `.env.example` exists

#### 2. Database Management Scripts
- All scripts exist and are executable:
  - `db-start.sh`
  - `db-stop.sh`
  - `db-reset.sh`
  - `db-logs.sh`
  - `init-db.sh`

#### 3. Environment Configuration
- `.env` file exists
- Required variables present:
  - `POSTGRES_USER`
  - `POSTGRES_PASSWORD`
  - `POSTGRES_DB`
  - `POSTGRES_PORT`

#### 4. PostgreSQL Container Status
- Docker daemon running
- Container exists
- Container running
- Container healthy

#### 5. Database Connectivity
- PostgreSQL accepts connections
- Database `moneymatters_dev` exists

#### 6. Entity Framework Core Migrations
- Migrations applied
- `CompleteSchemaInitial` migration exists

#### 7. Database Schema
- All 10 tables exist:
  - Users
  - Accounts
  - Transactions
  - Bills
  - IncomeStreams
  - Goals
  - GoalAccounts
  - Alerts
  - ForecastSnapshots
  - Settings
- Exactly 11 tables total (including `__EFMigrationsHistory`)

#### 8. .NET User Secrets Configuration
- `ConnectionStrings:DefaultConnection` configured

#### 9. Data Persistence
- Seed data exists
- Docker volume exists

#### 9a. Seed Data Validation (runs if seed data exists)
- Exactly 1 user
- Exactly 5 accounts
- Exactly 6 bills
- Exactly 2 income streams
- Exactly 3 goals
- Exactly 3 goal-account links
- At least 9 transactions
- Exactly 5 settings
- Development user has correct email (`dev@moneymatters.local`)

#### 10. Security Verification
- `.env` is gitignored
- `.env` not staged for commit
- `.env.example` is tracked in git

#### 11. Documentation
- `docs/quick-start.md` has database setup section
- `backend/README.md` has Docker Compose instructions

### Tier 2: xUnit Tests (C# Test Files)

The xUnit test suite performs **25+ tests** across 4 test files:

#### SchemaValidationTests.cs
Tests schema configuration and structure:

- All 10 required tables exist
- User email has unique index
- Setting has composite unique index (UserId + SettingKey)
- GoalAccount has composite unique index (GoalId + AccountId)
- Transaction has composite index (AccountId + Date)
- Alert has descending index on TriggeredAt
- ForecastSnapshot has composite index (UserId + Domain + HorizonDays + GeneratedAt)
- Cascade delete: Account → Transactions
- Cascade delete: User → Accounts
- Cascade delete: User → Bills
- Cascade delete: User → Goals
- Monetary fields use decimal(18,2) precision
- BaseEntity properties exist on all entities
- All DbSets accessible
- Required fields are non-nullable
- Optional fields are nullable
- Bill deletion sets Transaction.BillId to null (SetNull behavior)

#### SeedDataValidationTests.cs
Tests seed data integrity:

- Exactly 1 user created
- User has correct email and properties
- Exactly 5 accounts (2 personal, 2 business, 1 credit card)
- Accounts have correct domain distribution
- Exactly 6 bills (4 personal, 2 business)
- Bills have correct domain distribution
- Exactly 2 income streams with correct properties
- Exactly 3 goals with correct amounts and strategies
- Exactly 3 goal-account links
- Goal-account relationships correct
- At least 9 transactions
- Transactions have correct types (income, bills, goals, misc)
- Exactly 5 settings with correct keys
- All relationships established correctly
- Seed data is idempotent (no duplicates on re-run)
- Account balances match expected values
- Bills have valid due dates
- All entities have BaseEntity properties populated

#### CrudOperationTests.cs
Tests basic CRUD operations on all entities:

- User: Create, Read, Update, Delete
- Account: Create, Read, Update, Delete
- Transaction: Create, Read, Update, Delete
- Bill: Create, Read, Update, Delete
- Goal: Create, Read, Update, Delete
- User deletion cascades to Accounts
- User deletion cascades to Bills
- User deletion cascades to Goals
- Account deletion cascades to Transactions
- Bill deletion sets Transaction.BillId to null
- Navigation properties load correctly
- GoalAccount link creation and deletion

#### ConstraintValidationTests.cs
Tests database constraints prevent invalid data:

- User email must be unique
- User email is required
- Setting (UserId + SettingKey) must be unique
- GoalAccount (GoalId + AccountId) must be unique
- Transaction cannot have invalid AccountId (FK constraint)
- Bill cannot have invalid UserId (FK constraint)
- Account name is required
- Bill name is required
- Transaction BillId can be null (optional)
- Transaction GoalId can be null (optional)
- Decimal fields handle precision correctly (18,2)
- Enum fields accept valid values
- Goal FundingStrategy enum is valid
- BaseEntity timestamps are set
- UpdatedAt changes on modification

## How to Run Verification Tests

### Prerequisites

Before running verification tests:

1. **PostgreSQL container must be running**:
   ```bash
   ./backend/scripts/db-start.sh
   ```

2. **Migrations must be applied** (or run API once to apply):
   ```bash
   cd backend
   dotnet ef database update --project MoneyMatters.Api
   ```

3. **Seed data populated** (run API once):
   ```bash
   cd backend
   dotnet run --project MoneyMatters.Api
   # Stop after it starts (Ctrl+C)
   ```

### Step-by-Step Verification Workflow

#### Option 1: Run Everything (Recommended)

```bash
# From project root
./backend/scripts/run-verification-tests.sh
```

This runs:
1. Bash tests (infrastructure + seed data)
2. xUnit tests (schema + CRUD + constraints)
3. Comprehensive summary

**Expected output:**
```
========================================
ALL VERIFICATION TESTS PASSED! 🎉
========================================
Database schema and migrations are fully verified.

Summary:
  ✅ Docker container healthy
  ✅ Database connectivity verified
  ✅ Migrations applied correctly
  ✅ All 10 tables exist
  ✅ Seed data populated (1 user, 5 accounts, 6 bills, etc.)
  ✅ Schema validation passed
  ✅ CRUD operations functional
  ✅ Constraints enforced
  ✅ Foreign keys working
  ✅ Security checks passed
```

#### Option 2: Run Bash Tests Only

```bash
# From project root
./backend/scripts/verify-db-setup.sh
```

**Use when:**
- Checking infrastructure setup
- Validating seed data counts
- Quick verification before running xUnit tests

#### Option 3: Run xUnit Tests Only

```bash
# From backend directory
cd backend
dotnet test MoneyMatters.Infrastructure.Tests/MoneyMatters.Infrastructure.Tests.csproj \
  --filter "FullyQualifiedName~SchemaValidationTests|FullyQualifiedName~SeedDataValidationTests|FullyQualifiedName~CrudOperationTests|FullyQualifiedName~ConstraintValidationTests" \
  --verbosity normal
```

**Use when:**
- Debugging specific schema issues
- Testing constraint violations
- Validating CRUD operations

## Interpreting Test Results

### Bash Script Output

**Success:**
```
✅ PASS: PostgreSQL container is healthy
✅ PASS: All 10 tables exist
✅ PASS: Exactly 1 user exists
```

**Failure:**
```
❌ FAIL: Expected 5 accounts, found 3
```

**Info (not a failure):**
```
ℹ INFO: Table 'Users' exists
```

### xUnit Test Output

**Success:**
```
Passed MoneyMatters.Infrastructure.Tests.Data.SchemaValidationTests.Schema_HasAllRequiredTables [10ms]
```

**Failure:**
```
Failed MoneyMatters.Infrastructure.Tests.Data.ConstraintValidationTests.User_EmailMustBeUnique [15ms]
  Error Message:
   Expected an exception, but none was thrown
```

## Troubleshooting Common Failures

### "PostgreSQL container is not running"

**Cause:** Docker container not started

**Fix:**
```bash
./backend/scripts/db-start.sh
```

### "No migrations applied"

**Cause:** Database exists but migrations not run

**Fix:**
```bash
cd backend
dotnet ef database update --project MoneyMatters.Api
```

### "No seed data found"

**Cause:** API hasn't run yet to populate seed data

**Fix:**
```bash
cd backend
dotnet run --project MoneyMatters.Api
# Wait for startup, then Ctrl+C
```

### "Expected 5 accounts, found 0"

**Cause:** Seed data not populated or database reset without re-seeding

**Fix:**
1. Delete database and recreate:
   ```bash
   ./backend/scripts/db-reset.sh
   ```
2. Run API to trigger seeding:
   ```bash
   cd backend
   dotnet run --project MoneyMatters.Api
   ```

### xUnit Test Failures

**Cause:** Entity configuration or migration issues

**Fix:**
1. Review the specific test failure message
2. Check entity configuration in `ApplicationDbContext.cs`
3. Verify migration file `CompleteSchemaInitial.cs`
4. If needed, create new migration:
   ```bash
   cd backend
   dotnet ef migrations add FixSchemaIssue --project MoneyMatters.Infrastructure --startup-project MoneyMatters.Api
   dotnet ef database update --project MoneyMatters.Api
   ```

## CI/CD Integration

### GitHub Actions Example

```yaml
name: Database Verification

on: [push, pull_request]

jobs:
  verify-database:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3

      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '10.0.x'

      - name: Start PostgreSQL
        run: |
          cp .env.example .env
          ./backend/scripts/db-start.sh
          sleep 10  # Wait for container to be healthy

      - name: Apply Migrations
        run: |
          cd backend
          dotnet ef database update --project MoneyMatters.Api

      - name: Run API to Seed Data
        run: |
          cd backend
          dotnet run --project MoneyMatters.Api &
          sleep 15  # Wait for API startup and seeding
          kill %1

      - name: Run Verification Tests
        run: ./backend/scripts/run-verification-tests.sh
```

### Azure DevOps Example

```yaml
steps:
- script: |
    cp .env.example .env
    ./backend/scripts/db-start.sh
  displayName: 'Start PostgreSQL'

- task: DotNetCoreCLI@2
  displayName: 'Apply Migrations'
  inputs:
    command: 'custom'
    custom: 'ef'
    arguments: 'database update --project backend/MoneyMatters.Api'

- script: |
    cd backend
    dotnet run --project MoneyMatters.Api &
    sleep 15
    kill %1
  displayName: 'Seed Data'

- script: ./backend/scripts/run-verification-tests.sh
  displayName: 'Run Database Verification Tests'
```

## Development Workflow

### When to Run Verification Tests

**Always run before:**
- Committing database schema changes
- Creating pull requests with migration changes
- Deploying to any environment
- Closing database-related issues

**Run periodically:**
- After pulling changes that include migrations
- When debugging database connectivity issues
- Before major refactoring

### Verification as Part of Development

1. **Make database changes** (entity, configuration, migration)
2. **Run verification tests**:
   ```bash
   ./backend/scripts/run-verification-tests.sh
   ```
3. **Fix any failures** before committing
4. **Commit with passing tests**

## Test Coverage Summary

| Category | Bash Tests | xUnit Tests | Total |
|----------|-----------|-------------|-------|
| Infrastructure | 13 | 0 | 13 |
| Schema | 3 | 17 | 20 |
| Seed Data | 10 | 18 | 28 |
| CRUD Operations | 0 | 12 | 12 |
| Constraints | 0 | 17 | 17 |
| Security | 3 | 0 | 3 |
| Documentation | 2 | 0 | 2 |
| **Total** | **31** | **64** | **95** |

## Files Involved

### Test Files
- `/backend/scripts/verify-db-setup.sh` - Bash verification script
- `/backend/scripts/run-verification-tests.sh` - Master orchestration script
- `/backend/MoneyMatters.Infrastructure.Tests/Data/SchemaValidationTests.cs` - Schema tests
- `/backend/MoneyMatters.Infrastructure.Tests/Data/SeedDataValidationTests.cs` - Seed data tests
- `/backend/MoneyMatters.Infrastructure.Tests/Data/CrudOperationTests.cs` - CRUD tests
- `/backend/MoneyMatters.Infrastructure.Tests/Data/ConstraintValidationTests.cs` - Constraint tests

### Source Files Being Tested
- `/backend/MoneyMatters.Infrastructure/Data/ApplicationDbContext.cs` - EF Core context
- `/backend/MoneyMatters.Infrastructure/Data/SeedData.cs` - Seed data logic
- `/backend/MoneyMatters.Infrastructure/Migrations/*` - Migration files
- `/backend/MoneyMatters.Core/Entities/*` - Entity definitions

## Next Steps

After verification tests pass:

1. **Build API endpoints** - Use validated schema to build controllers
2. **Write integration tests** - Test API endpoints with real database
3. **Implement business logic** - CQRS handlers, forecast engine, alert engine
4. **Deploy to staging** - Apply migrations to staging environment

## Related Documentation

- [Database Schema](database-schema.md) - Complete schema specification
- [Backend README](../backend/README.md) - Backend setup and commands
- [Quick Start Guide](quick-start.md) - Fast setup instructions

---

🤖 Submitted by George with love ♥
