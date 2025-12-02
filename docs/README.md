# Money Matters Documentation

Welcome to the Money Matters documentation. This directory contains all project documentation organized by category.

## Quick Links

- [Getting Started](../README.md) - Project overview and quick start
- [Product Specification](product-spec.md) - Complete product requirements and UI spec
- [Implementation Plan](implementation-plan.md) - Phase-by-phase development roadmap
- [Security Guidelines](security.md) - Security best practices and requirements
- [Quick Start Guide](quick-start.md) - Fast setup instructions

## Documentation Structure

This is the project-wide documentation directory. Frontend-specific and backend-specific documentation can be found in their respective directories.

```
docs/                                   # Project-wide documentation (you are here)
├── README.md                           # This file - Documentation index
├── product-spec.md                     # Product requirements and design
├── implementation-plan.md              # Development roadmap
├── database-schema.md                  # Database schema design
├── security.md                         # Security guidelines
├── quick-start.md                      # Quick setup guide
│
├── components/                         # Shared component documentation
│   ├── forms-api-reference.md         # Form components API reference
│   └── form-components-delivery.md    # Form components delivery summary
│
└── development/                        # Development guides and tools
    ├── storybook-guide.md             # Storybook setup and usage
    └── storybook-implementation.md    # Storybook implementation details

frontend/docs/                          # Frontend-specific documentation
├── architecture/                       # Frontend architecture docs
├── components/                         # Component-specific docs
└── development/                        # Frontend development guides

backend/                                # Backend-specific documentation
├── README.md                           # Backend architecture and setup
└── TESTING.md                          # Backend testing guide
```

## Documentation by Topic

### Product & Planning

- **[Product Specification](product-spec.md)** - Detailed product requirements, UI components, screen layouts, and API bindings. Essential reading for understanding what Money Matters does and how it should work.

- **[Implementation Plan](implementation-plan.md)** - Phase-by-phase implementation plan with all features organized into logical development phases. Track progress and understand the development roadmap.

- **[Database Schema](database-schema.md)** - Complete database schema design with all entities, relationships, indexes, and constraints. Essential reference for backend development.

### Getting Started

- **[Quick Start Guide](quick-start.md)** - Fast setup instructions to get the project running locally. Perfect for new developers joining the project.

- **[README](../README.md)** - Project overview, tech stack, and basic commands. Start here for a high-level understanding.

### Security

- **[Security Guidelines](security.md)** - Critical security rules, secrets management, Azure Key Vault setup, and security best practices. **READ THIS BEFORE COMMITTING CODE**.

### Components

- **[Forms API Reference](components/forms-api-reference.md)** - Comprehensive API documentation for all form components including Form, TextField, NumberField, Select, TextArea, and SearchInput. Includes props, examples, and best practices.

- **[Form Components Delivery](components/form-components-delivery.md)** - Delivery summary for the form components implementation. Details what was delivered, features implemented, and usage examples.

### Development Tools

- **[Storybook Guide](development/storybook-guide.md)** - Complete guide to implementing Storybook for component development and documentation. Includes setup instructions, configuration, best practices, and story examples.

## Key Concepts

### What is Money Matters?

Money Matters is a cash-flow intelligence dashboard for entrepreneurs. It monitors cash across accounts, tracks bills and income, forecasts cash flow, warns before shortfalls, and tracks savings goals.

**Key distinction**: This is NOT a budgeting app—it's a financial forecasting and alerting system focused on forward-looking insights (30-90 days), not historical budgeting.

### Tech Stack

**Frontend:**
- React 19 with TypeScript
- Vite build tool
- Bun package manager (NOT npm)
- Tailwind CSS for styling
- React Router v6
- React Hook Form + Zod validation
- Axios for API calls

**Backend:**
- .NET 10 with C#
- Clean Architecture with CQRS
- PostgreSQL database
- Entity Framework Core 10

### Repository Structure

This is a monorepo with separate frontend and backend:
```
money-matters/
├── frontend/                      # React + TypeScript + Vite
│   ├── src/                      # Application source code
│   ├── e2e/                      # Playwright E2E tests
│   ├── docs/                     # Frontend-specific docs
│   ├── README.md                 # Frontend setup guide
│   └── TESTING.md                # Frontend testing guide
├── backend/                       # .NET 10 with Clean Architecture
│   ├── MoneyMatters.Api           # Web API layer
│   ├── MoneyMatters.Api.Tests     # API tests
│   ├── MoneyMatters.Application   # Business logic
│   ├── MoneyMatters.Application.Tests
│   ├── MoneyMatters.Core          # Domain models
│   ├── MoneyMatters.Core.Tests
│   ├── MoneyMatters.Infrastructure # Data access
│   ├── MoneyMatters.Infrastructure.Tests
│   ├── README.md                  # Backend architecture guide
│   └── TESTING.md                 # Backend testing guide
├── docs/                          # Project-wide documentation (you are here)
├── CLAUDE.md                      # Claude Code configuration
└── README.md                      # Project overview
```

## Development Phases

The project is organized into phases (see [implementation-plan.md](implementation-plan.md)):

- **Phase 1**: Foundation & Infrastructure ✅ (In Progress)
- **Phase 2**: Core Financial Data Management
- **Phase 3**: Forecasting & Analytics
- **Phase 4**: Alerts & Notifications
- **Phase 5**: Goals & Savings
- **Phase 6**: Polish & Optimization

## Common Commands

### Frontend

```bash
cd frontend

# Install dependencies (use Bun, NOT npm)
bun install

# Start dev server
bun run dev

# Build for production
bun run build

# Linting and formatting
bun run lint:fix
bun run format
```

### Backend

```bash
cd backend

# Restore dependencies
dotnet restore

# Run database migrations
dotnet ef database update --project MoneyMatters.Api

# Run the API
dotnet run --project MoneyMatters.Api

# Run tests
dotnet test
```

## Contributing

### Before You Code

1. Read [SECURITY.md](security.md) - Critical security rules
2. Review [PRODUCT_SPEC.md](product-spec.md) - Understand the requirements
3. Check [IMPLEMENTATION_PLAN.md](implementation-plan.md) - See what's being worked on
4. Read [CLAUDE.md](../CLAUDE.md) - Coding conventions and git workflow

### Coding Standards

- Frontend: Use Bun, not npm
- Always run `bun run lint:fix` and `bun run format` before committing
- Backend: Run `dotnet test` before committing
- Git: All commits use the attribution: "🤖 Submitted by George with love ♥"

### Documentation Standards

- All docs use lowercase names with dash separators (e.g., `form-components-delivery.md`)
- Keep docs up-to-date as code changes
- Add new docs to appropriate category folder
- Update this README when adding new documentation

## Design Principles

From the product spec:

1. **No fluff** - Focus on clarity and decision support
2. **Data-dense but readable** - Show important information efficiently
3. **Strong use of status colors** - Green/yellow/red for quick scanning
4. **Charts and visualizations** - Make trends obvious
5. **Accessible** - WCAG 2.1 AA compliant
6. **Responsive** - Mobile-first design

## Resources

### Internal Documentation

- [Product Spec](product-spec.md) - What we're building
- [Implementation Plan](implementation-plan.md) - How we're building it
- [Security](security.md) - How we're securing it
- [Components](components/) - Component library documentation
- [Development](development/) - Development tools and guides

### External Resources

- [React Documentation](https://react.dev/)
- [TypeScript Handbook](https://www.typescriptlang.org/docs/)
- [Tailwind CSS](https://tailwindcss.com/docs)
- [React Hook Form](https://react-hook-form.com/)
- [Zod](https://zod.dev/)
- [.NET Documentation](https://learn.microsoft.com/en-us/dotnet/)

## Getting Help

- Check existing documentation first
- Review related GitHub issues
- Consult the Product Spec for requirements
- Check the Implementation Plan for context

## Documentation Updates

When adding new documentation:

1. Create file in appropriate category folder
2. Use lowercase-with-dashes naming
3. Add link to this README in appropriate section
4. Update table of contents if needed
5. Commit with clear message about documentation changes

---

🤖 Submitted by George with love ♥
