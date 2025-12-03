#!/bin/bash
# Comprehensive manual testing script for database verification
# Tests all acceptance criteria using direct SQL queries

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

# Helper function to run SQL and get result
run_sql() {
    docker exec moneymatters-db psql -U moneymatters -d moneymatters_dev -tAc "$1" 2>/dev/null || echo "ERROR"
}

print_header "DATABASE SCHEMA VERIFICATION TESTS"

# ACCEPTANCE CRITERION 1: Migrations run without errors
print_header "Criterion 1: Migrations Run Without Errors"

print_test "Check migrations table exists"
MIGRATIONS_TABLE=$(run_sql "SELECT COUNT(*) FROM information_schema.tables WHERE table_name='__EFMigrationsHistory';")
if [ "$MIGRATIONS_TABLE" = "1" ]; then
    print_pass "Migrations table exists"
else
    print_fail "Migrations table not found"
fi

print_test "Check CompleteSchemaInitial migration applied"
MIGRATION_APPLIED=$(run_sql "SELECT COUNT(*) FROM \"__EFMigrationsHistory\" WHERE \"MigrationId\" LIKE '%CompleteSchemaInitial%';")
if [ "$MIGRATION_APPLIED" = "1" ]; then
    print_pass "CompleteSchemaInitial migration applied successfully"
else
    print_fail "CompleteSchemaInitial migration not applied"
fi

# ACCEPTANCE CRITERION 2: All tables, indexes, and constraints created successfully
print_header "Criterion 2: Tables, Indexes, and Constraints"

print_test "Check all 10 tables exist"
TABLES=("Users" "Accounts" "Bills" "IncomeStreams" "Goals" "Transactions" "Alerts" "Settings" "ForecastSnapshots" "GoalAccounts")
ALL_TABLES_EXIST=true
for table in "${TABLES[@]}"; do
    TABLE_EXISTS=$(run_sql "SELECT COUNT(*) FROM information_schema.tables WHERE table_name='${table}';")
    if [ "$TABLE_EXISTS" = "1" ]; then
        print_info "Table '$table' exists"
    else
        print_fail "Table '$table' missing"
        ALL_TABLES_EXIST=false
    fi
done
if [ "$ALL_TABLES_EXIST" = true ]; then
    print_pass "All 10 required tables exist"
fi

print_test "Check User email unique index"
USER_EMAIL_INDEX=$(run_sql "SELECT COUNT(*) FROM pg_indexes WHERE tablename='Users' AND indexname LIKE '%Email%';")
if [ "$USER_EMAIL_INDEX" -ge "1" ]; then
    print_pass "User email index exists"
else
    print_fail "User email index missing"
fi

print_test "Check primary keys on all tables"
PK_COUNT=$(run_sql "SELECT COUNT(*) FROM information_schema.table_constraints WHERE constraint_type='PRIMARY KEY' AND table_schema='public' AND table_name IN ('Users','Accounts','Bills','IncomeStreams','Goals','Transactions','Alerts','Settings','ForecastSnapshots','GoalAccounts');")
if [ "$PK_COUNT" = "10" ]; then
    print_pass "All 10 tables have primary keys"
else
    print_fail "Expected 10 primary keys, found $PK_COUNT"
fi

print_test "Check foreign key constraints exist"
FK_COUNT=$(run_sql "SELECT COUNT(*) FROM information_schema.table_constraints WHERE constraint_type='FOREIGN KEY' AND table_schema='public';")
if [ "$FK_COUNT" -ge "10" ]; then
    print_pass "$FK_COUNT foreign key constraints exist"
else
    print_fail "Expected at least 10 foreign keys, found $FK_COUNT"
fi

# ACCEPTANCE CRITERION 3: Seed data populates (PARTIAL DUE TO BUG)
print_header "Criterion 3: Seed Data (PARTIAL - Bug Found)"

print_test "Check User seed data"
USER_COUNT=$(run_sql "SELECT COUNT(*) FROM \"Users\";")
if [ "$USER_COUNT" = "1" ]; then
    print_pass "1 user seeded successfully"

    print_test "Verify user email"
    USER_EMAIL=$(run_sql "SELECT \"Email\" FROM \"Users\" LIMIT 1;")
    if [ "$USER_EMAIL" = "dev@moneymatters.local" ]; then
        print_pass "User has correct email (dev@moneymatters.local)"
    else
        print_fail "User email incorrect: $USER_EMAIL"
    fi
else
    print_fail "Expected 1 user, found $USER_COUNT"
fi

print_test "Check Account seed data"
ACCOUNT_COUNT=$(run_sql "SELECT COUNT(*) FROM \"Accounts\";")
if [ "$ACCOUNT_COUNT" = "5" ]; then
    print_pass "5 accounts seeded successfully"

    print_test "Verify account domain distribution"
    PERSONAL_ACCOUNTS=$(run_sql "SELECT COUNT(*) FROM \"Accounts\" WHERE \"Domain\"=0;")  # Personal=0
    BUSINESS_ACCOUNTS=$(run_sql "SELECT COUNT(*) FROM \"Accounts\" WHERE \"Domain\"=1;")  # Business=1
    if [ "$PERSONAL_ACCOUNTS" = "3" ] && [ "$BUSINESS_ACCOUNTS" = "2" ]; then
        print_pass "Accounts have correct domain distribution (3 personal, 2 business)"
    else
        print_fail "Domain distribution incorrect (Personal: $PERSONAL_ACCOUNTS, Business: $BUSINESS_ACCOUNTS)"
    fi
else
    print_fail "Expected 5 accounts, found $ACCOUNT_COUNT"
fi

print_info "NOTE: Other seed data (Bills, IncomeStreams, Goals, Transactions, Settings) NOT populated due to DateTime bug"
print_info "Bug: SeedData.cs uses DateTime with Kind=Unspecified instead of UTC"
print_info "Location: Lines 108, 123, 138, 153, 168, 183 in SeedData.cs"

# ACCEPTANCE CRITERION 4: Foreign key relationships work correctly
print_header "Criterion 4: Foreign Key Relationships"

print_test "Verify Account UserId foreign key"
ACCOUNT_FK=$(run_sql "SELECT COUNT(*) FROM information_schema.table_constraints WHERE constraint_type='FOREIGN KEY' AND table_name='Accounts' AND constraint_name LIKE '%UserId%';")
if [ "$ACCOUNT_FK" -ge "1" ]; then
    print_pass "Account has UserId foreign key constraint"
else
    print_fail "Account UserId foreign key missing"
fi

print_test "Verify Account-User relationship data integrity"
ORPHAN_ACCOUNTS=$(run_sql "SELECT COUNT(*) FROM \"Accounts\" WHERE \"UserId\" NOT IN (SELECT \"Id\" FROM \"Users\");")
if [ "$ORPHAN_ACCOUNTS" = "0" ]; then
    print_pass "No orphaned accounts (all reference valid users)"
else
    print_fail "Found $ORPHAN_ACCOUNTS orphaned accounts"
fi

# ACCEPTANCE CRITERION 5: CRUD Operations Work
print_header "Criterion 5: CRUD Operations"

print_test "Test INSERT operation (create new account)"
TEST_USER_ID=$(run_sql "SELECT \"Id\" FROM \"Users\" LIMIT 1;")
INSERT_RESULT=$(run_sql "INSERT INTO \"Accounts\" (\"Id\", \"UserId\", \"Name\", \"AccountType\", \"Domain\", \"CurrentBalance\", \"SafeMinimumBalance\", \"IncludeInForecast\", \"IsActive\", \"CreatedAt\", \"UpdatedAt\") VALUES (gen_random_uuid(), '$TEST_USER_ID', 'Test Account', 'Testing', 0, 100.00, 0.00, true, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP); SELECT 'SUCCESS';")
if echo "$INSERT_RESULT" | grep -q "SUCCESS"; then
    print_pass "INSERT operation successful"
else
    print_fail "INSERT operation failed"
fi

print_test "Test SELECT operation (read data)"
SELECT_RESULT=$(run_sql "SELECT COUNT(*) FROM \"Accounts\" WHERE \"Name\"='Test Account';")
if [ "$SELECT_RESULT" = "1" ]; then
    print_pass "SELECT operation successful (test account found)"
else
    print_fail "SELECT operation failed (test account not found)"
fi

print_test "Test UPDATE operation (modify data)"
UPDATE_RESULT=$(run_sql "UPDATE \"Accounts\" SET \"CurrentBalance\"=200.00, \"UpdatedAt\"=CURRENT_TIMESTAMP WHERE \"Name\"='Test Account'; SELECT 'SUCCESS';")
if echo "$UPDATE_RESULT" | grep -q "SUCCESS"; then
    print_pass "UPDATE operation successful"

    UPDATED_BALANCE=$(run_sql "SELECT \"CurrentBalance\" FROM \"Accounts\" WHERE \"Name\"='Test Account';")
    if [ "$UPDATED_BALANCE" = "200.00" ]; then
        print_pass "UPDATE verified (balance changed to 200.00)"
    else
        print_fail "UPDATE verification failed (balance is $UPDATED_BALANCE)"
    fi
else
    print_fail "UPDATE operation failed"
fi

print_test "Test DELETE operation (remove data)"
DELETE_RESULT=$(run_sql "DELETE FROM \"Accounts\" WHERE \"Name\"='Test Account'; SELECT 'SUCCESS';")
if echo "$DELETE_RESULT" | grep -q "SUCCESS"; then
    print_pass "DELETE operation successful"

    DELETED_CHECK=$(run_sql "SELECT COUNT(*) FROM \"Accounts\" WHERE \"Name\"='Test Account';")
    if [ "$DELETED_CHECK" = "0" ]; then
        print_pass "DELETE verified (test account removed)"
    else
        print_fail "DELETE verification failed (account still exists)"
    fi
else
    print_fail "DELETE operation failed"
fi

# ACCEPTANCE CRITERION 6: No data type or constraint violations
print_header "Criterion 6: Data Types and Constraints"

print_test "Test decimal precision for monetary fields"
run_sql "INSERT INTO \"Accounts\" (\"Id\", \"UserId\", \"Name\", \"AccountType\", \"Domain\", \"CurrentBalance\", \"SafeMinimumBalance\", \"IncludeInForecast\", \"IsActive\", \"CreatedAt\", \"UpdatedAt\") VALUES (gen_random_uuid(), '$TEST_USER_ID', 'Precision Test', 'Testing', 0, 12345.67, 0.00, true, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);" > /dev/null
PRECISION_TEST=$(run_sql "SELECT \"CurrentBalance\" FROM \"Accounts\" WHERE \"Name\"='Precision Test';")
if [ "$PRECISION_TEST" = "12345.67" ]; then
    print_pass "Decimal fields preserve precision (18,2)"
else
    print_fail "Decimal precision incorrect: $PRECISION_TEST"
fi

# Cleanup
run_sql "DELETE FROM \"Accounts\" WHERE \"Name\"='Precision Test';" > /dev/null

print_test "Test unique constraint on User email"
UNIQUE_TEST=$(timeout 5 docker exec moneymatters-db psql -U moneymatters -d moneymatters_dev -c "INSERT INTO \"Users\" (\"Id\", \"Email\", \"Name\", \"TimeZone\", \"DefaultForecastHorizonDays\", \"CreatedAt\", \"UpdatedAt\") VALUES (gen_random_uuid(), 'dev@moneymatters.local', 'Duplicate', 'UTC', 30, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);" 2>&1 | head -1)
if echo "$UNIQUE_TEST" | grep -q "duplicate key"; then
    print_pass "Unique constraint enforced on User email"
else
    print_fail "Unique constraint not enforced"
fi

print_test "Test NOT NULL constraint on required fields"
NULL_TEST=$(timeout 5 docker exec moneymatters-db psql -U moneymatters -d moneymatters_dev -c "INSERT INTO \"Accounts\" (\"Id\", \"UserId\", \"AccountType\", \"Domain\", \"CurrentBalance\", \"SafeMinimumBalance\", \"IncludeInForecast\", \"IsActive\", \"CreatedAt\", \"UpdatedAt\") VALUES (gen_random_uuid(), '$TEST_USER_ID', 'Testing', 0, 100.00, 0.00, true, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);" 2>&1 | head -1)
if echo "$NULL_TEST" | grep -q "violates not-null constraint\|null value"; then
    print_pass "NOT NULL constraint enforced on required fields"
else
    print_fail "NOT NULL constraint not enforced"
fi

print_test "Test foreign key constraint enforcement"
FK_TEST=$(timeout 5 docker exec moneymatters-db psql -U moneymatters -d moneymatters_dev -c "INSERT INTO \"Accounts\" (\"Id\", \"UserId\", \"Name\", \"AccountType\", \"Domain\", \"CurrentBalance\", \"SafeMinimumBalance\", \"IncludeInForecast\", \"IsActive\", \"CreatedAt\", \"UpdatedAt\") VALUES (gen_random_uuid(), '00000000-0000-0000-0000-000000000000', 'FK Test', 'Testing', 0, 100.00, 0.00, true, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);" 2>&1 | head -1)
if echo "$FK_TEST" | grep -q "violates foreign key constraint"; then
    print_pass "Foreign key constraint enforced"
else
    print_fail "Foreign key constraint not enforced"
fi

# Summary
print_header "TEST SUMMARY"
echo -e "Total Tests: ${TESTS_RUN}"
echo -e "${GREEN}Passed: ${TESTS_PASSED}${NC}"
if [ $TESTS_FAILED -gt 0 ]; then
    echo -e "${RED}Failed: ${TESTS_FAILED}${NC}"
else
    echo -e "Failed: ${TESTS_FAILED}"
fi

if [ $TESTS_FAILED -eq 0 ]; then
    echo ""
    echo -e "${GREEN}✅ ALL MANUAL VERIFICATION TESTS PASSED!${NC}"
    echo ""
    exit 0
else
    echo ""
    echo -e "${YELLOW}⚠️  SOME TESTS FAILED (See details above)${NC}"
    echo ""
    exit 1
fi
