import { expect, test } from '@playwright/test';
import { mkdir } from 'node:fs/promises';
import path from 'node:path';
import { FeatureContext, runFeature, StepDefinition } from '../support/gherkin-runner';

const featurePath = path.resolve(__dirname, '../features/home-dashboard.feature');

const steps: StepDefinition[] = [
  {
    regex: /^the dashboard is available$/i,
    action: async ({ page }) => {
      await page.goto('/', { waitUntil: 'networkidle' });
    }
  },
  {
    regex: /^the visitor lands on the hero and stats$/i,
    action: async ({ page }) => {
      await expect(page.getByTestId('hero-title')).toBeVisible();
      await expect(page.locator('.stats-grid')).toBeVisible();
    }
  },
  {
    regex: /^the hero title contains "(.+)"$/i,
    action: async ({ page, match }) => {
      await expect(page.getByTestId('hero-title')).toContainText(match[1]);
    }
  },
  {
    regex: /^the stats deck mentions approved, declined, reversals, refunds$/i,
    action: async ({ page }) => {
      await Promise.all([
        expect(page.getByTestId('stat-approved')).toBeVisible(),
        expect(page.getByTestId('stat-declined')).toBeVisible(),
        expect(page.getByTestId('stat-reversed')).toBeVisible(),
        expect(page.getByTestId('stat-refunded')).toBeVisible()
      ]);
    }
  },
  {
    regex: /^the merchant summary table is visible$/i,
    action: async ({ page }) => {
      await expect(page.locator('.merchant-table')).toBeVisible();
    }
  },
  {
    regex: /^the infrastructure board lists Redis cache and SQL Server \(DACPAC\)$/i,
    action: async ({ page }) => {
      await expect(page.getByTestId('infrastructure-panel')).toContainText('Redis cache');
      await expect(page.getByTestId('infrastructure-panel')).toContainText('SQL Server (DACPAC)');
    }
  },
  {
    regex: /^I capture a screenshot of the dashboard for review$/i,
    action: async ({ page, context }) => {
      const screenshotDir = path.resolve(__dirname, '../../artifacts/playwright');
      await mkdir(screenshotDir, { recursive: true });
      const screenshotPath = path.join(screenshotDir, 'home-dashboard.png');
      await page.screenshot({ path: screenshotPath, fullPage: true });
      context.screenshotPath = screenshotPath;
    }
  }
];

test('Playwright gherkin scenario - operations dashboard', async ({ page }) => {
  const context: FeatureContext = await runFeature(page, featurePath, steps);
  expect(context.screenshotPath).toBeDefined();
});
