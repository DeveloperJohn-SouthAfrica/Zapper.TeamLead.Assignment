# Payments Integration Service

A .NET 8 API for sub-merchant and stored-card payments, Acme Pay callbacks, payment-state queries, and daily settlement summaries. The local stack includes SQL Server, Redis, and a configurable Acme Pay stub.

## Prerequisites

- .NET 8 SDK
- Docker Engine or Docker Desktop with Docker Compose v2
- Linux users must have permission to access the Docker daemon

## Run locally

From the repository root:

```bash
scripts/local-up.sh
```

The script creates/trusts a local HTTPS development certificate, builds and starts SQL Server, Redis, and the Acme stub, builds and deploys the SQL DACPAC, then runs the API on `https://localhost:8081` and `http://localhost:8080`. The stub is available to the host at `http://localhost:8082`. Press `Ctrl+C` to stop the API process; the containers remain running.

To run the API in Compose instead:

```bash
scripts/local-up.sh --compose
```

The API is available at `https://localhost:8081`; the Compose network routes it to `http://acme-pay-stub:8080`. The local dashboard is served by the API.

The Acme stub defaults to an approved payment and sends authenticated payment-advice callbacks. Choose a scenario before starting the stack:

```bash
ACME_STUB_SCENARIO=declined scripts/local-up.sh
```

Supported scenarios are `approved`, `declined`, `complete`, `processing`, `timeout`, `gateway-error`, `duplicate`, `out-of-order`, `reversal`, and `refund`. The `timeout` scenario intentionally delays beyond the API client's 30-second timeout so the application exercises RTTQ reconciliation. Callback delivery can be disabled with `ACME_STUB_CALLBACKS_ENABLED=false`.

Local defaults are for development only. Override the SQL password, stub credentials, merchant ID, scenario, or ports with environment variables such as `SA_PASSWORD`, `ACME_PAY_API_KEY`, `ACME_ADVICE_USERNAME`, `ACME_ADVICE_PASSWORD`, `ACME_WEBHOOK_SECRET`, `ACME_PAY_MERCHANT_ID`, `ACME_STUB_PORT`, `SQL_PORT`, and `REDIS_PORT`. Do not reuse the sample values outside a local environment. AWS Secrets Manager is disabled by the local Compose configuration.

Stop containers with:

```bash
docker compose down
```

Remove local SQL and Redis data as well with `docker compose down --volumes`.

## Validate

Run the unit tests:

```bash
dotnet test src/AcmeIntegreatedAssessement/AcmeIntegreatedAssessement.Application.UnitTests/AcmeIntegreatedAssessement.Application.UnitTests.csproj
```

Build the API:

```bash
dotnet build src/AcmeIntegreatedAssessement/AcmeIntegreatedAssessement.Api/AcmeIntegreatedAssessement.Api.csproj
```

The unit tests cover the Appendix A payment envelope/response, unknown response-field tolerance, RTTQ request construction, rate-limit retry identity, callback Basic/HMAC verification, atomic stub idempotency, and daily-summary aggregation. The SQL lifecycle and full callback-to-summary path still require database-backed integration verification.

## Documentation

- [Solution, design trade-offs, AWS approach, and AI workflow](SOLUTION.md)
- [Specification compliance evidence and remaining limitations](docs/spec-compliance-evaluation.md)
- [Assignment specification and Appendix A](docs/Zapper%20-%20Team_Lead%20Assignment.md)
- [AI workflow artifacts and walkthrough outline](ai/)
- [Architecture analysis](docs/analysis/structured-systems-analysis.md)
- [Design artifacts](docs/design/README.md)
