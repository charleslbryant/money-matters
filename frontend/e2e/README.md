# E2E Tests

End-to-end tests for the Money Matters application using Playwright.

## Overview

These tests verify complete user workflows through the actual application, running in real browsers (Chromium, Firefox, WebKit, and mobile viewports).

## Test Structure

```
e2e/
├── example.spec.ts          # Example test patterns
├── README.md                # This file
└── (future tests organized by feature)
    ├── auth/
    ├── dashboard/
    ├── bills/
    ├── goals/
    └── forecasts/
```

## Running Tests

```bash
# Run all E2E tests
bun run test:e2e

# Run with UI (recommended for development)
bun run test:e2e:ui

# Run in headed mode (see the browser)
bun run test:e2e:headed

# Debug a specific test
bun run test:e2e:debug

# Run specific file
bunx playwright test e2e/example.spec.ts

# Run in specific browser
bunx playwright test --project=chromium
```

## Writing Tests

### Basic Structure

```typescript
import { test, expect } from '@playwright/test';

test.describe('Feature Name', () => {
  test('specific user action', async ({ page }) => {
    // Arrange: Navigate to the page
    await page.goto('/path');

    // Act: Perform user actions
    await page.click('button');
    await page.fill('input[name="field"]', 'value');

    // Assert: Verify outcomes
    await expect(page.locator('selector')).toBeVisible();
  });
});
```

### Best Practices

1. **Use data-testid attributes** for stable selectors
2. **Test user flows**, not implementation details
3. **Keep tests independent** - each test should set up its own state
4. **Use Page Object pattern** for complex pages
5. **Avoid hard-coded waits** - use Playwright's auto-waiting
6. **Clean up after tests** - delete created data

### Example Test

```typescript
test('user can create a new bill', async ({ page }) => {
  // Navigate to bills page
  await page.goto('/bills');

  // Click "Add Bill" button
  await page.click('[data-testid="add-bill-button"]');

  // Fill out the form
  await page.fill('[name="name"]', 'Electric Bill');
  await page.fill('[name="amount"]', '150.00');
  await page.selectOption('[name="frequency"]', 'monthly');

  // Submit
  await page.click('button[type="submit"]');

  // Verify bill appears in list
  await expect(page.locator('text=Electric Bill')).toBeVisible();
  await expect(page.locator('text=$150.00')).toBeVisible();
});
```

## Test Organization

As the application grows, organize tests by feature:

```
e2e/
├── auth/
│   ├── login.spec.ts
│   ├── signup.spec.ts
│   └── logout.spec.ts
├── dashboard/
│   ├── overview.spec.ts
│   └── accounts.spec.ts
├── bills/
│   ├── create-bill.spec.ts
│   ├── edit-bill.spec.ts
│   └── delete-bill.spec.ts
├── goals/
│   └── savings-goals.spec.ts
└── shared/
    ├── fixtures.ts      # Shared test data
    └── helpers.ts       # Shared test utilities
```

## Debugging

### Visual Debugging

```bash
# UI mode - interactive test runner
bun run test:e2e:ui

# Debug mode - step through tests
bun run test:e2e:debug
```

### Reports

After test run:
```bash
bunx playwright show-report
```

### Screenshots and Videos

Playwright automatically captures:
- Screenshots on failure
- Videos on failure (configurable)
- Traces on retry

Find them in: `test-results/` and `playwright-report/`

## CI/CD

These tests run automatically in CI on:
- Pull requests
- Merges to main
- Releases

The CI configuration uses headless Chromium for faster execution.

## Current Status

**Tests implemented**: 3 example tests

These example tests demonstrate:
- Homepage loading
- Navigation visibility
- Mobile responsiveness

**Next tests to implement** (when features are ready):
- User authentication flows
- Dashboard account viewing
- Bill creation and management
- Goal tracking
- Forecast viewing
- Transaction management

---

🤖 Submitted by George with love ♥
