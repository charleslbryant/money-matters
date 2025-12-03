#!/bin/bash
# Tail PostgreSQL container logs

# Get the project root (two levels up from this script)
PROJECT_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"

cd "$PROJECT_ROOT"
docker-compose logs -f postgres
