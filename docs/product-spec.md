# App

Personal and business financial forecasting and alerting system. The purpose of this app is to monitor upcoming bills, predict cash shortfalls, track savings goals, and give me a daily status of my financial health across all accounts.

This is not a budgeting app. It's a cash-flow intelligence dashboard.

## Overall product purpose

A financial nervous system for an entrepreneur that:

-   Monitors cash across personal and business accounts
-   Tracks upcoming bills and income
-   Forecasts cash vs obligations and goals
-   Warns before cash shortfalls or missed goals
-   Tracks progress toward savings and planned purchases

## General Requirements

-   Clean, modern, minimal layout
-   Light and dark mode
-   Mobile-first responsive design
-   All data comes from an API (assume REST endpoints)
-   Support both personal and business financial views
-   No fluff — focus on clarity, forecasts, and decision support

# Screens

## 1. Dashboard (Home)

This is the operator's daily view. Include:

### Status Header

-   Personal status: green/yellow/red
-   Business status: green/yellow/red
-   Total personal cash
-   Total business cash
-   Days of runway before next projected shortfall

### Forecast Chart

-   Line graph of projected balances for next 30–90 days
-   Overlay upcoming bills
-   Mark safe-min balance line
-   Toggle: Personal / Business / Combined

### Upcoming Bills

Table or list:

-   Name
-   Due date
-   Amount
-   Covered or uncovered (fully / partially / not at all)
-   Source account

### Goals Panel

Card list of savings goals:

-   Goal name
-   Target amount + target date
-   Current amount
-   On track / behind / ahead indicator
-   Tap to open goal details

### Alerts Section

-   List active alerts grouped by severity
-   Tap an alert for full explanation and recommended actions

## 2. Bills & Obligations

A management screen for all recurring expenses.

Include:

-   List of bills with next due date, amount, frequency, priority
-   Filters: personal / business / all
-   Add/edit bill form
-   Bill detail view showing payment history and forecasted coverage

## 3. Income Streams

List and manage income sources.

Include:

-   Pay frequency
-   Expected amount
-   Last received
-   Next expected window
-   Late/missed indicator
-   Link to associated account

## 4. Goals

A complete goals module for savings and planned purchases.

Goal list view:

-   Goal cards sorted by urgency or date
-   Show progress ring or bar
-   Show projected completion date
-   Status (on track, behind, ahead)

Goal detail view:

-   Target amount + date
-   Contribution rules
-   Contributions history
-   Projection chart
-   Recommended adjustment if behind

Add/edit goal form:

-   Name, amount, date
-   Domain (personal/business)
-   Funding rule (fixed amount, percent of income, surplus)
-   Source accounts

## 5. Transactions

A simple ledger.

Include:

-   Filter by account, category, date
-   Show normalized merchant
-   Tagging support for goals, bills, transfers
-   Transaction detail panel

## 6. Accounts

Show all linked accounts:

-   Bank/institution name
-   Account type
-   Current balance
-   Safe minimum balance
-   Toggle whether account participates in forecast
-   Detail view with historical balance chart

## 7. Alerts Center

Full alerts management screen.

-   Grouped by type (shortfall, bill risk, income delayed, goal risk, overspend)
-   Each alert shows: – What triggered it – When it becomes a problem – Recommended action
-   Acknowledge, snooze, resolve

## 8. Settings

-   Set time horizon (30/60/90 days)
-   Edit safe minimum balances
-   Configure alert preferences
-   Personal vs business toggles
-   Manage connections to bank aggregator
-   Profile & timezone

# Components

## AppShell

-   Header with app name, high level status, user menu
-   Left sidebar for main navigation on desktop
-   Bottom nav bar for mobile

## PageLayout

-   Page title, description, primary actions
-   Regions for filters, main content, secondary panels

## Section

-   Section header with title and optional description
-   Inline actions (filters, view toggles)

## Data display

### StatCard

-   Label
-   Main value (number or currency)
-   Delta or trend indicator
-   Optional status (green, yellow, red)

### InfoCard

-   Title
-   Body content (text, small table, or list)
-   Optional footer actions

### DataTable

-   Sortable columns
-   Sticky header on desktop
-   Pagination or infinite scroll
-   Row selection
-   Empty state message

### ListItem

-   Leading icon or avatar
-   Title, description, metadata line (dates, amounts, tags)
-   Optional right-side value or status pill

### Badge

-   Variants: success, warning, danger, info, neutral
-   Used for statuses like Covered, Uncovered, On track, Behind

### Chip / Tag

-   For filters and labels (Personal, Business, Goal, Bill, etc.)

### Chart primitives

-   Line chart for balances and forecasts
-   Bar chart for category spending
-   Donut chart for goal progress
-   All with tooltips and legend

## Input

-   Button
-   Primary, secondary, ghost, danger
-   Sizes: default and small
-   Loading state
-   TextField
-   Label, placeholder, helper text, validation errors
-   NumberField
-   For amounts with currency suffix
-   Select
-   For categories, accounts, frequency

### DatePicker

-   For target dates and due dates

### Toggle / Switch

-   For feature and setting toggles
-   SegmentedControl
-   For time horizon (30, 60, 90 days) and view mode (Personal, Business, Combined)

### SearchInput

-   For transactions and lists
-   Optional filters dropdown

## Feedback

### AlertBanner

-   Types: info, success, warning, danger
-   Message plus optional description and action button

### Toast / Snackbar

-   Short confirmations or errors

### Modal / Dialog

-   For critical actions and forms
-   With title, description, content, primary and secondary buttons

### EmptyState

-   Used when there is no data configured yet
-   Shows icon, message, and primary action

# Interactions and Behavior

-   Dashboard refreshes daily at a configured time and supports manual refresh
-   Alerts automatically clear when the underlying issue is resolved
-   Trends should show color-coded deviations
-   Goals update as soon as new transactions are tagged
-   Transactions should show transfer linking
-   Forecast chart should animate smooth curves with clear markers

States for all async interactions:

-   loading: skeleton states for tables, charts, cards
-   empty: use EmptyState with primary action
-   error: AlertBanner with retry button
-   success: Toast after saves or updates

Key interactions:

-   Changing time horizon refetches forecast and dashboard summary
-   Toggling personal/business/combined refetches or reuses appropriate forecast data
-   Editing a bill or goal updates relevant dashboard sections immediately after success
-   Resolving an alert removes it from the active list and updates status indicators

# Style Direction

-   Clean, neutral colors with strong semantic color: red/yellow/green
-   Cards, tables, and charts — minimal but readable
-   Icons for bills, goals, income, business, personal
-   Animations subtle
-   Typography: modern sans (Inter, Roboto, or SF Pro)

# Design System

Define global tokens first. I want a clean, modern, neutral aesthetic.

## Color

Define base palette:

-   neutral: background, surface, border, subtle text, strong text
-   primary: for main actions and highlights
-   accent: optional, for charts and decorative visuals
-   semantic: success, warning, danger, info

Define semantic color tokens:

-   color-bg-body
-   color-bg-surface
-   color-bg-surface-alt
-   color-border-subtle
-   color-border-strong
-   color-text-main
-   color-text-muted
-   color-text-inverse
-   color-primary
-   color-primary-soft
-   color-success
-   color-warning
-   color-danger
-   color-info

Define variants for light mode and dark mode. Use the same semantic naming so I can theme swap without changing components.

## Typography

Define typographic tokens:

-   font-family-base
-   font-size-xs, sm, md, lg, xl, 2xl
-   font-weight-regular, medium, semibold
-   line-height-tight, normal, relaxed

Assign them to roles:

-   heading-page
-   heading-section
-   label
-   body
-   caption
-   data-number (for numeric values and KPIs)

## Spacing and layout

Define a simple spacing scale:

-   space-0, 4, 8, 12, 16, 24, 32, 40

Define radii:

-   radius-none
-   radius-sm
-   radius-md
-   radius-lg
-   radius-pill

Define shadows for surfaces:

-   shadow-subtle
-   shadow-card
-   shadow-popover

## Breakpoints

Define responsive breakpoints:

-   mobile
-   tablet
-   desktop

This app must be mobile first, then scale up.

# Information architecture

Use the following top-level navigation:

-   Dashboard
-   Bills
-   Income
-   Goals
-   Accounts
-   Transactions
-   Alerts
-   Settings

Map each area to components.

## Dashboard

### Status header

-   StatCards showing:
    -   Personal status, total cash, days of runway
    -   Business status, total cash, days of runway

### Forecast section

-   Line chart of balances for selected horizon
-   SegmentedControl to switch Personal / Business / Combined
-   Safe minimum balance line overlay

### Upcoming obligations section

-   DataTable or list of bills for next 30 days
-   Columns: name, due date, amount, coverage status, account
-   Badge for coverage status: Covered, Partially, Uncovered

### Goals section

-   Grid of goal cards using InfoCard or specialized GoalCard
-   Show progress bar, target amount and date, projected completion, status badge

### Alerts panel

-   List of high severity alerts using ListItem with Badge for severity
-   Tapping opens full alert detail view

## Bills

-   Table of all bills
-   Filters by domain (personal, business), status, priority
-   Row click opens BillDetail view
-   BillDetail uses InfoCard plus a small timeline of payment history and forecasted coverage

## Income

-   List of income streams
-   Columns: name, typical amount, frequency, last received, next expected window, status
-   Detail view shows historical deposits and variance

## Goals

-   Goals list as cards or table
-   Detail view with:
    -   InfoCard for goal meta
    -   Progress chart
    -   Contributions history list
    -   Recommended contribution if behind

## Accounts

-   Table of accounts with name, type, domain, current balance, safeMinBalance, includeInForecast toggle
-   Account detail with historical balance chart and list of latest transactions

## Transactions

-   Table with filters: account, date range, category, amount range, tags
-   Row detail allows tagging transaction as bill, income, goal contribution, transfer

## Alerts

-   List grouped by type and severity
-   Each alert detail shows:
    -   What condition triggered it
    -   Time window
    -   Amounts and accounts involved
    -   Suggested actions
-   Buttons to acknowledge, snooze, resolve

## Settings

-   Time horizon selection
-   Alert preferences (channels and thresholds)
-   Safe minimum balances per account
-   Domain configuration (which accounts are personal or business)
-   Bank connection management

# API bindings

Design the UI assuming these API endpoints exist. Bind UI components to them.

## Dashboard

-   GET /dashboard/summary
    -   Returns:
        -   personalStatus, businessStatus
        -   totalPersonalCash, totalBusinessCash
        -   personalRunwayDays, businessRunwayDays
-   GET /forecasts/current?horizonDays=30&scope=personal\|business\|combined
    -   Bind to forecast line chart
-   GET /bills/upcoming?days=30
    -   Bind to Upcoming Bills table
-   GET /goals/summary
    -   Bind to Goals overview cards
-   GET /alerts?severity=high&state=new
    -   Bind to Alerts panel

## Bills

-   GET /bills
    -   Bind to Bills list
-   GET /bills/{id}
    -   Bind to BillDetail
-   PATCH /bills/{id}
    -   Called from edit forms

## Income

-   GET /income-streams
    -   Bind to Income list and details

## Goals

-   GET /goals
    -   Bind to Goals list
-   GET /goals/{id}
    -   Bind to GoalDetail view
-   POST /goals and PATCH /goals/{id}
    -   Bind to goal create and edit forms

## Accounts

-   GET /accounts
    -   Bind to Accounts list
-   PATCH /accounts/{id}
    -   For includeInForecast and safeMinBalance

## Transactions

-   GET /transactions with filters
    -   Bind to transaction table
-   PATCH /transactions/{id}
    -   For tagging and categorization

## Alerts

-   GET /alerts
-   PATCH /alerts/{id} with state changes (acknowledge, snooze, resolve)

## Settings

-   GET /settings
-   PATCH /settings

Make sure the components expect structured JSON and can handle loading, empty, and error states for each API call.

# Deliverables

-   All screens listed above
-   Layouts and components
-   A structured component hierarchy
-   Navigation flow
-   Interaction wiring
-   Sample data bindings
-   Ready-to-use UI kit or component set
-   Design tokens in a format usable by a component library
-   Example layouts for mobile and desktop breakpoints
-   Clear mapping of where each API endpoint is used
-   Example wireframes or UI spec that can be implemented directly
