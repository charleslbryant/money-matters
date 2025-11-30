# Money Matters - Frontend

React-based frontend for the Money Matters personal finance management system.

## Tech Stack

- **React 19** with TypeScript
- **Vite** for build tooling
- **Bun** for package management and runtime
- **Tailwind CSS** for styling
- **React Router v6** for routing
- **Axios** for API communication
- **ESLint + Prettier** for code quality

## Project Structure

```
frontend/
├── src/
│   ├── components/      # Reusable React components
│   │   └── ErrorBoundary.tsx
│   ├── hooks/          # Custom React hooks
│   ├── services/       # API clients and external services
│   │   └── api.ts
│   ├── types/          # TypeScript type definitions
│   ├── utils/          # Utility functions
│   ├── App.tsx         # Main application component
│   └── main.tsx        # Application entry point
├── public/             # Static assets
└── .env.example        # Environment variables template
```

## Getting Started

### Prerequisites

- [Bun](https://bun.sh/) 1.0+ (faster alternative to Node.js and npm)

### Installation

1. Install dependencies:
```bash
bun install
```

2. Create environment file:
```bash
cp .env.example .env
```

3. Update `.env` with your configuration:
```env
VITE_API_BASE_URL=http://localhost:3000/api
VITE_APP_ENV=development
```

### Development

Start the development server:
```bash
bun run dev
```

The app will be available at `http://localhost:5173/`

### Available Scripts

- `bun run dev` - Start development server
- `bun run build` - Build for production
- `bun run preview` - Preview production build
- `bun run lint` - Run ESLint
- `bun run lint:fix` - Fix ESLint errors
- `bun run format` - Format code with Prettier
- `bun run format:check` - Check code formatting

> **Note**: Bun is significantly faster than npm for installing dependencies and running scripts. If you prefer npm, it will still work, but Bun is recommended for better performance.

## Features

### API Client

The API client (`src/services/api.ts`) includes:
- Automatic authentication token injection
- 401 response handling with automatic logout
- Request/response interceptors
- Configurable timeout (10s default)

### Error Boundary

Global error boundary catches React errors and displays a user-friendly error screen with:
- Error message display
- Page reload functionality
- Error details (in development)

### Routing

React Router v6 is configured for client-side routing. Routes are defined in `App.tsx`.

## Environment Variables

**SECURITY**: Never commit `.env` files to version control. Only `.env.example` should be committed.

Required variables:
- `VITE_API_BASE_URL` - Backend API base URL
- `VITE_APP_ENV` - Environment (development/production)

## Code Quality

### ESLint

ESLint is configured with:
- TypeScript support
- React hooks rules
- React refresh rules
- Prettier integration

### Prettier

Prettier ensures consistent code formatting across the project.

## Security

- Environment variables are properly gitignored
- API tokens are stored in localStorage (to be replaced with httpOnly cookies)
- CSRF protection to be implemented
- Input validation on all forms
- XSS prevention via React's built-in escaping

See `SECURITY.md` in the project root for detailed security guidelines.

## Contributing

1. Follow the existing code style
2. Run `bun run lint:fix` before committing
3. Run `bun run format` to ensure consistent formatting
4. Test your changes thoroughly
5. Update documentation as needed
