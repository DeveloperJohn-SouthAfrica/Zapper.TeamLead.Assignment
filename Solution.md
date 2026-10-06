# Solution

## Structured Systems Analysis & Design phase

We begin from the specification in [Zapper - Team_Lead Assignment.md](docs/Zapper%20-%20Team_Lead%20Assignment.md), holding off on stack/architecture choices until the analysis artifacts below inform our decisions. The [Structured Systems Analysis](docs/analysis/structured-systems-analysis.md) report anchors this phase, and lives alongside the DFD, ERD, and sequence‐diagram subfiles that capture the modular components, data flows (including payment advice, transaction status, and real-time queries), and behavioral contracts.

## Object-Oriented Analysis & Design phase

Phase 2 translates the structured systems artifacts into domain-aligned object models, respecting ACID/SOLID/DRY/12-Factor goals. See [object-oriented-analysis.md](docs/design/object-oriented-analysis.md) for the mapping from flows to services plus links to the payment, notification, and summary class diagrams. These remain stack-agnostic yet provide the vocabulary and responsibilities we will implement later.

## Adaptive analysis & design phase

Phase 3 will translate the OO models into implementation-ready modules within whichever stack we select. [adaptive-design.md](docs/design/adaptive-design.md) explains how we will iterate, validate, and evolve the diagrams (DFD, ERD, sequences, classes) into resilient components, while keeping the ISO/IEC 25010 quality attributes, ACID, SOLID, DRY, and 12-Factor commitments alive. This adaptive stage also identifies instrumentation targets, scaling knobs, and observability checks before we write the first line of code.

## Selected Architecture Stack

We chose a **Blazor WebAssembly** clean architecture, leveraging **MassTransit** for pub/sub to handle distributed, event-driven communication. We employ the **Mediator** pattern for fluent validation and segregated, highly structured controller business logic, complemented by the **CQRS** pattern and additional architectural frameworks to meet the team's development needs.

## Tooling and agent context

See [Setup.md](docs/setup.md) for how we orchestrate tooling before coding. It documents our use of Copilot CLI (each session pinned to BYOM/BYOK keys), Impeccable, Graphify, and Context7 so AI helpers remain context aware, gate UX regressions, and focus on documentation-driven implementation rather than aimless exploration.

## Containerized infrastructure and SQL deployment

The docker stack described in [docs/task3-infrastructure-tour.md](docs/task3-infrastructure-tour.md) mirrors the requested Jelastic-style environment: an NGINX+confd reverse proxy with Cloudflare origin certs fronts a stateless ASP.NET Core API, Redis (free tier) holds the only mutable state, and SQL Server Developer Edition exposes both TDS and SSH for tooling. Every persistence call flows through stored procedures (deployed with the SQL DACPAC pipeline in `scripts/deploy-dacpac.sh`), so the infrastructure layer owns audit/logging, environment configuration is driven by AWS AppConfig (with environment variables taking precedence), and the docker compose definitions keep networking, SSH access, and TLS termination explicit.

## Next steps

-   Use the artifacts in [analysis](docs/analysis) to seed the Object-Oriented analysis and design pass in [design](docs/design).
-   ## Assumptions

- **Acme Pay stub configuration**: We assume environment variables `AcmePay:BaseUrl` (e.g., `https://sandbox.acmepay.example/`) and optional `AcmePay:ApiKey` are defined; the client honor HMAC header `x-api-key`.
- **Initial stub behavior**: The Acme Pay stub returns a JSON envelope `{ "TransactionID": "<uuid>" }` on successful payment submission; we record it as `AcmeTransactionId`. Notifications and retry logic are simulated separately and documented in infrastructure tests.