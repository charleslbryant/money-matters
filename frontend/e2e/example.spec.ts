import { test, expect } from '@playwright/test';

/**
 * Example E2E test for the Money Matters application.
 * This demonstrates basic Playwright testing patterns.
 */
test.describe('Money Matters App', () => {
  test('homepage loads successfully', async ({ page }) => {
    // Navigate to the homepage
    await page.goto('/');

    // Wait for the page to load
    await page.waitForLoadState('networkidle');

    // Verify the page title or main heading exists
    await expect(page).toHaveTitle(/Money Matters/i);
  });

  test('has visible navigation', async ({ page }) => {
    await page.goto('/');

    // Check for navigation elements
    // Update selectors based on your actual app structure
    const navigation = page.locator('nav');
    await expect(navigation).toBeVisible();
  });

  test('is responsive on mobile', async ({ page }) => {
    // Set mobile viewport
    await page.setViewportSize({ width: 375, height: 667 });
    await page.goto('/');

    // Verify mobile layout renders correctly
    await expect(page).toHaveTitle(/Money Matters/i);
  });
});
