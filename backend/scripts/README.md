# Database Management Scripts

This directory contains scripts for managing the local PostgreSQL development environment.

## Available Scripts

### Database Operations

- **`db-start.sh`** - Start PostgreSQL container and wait for healthy status
- **`db-stop.sh`** - Stop PostgreSQL container gracefully
- **`db-reset.sh`** - Reset database to clean state (DESTRUCTIVE: deletes all data)
- **`db-logs.sh`** - Tail PostgreSQL container logs for debugging
- **`init-db.sh`** - PostgreSQL initialization script (runs on first container start)

### Verification

- **`verify-db-setup.sh`** - Comprehensive test suite to verify database setup

## Quick Start

```bash
# Start the database
./backend/scripts/db-start.sh

# Stop the database
./backend/scripts/db-stop.sh

# View logs
./backend/scripts/db-logs.sh

# Reset database (deletes all data - requires confirmation)
./backend/scripts/db-reset.sh

# Verify setup is correct
./backend/scripts/verify-db-setup.sh
```

## Verification Script

The `verify-db-setup.sh` script runs a comprehensive test suite to validate the database setup. It's useful for:

- **Initial setup verification** - Confirm everything is configured correctly
- **Troubleshooting** - Quickly identify what's broken
- **CI/CD pipelines** - Automated testing of database setup
- **Onboarding** - New developers can verify their setup

### What It Tests

The verification script tests **11 categories** with **26 individual tests**:

1. **Docker Compose Configuration**
   - docker-compose.yml exists
   - .env.example exists

2. **Database Management Scripts**
   - All scripts exist and are executable

3. **Environment Configuration**
   - .env file exists
   - Required variables present (POSTGRES_USER, POSTGRES_PASSWORD, etc.)

4. **PostgreSQL Container Status**
   - Docker daemon running
   - Container exists and is running
   - Container is healthy

5. **Database Connectivity**
   - PostgreSQL accepts connections
   - Database 'moneymatters_dev' exists

6. **Entity Framework Core Migrations**
   - Migrations have been applied
   - CompleteSchemaInitial migration exists

7. **Database Schema**
   - All core tables exist (Users, Accounts, Bills, etc.)

8. **.NET User Secrets Configuration**
   - ConnectionStrings:DefaultConnection is configured

9. **Data Persistence**
   - Seed data exists
   - Docker volume exists

10. **Security Verification**
    - .env is gitignored
    - .env is not staged for commit
    - .env.example is tracked in git

11. **Documentation**
    - quick-start.md has Database Setup section
    - backend README has Docker Compose instructions

### Running the Verification Script

```bash
# Run from project root
./backend/scripts/verify-db-setup.sh
```

### Sample Output

```
========================================
TEST 1: Docker Compose Configuration
========================================

TEST: Checking if docker-compose.yml exists
✅ PASS: docker-compose.yml found
TEST: Checking if .env.example exists
✅ PASS: .env.example found

...

========================================
TEST SUMMARY
========================================
Total Tests: 26
Passed: 26
Failed: 0

✅ ALL TESTS PASSED!
```

### Exit Codes

- **0** - All tests passed
- **1** - One or more tests failed

This makes it perfect for use in CI/CD pipelines:

```bash
# In CI/CD pipeline
./backend/scripts/verify-db-setup.sh || exit 1
```

## Troubleshooting

### Common Issues

**Container won't start:**
```bash
# Check if port 5432 is already in use
docker ps | grep 5432

# Check Docker logs
./backend/scripts/db-logs.sh
```

**Connection refused:**
```bash
# Verify container is healthy
docker ps | grep moneymatters-db

# Wait for container to be ready
./backend/scripts/db-start.sh
```

**Migration errors:**
```bash
# Verify User Secrets are configured
cd backend/MoneyMatters.Api
dotnet user-secrets list

# Set connection string if missing
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=moneymatters_dev;Username=moneymatters;Password=dev_password_change_me"
```

**Tests failing:**
```bash
# Run verification to see what's wrong
./backend/scripts/verify-db-setup.sh

# Check the specific failing test and follow the error message
```

## Advanced Usage

### Custom Database Port

If port 5432 is already in use:

1. Edit `.env` and change `POSTGRES_PORT=5433`
2. Update User Secrets connection string to match:
   ```bash
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5433;Database=moneymatters_dev;Username=moneymatters;Password=dev_password_change_me" --project backend/MoneyMatters.Api
   ```

### Complete Reset

To completely reset the database and Docker volume:

```bash
# Stop and remove everything
./backend/scripts/db-reset.sh

# This will:
# 1. Stop the container
# 2. Remove the Docker volume
# 3. Start a fresh container
# 4. Run migrations
```

## CI/CD Integration

### GitHub Actions Example

```yaml
- name: Start PostgreSQL
  run: ./backend/scripts/db-start.sh

- name: Verify Database Setup
  run: ./backend/scripts/verify-db-setup.sh

- name: Run Backend Tests
  run: dotnet test backend/MoneyMatters.sln
```

### Local Pre-Commit Hook

Add to `.git/hooks/pre-commit`:

```bash
#!/bin/bash
# Verify database setup before committing database changes
if git diff --cached --name-only | grep -q "backend\|docker-compose.yml"; then
  echo "Database files changed, running verification..."
  ./backend/scripts/verify-db-setup.sh || exit 1
fi
```

---

🤖 Submitted by George with love ♥
