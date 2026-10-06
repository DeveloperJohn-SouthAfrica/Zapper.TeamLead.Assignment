# Product

<!-- impeccable:product-schema 1 -->

## Platform
web

## Stack
delegated: .NET 8 containerized service with optional web-based UI for merchant reporting

## Users
Primary users are merchants’ administrators reviewing daily settlement summaries; secondary users are Zapper customers initiating payments via the mobile app.

## Product Purpose
Enable tokenized QR‑code payments through Acme Pay and provide sub‑merchant daily payment summaries for settlement.

## Positioning
As a payment facilitator, the service reliably handles token management, idempotent payment processing, and asynchronous Acme Pay notifications in a containerized .NET environment.

## Operating Context
Runs locally or in AWS as a Docker container; configured via environment variables; integrates with a stubbed Acme Pay API; persists data in a SQL database.

## Capabilities & Constraints
- Submit and track payments (approved, declined, reversed, refunded)<br/>
- Idempotent handling of duplicate requests and out‑of‑order notifications<br/>
- Daily summary report grouped by sub‑merchant and currency<br/>
- Environment‑driven configuration; stubbed payment provider per assignment<br/>
- Persistence in SQL Server, PostgreSQL, or SQLite (local for demo)

## Evidence on Hand
- Assignment spec and API documentation in `src/intent/.specs/zapper-team-lead-assignment`<br/>
- Initial solution scaffold in `src/AcmeIntegreatedAssessement`

## Product Principles
1. **Reliability:** Ensure consistent state under failures and concurrency.<br/>
2. **Transparency:** Clear, auditable payment state transitions.<br/>
3. **Simplicity:** Containerized local run with minimal setup.<br/>
4. **Traceability:** Comprehensive audit logs for notifications.<br/>
5. **Developer Productivity:** AI‑assisted workflow with robust tests.

