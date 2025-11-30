# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Money Matters is a cash-flow intelligence dashboard for entrepreneurs. It monitors cash across accounts, tracks bills and income, forecasts cash flow, warns before shortfalls, and tracks savings goals. This is NOT a budgeting app—it's a financial forecasting and alerting system.

**Key distinction**: The app focuses on forward-looking forecasting (30-90 days), not historical budgeting.

## Repository Structure

This is a monorepo with separate frontend and backend:

```
money-matters/
├── frontend/           # React + TypeScript + Vite
├── src/backend/       # .NET 10 with Clean Architecture
├── PRODUCT_SPEC.md    # Detailed product requirements and UI spec
├── IMPLEMENTATION_PLAN.md  # Phase-by-phase implementation plan
└── SECURITY.md        # Critical security guidelines
```

## Tech Stack & Package Management

### Frontend (frontend/)
- **Runtime**: Bun (NOT npm - use `bun` commands, not `npm`)
- **Framework**: React 19 with TypeScript
- **Build**: Vite
- **Styling**: Tailwind CSS
- **Routing**: React Router v6
- **API Client**: Axios with interceptors (see `src/services/api.ts`)

### Backend (src/backend/)
- **.NET 10** with C#
- **Architecture**: Clean Architecture with CQRS pattern
- **Database**: PostgreSQL with Entity Framework Core 10
- **Structure**:
  - `MoneyMatters.Api` - Web API layer
  - `MoneyMatters.Application` - Business logic, CQRS
  - `MoneyMatters.Core` - Domain models, interfaces
  - `MoneyMatters.Infrastructure` - Data access, external services

## Common Development Commands

### Frontend Development

```bash
cd frontend

# Install dependencies (use Bun, NOT npm)
bun install

# Start dev server (http://localhost:5173)
bun run dev

# Build for production
bun run build

# Linting and formatting
bun run lint          # Check for errors
bun run lint:fix      # Auto-fix errors
bun run format        # Format code with Prettier
bun run format:check  # Check formatting
```

### Backend Development

```bash
cd src/backend

# Restore dependencies
dotnet restore

# Run database migrations
dotnet ef database update --project MoneyMatters.Api

# Run the API (https://localhost:7001)
dotnet run --project MoneyMatters.Api

# Run tests
dotnet test

# Build solution
dotnet build
```

## Architecture Notes

### Frontend Architecture

The frontend follows a component-driven architecture:

- **API Client** (`src/services/api.ts`): Pre-configured Axios instance with:
  - Request interceptor for auth token injection
  - Response interceptor for 401 handling (auto-logout)
  - Base URL from environment variables

- **Error Boundary** (`src/components/ErrorBoundary.tsx`): Global error handler that catches React errors and displays user-friendly error screen

- **Routing**: React Router v6 configured in `App.tsx`

- **Environment Variables**: All Vite env vars must be prefixed with `VITE_`

### Backend Architecture (Clean Architecture)

The backend uses Clean Architecture with CQRS:

- **Core**: Domain entities, value objects, interfaces (no dependencies)
- **Application**: Business logic, commands/queries (MediatR), validators (FluentValidation)
- **Infrastructure**: Data access (EF Core), external services, repository implementations
- **Api**: Controllers, middleware, dependency injection configuration

**Key patterns**:
- Commands and Queries separated via MediatR
- Repository pattern for data access
- Domain-driven design for business logic

## Critical Security Rules

🔒 **THIS IS A PUBLIC REPOSITORY** - Never commit:
- Database passwords or connection strings with credentials
- API keys, tokens, or secrets
- `.env` files (only `.env.example` is safe)
- `appsettings.Development.json` or any env-specific config files

**What to commit**:
- `.env.example` (template with placeholder values)
- `appsettings.example.json` (template with placeholder values)

**Production secrets**: Store ALL production secrets in Azure Key Vault, referenced via Managed Identity.

See `SECURITY.md` for comprehensive security guidelines.

## Domain Model Overview

The app separates **Personal** and **Business** finances. Core entities:

- **Account**: Bank accounts with balances, safe minimums, domain (personal/business)
- **Transaction**: Individual transactions, can be tagged for bills/goals/income
- **Bill**: Recurring obligations with due dates, amounts, frequencies
- **IncomeStream**: Expected income with frequency and typical amounts
- **Goal**: Savings goals with targets, dates, progress tracking, funding rules
- **Alert**: System-generated warnings for shortfalls, bill risks, goal delays
- **Forecast**: Cash flow projections for 30-90 day horizons

## Key Business Logic

### Forecast Engine
The forecasting engine calculates:
- Projected balance over time (30/60/90 day horizons)
- Days of runway before cash shortfall
- Bill coverage status (covered/partially covered/uncovered)
- Goal completion projections
- Personal vs Business vs Combined views

### Alert Generation
Alerts are automatically generated for:
- Cash shortfalls (projected balance < safe minimum)
- Bill risks (upcoming bills not covered by projected balance)
- Income delays (expected income not received)
- Goal risks (savings goal falling behind schedule)
- Status indicators: green (healthy), yellow (warning), red (critical)

## API Endpoint Conventions

The backend exposes RESTful endpoints following this pattern:

```
/api/dashboard/summary              # Dashboard overview
/api/forecasts/current?horizonDays=30&scope=personal|business|combined
/api/bills                          # List bills
/api/bills/upcoming?days=30         # Upcoming bills
/api/goals                          # List goals
/api/goals/summary                  # Goals for dashboard
/api/accounts                       # List accounts
/api/transactions                   # Transactions with filtering
/api/income-streams                 # Income sources
/api/alerts                         # Alert management
/api/settings                       # User settings
```

All endpoints return JSON. API client handles auth headers automatically.

## Design System

The app uses a minimal, clean design system defined in the PRODUCT_SPEC.md:

- **Colors**: Semantic colors (green/yellow/red for status), neutral backgrounds
- **Components**: StatCard, InfoCard, DataTable, Badge, Chip, Charts (line/bar/donut)
- **Themes**: Light and dark mode support
- **Responsive**: Mobile-first design (mobile → tablet → desktop)

Key UI principles:
- No fluff - focus on clarity and decision support
- Data-dense but readable
- Strong use of status colors for quick scanning
- Charts and visualizations for trends

## Git Workflow

All commits and PRs must use this attribution:

```
🤖 Submitted by George with love ♥
```

**Never use** the default Claude Code attribution format.

## Development Workflow

1. **Frontend changes**: Always run `bun run lint:fix` and `bun run format` before committing
2. **Backend changes**: Run `dotnet test` to ensure tests pass
3. **New features**: Reference PRODUCT_SPEC.md for UI requirements and IMPLEMENTATION_PLAN.md for task organization
4. **Database changes**: Create EF Core migrations and update the migration in the commit
5. **API changes**: Update Swagger/OpenAPI documentation

## Important Files to Reference

- **PRODUCT_SPEC.md**: Complete UI specification, component library, screen layouts, API bindings
- **IMPLEMENTATION_PLAN.md**: Phased implementation plan with all features organized
- **SECURITY.md**: Security guidelines, secrets management, Azure Key Vault setup
- **frontend/README.md**: Frontend-specific setup and commands
- **src/backend/README.md**: Backend-specific architecture notes

## Current Development Phase

Phase 1: Foundation & Infrastructure (In Progress)
- ✅ React frontend foundation complete
- ✅ Migrated to Bun for package management
- 🚧 .NET backend foundation in progress
- 🚧 Azure infrastructure setup pending

Refer to IMPLEMENTATION_PLAN.md for the complete roadmap.
