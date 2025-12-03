#!/bin/bash
# Comprehensive database setup verification script
# Tests all acceptance criteria for Issue #41

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Test counters
TESTS_RUN=0
TESTS_PASSED=0
TESTS_FAILED=0

# Get the project root (two levels up from this script)
PROJECT_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"

print_header() {
    echo ""
    echo -e "${BLUE}========================================${NC}"
    echo -e "${BLUE}$1${NC}"
    echo -e "${BLUE}========================================${NC}"
    echo ""
}

print_test() {
    echo -e "${YELLOW}TEST:${NC} $1"
    TESTS_RUN=$((TESTS_RUN + 1))
}

print_pass() {
    echo -e "${GREEN}✅ PASS:${NC} $1"
    TESTS_PASSED=$((TESTS_PASSED + 1))
}

print_fail() {
    echo -e "${RED}❌ FAIL:${NC} $1"
    TESTS_FAILED=$((TESTS_FAILED + 1))
}

print_info() {
    echo -e "${BLUE}ℹ INFO:${NC} $1"
}

print_summary() {
    echo ""
    echo -e "${BLUE}========================================${NC}"
    echo -e "${BLUE}TEST SUMMARY${NC}"
    echo -e "${BLUE}========================================${NC}"
    echo -e "Total Tests: ${TESTS_RUN}"
    echo -e "${GREEN}Passed: ${TESTS_PASSED}${NC}"
    if [ $TESTS_FAILED -gt 0 ]; then
        echo -e "${RED}Failed: ${TESTS_FAILED}${NC}"
    else
        echo -e "Failed: ${TESTS_FAILED}"
    fi

    if [ $TESTS_FAILED -eq 0 ]; then
        echo ""
        echo -e "${GREEN}✅ ALL TESTS PASSED!${NC}"
        echo ""
        return 0
    else
        echo ""
        echo -e "${RED}❌ SOME TESTS FAILED${NC}"
        echo ""
        return 1
    fi
}

# Test 1: Docker Compose file exists
print_header "TEST 1: Docker Compose Configuration"
print_test "Checking if docker-compose.yml exists"
if [ -f "$PROJECT_ROOT/docker-compose.yml" ]; then
    print_pass "docker-compose.yml found"
else
    print_fail "docker-compose.yml not found"
fi

print_test "Checking if .env.example exists"
if [ -f "$PROJECT_ROOT/.env.example" ]; then
    print_pass ".env.example found"
else
    print_fail ".env.example not found"
fi

# Test 2: Database scripts exist and are executable
print_header "TEST 2: Database Management Scripts"
SCRIPTS=("db-start.sh" "db-stop.sh" "db-reset.sh" "db-logs.sh" "init-db.sh")
for script in "${SCRIPTS[@]}"; do
    print_test "Checking if $script exists and is executable"
    if [ -x "$PROJECT_ROOT/backend/scripts/$script" ]; then
        print_pass "$script is executable"
    else
        print_fail "$script is not executable or does not exist"
    fi
done

# Test 3: .env file setup
print_header "TEST 3: Environment Configuration"
print_test "Checking if .env file exists"
if [ -f "$PROJECT_ROOT/.env" ]; then
    print_pass ".env file found"

    print_test "Checking if .env contains required variables"
    REQUIRED_VARS=("POSTGRES_USER" "POSTGRES_PASSWORD" "POSTGRES_DB" "POSTGRES_PORT")
    ENV_VALID=true
    for var in "${REQUIRED_VARS[@]}"; do
        if grep -q "^${var}=" "$PROJECT_ROOT/.env"; then
            print_info "$var is defined"
        else
            print_fail "$var is missing from .env"
            ENV_VALID=false
        fi
    done
    if [ "$ENV_VALID" = true ]; then
        print_pass "All required environment variables present"
    fi
else
    print_fail ".env file not found (run: cp .env.example .env)"
fi

# Test 4: Docker container status
print_header "TEST 4: PostgreSQL Container Status"
print_test "Checking if Docker daemon is running"
if docker info > /dev/null 2>&1; then
    print_pass "Docker daemon is running"
else
    print_fail "Docker daemon is not running"
    print_summary
    exit 1
fi

print_test "Checking if PostgreSQL container exists"
if docker ps -a | grep -q "moneymatters-db"; then
    print_pass "PostgreSQL container exists"

    print_test "Checking if PostgreSQL container is running"
    if docker ps | grep -q "moneymatters-db"; then
        print_pass "PostgreSQL container is running"

        print_test "Checking if PostgreSQL container is healthy"
        HEALTH_STATUS=$(docker inspect --format='{{.State.Health.Status}}' moneymatters-db 2>/dev/null || echo "no-health-check")
        if [ "$HEALTH_STATUS" = "healthy" ]; then
            print_pass "PostgreSQL container is healthy"
        else
            print_fail "PostgreSQL container health status: $HEALTH_STATUS"
        fi
    else
        print_fail "PostgreSQL container is not running (run: ./backend/scripts/db-start.sh)"
    fi
else
    print_fail "PostgreSQL container does not exist (run: ./backend/scripts/db-start.sh)"
fi

# Test 5: Database connectivity
print_header "TEST 5: Database Connectivity"
print_test "Checking if PostgreSQL accepts connections"
if docker exec moneymatters-db pg_isready -U moneymatters -d moneymatters_dev > /dev/null 2>&1; then
    print_pass "PostgreSQL accepts connections"

    print_test "Checking if database 'moneymatters_dev' exists"
    DB_EXISTS=$(docker exec moneymatters-db psql -U moneymatters -lqt | cut -d \| -f 1 | grep -w moneymatters_dev | wc -l)
    if [ "$DB_EXISTS" -eq 1 ]; then
        print_pass "Database 'moneymatters_dev' exists"
    else
        print_fail "Database 'moneymatters_dev' does not exist"
    fi
else
    print_fail "PostgreSQL is not accepting connections"
fi

# Test 6: EF Core migrations
print_header "TEST 6: Entity Framework Core Migrations"
print_test "Checking if migrations have been applied"
MIGRATIONS_APPLIED=$(docker exec moneymatters-db psql -U moneymatters -d moneymatters_dev -tAc "SELECT COUNT(*) FROM \"__EFMigrationsHistory\";" 2>/dev/null || echo "0")
if [ "$MIGRATIONS_APPLIED" -gt 0 ]; then
    print_pass "$MIGRATIONS_APPLIED migration(s) applied"

    print_test "Checking if CompleteSchemaInitial migration exists"
    MIGRATION_EXISTS=$(docker exec moneymatters-db psql -U moneymatters -d moneymatters_dev -tAc "SELECT COUNT(*) FROM \"__EFMigrationsHistory\" WHERE \"MigrationId\" LIKE '%CompleteSchemaInitial%';" 2>/dev/null || echo "0")
    if [ "$MIGRATION_EXISTS" -eq 1 ]; then
        print_pass "CompleteSchemaInitial migration applied"
    else
        print_fail "CompleteSchemaInitial migration not found"
    fi
else
    print_fail "No migrations applied (run: dotnet ef database update)"
fi

# Test 7: Database schema
print_header "TEST 7: Database Schema"
print_test "Checking if all 10 tables exist"
TABLES=("Users" "Accounts" "Bills" "IncomeStreams" "Goals" "Transactions" "Alerts" "Settings" "ForecastSnapshots" "GoalAccounts")
TABLES_VALID=true
for table in "${TABLES[@]}"; do
    TABLE_EXISTS=$(docker exec moneymatters-db psql -U moneymatters -d moneymatters_dev -tAc "SELECT COUNT(*) FROM information_schema.tables WHERE table_name='${table}';" 2>/dev/null || echo "0")
    if [ "$TABLE_EXISTS" -eq 1 ]; then
        print_info "Table '$table' exists"
    else
        print_fail "Table '$table' does not exist"
        TABLES_VALID=false
    fi
done
if [ "$TABLES_VALID" = true ]; then
    print_pass "All 10 tables exist"
fi

print_test "Verifying table count (should be exactly 11 including __EFMigrationsHistory)"
TABLE_COUNT=$(docker exec moneymatters-db psql -U moneymatters -d moneymatters_dev -tAc "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = 'public';" 2>/dev/null || echo "0")
if [ "$TABLE_COUNT" -eq 11 ]; then
    print_pass "Exactly 11 tables exist (10 entities + migrations history)"
else
    print_fail "Expected 11 tables, found $TABLE_COUNT"
fi

# Test 8: User Secrets configuration
print_header "TEST 8: .NET User Secrets Configuration"
print_test "Checking if User Secrets are configured"
USER_SECRETS=$(dotnet user-secrets list --project "$PROJECT_ROOT/backend/MoneyMatters.Api" 2>/dev/null || echo "")
if echo "$USER_SECRETS" | grep -q "ConnectionStrings:DefaultConnection"; then
    print_pass "ConnectionStrings:DefaultConnection is set in User Secrets"
else
    print_fail "ConnectionStrings:DefaultConnection not set in User Secrets"
    print_info "Run: dotnet user-secrets set \"ConnectionStrings:DefaultConnection\" \"Host=localhost;Port=5432;Database=moneymatters_dev;Username=moneymatters;Password=dev_password_change_me\" --project backend/MoneyMatters.Api"
fi

# Test 9: Data persistence
print_header "TEST 9: Data Persistence"
print_test "Checking if seed data exists"
USER_COUNT=$(docker exec moneymatters-db psql -U moneymatters -d moneymatters_dev -tAc "SELECT COUNT(*) FROM \"Users\";" 2>/dev/null || echo "0")
if [ "$USER_COUNT" -gt 0 ]; then
    print_pass "Seed data exists ($USER_COUNT users found)"
else
    print_fail "No seed data found (API may not have run yet)"
fi

print_test "Checking Docker volume for persistence"
if docker volume ls | grep -q "money-matters_postgres_data"; then
    print_pass "Docker volume 'money-matters_postgres_data' exists"
else
    print_fail "Docker volume 'money-matters_postgres_data' does not exist"
fi

# Test 9a: Seed Data Counts (only run if seed data exists)
if [ "$USER_COUNT" -gt 0 ]; then
    print_header "TEST 9a: Seed Data Validation"

    print_test "Verifying exactly 1 user exists"
    if [ "$USER_COUNT" -eq 1 ]; then
        print_pass "Exactly 1 user exists"
    else
        print_fail "Expected 1 user, found $USER_COUNT"
    fi

    print_test "Verifying exactly 5 accounts exist"
    ACCOUNT_COUNT=$(docker exec moneymatters-db psql -U moneymatters -d moneymatters_dev -tAc "SELECT COUNT(*) FROM \"Accounts\";" 2>/dev/null || echo "0")
    if [ "$ACCOUNT_COUNT" -eq 5 ]; then
        print_pass "Exactly 5 accounts exist"
    else
        print_fail "Expected 5 accounts, found $ACCOUNT_COUNT"
    fi

    print_test "Verifying exactly 6 bills exist"
    BILL_COUNT=$(docker exec moneymatters-db psql -U moneymatters -d moneymatters_dev -tAc "SELECT COUNT(*) FROM \"Bills\";" 2>/dev/null || echo "0")
    if [ "$BILL_COUNT" -eq 6 ]; then
        print_pass "Exactly 6 bills exist"
    else
        print_fail "Expected 6 bills, found $BILL_COUNT"
    fi

    print_test "Verifying exactly 2 income streams exist"
    INCOME_COUNT=$(docker exec moneymatters-db psql -U moneymatters -d moneymatters_dev -tAc "SELECT COUNT(*) FROM \"IncomeStreams\";" 2>/dev/null || echo "0")
    if [ "$INCOME_COUNT" -eq 2 ]; then
        print_pass "Exactly 2 income streams exist"
    else
        print_fail "Expected 2 income streams, found $INCOME_COUNT"
    fi

    print_test "Verifying exactly 3 goals exist"
    GOAL_COUNT=$(docker exec moneymatters-db psql -U moneymatters -d moneymatters_dev -tAc "SELECT COUNT(*) FROM \"Goals\";" 2>/dev/null || echo "0")
    if [ "$GOAL_COUNT" -eq 3 ]; then
        print_pass "Exactly 3 goals exist"
    else
        print_fail "Expected 3 goals, found $GOAL_COUNT"
    fi

    print_test "Verifying exactly 3 goal-account links exist"
    GOALACCOUNT_COUNT=$(docker exec moneymatters-db psql -U moneymatters -d moneymatters_dev -tAc "SELECT COUNT(*) FROM \"GoalAccounts\";" 2>/dev/null || echo "0")
    if [ "$GOALACCOUNT_COUNT" -eq 3 ]; then
        print_pass "Exactly 3 goal-account links exist"
    else
        print_fail "Expected 3 goal-account links, found $GOALACCOUNT_COUNT"
    fi

    print_test "Verifying at least 9 transactions exist"
    TRANSACTION_COUNT=$(docker exec moneymatters-db psql -U moneymatters -d moneymatters_dev -tAc "SELECT COUNT(*) FROM \"Transactions\";" 2>/dev/null || echo "0")
    if [ "$TRANSACTION_COUNT" -ge 9 ]; then
        print_pass "At least 9 transactions exist ($TRANSACTION_COUNT found)"
    else
        print_fail "Expected at least 9 transactions, found $TRANSACTION_COUNT"
    fi

    print_test "Verifying exactly 5 settings exist"
    SETTING_COUNT=$(docker exec moneymatters-db psql -U moneymatters -d moneymatters_dev -tAc "SELECT COUNT(*) FROM \"Settings\";" 2>/dev/null || echo "0")
    if [ "$SETTING_COUNT" -eq 5 ]; then
        print_pass "Exactly 5 settings exist"
    else
        print_fail "Expected 5 settings, found $SETTING_COUNT"
    fi

    print_test "Verifying development user email"
    DEV_EMAIL=$(docker exec moneymatters-db psql -U moneymatters -d moneymatters_dev -tAc "SELECT \"Email\" FROM \"Users\" LIMIT 1;" 2>/dev/null || echo "")
    if [ "$DEV_EMAIL" = "dev@moneymatters.local" ]; then
        print_pass "Development user has correct email"
    else
        print_fail "Development user email is '$DEV_EMAIL', expected 'dev@moneymatters.local'"
    fi
fi

# Test 10: Security verification
print_header "TEST 10: Security Verification"
print_test "Checking if .env is gitignored"
cd "$PROJECT_ROOT"
if git check-ignore .env > /dev/null 2>&1; then
    print_pass ".env is properly gitignored"
else
    print_fail ".env is NOT gitignored (security risk!)"
fi

print_test "Checking if .env is NOT staged for commit"
if git status --porcelain | grep -q "^A  .env$"; then
    print_fail ".env is staged for commit (security risk!)"
else
    print_pass ".env is not staged for commit"
fi

print_test "Checking if .env.example is tracked"
if git ls-files --error-unmatch .env.example > /dev/null 2>&1; then
    print_pass ".env.example is tracked in git"
else
    print_fail ".env.example is not tracked in git"
fi

# Test 11: Documentation
print_header "TEST 11: Documentation"
print_test "Checking if quick-start.md has database setup section"
if grep -q "Database Setup" "$PROJECT_ROOT/docs/quick-start.md"; then
    print_pass "docs/quick-start.md contains Database Setup section"
else
    print_fail "docs/quick-start.md missing Database Setup section"
fi

print_test "Checking if backend README has Docker Compose instructions"
if grep -q "Docker Compose" "$PROJECT_ROOT/backend/README.md"; then
    print_pass "backend/README.md contains Docker Compose instructions"
else
    print_fail "backend/README.md missing Docker Compose instructions"
fi

# Print final summary
print_summary
exit $?
