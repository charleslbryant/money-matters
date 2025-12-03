#!/bin/bash
# Stop PostgreSQL container gracefully

set -e

# Get the project root (two levels up from this script)
PROJECT_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"

echo "Stopping PostgreSQL container..."
cd "$PROJECT_ROOT"
docker-compose stop postgres

echo "✅ PostgreSQL stopped"
