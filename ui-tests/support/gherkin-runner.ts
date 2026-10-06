import { promises as fs } from 'node:fs';
import { Page } from '@playwright/test';

export interface FeatureContext {
  screenshotPath?: string;
}

export type StepDefinition = {
  regex: RegExp;
  action: (options: { page: Page; context: FeatureContext; match: RegExpMatchArray }) => Promise<void>;
};

export async function runFeature(
  page: Page,
  featurePath: string,
  definitions: StepDefinition[]
): Promise<FeatureContext> {
  const payload = await fs.readFile(featurePath, 'utf8');
  const context: FeatureContext = {};
  const lines = payload
    .split(/\r?\n/)
    .map((line) => line.trim())
    .filter((line) => line && !line.startsWith('Feature') && !line.startsWith('Scenario'));

  for (const line of lines) {
    const text = line.replace(/^(Given|When|Then|And|But)\s+/i, '');
    const match = definitions
      .map((definition) => ({ definition, match: definition.regex.exec(text) }))
      .find((candidate) => candidate.match !== null);

    if (!match) {
      throw new Error(`No step definition for "${text}"`);
    }

    await match.definition.action({ page, context, match: match.match! });
  }

  return context;
}
