# Developer and AI-assisted workflow

## Required development tools

- .NET 8 SDK
- Docker Engine or Docker Desktop with Docker Compose v2
- Git

Local service setup and commands are in [`../README.md`](../README.md). Node.js and Python tooling are not prerequisites for the .NET solution.

## Repository context and AI guidance

- Start from the assignment specification in [`Zapper - Team_Lead Assignment.md`](Zapper%20-%20Team_Lead%20Assignment.md), then use [`../SOLUTION.md`](../SOLUTION.md) for implementation decisions and evidence boundaries.
- Follow repository-specific guidance in [`../AGENTS.md`](../AGENTS.md), [`../.github/copilot-instructions.md`](../.github/copilot-instructions.md), and [`../.copilot/copilot-instructions.md`](../.copilot/copilot-instructions.md).
- The Graphify skill under `.github/skills/graphify/` can map source dependencies before editing cross-cutting payment flows.
- The Impeccable skill under `.github/skills/impeccable/` is for actual UI changes; respect the existing hook rather than adding duplicate visual checks.
- If a Context7 MCP server is available in the agent environment, use it to verify external library behavior. Its availability is optional and should not be represented as a project runtime dependency.

## Suggested task workflow

1. Read the relevant specification and follow the payment path from API to application handler, repository, stored procedure, and callback.
2. Break work into a small acceptance-path change with explicit evidence.
3. Keep exact Acme JSON and authentication conventions separate from app envelope conventions.
4. Run the focused unit test, API/stub build, and local configuration checks for the changed path.
5. Reconcile documentation with what was actually built and verified. Do not claim live sandbox, SQL integration, AWS, or hosted-video validation unless it was performed.

The current remediation decisions and validation limitations are recorded in [`../ai/spec-compliance-remediation.md`](../ai/spec-compliance-remediation.md).
