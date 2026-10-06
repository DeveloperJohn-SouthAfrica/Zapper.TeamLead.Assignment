import { defineConfig } from '@playwright/test';

export default defineConfig({
  testDir: './tests',
  timeout: 120 * 1000,
  expect: {
    timeout: 10 * 1000
  },
  fullyParallel: false,
  retries: process.env.CI ? 2 : 0,
  use: {
    baseURL: 'http://127.0.0.1:5200',
    trace: 'on-first-retry',
    viewport: {
      width: 1365,
      height: 768
    },
    screenshot: 'only-on-failure',
    video: 'retain-on-failure'
  },
  webServer: {
    command: 'dotnet run --project src/AcmeIntegreatedAssessement/AcmeIntegreatedAssessement.Api/AcmeIntegreatedAssessement.Api.csproj --urls http://127.0.0.1:5200',
    port: 5200,
    env: {
      ASPNETCORE_ENVIRONMENT: 'Development'
    },
    reuseExistingServer: !process.env.CI,
    timeout: 120 * 1000
  }
});
