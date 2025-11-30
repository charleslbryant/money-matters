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
├── docs/              # All project documentation
│   ├── README.md                        # Documentation index
│   ├── product-spec.md                  # Product requirements
│   ├── implementation-plan.md           # Development roadmap
│   ├── security.md                      # Security guidelines
│   ├── quick-start.md                   # Setup guide
│   ├── components/                      # Component docs
│   │   ├── forms-api-reference.md      # Form components API
│   │   └── form-components-delivery.md # Delivery summaries
│   └── development/                     # Development guides
│       └── storybook-guide.md          # Storybook setup
├── CLAUDE.md          # This file - Claude Code configuration
└── README.md          # Project overview
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

See `docs/security.md` for comprehensive security guidelines.

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

The app uses a minimal, clean design system defined in `docs/product-spec.md`:

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
3. **New features**: Reference `docs/product-spec.md` for UI requirements and `docs/implementation-plan.md` for task organization
4. **Database changes**: Create EF Core migrations and update the migration in the commit
5. **API changes**: Update Swagger/OpenAPI documentation
6. **Documentation changes**: Update `docs/README.md` index when adding new documentation (see Documentation Maintenance below)

## Important Files to Reference

- **docs/README.md**: Documentation index with links to all docs
- **docs/product-spec.md**: Complete UI specification, component library, screen layouts, API bindings
- **docs/implementation-plan.md**: Phased implementation plan with all features organized
- **docs/security.md**: Security guidelines, secrets management, Azure Key Vault setup
- **docs/quick-start.md**: Fast setup instructions
- **frontend/README.md**: Frontend-specific setup and commands
- **src/backend/README.md**: Backend-specific architecture notes

## Documentation Maintenance

### Documentation Structure

All project documentation lives in the `docs/` directory with a centralized index at `docs/README.md`.

### File Naming Convention

**IMPORTANT**: All documentation files MUST use lowercase names with dash separators:

✅ **Correct**:
- `product-spec.md`
- `implementation-plan.md`
- `form-components-delivery.md`
- `storybook-guide.md`

❌ **Incorrect**:
- `PRODUCT_SPEC.md`
- `ProductSpec.md`
- `product_spec.md`
- `ProductSpecification.md`

### Documentation Categories

Organize documentation into these categories:

```
docs/
├── README.md                    # Main index (MUST be updated)
├── *.md                        # Root-level docs (product, implementation, etc.)
├── components/                 # Component-specific documentation
│   ├── *-api-reference.md     # API references
│   └── *-delivery.md          # Delivery summaries
├── development/                # Development guides and tools
│   └── *-guide.md             # Setup/usage guides
└── architecture/               # Architecture documentation
    └── *.md                    # Architecture docs
```

### Adding New Documentation

When creating new documentation, **ALWAYS**:

1. **Create the file** in the appropriate category folder
2. **Use lowercase-with-dashes** naming convention
3. **Update `docs/README.md`** to include the new document:
   - Add to table of contents
   - Add to appropriate topic section
   - Add to file structure tree
   - Add brief description

4. **Link from related docs** if applicable (e.g., README.md, CLAUDE.md)
5. **Use consistent formatting**:
   - Markdown with proper headers
   - Code blocks with language tags
   - Examples where helpful
   - Attribution footer: `🤖 Submitted by George with love ♥`

### Updating `docs/README.md` Index

The `docs/README.md` file is the central hub for all documentation. When adding new docs:

#### Update the File Structure Tree

```markdown
## Documentation Structure

```
docs/
├── README.md
├── your-new-doc.md              # Add here
├── components/
│   └── your-component-doc.md    # Or here
└── development/
    └── your-guide.md            # Or here
```
```

#### Add to Appropriate Topic Section

Find the relevant section (Product & Planning, Components, Development Tools, etc.) and add a link:

```markdown
### Components

- **[Your Component](components/your-component.md)** - Brief description of what it documents
```

#### Add to Quick Links (if important)

For critical documents, add to the Quick Links section at the top.

### Documentation Quality Standards

All documentation should:

- ✅ Be clear and concise
- ✅ Include practical examples
- ✅ Use proper Markdown formatting
- ✅ Be kept up-to-date with code changes
- ✅ Include code snippets with syntax highlighting
- ✅ Have a clear table of contents for long docs
- ✅ Use relative links for internal references
- ✅ End with attribution footer

### Example: Adding a New Component Doc

```bash
# 1. Create the documentation file
touch docs/components/button-components.md

# 2. Write the documentation with proper naming
# File: docs/components/button-components.md

# 3. Update docs/README.md:
#    - Add to file structure tree
#    - Add to "Components" section
#    - Add brief description

# 4. Link from main README.md if it's a major component

# 5. Commit with clear message
git add docs/components/button-components.md docs/README.md
git commit -m "Add button components documentation

Added comprehensive documentation for button components including
variants, props, and usage examples.

🤖 Submitted by George with love ♥"
```

### Documentation Review Checklist

Before committing documentation changes:

- [ ] File uses lowercase-with-dashes naming
- [ ] File is in correct category folder
- [ ] `docs/README.md` index updated
- [ ] Links are relative and working
- [ ] Code examples are tested
- [ ] Proper Markdown formatting
- [ ] Attribution footer included
- [ ] No typos or grammar errors

### Common Documentation Tasks

#### Creating a New Component Delivery Doc

```bash
# Location: docs/components/
# Name: component-name-delivery.md
# Update: docs/README.md under "Components" section
```

#### Creating a New Development Guide

```bash
# Location: docs/development/
# Name: tool-name-guide.md
# Update: docs/README.md under "Development Tools" section
```

#### Updating Product Spec

```bash
# File: docs/product-spec.md
# Also update: Version/date in document if using versions
# Consider: If major changes, note in docs/README.md description
```

## Current Development Phase

Phase 1: Foundation & Infrastructure (In Progress)
- ✅ React frontend foundation complete
- ✅ Migrated to Bun for package management
- ✅ Form components with React Hook Form + Zod validation
- 🚧 .NET backend foundation in progress
- 🚧 Azure infrastructure setup pending

Refer to `docs/implementation-plan.md` for the complete roadmap.
