# Money Matters

**Personal and business financial forecasting and alerting system**

Money Matters is a cash-flow intelligence dashboard designed for entrepreneurs. It monitors cash across accounts, tracks upcoming bills and income, forecasts cash flow, warns before shortfalls, and tracks progress toward savings goals.

## Features

- **Dashboard**: Real-time financial health status with personal and business views
- **Cash Flow Forecasting**: 30-90 day projections with bill coverage analysis
- **Bill Management**: Track recurring obligations and payment coverage
- **Income Tracking**: Monitor expected and actual income streams
- **Goals Management**: Track savings goals and planned purchases
- **Smart Alerts**: Warnings for shortfalls, bill risks, and goal delays
- **Transactions**: Complete transaction ledger with tagging and categorization
- **Multi-Domain**: Separate personal and business financial tracking

## Tech Stack

### Frontend
- React 19 with TypeScript
- Vite for build tooling
- Bun package manager
- Tailwind CSS for styling
- React Router v6 for routing
- React Hook Form + Zod for forms and validation
- Axios for API communication

### Backend
- .NET 10 with C#
- Entity Framework Core 10
- PostgreSQL 15+
- Clean Architecture with CQRS pattern
- RESTful API with Swagger/OpenAPI

### Infrastructure
- Azure App Service (API)
- Azure Static Web Apps (Frontend)
- Azure Database for PostgreSQL
- Azure Application Insights
- Azure Key Vault

## 🔒 Security Notice

**This is a PUBLIC repository. Never commit secrets, API keys, passwords, or connection strings.**

All sensitive configuration MUST be:
- Stored in Azure Key Vault (production)
- Stored in local environment variables (development)
- Added to .gitignore
- Never hardcoded in source files

See [SECURITY.md](./docs/security.md) for complete security guidelines.

## Getting Started

### Prerequisites

- Bun 1.2+ (package manager)
- .NET 10 SDK
- PostgreSQL 15+
- Azure CLI (for deployment)

### Local Development

See [Quick Start Guide](./docs/quick-start.md) for detailed setup instructions.

#### Backend Setup

```bash
cd src/backend
dotnet restore
dotnet ef database update --project MoneyMatters.Api
dotnet run --project MoneyMatters.Api
```

The API will be available at `https://localhost:7001`

#### Frontend Setup

```bash
cd frontend
bun install
bun run dev
```

The app will be available at `http://localhost:5173`

### Environment Variables

See `.env.example` files in both frontend and backend directories.

## Project Structure

```
money-matters/
├── frontend/                 # React + TypeScript + Vite
│   ├── src/
│   │   ├── components/       # Reusable UI components
│   │   ├── services/         # API client and utilities
│   │   └── App.tsx
│   └── package.json
├── src/backend/             # .NET 10 API
│   ├── MoneyMatters.Api
│   ├── MoneyMatters.Application
│   ├── MoneyMatters.Core
│   └── MoneyMatters.Infrastructure
├── docs/                    # All documentation
│   ├── README.md           # Documentation index
│   ├── product-spec.md     # Product specification
│   ├── implementation-plan.md
│   ├── security.md
│   ├── components/         # Component documentation
│   └── development/        # Development guides
├── CLAUDE.md               # Claude Code configuration
└── README.md              # This file
```

## Documentation

📚 **[View All Documentation](./docs/README.md)**

Key documents:
- [Product Specification](./docs/product-spec.md) - Complete product requirements
- [Implementation Plan](./docs/implementation-plan.md) - Development roadmap
- [Security Guidelines](./docs/security.md) - Security best practices
- [Quick Start Guide](./docs/quick-start.md) - Fast setup instructions
- [Storybook Guide](./docs/development/storybook-guide.md) - Component development
- [Form Components](./docs/components/forms-api-reference.md) - Form component API

## Contributing

See [CONTRIBUTING.md](./CONTRIBUTING.md) for guidelines.

## License

MIT License - see [LICENSE](./LICENSE) for details.

---

🤖 Submitted by George with love ♥
