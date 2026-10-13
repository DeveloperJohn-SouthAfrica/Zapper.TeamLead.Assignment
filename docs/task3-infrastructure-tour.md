# Infrastructure and implementation tour

This document maps the assignment requirements to the checked-in .NET 8 API, SQL database project, local Compose services, and current verification limits.

## Requirement map

| Requirement | Main implementation |
|---|---|
| Submit/query payment | `AcmeIntegreatedAssessement.Api/Controllers/CommandsController.cs`; `Application/Commands/SubmitPayment/SubmitPaymentCommandHandler.cs`; `Infrastructure/Services/AcmePayClient.cs` |
| Acme payment and RTTQ contract | `Infrastructure/Services/AcmePayClient.cs`; `AcmePayStub/Program.cs` |
| Payment advice and reversal/refund callbacks | API controller and `Api/Services/AcmeCallbackAuthentication.cs`; notification command handlers; `Database/SchemaObjects/StoredProcedures/ApplyPaymentNotification.sql` |
| Payment, change, notification persistence | `Infrastructure/Repositories/PaymentRepository.cs`; SQL schema objects and stored procedures |
| Daily report | `Database/SchemaObjects/StoredProcedures/GetDailyPaymentSummary.sql`; `Application/Queries/GetDailyPaymentSummary`; `Api/Controllers/QueriesController.cs` |
| Local container stack | Root `docker-compose.yml`; API and stub Dockerfiles; `scripts/local-up.sh` |

Payment and notification operations use SQL stored procedures. Sub-merchant and stored-token reads on the payment path also use SQL stored procedures. EF-backed repository base classes remain in the generated infrastructure for other repository methods; they are not the source of the payment-path seed lookup. The API, database, and stub are separate deployment units, but Redis and MassTransit are not the durability boundary for payment state.

## Local services

- `acme-web`: .NET 8 API with HTTPS on host port 8081 and HTTP on 8080.
- `acme-pay-stub`: .NET 8 Appendix A-compatible local gateway at `http://localhost:8082`, or `http://acme-pay-stub:8080` from the Compose network.
- `sqlserver`: SQL Server Developer, with persistent local volume and DACPAC deployment.
- `redis`: supporting cache infrastructure; SQL remains the payment source of truth.
- Optional `nginx`, `ssh-proxy`, and `dacpac` profile services remain development/tooling infrastructure and are not prerequisites for an AWS design.

The stub has configurable approved, declined, complete, processing, timeout, gateway-error, duplicate, out-of-order, reversal, and refund scenarios. Its callback delivery can be disabled for isolated API work. Local Compose/launch-profile credentials are placeholders, not deployment secrets.

See [`../README.md`](../README.md) for startup, ports, and commands. For AWS decisions, shutdown behavior, diagnostic signals, and data-model trade-offs, see [`../SOLUTION.md`](../SOLUTION.md).

## Verification boundary

The API is compiled as a dependency of the unit-test project. The stub builds independently and its container has passed basic payment/RTTQ/idempotency/authentication HTTP smoke checks. The unit suite exercises the wire contract, authentication utilities, stub-store idempotency, completion mapping, and summary aggregation. Compose parsing and launch-script syntax have been checked. The repository does not currently claim a live SQL-backed callback-to-report integration test, an external Acme sandbox run, an AWS deployment, or a load test.
