#!/bin/bash
# Start PostgreSQL container and wait for healthy status

set -e

# Get the project root (two levels up from this script)
PROJECT_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"

echo "Starting PostgreSQL container..."
cd "$PROJECT_ROOT"
docker-compose up -d postgres

echo "Waiting for PostgreSQL to be ready..."
until docker-compose exec -T postgres pg_isready -U moneymatters -d moneymatters_dev > /dev/null 2>&1; do
  echo "  Waiting for database..."
  sleep 2
done

echo "✅ PostgreSQL is ready!"
echo "Connection string: Host=localhost;Port=5432;Database=moneymatters_dev;Username=moneymatters;Password=dev_password_change_me"
