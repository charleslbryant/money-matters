# Database Schema Design

This document defines the complete database schema for Money Matters, a cash-flow intelligence dashboard.

## Overview

The system separates **Personal** and **Business** finances. Core entities track accounts, transactions, recurring bills, income streams, savings goals, and system-generated alerts.

## Domain Types

### FinancialDomain (Enum)
- `Personal`
- `Business`

### BillFrequency (Enum)
- `Weekly`
- `BiWeekly`
- `Monthly`
- `Quarterly`
- `Annually`

### IncomeFrequency (Enum)
- `Weekly`
- `BiWeekly`
- `SemiMonthly`
- `Monthly`
- `Irregular`

### GoalFundingStrategy (Enum)
- `FixedAmount` - Contribute a fixed amount per period
- `PercentOfIncome` - Contribute percentage of income
- `Surplus` - Contribute excess after bills and minimums

### AlertType (Enum)
- `CashShortfall`
- `BillRisk`
- `IncomeDelayed`
- `GoalRisk`
- `LowBalance`

### AlertSeverity (Enum)
- `Info`
- `Warning`
- `Critical`

### AlertState (Enum)
- `New`
- `Acknowledged`
- `Snoozed`
- `Resolved`

### StatusIndicator (Enum)
- `Green` - Healthy
- `Yellow` - Warning
- `Red` - Critical

## Entities

### User

Represents the app user with settings and preferences.

**Properties:**
- `Id` (Guid, PK)
- `Email` (string, unique, required)
- `Name` (string, required)
- `TimeZone` (string, required, default: "America/New_York")
- `DefaultForecastHorizonDays` (int, required, default: 30)
- `CreatedAt` (DateTime, required)
- `UpdatedAt` (DateTime, required)

**Relationships:**
- Has many Accounts
- Has many Bills
- Has many IncomeStreams
- Has many Goals
- Has many Settings
- Has many Alerts

**Indexes:**
- Unique index on Email

---

### Account

Represents a financial account (checking, savings, credit card, etc.).

**Properties:**
- `Id` (Guid, PK)
- `UserId` (Guid, FK to User, required)
- `Name` (string, required) - e.g., "Chase Checking"
- `Institution` (string, nullable) - e.g., "Chase Bank"
- `AccountType` (string, required) - e.g., "Checking", "Savings", "Credit Card"
- `Domain` (FinancialDomain, required) - Personal or Business
- `CurrentBalance` (decimal(18,2), required)
- `SafeMinimumBalance` (decimal(18,2), required, default: 0)
- `IncludeInForecast` (bool, required, default: true)
- `IsActive` (bool, required, default: true)
- `ExternalAccountId` (string, nullable) - ID from bank aggregator
- `LastSyncedAt` (DateTime, nullable)
- `CreatedAt` (DateTime, required)
- `UpdatedAt` (DateTime, required)

**Relationships:**
- Belongs to User
- Has many Transactions
- Has many Bills (as DefaultAccount)
- Has many GoalAccounts

**Indexes:**
- Index on UserId
- Index on Domain
- Index on IsActive

---

### Transaction

Represents individual financial transactions.

**Properties:**
- `Id` (Guid, PK)
- `AccountId` (Guid, FK to Account, required)
- `Amount` (decimal(18,2), required) - Positive for income, negative for expenses
- `Date` (DateTime, required)
- `Description` (string, required)
- `NormalizedMerchant` (string, nullable) - Cleaned merchant name
- `Category` (string, nullable)
- `IsReconciled` (bool, required, default: false)
- `BillId` (Guid, FK to Bill, nullable) - If transaction pays a bill
- `IncomeStreamId` (Guid, FK to IncomeStream, nullable) - If transaction is income
- `GoalId` (Guid, FK to Goal, nullable) - If transaction is a goal contribution
- `TransferAccountId` (Guid, FK to Account, nullable) - If transaction is a transfer
- `Notes` (string, nullable)
- `ExternalTransactionId` (string, nullable) - ID from bank aggregator
- `CreatedAt` (DateTime, required)
- `UpdatedAt` (DateTime, required)

**Relationships:**
- Belongs to Account
- Optionally belongs to Bill
- Optionally belongs to IncomeStream
- Optionally belongs to Goal
- Optionally links to another Account (transfers)

**Indexes:**
- Index on AccountId
- Index on Date (DESC)
- Index on BillId
- Index on IncomeStreamId
- Index on GoalId

---

### Bill

Represents recurring financial obligations.

**Properties:**
- `Id` (Guid, PK)
- `UserId` (Guid, FK to User, required)
- `Name` (string, required) - e.g., "Electric Bill", "Rent"
- `Amount` (decimal(18,2), required) - Typical/expected amount
- `Frequency` (BillFrequency, required)
- `DayOfMonth` (int, nullable) - For monthly bills (1-31)
- `DayOfWeek` (int, nullable) - For weekly bills (0=Sunday, 6=Saturday)
- `NextDueDate` (DateTime, required)
- `Domain` (FinancialDomain, required)
- `DefaultAccountId` (Guid, FK to Account, nullable) - Preferred payment account
- `Priority` (int, required, default: 5) - 1 (highest) to 10 (lowest)
- `IsAutoPay` (bool, required, default: false)
- `IsActive` (bool, required, default: true)
- `Notes` (string, nullable)
- `CreatedAt` (DateTime, required)
- `UpdatedAt` (DateTime, required)

**Relationships:**
- Belongs to User
- Optionally belongs to DefaultAccount
- Has many Transactions
- Has many BillPayments (calculated from transactions)

**Indexes:**
- Index on UserId
- Index on Domain
- Index on NextDueDate
- Index on IsActive

---

### IncomeStream

Represents expected income sources.

**Properties:**
- `Id` (Guid, PK)
- `UserId` (Guid, FK to User, required)
- `Name` (string, required) - e.g., "Salary", "Client ABC Payment"
- `TypicalAmount` (decimal(18,2), required)
- `Frequency` (IncomeFrequency, required)
- `Domain` (FinancialDomain, required)
- `AccountId` (Guid, FK to Account, required) - Where income is deposited
- `LastReceivedDate` (DateTime, nullable)
- `LastReceivedAmount` (decimal(18,2), nullable)
- `NextExpectedDate` (DateTime, nullable)
- `NextExpectedWindowStart` (DateTime, nullable) - Expected date range start
- `NextExpectedWindowEnd` (DateTime, nullable) - Expected date range end
- `IsActive` (bool, required, default: true)
- `Notes` (string, nullable)
- `CreatedAt` (DateTime, required)
- `UpdatedAt` (DateTime, required)

**Relationships:**
- Belongs to User
- Belongs to Account
- Has many Transactions

**Indexes:**
- Index on UserId
- Index on Domain
- Index on NextExpectedDate
- Index on IsActive

---

### Goal

Represents savings goals and planned purchases.

**Properties:**
- `Id` (Guid, PK)
- `UserId` (Guid, FK to User, required)
- `Name` (string, required) - e.g., "Emergency Fund", "New Laptop"
- `TargetAmount` (decimal(18,2), required)
- `CurrentAmount` (decimal(18,2), required, default: 0)
- `TargetDate` (DateTime, required)
- `Domain` (FinancialDomain, required)
- `FundingStrategy` (GoalFundingStrategy, required)
- `FixedContributionAmount` (decimal(18,2), nullable) - For FixedAmount strategy
- `PercentOfIncome` (decimal(5,2), nullable) - For PercentOfIncome strategy (0-100)
- `Priority` (int, required, default: 5) - 1 (highest) to 10 (lowest)
- `IsActive` (bool, required, default: true)
- `Notes` (string, nullable)
- `CreatedAt` (DateTime, required)
- `UpdatedAt` (DateTime, required)

**Relationships:**
- Belongs to User
- Has many GoalAccounts (source accounts)
- Has many Transactions (contributions)

**Indexes:**
- Index on UserId
- Index on Domain
- Index on TargetDate
- Index on IsActive

---

### GoalAccount

Join table linking Goals to source Accounts.

**Properties:**
- `Id` (Guid, PK)
- `GoalId` (Guid, FK to Goal, required)
- `AccountId` (Guid, FK to Account, required)
- `CreatedAt` (DateTime, required)

**Relationships:**
- Belongs to Goal
- Belongs to Account

**Indexes:**
- Unique composite index on (GoalId, AccountId)
- Index on GoalId
- Index on AccountId

---

### Alert

System-generated alerts for cash shortfalls, bill risks, etc.

**Properties:**
- `Id` (Guid, PK)
- `UserId` (Guid, FK to User, required)
- `Type` (AlertType, required)
- `Severity` (AlertSeverity, required)
- `State` (AlertState, required, default: New)
- `Title` (string, required) - e.g., "Cash shortfall projected"
- `Message` (string, required) - Detailed explanation
- `RecommendedAction` (string, nullable) - Suggested next steps
- `Domain` (FinancialDomain, nullable) - If alert is specific to Personal/Business
- `RelatedAccountId` (Guid, FK to Account, nullable)
- `RelatedBillId` (Guid, FK to Bill, nullable)
- `RelatedGoalId` (Guid, FK to Goal, nullable)
- `RelatedIncomeStreamId` (Guid, FK to IncomeStream, nullable)
- `TriggeredAt` (DateTime, required)
- `AcknowledgedAt` (DateTime, nullable)
- `SnoozedUntil` (DateTime, nullable)
- `ResolvedAt` (DateTime, nullable)
- `ExpiresAt` (DateTime, nullable) - Auto-resolve after this date
- `CreatedAt` (DateTime, required)
- `UpdatedAt` (DateTime, required)

**Relationships:**
- Belongs to User
- Optionally relates to Account
- Optionally relates to Bill
- Optionally relates to Goal
- Optionally relates to IncomeStream

**Indexes:**
- Index on UserId
- Index on Type
- Index on Severity
- Index on State
- Index on TriggeredAt (DESC)

---

### ForecastSnapshot

Stores cached forecast calculations for performance.

**Properties:**
- `Id` (Guid, PK)
- `UserId` (Guid, FK to User, required)
- `Domain` (FinancialDomain, required)
- `HorizonDays` (int, required) - 30, 60, or 90
- `GeneratedAt` (DateTime, required)
- `StartDate` (DateTime, required)
- `EndDate` (DateTime, required)
- `ForecastData` (string, required) - JSON blob with daily projections
- `RunwayDays` (int, nullable) - Days before cash shortfall
- `Status` (StatusIndicator, required)
- `CreatedAt` (DateTime, required)

**Relationships:**
- Belongs to User

**Indexes:**
- Index on UserId
- Index on Domain
- Composite index on (UserId, Domain, HorizonDays, GeneratedAt DESC)

**Notes:**
- ForecastData JSON structure:
```json
{
  "dailyProjections": [
    {
      "date": "2025-12-01",
      "projectedBalance": 5000.00,
      "inflows": 3000.00,
      "outflows": 1500.00,
      "billsCovered": ["bill-id-1", "bill-id-2"]
    }
  ]
}
```

---

### Settings

User-specific application settings and preferences.

**Properties:**
- `Id` (Guid, PK)
- `UserId` (Guid, FK to User, required)
- `SettingKey` (string, required) - e.g., "AlertThreshold.CashShortfall"
- `SettingValue` (string, required) - JSON or string value
- `CreatedAt` (DateTime, required)
- `UpdatedAt` (DateTime, required)

**Relationships:**
- Belongs to User

**Indexes:**
- Unique composite index on (UserId, SettingKey)

**Common Settings Keys:**
- `AlertThreshold.CashShortfall` - Number of days ahead to warn
- `AlertThreshold.BillRisk` - Days before due date to warn
- `AlertChannels.Email` - Boolean for email alerts
- `AlertChannels.InApp` - Boolean for in-app alerts
- `DashboardRefreshTime` - Time of day for auto refresh

---

## Relationships Summary

```
User
├── Accounts (1:many)
├── Bills (1:many)
├── IncomeStreams (1:many)
├── Goals (1:many)
├── Alerts (1:many)
├── ForecastSnapshots (1:many)
└── Settings (1:many)

Account
├── User (many:1)
├── Transactions (1:many)
├── Bills (1:many) - as DefaultAccount
├── IncomeStreams (1:many)
└── GoalAccounts (1:many)

Transaction
├── Account (many:1)
├── Bill (many:1, optional)
├── IncomeStream (many:1, optional)
├── Goal (many:1, optional)
└── TransferAccount (many:1, optional)

Bill
├── User (many:1)
├── DefaultAccount (many:1, optional)
└── Transactions (1:many)

IncomeStream
├── User (many:1)
├── Account (many:1)
└── Transactions (1:many)

Goal
├── User (many:1)
├── GoalAccounts (1:many)
└── Transactions (1:many)

GoalAccount
├── Goal (many:1)
└── Account (many:1)

Alert
├── User (many:1)
├── Account (many:1, optional)
├── Bill (many:1, optional)
├── Goal (many:1, optional)
└── IncomeStream (many:1, optional)

ForecastSnapshot
└── User (many:1)

Settings
└── User (many:1)
```

## Database Constraints

### Unique Constraints
- User.Email (unique)
- GoalAccount (GoalId, AccountId) composite unique
- Settings (UserId, SettingKey) composite unique

### Check Constraints
- Account.CurrentBalance: Can be negative (credit cards, overdrafts)
- Account.SafeMinimumBalance >= 0
- Bill.Amount > 0
- Bill.DayOfMonth: 1-31 (if not null)
- Bill.DayOfWeek: 0-6 (if not null)
- Bill.Priority: 1-10
- IncomeStream.TypicalAmount > 0
- Goal.TargetAmount > 0
- Goal.CurrentAmount >= 0
- Goal.PercentOfIncome: 0-100 (if not null)
- Goal.Priority: 1-10
- User.DefaultForecastHorizonDays: 30, 60, or 90

### Cascade Deletes
- User deleted → cascade delete all owned entities (Accounts, Bills, etc.)
- Account deleted → cascade delete Transactions
- Bill deleted → set BillId to null on Transactions
- Goal deleted → cascade delete GoalAccounts and set GoalId to null on Transactions
- IncomeStream deleted → set IncomeStreamId to null on Transactions

## Performance Indexes

### High Priority Indexes
1. Transaction.AccountId, Transaction.Date DESC (for account ledgers)
2. Bill.UserId, Bill.NextDueDate (for upcoming bills queries)
3. Alert.UserId, Alert.State, Alert.Severity (for active alerts)
4. ForecastSnapshot (UserId, Domain, HorizonDays, GeneratedAt DESC) (for latest forecasts)

### Medium Priority Indexes
5. Transaction.BillId (for bill payment history)
6. Transaction.GoalId (for goal contribution history)
7. IncomeStream.NextExpectedDate (for overdue income detection)
8. Goal.UserId, Goal.TargetDate (for goals dashboard)

## Data Volume Estimates

Assuming single-user development database:

- Users: 1-10
- Accounts: 5-15 per user
- Transactions: 100-500 per month per user
- Bills: 10-30 per user
- IncomeStreams: 1-5 per user
- Goals: 3-10 per user
- Alerts: 10-50 active at any time
- ForecastSnapshots: 6-20 (cached per domain/horizon combination)
- Settings: 10-20 per user

## Seed Data Requirements

For development, seed the following:

1. **One User**
   - Email: dev@moneym matters.local
   - Name: Development User

2. **Accounts** (Personal and Business)
   - Personal Checking ($5,000)
   - Personal Savings ($15,000)
   - Business Checking ($25,000)
   - Business Savings ($50,000)
   - Credit Card (-$2,500)

3. **Bills** (mix of Personal and Business)
   - Rent ($2,000, monthly)
   - Electric ($150, monthly)
   - Internet ($80, monthly)
   - Phone ($60, monthly)
   - SaaS subscriptions ($200, monthly)
   - Office rent (business, $1,500, monthly)

4. **Income Streams**
   - Salary (personal, $6,000, semi-monthly)
   - Client revenue (business, $10,000, irregular)

5. **Goals**
   - Emergency fund (personal, $20,000 target)
   - New laptop (business, $3,000 target, 90 days)
   - Vacation (personal, $5,000 target, 180 days)

6. **Transactions** (last 90 days)
   - Income deposits
   - Bill payments
   - Goal contributions
   - Miscellaneous expenses

7. **Settings**
   - Default alert preferences
   - Default forecast horizon (30 days)

## Migration Strategy

1. **Migration 1: Initial Schema**
   - Create all tables
   - Add all columns with proper types
   - Define primary keys

2. **Migration 2: Relationships and Constraints**
   - Add foreign keys
   - Add unique constraints
   - Add check constraints

3. **Migration 3: Indexes**
   - Add performance indexes
   - Add unique indexes

4. **Migration 4: Seed Data**
   - Insert development seed data

## Notes

- All monetary values use `decimal(18,2)` for precision
- All dates/times stored in UTC; display converted to user's timezone
- Soft deletes not implemented initially (use IsActive flags where needed)
- Audit fields (CreatedAt, UpdatedAt) on all entities
- GUIDs used for all primary keys for distributed system compatibility

---

🤖 Submitted by George with love ♥
