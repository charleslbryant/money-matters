#!/bin/bash
# Master verification script that runs all database verification tests
# Combines bash verification script and xUnit tests for comprehensive validation

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Get the project root (two levels up from this script)
PROJECT_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"

print_header() {
    echo ""
    echo -e "${BLUE}========================================${NC}"
    echo -e "${BLUE}$1${NC}"
    echo -e "${BLUE}========================================${NC}"
    echo ""
}

print_info() {
    echo -e "${BLUE}ℹ INFO:${NC} $1"
}

print_success() {
    echo -e "${GREEN}✅ SUCCESS:${NC} $1"
}

print_fail() {
    echo -e "${RED}❌ FAIL:${NC} $1"
}

# Track overall success
BASH_TESTS_PASSED=false
XUNIT_TESTS_PASSED=false

print_header "DATABASE VERIFICATION TEST SUITE"
print_info "Running comprehensive database verification tests"
print_info "Project root: $PROJECT_ROOT"
echo ""

# Phase 1: Run bash verification script
print_header "PHASE 1: Infrastructure & Seed Data Verification (Bash)"
print_info "Running verify-db-setup.sh script..."
echo ""

if bash "$PROJECT_ROOT/backend/scripts/verify-db-setup.sh"; then
    BASH_TESTS_PASSED=true
    print_success "Bash verification tests passed"
else
    print_fail "Bash verification tests failed"
    echo ""
    echo -e "${RED}Some infrastructure tests failed. Fix these before running xUnit tests.${NC}"
    echo ""
    exit 1
fi

# Phase 2: Run xUnit schema validation tests
print_header "PHASE 2: Schema & CRUD Validation (xUnit)"
print_info "Running .NET xUnit tests for database verification..."
echo ""

# Change to backend directory
cd "$PROJECT_ROOT/backend"

# Run only the verification test files
print_info "Running SchemaValidationTests..."
print_info "Running SeedDataValidationTests..."
print_info "Running CrudOperationTests..."
print_info "Running ConstraintValidationTests..."
echo ""

# Run dotnet test with filter for verification tests
if dotnet test MoneyMatters.Infrastructure.Tests/MoneyMatters.Infrastructure.Tests.csproj \
    --filter "FullyQualifiedName~MoneyMatters.Infrastructure.Tests.Data.SchemaValidationTests|FullyQualifiedName~MoneyMatters.Infrastructure.Tests.Data.SeedDataValidationTests|FullyQualifiedName~MoneyMatters.Infrastructure.Tests.Data.CrudOperationTests|FullyQualifiedName~MoneyMatters.Infrastructure.Tests.Data.ConstraintValidationTests" \
    --verbosity minimal; then
    XUNIT_TESTS_PASSED=true
    print_success "xUnit verification tests passed"
else
    print_fail "xUnit verification tests failed"
fi

# Final Summary
print_header "VERIFICATION TEST SUMMARY"

if [ "$BASH_TESTS_PASSED" = true ]; then
    echo -e "${GREEN}✅ Infrastructure & Seed Data Tests: PASSED${NC}"
else
    echo -e "${RED}❌ Infrastructure & Seed Data Tests: FAILED${NC}"
fi

if [ "$XUNIT_TESTS_PASSED" = true ]; then
    echo -e "${GREEN}✅ Schema & CRUD Validation Tests: PASSED${NC}"
else
    echo -e "${RED}❌ Schema & CRUD Validation Tests: FAILED${NC}"
fi

echo ""

if [ "$BASH_TESTS_PASSED" = true ] && [ "$XUNIT_TESTS_PASSED" = true ]; then
    print_header "ALL VERIFICATION TESTS PASSED! 🎉"
    echo -e "${GREEN}Database schema and migrations are fully verified.${NC}"
    echo ""
    echo "Summary:"
    echo "  ✅ Docker container healthy"
    echo "  ✅ Database connectivity verified"
    echo "  ✅ Migrations applied correctly"
    echo "  ✅ All 10 tables exist"
    echo "  ✅ Seed data populated (1 user, 5 accounts, 6 bills, etc.)"
    echo "  ✅ Schema validation passed"
    echo "  ✅ CRUD operations functional"
    echo "  ✅ Constraints enforced"
    echo "  ✅ Foreign keys working"
    echo "  ✅ Security checks passed"
    echo ""
    exit 0
else
    print_header "VERIFICATION TESTS FAILED"
    echo -e "${RED}Some verification tests failed. Review the output above for details.${NC}"
    echo ""
    exit 1
fi
