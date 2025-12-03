#!/bin/bash
# Reset database to clean state (DESTRUCTIVE: deletes all data)

set -e

# Get the project root (two levels up from this script)
PROJECT_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"

echo "⚠️  WARNING: This will delete ALL data in the database!"
read -p "Are you sure? (yes/no): " -r
echo

if [[ ! $REPLY =~ ^yes$ ]]; then
  echo "Aborted."
  exit 1
fi

echo "Stopping PostgreSQL container..."
cd "$PROJECT_ROOT"
docker-compose down postgres

echo "Removing database volume..."
docker volume rm money-matters_postgres_data || true

echo "Starting fresh PostgreSQL container..."
docker-compose up -d postgres

echo "Waiting for PostgreSQL to be ready..."
until docker-compose exec -T postgres pg_isready -U moneymatters -d moneymatters_dev > /dev/null 2>&1; do
  echo "  Waiting for database..."
  sleep 2
done

echo "Running EF Core migrations..."
cd "$PROJECT_ROOT/backend/MoneyMatters.Api"
dotnet ef database update

echo "✅ Database reset complete!"
