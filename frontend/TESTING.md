# Money Matters Frontend - Testing Guide

Comprehensive testing guide for the Money Matters frontend using Playwright and Storybook.

## Table of Contents

- [Test Types](#test-types)
- [Running Tests](#running-tests)
- [E2E Tests with Playwright](#e2e-tests-with-playwright)
- [Storybook Interaction Tests](#storybook-interaction-tests)
- [Writing Tests](#writing-tests)
- [Best Practices](#best-practices)
- [CI/CD Integration](#cicd-integration)
- [Troubleshooting](#troubleshooting)

## Test Types

We use two complementary testing approaches:

### 1. **End-to-End (E2E) Tests** - Playwright
- Full application workflow testing
- Tests user journeys through the actual app
- Runs in real browsers (Chromium, Firefox, WebKit)
- Located in: `e2e/`

### 2. **Component Interaction Tests** - Storybook + Playwright
- Isolated component testing
- User interaction simulation
- Visual regression testing
- Runs in Storybook stories with `play` functions

## Running Tests

### E2E Tests (Playwright)

```bash
# Run all E2E tests (headless mode)
bun run test:e2e

# Run with UI mode (visual test runner)
bun run test:e2e:ui

# Run in headed mode (see browser)
bun run test:e2e:headed

# Debug mode (step-through debugging)
bun run test:e2e:debug

# Run specific test file
bunx playwright test e2e/example.spec.ts

# Run tests in specific browser
bunx playwright test --project=chromium
bunx playwright test --project=firefox
bunx playwright test --project=webkit

# Run on mobile
bunx playwright test --project="Mobile Chrome"
```

### Storybook Tests

```bash
# Run Storybook test-runner (requires Storybook to be running)
bun run test:storybook

# Run Storybook tests in CI (builds and serves Storybook)
bun run test:storybook:ci

# Start Storybook to manually test interactions
bun run storybook
# Then open http://localhost:6006 and click "Interactions" panel
```

### Run All Tests

```bash
# Run both E2E and Storybook tests
bun run test
```

## E2E Tests with Playwright

### Test Structure

E2E tests are located in the `e2e/` directory:

```
frontend/
├── e2e/
│   ├── example.spec.ts          # Example test patterns
│   ├── authentication.spec.ts   # Auth flows
│   ├── dashboard.spec.ts        # Dashboard features
│   └── bills.spec.ts            # Bill management
├── playwright.config.ts         # Playwright configuration
└── playwright-report/           # Test reports (generated)
```

### Writing E2E Tests

```typescript
import { test, expect } from '@playwright/test';

test.describe('Dashboard', () => {
  test('user can view account balances', async ({ page }) => {
    // Navigate to the dashboard
    await page.goto('/dashboard');

    // Wait for accounts to load
    await page.waitForSelector('[data-testid="account-card"]');

    // Verify account cards are visible
    const accountCards = page.locator('[data-testid="account-card"]');
    await expect(accountCards).toHaveCount(3);

    // Check first account balance
    const firstBalance = page.locator('[data-testid="account-balance"]').first();
    await expect(firstBalance).toBeVisible();
  });

  test('user can add a new transaction', async ({ page }) => {
    await page.goto('/dashboard');

    // Click "Add Transaction" button
    await page.click('button:has-text("Add Transaction")');

    // Fill out the form
    await page.fill('[name="amount"]', '50.00');
    await page.fill('[name="description"]', 'Grocery shopping');
    await page.selectOption('[name="category"]', 'groceries');

    // Submit
    await page.click('button[type="submit"]');

    // Verify success message
    await expect(page.locator('text=Transaction added')).toBeVisible();
  });
});
```

### Page Object Pattern

For complex pages, use Page Objects:

```typescript
// e2e/pages/DashboardPage.ts
import { Page, Locator } from '@playwright/test';

export class DashboardPage {
  readonly page: Page;
  readonly accountCards: Locator;
  readonly addTransactionButton: Locator;

  constructor(page: Page) {
    this.page = page;
    this.accountCards = page.locator('[data-testid="account-card"]');
    this.addTransactionButton = page.locator('button:has-text("Add Transaction")');
  }

  async goto() {
    await this.page.goto('/dashboard');
  }

  async getAccountCount() {
    return await this.accountCards.count();
  }

  async addTransaction(amount: string, description: string) {
    await this.addTransactionButton.click();
    await this.page.fill('[name="amount"]', amount);
    await this.page.fill('[name="description"]', description);
    await this.page.click('button[type="submit"]');
  }
}

// In test file
import { DashboardPage } from './pages/DashboardPage';

test('add transaction via page object', async ({ page }) => {
  const dashboard = new DashboardPage(page);
  await dashboard.goto();
  await dashboard.addTransaction('100.00', 'Test transaction');
});
```

### Configuration

Playwright is configured in `playwright.config.ts`:

- **Base URL**: `http://localhost:5173` (Vite dev server)
- **Browsers**: Chromium, Firefox, WebKit, Mobile Chrome, Mobile Safari
- **Retries**: 2 on CI, 0 locally
- **Screenshots**: On failure
- **Videos**: On failure
- **Traces**: On first retry

## Storybook Interaction Tests

### What Are Interaction Tests?

Interaction tests simulate user interactions directly in Storybook stories using the `play` function. They run with Playwright and can be tested in isolation or as part of CI.

### Writing Interaction Tests

```typescript
import type { Meta, StoryObj } from '@storybook/react';
import { userEvent, within, expect } from '@storybook/test';
import { Button } from './Button';

const meta = {
  title: 'Components/Button',
  component: Button,
} satisfies Meta<typeof Button>;

export default meta;
type Story = StoryObj<typeof meta>;

// Story with interaction tests
export const Interactive: Story = {
  args: {
    label: 'Click Me',
    onClick: () => console.log('Clicked!'),
  },
  play: async ({ canvasElement }) => {
    const canvas = within(canvasElement);

    // Find the button
    const button = canvas.getByRole('button', { name: /click me/i });

    // Verify it's visible
    await expect(button).toBeInTheDocument();

    // Click the button
    await userEvent.click(button);

    // Verify state after click (if applicable)
    await expect(button).toHaveAttribute('aria-pressed', 'true');
  },
};
```

### Interaction Test Patterns

#### 1. Form Interactions

```typescript
export const FormInteraction: Story = {
  play: async ({ canvasElement }) => {
    const canvas = within(canvasElement);

    // Type in text field
    const emailInput = canvas.getByLabelText(/email/i);
    await userEvent.type(emailInput, 'test@example.com');
    await expect(emailInput).toHaveValue('test@example.com');

    // Select from dropdown
    const countrySelect = canvas.getByLabelText(/country/i);
    await userEvent.selectOptions(countrySelect, 'US');

    // Check checkbox
    const agreeCheckbox = canvas.getByRole('checkbox', { name: /agree/i });
    await userEvent.click(agreeCheckbox);
    await expect(agreeCheckbox).toBeChecked();

    // Submit form
    const submitButton = canvas.getByRole('button', { name: /submit/i });
    await userEvent.click(submitButton);
  },
};
```

#### 2. Validation Testing

```typescript
export const ValidationTest: Story = {
  play: async ({ canvasElement }) => {
    const canvas = within(canvasElement);

    const emailInput = canvas.getByLabelText(/email/i);

    // Type invalid email
    await userEvent.type(emailInput, 'invalid-email');
    await userEvent.tab(); // Blur to trigger validation

    // Wait for error message
    await new Promise((resolve) => setTimeout(resolve, 100));

    // Verify error message appears
    const errorMessage = canvas.getByText(/invalid email/i);
    await expect(errorMessage).toBeInTheDocument();
  },
};
```

#### 3. Keyboard Navigation

```typescript
export const KeyboardNavigation: Story = {
  play: async ({ canvasElement }) => {
    const canvas = within(canvasElement);

    // Focus first input
    const firstInput = canvas.getByLabelText(/first name/i);
    await firstInput.focus();

    // Tab to next input
    await userEvent.tab();

    // Verify focus moved
    const lastNameInput = canvas.getByLabelText(/last name/i);
    await expect(lastNameInput).toHaveFocus();
  },
};
```

#### 4. State Changes

```typescript
export const StateChange: Story = {
  play: async ({ canvasElement }) => {
    const canvas = within(canvasElement);

    const toggleButton = canvas.getByRole('button', { name: /toggle/i });

    // Initial state
    await expect(toggleButton).toHaveAttribute('aria-pressed', 'false');

    // Click to toggle
    await userEvent.click(toggleButton);

    // Verify state changed
    await expect(toggleButton).toHaveAttribute('aria-pressed', 'true');

    // Toggle back
    await userEvent.click(toggleButton);
    await expect(toggleButton).toHaveAttribute('aria-pressed', 'false');
  },
};
```

### Running Interaction Tests

```bash
# In Storybook UI (interactive mode)
bun run storybook
# Navigate to a story with interactions
# Click "Interactions" panel to see the test run

# With test-runner (automated)
# First, start Storybook
bun run storybook

# In another terminal
bun run test:storybook
```

## Best Practices

### Test Organization

```
e2e/
├── auth/
│   ├── login.spec.ts
│   └── signup.spec.ts
├── dashboard/
│   ├── overview.spec.ts
│   └── accounts.spec.ts
├── bills/
│   ├── create.spec.ts
│   ├── edit.spec.ts
│   └── delete.spec.ts
└── shared/
    ├── fixtures.ts
    └── helpers.ts
```

### Use Data Test IDs

```typescript
// Component
<div data-testid="account-card">
  <span data-testid="account-balance">{balance}</span>
</div>

// Test
const accountCard = page.locator('[data-testid="account-card"]');
const balance = page.locator('[data-testid="account-balance"]');
```

### Avoid Hard-Coded Waits

```typescript
// ❌ Bad
await page.waitForTimeout(5000);

// ✅ Good
await page.waitForSelector('[data-testid="loaded"]');
await page.waitForLoadState('networkidle');
await expect(element).toBeVisible();
```

### Test User Flows, Not Implementation

```typescript
// ❌ Bad - testing implementation
test('state updates when button clicked', async ({ page }) => {
  await page.click('button');
  // Check internal state
});

// ✅ Good - testing user outcome
test('user can submit form', async ({ page }) => {
  await page.fill('[name="email"]', 'test@example.com');
  await page.click('button[type="submit"]');
  await expect(page.locator('text=Success')).toBeVisible();
});
```

### Keep Tests Independent

```typescript
// ❌ Bad - tests depend on each other
test.describe.serial('dependent tests', () => {
  test('create account', async ({ page }) => {
    // Creates account, depends on state
  });

  test('view account', async ({ page }) => {
    // Depends on previous test creating account
  });
});

// ✅ Good - tests are independent
test('create account', async ({ page }) => {
  // Setup: Create any necessary data
  // Test: Create account
  // Cleanup: Delete account
});

test('view account', async ({ page }) => {
  // Setup: Create account via API
  // Test: View account
  // Cleanup: Delete account
});
```

### Use Fixtures for Common Setup

```typescript
// playwright.config.ts
import { test as base } from '@playwright/test';

type MyFixtures = {
  authenticatedPage: Page;
};

export const test = base.extend<MyFixtures>({
  authenticatedPage: async ({ page }, use) => {
    // Login
    await page.goto('/login');
    await page.fill('[name="email"]', 'test@example.com');
    await page.fill('[name="password"]', 'password');
    await page.click('button[type="submit"]');
    await page.waitForURL('/dashboard');

    await use(page);
  },
});

// In test file
import { test } from './fixtures';

test('authenticated user can view dashboard', async ({ authenticatedPage }) => {
  // Page is already logged in
  await expect(authenticatedPage.locator('h1')).toHaveText('Dashboard');
});
```

## CI/CD Integration

### GitHub Actions Example

```yaml
name: E2E Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - uses: oven-sh/setup-bun@v1

      - name: Install dependencies
        run: bun install

      - name: Install Playwright browsers
        run: bunx playwright install --with-deps chromium

      - name: Run E2E tests
        run: bun run test:e2e

      - name: Run Storybook tests
        run: bun run test:storybook:ci

      - name: Upload test results
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: playwright-report
          path: playwright-report/
```

## Troubleshooting

### Tests Timing Out

```typescript
// Increase timeout for slow operations
test('slow operation', async ({ page }) => {
  test.setTimeout(60000); // 60 seconds

  await page.goto('/slow-page');
  await page.waitForSelector('.loaded', { timeout: 30000 });
});
```

### Flaky Tests

```typescript
// Use auto-waiting assertions
await expect(page.locator('.element')).toBeVisible();

// Retry specific actions
await expect(async () => {
  await page.click('button');
  await expect(page.locator('.result')).toBeVisible();
}).toPass({
  intervals: [1000, 2000, 5000],
  timeout: 10000,
});
```

### Storybook Test-Runner Not Finding Stories

1. Ensure Storybook is running: `bun run storybook`
2. Check the port: `http://localhost:6006`
3. Build Storybook for CI: `bun run build-storybook`

### Browser Not Found

```bash
# Reinstall Playwright browsers
bunx playwright install
bunx playwright install chromium
```

## Debugging

### Debug Mode

```bash
# Opens inspector with step-through debugging
bun run test:e2e:debug
```

### Screenshots and Videos

Playwright automatically captures:
- **Screenshots**: On test failure
- **Videos**: On test failure (if configured)
- **Traces**: On first retry

View in the HTML report:
```bash
bunx playwright show-report
```

### Console Logs

```typescript
// Capture browser console logs
page.on('console', (msg) => console.log('BROWSER:', msg.text()));

// Network requests
page.on('request', (req) => console.log('REQUEST:', req.url()));
page.on('response', (res) => console.log('RESPONSE:', res.url(), res.status()));
```

## Test Coverage Goals

- **E2E Tests**: Cover all critical user journeys
- **Storybook Tests**: 100% of interactive components
- **Combined**: End-to-end coverage of all features

### What to Test

✅ **Test**:
- User authentication flows
- Form submissions with validation
- Navigation between pages
- Data creation, reading, updating, deleting
- Error handling and edge cases
- Mobile responsiveness
- Accessibility features

❌ **Don't Test**:
- Third-party library internals
- Implementation details
- Styling (unless visual regression testing)

## Resources

- [Playwright Documentation](https://playwright.dev/)
- [Storybook Testing](https://storybook.js.org/docs/writing-tests)
- [Testing Library](https://testing-library.com/)
- [Web Accessibility](https://www.w3.org/WAI/fundamentals/)

---

🤖 Submitted by George with love ♥
