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
- React 18 with TypeScript
- Vite for build tooling
- Tailwind CSS for styling
- React Router v6 for routing
- React Query for data fetching
- Recharts for data visualization

### Backend
- .NET 8 with C#
- Entity Framework Core 8
- PostgreSQL 15+
- Clean Architecture with CQRS pattern
- RESTful API with Swagger/OpenAPI

### Infrastructure
- Azure App Service (API)
- Azure Static Web Apps (Frontend)
- Azure Database for PostgreSQL
- Azure Application Insights
- Azure Key Vault

## Getting Started

### Prerequisites

- Node.js 18+ and npm
- .NET 8 SDK
- PostgreSQL 15+
- Azure CLI (for deployment)

### Local Development

#### Backend Setup

```bash
cd src/backend
dotnet restore
dotnet ef database update
dotnet run --project MoneyMatters.Api
```

The API will be available at `https://localhost:7001`

#### Frontend Setup

```bash
cd src/frontend
npm install
npm run dev
```

The app will be available at `http://localhost:5173`

### Environment Variables

See `.env.example` files in both frontend and backend directories.

## Project Structure

```
money-matters/
├── src/
│   ├── backend/              # .NET 8 API
│   │   ├── MoneyMatters.Api
│   │   ├── MoneyMatters.Application
│   │   ├── MoneyMatters.Core
│   │   └── MoneyMatters.Infrastructure
│   └── frontend/             # React app
│       ├── src/
│       │   ├── components/
│       │   ├── screens/
│       │   ├── hooks/
│       │   ├── services/
│       │   └── types/
│       └── public/
├── docs/                     # Documentation
├── PRODUCT_SPEC.md          # Product specification
└── IMPLEMENTATION_PLAN.md   # Implementation plan
```

## Documentation

- [Product Specification](./PRODUCT_SPEC.md)
- [Implementation Plan](./IMPLEMENTATION_PLAN.md)
- [API Documentation](./docs/api.md) (Coming soon)
- [Architecture](./docs/architecture.md) (Coming soon)

## Contributing

See [CONTRIBUTING.md](./CONTRIBUTING.md) for guidelines.

## License

MIT License - see [LICENSE](./LICENSE) for details.

---

🤖 Submitted by George with love ♥
