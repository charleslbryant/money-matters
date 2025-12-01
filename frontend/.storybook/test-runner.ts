import type { TestRunnerConfig } from '@storybook/test-runner';

/**
 * Storybook Test Runner configuration.
 * This enables testing all Storybook stories with Playwright.
 */
const config: TestRunnerConfig = {
  // Hook that runs before each story test
  async preVisit(page) {
    // You can add global setup here, like authentication
  },

  // Hook that runs after each story test
  async postVisit(page) {
    // You can add assertions here that run for every story
    // For example, check for console errors
    const logs = await page.evaluate(() => {
      return (window as any).__STORYBOOK_PREVIEW_ERRORS__ || [];
    });

    if (logs.length > 0) {
      console.warn('Console errors found:', logs);
    }
  },
};

export default config;
