# Money Matters - Implementation Plan

## 📍 Current Status (Updated: 2025-11-30)

### Phase Overview
- ✅ **Phase 1**: Foundation & Infrastructure - **90% Complete**
- 🚧 **Phase 2**: Database & Domain Model - **IN PROGRESS** (Critical Path)
- ⏳ **Phase 3**: Backend API Implementation - **Blocked** (waiting on Phase 2)
- ⏳ **Phase 4**: Frontend - Design System & Components - **Partial** (40% complete)
- ⏳ **Phase 5-10**: Not started

### 🎯 Current Priority: **Phase 2 - Database Foundation**

**Active Work:**
- Issue #41: Setup Local PostgreSQL Development Environment (IMMEDIATE)
- Issue #42: Verify Database Schema and Migrations (NEXT)

### 🔥 Critical Path to Backend Development

The following tasks MUST be completed in order before backend API development can begin:

1. **#41: Setup Local PostgreSQL** ← **START HERE**
   - Docker Compose configuration
   - Local database environment
   - Connection string configuration
   - Development setup documentation

2. **#42: Verify Database Schema**
   - Run migrations against local PostgreSQL
   - Verify all tables, indexes, constraints
   - Test seed data
   - Validate schema integrity

3. **#7: Repository Pattern & Data Access**
   - Implement repository interfaces
   - Implement repository classes
   - Unit of work pattern
   - Database indexes

4. **#8-15: Backend API Endpoints** (Can start in parallel once #7 is done)
   - Accounts, Transactions, Bills, Income, Goals, Alerts, Dashboard, Settings

**Blocking:** All Phase 3 backend work (Issues #8-17) is blocked until Phase 2 is complete.

### ✅ Completed Work

**Phase 1 (Foundation):**
- ✅ Issue #2, #40: .NET 10 Backend Foundation
- ✅ Issue #3: React Frontend Foundation with Bun
- ✅ Issue #20: Form Components (React Hook Form + Zod + Storybook)
- ✅ Issue #30: Backend Testing Suite Setup

**Phase 2 (Database):**
- ✅ Issue #6: Database Schema Design & Migrations

### 📋 Next Phase After Critical Path

Once Phase 2 is complete, the immediate next steps are:
1. Phase 3.1-3.8: Build all API endpoints (parallel work possible)
2. Phase 3.9-3.10: Forecast and Alert engines (depends on API endpoints)
3. Phase 4: Complete remaining UI components (can work in parallel)
4. Phase 5: Wire up frontend screens to APIs

### 📊 Progress Summary

| Phase | Status | Completion | Blocking Issues |
|-------|--------|-----------|-----------------|
| Phase 1 | 🟢 90% | Backend/Frontend foundation done | #1, #4, #5 remaining |
| Phase 2 | 🟡 50% | Schema done, need local DB | #41, #42, #7 |
| Phase 3 | 🔴 0% | Blocked by Phase 2 | All issues blocked |
| Phase 4 | 🟡 40% | Forms done, need data/feedback | #18, #19 |
| Phase 5-10 | ⏳ 0% | Not started | N/A |

---

## Project Overview

**Money Matters** is a personal and business financial forecasting and alerting system. A cash-flow intelligence dashboard for entrepreneurs.

### Tech Stack

- **Frontend**: React (TypeScript) with world-class architecture
- **Backend**: .NET 10 (C#) with clean architecture
- **Database**: PostgreSQL
- **Hosting**: Azure (App Service, Azure Database for PostgreSQL)
- **Repository**: Public GitHub repository

### 🔒 Security First

**CRITICAL: This repository is PUBLIC. Never commit secrets, credentials, or sensitive data.**

All secrets MUST be stored in:
- **Azure Key Vault** for production secrets
- **Local environment variables** for development (never committed)
- **GitHub Secrets** for CI/CD workflows

See SECURITY.md for complete security guidelines.

---

## Phase 1: Foundation & Infrastructure

### 1.1 Repository & Project Setup

- [x] Initialize git repository
- [ ] Create GitHub repository (public)
- [ ] Setup .gitignore for .NET and React
- [ ] Create basic README.md
- [ ] Setup branch protection rules
- [ ] Configure GitHub Actions workflows

### 1.2 Backend Foundation (.NET) ✅ **COMPLETE** (Issue #2, #40)

- [x] Create .NET 10 solution structure
  - MoneyMatters.Api (Web API)
  - MoneyMatters.Core (Domain models, interfaces)
  - MoneyMatters.Infrastructure (Data access, external services)
  - MoneyMatters.Application (Business logic, CQRS)
- [x] Setup Entity Framework Core with PostgreSQL
- [x] Configure dependency injection
- [x] Setup Swagger/OpenAPI
- [x] Implement basic health check endpoint
- [x] Configure CORS for frontend
- [x] Setup logging (Serilog)
- [x] Configure app settings for multiple environments
- [ ] **SECURITY**: Create appsettings.example.json (safe to commit) and add appsettings.*.json to .gitignore (Part of #41)

### 1.3 Frontend Foundation (React) ✅ **COMPLETE** (Issue #3)

- [x] Create React app with Vite + TypeScript
- [x] Setup folder structure (components, hooks, services, types, utils)
- [x] Configure ESLint and Prettier
- [x] Setup Tailwind CSS for styling
- [x] Configure React Router v6
- [x] Setup API client (Axios with interceptors)
- [x] Implement error boundary
- [x] Setup environment variables (.env)
- [x] **SECURITY**: Create .env.example (safe to commit) - never commit actual .env files
- [x] **SECURITY**: Verify .env* is in .gitignore (except .env.example)
- [x] **BONUS**: Migrated to Bun for package management

### 1.4 Design System Implementation 🚧 **PARTIAL** (Issue #4)

- [ ] Create design tokens file (colors, typography, spacing, shadows)
- [ ] Implement theme provider (light/dark mode)
- [x] Build base UI components: (Issue #20 - Form Components)
  - [x] Button (primary, secondary, ghost, danger)
  - [x] TextField, NumberField, Select
  - [x] DatePicker
  - [ ] Toggle/Switch
  - [ ] SegmentedControl
  - [ ] Badge
  - [ ] Chip/Tag
- [ ] Create layout components:
  - [ ] AppShell
  - [ ] PageLayout
  - [ ] Section

### 1.5 Azure Infrastructure Setup

- [ ] Create Azure Resource Group
- [ ] Provision Azure Database for PostgreSQL
- [ ] Create App Service for .NET API
- [ ] Create Static Web App for React frontend
- [ ] Configure Application Insights
- [ ] Setup Azure Key Vault for secrets
- [ ] Configure connection strings and environment variables
- [ ] **SECURITY**: Store ALL secrets in Azure Key Vault (DB passwords, API keys, connection strings)
- [ ] **SECURITY**: Configure App Service to reference Key Vault secrets
- [ ] **SECURITY**: Enable Managed Identity for App Service to access Key Vault
- [ ] **SECURITY**: Never store secrets in appsettings.json or environment variable files in repo

---

## Phase 2: Database & Domain Model 🚧 **IN PROGRESS - CRITICAL PATH**

### 2.1 Database Schema Design ✅ **COMPLETE** (Issue #6)

- [x] Design and document database schema
- [x] Create entity models:
  - Account (bank accounts)
  - Transaction
  - Bill (recurring obligations)
  - IncomeStream
  - Goal (savings goals)
  - Alert
  - User/Settings
- [x] Create EF Core migrations
- [x] Setup seed data for development

### 2.1.1 Local Database Setup ⏳ **NEXT** (Issue #41 - IMMEDIATE PRIORITY)

- [ ] Create `docker-compose.yml` with PostgreSQL 15+ service
- [ ] Configure PostgreSQL with appropriate settings for development
- [ ] Create `appsettings.Development.json` with local connection string
- [ ] Create `appsettings.example.json` template (safe to commit)
- [ ] Add database management scripts (db:start, db:stop, db:reset)
- [ ] Update `docs/quick-start.md` with database setup instructions
- [ ] Test database connection from .NET API
- [ ] **SECURITY**: Ensure no credentials committed to repository

### 2.1.2 Database Verification ⏳ **BLOCKED BY #41** (Issue #42)

- [ ] Run database migrations against local PostgreSQL
- [ ] Verify all 10 tables are created correctly
- [ ] Verify all indexes are created
- [ ] Verify all foreign key constraints are working
- [ ] Verify unique constraints are enforced
- [ ] Run seed data script and verify data populates correctly
- [ ] Test cascading deletes work as expected
- [ ] Verify decimal precision for monetary fields
- [ ] Manually test basic CRUD operations on each table
- [ ] Document any schema issues or adjustments needed
- [ ] Create database diagram/ERD for documentation

### 2.2 Repository Pattern & Data Access ⏳ **BLOCKED BY #42** (Issue #7)

- [ ] Implement repository interfaces in Core
- [ ] Implement repository classes in Infrastructure
- [ ] Create unit of work pattern
- [ ] Add database indexes for performance
- [ ] Implement soft delete pattern

---

## Phase 3: Backend API Implementation

### 3.1 Core API Endpoints - Accounts

- [ ] GET /api/accounts - List all accounts
- [ ] GET /api/accounts/{id} - Get account details
- [ ] POST /api/accounts - Create account
- [ ] PATCH /api/accounts/{id} - Update account
- [ ] DELETE /api/accounts/{id} - Delete account
- [ ] GET /api/accounts/{id}/transactions - Get account transactions

### 3.2 Core API Endpoints - Transactions

- [ ] GET /api/transactions - List transactions with filters
- [ ] GET /api/transactions/{id} - Get transaction details
- [ ] POST /api/transactions - Create transaction
- [ ] PATCH /api/transactions/{id} - Update/tag transaction
- [ ] DELETE /api/transactions/{id} - Delete transaction

### 3.3 Core API Endpoints - Bills

- [ ] GET /api/bills - List all bills
- [ ] GET /api/bills/{id} - Get bill details
- [ ] GET /api/bills/upcoming?days=30 - Get upcoming bills
- [ ] POST /api/bills - Create bill
- [ ] PATCH /api/bills/{id} - Update bill
- [ ] DELETE /api/bills/{id} - Delete bill

### 3.4 Core API Endpoints - Income Streams

- [ ] GET /api/income-streams - List income streams
- [ ] GET /api/income-streams/{id} - Get income stream details
- [ ] POST /api/income-streams - Create income stream
- [ ] PATCH /api/income-streams/{id} - Update income stream
- [ ] DELETE /api/income-streams/{id} - Delete income stream

### 3.5 Core API Endpoints - Goals

- [ ] GET /api/goals - List all goals
- [ ] GET /api/goals/{id} - Get goal details
- [ ] GET /api/goals/summary - Get goals summary for dashboard
- [ ] POST /api/goals - Create goal
- [ ] PATCH /api/goals/{id} - Update goal
- [ ] DELETE /api/goals/{id} - Delete goal

### 3.6 Core API Endpoints - Alerts

- [ ] GET /api/alerts - List alerts with filters
- [ ] GET /api/alerts/{id} - Get alert details
- [ ] PATCH /api/alerts/{id} - Update alert state (acknowledge, snooze, resolve)

### 3.7 Core API Endpoints - Dashboard

- [ ] GET /api/dashboard/summary - Get dashboard summary
- [ ] GET /api/forecasts/current - Get forecast data with filters

### 3.8 Core API Endpoints - Settings

- [ ] GET /api/settings - Get user settings
- [ ] PATCH /api/settings - Update settings

### 3.9 Business Logic - Forecast Engine

- [ ] Implement cash flow forecasting algorithm
- [ ] Calculate runway days
- [ ] Determine bill coverage status
- [ ] Project goal completion dates
- [ ] Calculate personal vs business metrics

### 3.10 Business Logic - Alert Engine

- [ ] Implement alert generation logic:
  - Cash shortfall alerts
  - Bill risk alerts (uncovered bills)
  - Income delayed alerts
  - Goal risk alerts (behind schedule)
  - Overspend alerts
- [ ] Implement alert resolution logic
- [ ] Alert severity calculation

### 3.11 Validation & Error Handling

- [ ] Implement FluentValidation for all DTOs
- [ ] Create custom exception types
- [ ] Implement global exception handler middleware
- [ ] Add validation for business rules

---

## Phase 4: Frontend - Design System & Components

### 4.1 Data Display Components

- [ ] StatCard component
- [ ] InfoCard component
- [ ] DataTable component (with sorting, pagination)
- [ ] ListItem component
- [ ] Chart components (using recharts or chart.js):
  - LineChart (for forecasts)
  - BarChart (for spending)
  - DonutChart (for goal progress)

### 4.2 Feedback Components

- [ ] AlertBanner component
- [ ] Toast/Snackbar component
- [ ] Modal/Dialog component
- [ ] EmptyState component
- [ ] Loading skeleton components

### 4.3 Form Components ✅ **COMPLETE** (Issue #20)

- [x] Form wrapper with validation (React Hook Form + Zod)
- [x] TextField with validation
- [x] NumberField with validation
- [x] Select with validation
- [x] DatePicker with validation
- [x] All form components with Storybook stories
- [ ] SearchInput component
- [ ] File upload component (if needed)

---

## Phase 5: Frontend - Feature Implementation

### 5.1 Dashboard Screen

- [ ] Create Dashboard page component
- [ ] Implement Status Header section
- [ ] Implement Forecast Chart section
- [ ] Implement Upcoming Bills section
- [ ] Implement Goals Panel section
- [ ] Implement Alerts Panel section
- [ ] Wire up all dashboard API endpoints
- [ ] Implement refresh functionality
- [ ] Add loading and error states

### 5.2 Bills Screen

- [ ] Create Bills list page
- [ ] Implement bills filters (domain, status, priority)
- [ ] Create BillDetail view
- [ ] Create Add/Edit Bill form
- [ ] Implement bill payment history timeline
- [ ] Wire up bills API endpoints
- [ ] Add loading, empty, and error states

### 5.3 Income Streams Screen

- [ ] Create Income Streams list page
- [ ] Create IncomeStreamDetail view
- [ ] Create Add/Edit Income Stream form
- [ ] Show historical deposits and variance
- [ ] Wire up income streams API endpoints
- [ ] Add loading, empty, and error states

### 5.4 Goals Screen

- [ ] Create Goals list page (card view)
- [ ] Create GoalDetail view with progress chart
- [ ] Create Add/Edit Goal form
- [ ] Show contributions history
- [ ] Display recommended adjustments
- [ ] Wire up goals API endpoints
- [ ] Add loading, empty, and error states

### 5.5 Accounts Screen

- [ ] Create Accounts list page
- [ ] Create AccountDetail view with balance chart
- [ ] Implement toggle for includeInForecast
- [ ] Show latest transactions per account
- [ ] Wire up accounts API endpoints
- [ ] Add loading, empty, and error states

### 5.6 Transactions Screen

- [ ] Create Transactions list page with filters
- [ ] Implement transaction filtering (account, date, category, amount)
- [ ] Create transaction detail panel
- [ ] Implement transaction tagging (bill, income, goal, transfer)
- [ ] Wire up transactions API endpoints
- [ ] Add loading, empty, and error states

### 5.7 Alerts Center Screen

- [ ] Create Alerts list page grouped by type/severity
- [ ] Create AlertDetail view
- [ ] Show alert trigger conditions and recommendations
- [ ] Implement acknowledge/snooze/resolve actions
- [ ] Wire up alerts API endpoints
- [ ] Add loading, empty, and error states

### 5.8 Settings Screen

- [ ] Create Settings page
- [ ] Time horizon selection
- [ ] Alert preferences configuration
- [ ] Safe minimum balances editor
- [ ] Domain configuration (personal/business)
- [ ] Profile and timezone settings
- [ ] Wire up settings API endpoints

---

## Phase 6: Navigation & User Experience

### 6.1 Navigation Implementation

- [ ] Implement top navigation/header
- [ ] Implement desktop sidebar navigation
- [ ] Implement mobile bottom navigation
- [ ] Add navigation icons
- [ ] Implement active route highlighting
- [ ] Add user menu with profile/settings/logout

### 6.2 Responsive Design

- [ ] Test all screens at mobile breakpoint
- [ ] Test all screens at tablet breakpoint
- [ ] Test all screens at desktop breakpoint
- [ ] Implement mobile-first responsive tables
- [ ] Optimize charts for mobile
- [ ] Ensure touch targets are appropriate size

### 6.3 Theme & Dark Mode

- [ ] Implement theme toggle in header
- [ ] Ensure all components support dark mode
- [ ] Test color contrast ratios
- [ ] Persist theme preference to localStorage
- [ ] Add smooth theme transitions

---

## Phase 7: Testing

### 7.1 Backend Testing ✅ **SETUP COMPLETE** (Issue #30)

- [x] Setup xUnit test project
- [x] Setup test infrastructure and patterns
- [ ] Write unit tests for business logic (pending business logic implementation)
- [ ] Write unit tests for forecast engine
- [ ] Write unit tests for alert engine
- [ ] Write integration tests for API endpoints
- [ ] Setup test database
- [ ] Mock external dependencies
- [ ] Achieve 80%+ code coverage

### 7.2 Frontend Testing

- [ ] Setup Vitest + React Testing Library
- [ ] Write component tests for UI components
- [ ] Write integration tests for screens
- [ ] Test form validation
- [ ] Test API error handling
- [ ] Test responsive behavior
- [ ] Achieve 70%+ code coverage

### 7.3 E2E Testing

- [ ] Setup Playwright or Cypress
- [ ] Write E2E tests for critical user flows:
  - Dashboard loading and interaction
  - Creating a bill
  - Creating a goal
  - Viewing transactions
  - Resolving alerts

---

## Phase 8: Security & Performance

### 8.1 Authentication & Authorization

- [ ] Implement JWT authentication
- [ ] Setup Azure AD B2C (or alternative)
- [ ] Implement login/logout flow
- [ ] Add auth middleware to API
- [ ] Protect frontend routes
- [ ] Implement token refresh
- [ ] Add role-based access control (if needed)

### 8.2 Security Hardening

- [ ] Enable HTTPS only
- [ ] Implement rate limiting
- [ ] Add request validation
- [ ] Sanitize user inputs
- [ ] Implement CSRF protection
- [ ] Add security headers
- [ ] Setup Azure Key Vault integration
- [ ] Implement secret rotation
- [ ] **SECURITY**: Audit repository for any committed secrets (use git-secrets or similar)
- [ ] **SECURITY**: Setup pre-commit hooks to prevent secret commits
- [ ] **SECURITY**: Review all PRs for hardcoded credentials
- [ ] **SECURITY**: Enable GitHub secret scanning
- [ ] **SECURITY**: Document secret management workflow in SECURITY.md

### 8.3 Performance Optimization

- [ ] Implement database query optimization
- [ ] Add database indexes
- [ ] Implement API response caching
- [ ] Add Redis cache (if needed)
- [ ] Optimize bundle size (code splitting)
- [ ] Implement lazy loading for routes
- [ ] Optimize images and assets
- [ ] Add CDN for static assets
- [ ] Implement pagination for large lists

---

## Phase 9: DevOps & Deployment

### 9.1 CI/CD Pipeline

- [ ] Create GitHub Actions workflow for backend:
  - Build .NET solution
  - Run tests
  - Deploy to Azure App Service
- [ ] Create GitHub Actions workflow for frontend:
  - Build React app
  - Run tests
  - Deploy to Azure Static Web Apps
- [ ] Setup staging environment
- [ ] Setup production environment
- [ ] Implement deployment approvals

### 9.2 Monitoring & Logging

- [ ] Configure Application Insights
- [ ] Setup custom metrics and dashboards
- [ ] Configure alerts for errors
- [ ] Implement structured logging
- [ ] Add performance monitoring
- [ ] Setup log retention policies

### 9.3 Database Management

- [ ] Setup automated backups
- [ ] Configure point-in-time restore
- [ ] Implement database migration strategy
- [ ] Setup connection pooling
- [ ] Configure database scaling

---

## Phase 10: Documentation & Launch

### 10.1 Documentation

- [ ] Write comprehensive README
- [ ] Document API endpoints (OpenAPI/Swagger)
- [ ] Create architecture documentation
- [ ] Write deployment guide
- [ ] Create user guide/help docs
- [ ] Document environment variables
- [ ] Create contributing guidelines

### 10.2 Launch Preparation

- [ ] Final security audit
- [ ] Performance testing
- [ ] Load testing
- [ ] User acceptance testing
- [ ] Create rollback plan
- [ ] Setup support channels

### 10.3 Post-Launch

- [ ] Monitor error rates
- [ ] Monitor performance metrics
- [ ] Gather user feedback
- [ ] Create backlog for improvements
- [ ] Plan future iterations

---

## Technology Choices Detail

### Frontend Stack
- **React 18** with TypeScript
- **Vite** for build tooling
- **React Router v6** for routing
- **Tailwind CSS** for styling
- **Recharts** or **Chart.js** for data visualization
- **Axios** for API calls
- **React Query** (TanStack Query) for data fetching and caching
- **Zustand** or **Jotai** for state management (lightweight)
- **React Hook Form** for form handling
- **Zod** for schema validation
- **date-fns** for date manipulation
- **Vitest** + **React Testing Library** for testing

### Backend Stack
- **.NET 10** with C#
- **Entity Framework Core 10** for ORM
- **PostgreSQL 15+** for database
- **FluentValidation** for validation
- **MediatR** for CQRS pattern
- **AutoMapper** for object mapping
- **Serilog** for logging
- **xUnit** for testing
- **Swagger/OpenAPI** for API documentation

### Azure Services
- **Azure App Service** for .NET API hosting
- **Azure Static Web Apps** for React hosting
- **Azure Database for PostgreSQL - Flexible Server**
- **Azure Application Insights** for monitoring
- **Azure Key Vault** for secrets management
- **Azure CDN** for static asset delivery
- **Azure AD B2C** for authentication (optional)

---

## Estimated Timeline

- **Phase 1**: 1-2 weeks
- **Phase 2**: 1 week
- **Phase 3**: 2-3 weeks
- **Phase 4**: 1-2 weeks
- **Phase 5**: 3-4 weeks
- **Phase 6**: 1 week
- **Phase 7**: 2 weeks
- **Phase 8**: 2 weeks
- **Phase 9**: 1 week
- **Phase 10**: 1 week

**Total estimated time**: 15-20 weeks for full implementation

---

## Success Metrics

- All 8 screens fully functional
- 80%+ backend code coverage
- 70%+ frontend code coverage
- <2s page load time
- <500ms API response time (p95)
- 99.9% uptime
- Zero critical security vulnerabilities
- Mobile responsive on all screens
- WCAG 2.1 AA accessibility compliance

---

## Next Steps

1. Initialize git repository
2. Create GitHub repository
3. Create GitHub Issues for all tasks
4. Begin Phase 1: Foundation & Infrastructure

🤖 Submitted by George with love ♥
