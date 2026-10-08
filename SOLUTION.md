# Payments Integration Service — solution notes

## Status and scope

This repository implements the assignment's .NET 8 payment API, SQL Server persistence/reporting, local Compose environment, and a configurable Acme Pay stub. It has not been deployed to AWS or tested against the external Acme sandbox. The required 5–10 minute hosted video walkthrough is still a release action; [`ai/walkthrough-outline.md`](ai/walkthrough-outline.md) is a recording guide, not a video or a claim that one exists.

The authoritative wire contract is [Appendix A](docs/Zapper%20-%20Team_Lead%20Assignment.md#appendix-a--acme-pay-payment-gateway-api). The repository's point-in-time evidence and remaining verification work are in [the compliance evaluation](docs/spec-compliance-evaluation.md).

## Architecture and payment lifecycle

- The ASP.NET Core API accepts the app's `MessageHeader`/payload envelope, validates the request, and exposes payment-state and daily-summary queries. App-level message dates use the South African UTC+02:00 offset; JSON fields serialize in PascalCase.
- The application handler loads the sub-merchant and stored token from SQL-backed stored-procedure repositories. It creates the account `TransactionID` and alphanumeric `OrderNumber`, persists a `Processing` payment first, then submits its stable identifiers and facilitator/card data to Acme.
- The typed Acme client sends `POST /api/payment` with a bearer token and the Appendix A envelope. It treats gateway `TransactionStatus` and acquirer `ResponseDetail.Code` as separate values, tolerates unknown response fields, and queries RTTQ by the same `TransactionID` after an ambiguous timeout, final rate limit, or gateway error. The local client retries a 429 using `Retry-After` without changing the transaction/order identifiers.
- A payment-advice callback is authenticated with configured HTTP Basic credentials. Reversal/refund callbacks are authenticated using Base64 HMAC-SHA256 over the exact raw request bytes. Acknowledgements return HTTP 200 only after the notification handler completes its SQL write.
- `ApplyPaymentNotification` atomically deduplicates notification IDs, retains each accepted event and its raw payload, and updates payment status only when the incoming event time is not older than the latest recorded change. Advice becomes an `Approved` or `Declined` event; applied adjustments become `Reversed` or `Refunded`. An approved EFT completion retains the `Complete` payment status while contributing to the approved-event reporting bucket.
- The Acme stub is a separate .NET 8 service in the solution and local Compose stack. It validates the request envelope, requires bearer authentication, protects transaction and retrieval-reference uniqueness atomically, exposes the documented payment and RTTQ paths, and has configurable outcomes for approval, decline, completion, processing, delayed timeout, gateway error, duplicate/out-of-order advice, reversal, and refund. Callback delivery uses the configured Basic/HMAC credentials and retries until it receives HTTP 200.

The API's `Processing` state means the request was accepted locally but Acme has not supplied a final outcome. It is not an additional Acme transaction status. Acme timestamps in Appendix A have no explicit offset and are interpreted as SAST; stored event timestamps are normalized for SQL comparisons and reporting.

## Data model and trade-offs

SQL Server stores payments, payment changes, and notification deliveries separately. A payment holds the stable app payment ID, Acme transaction UUID, account-wide retrieval reference, integer minor-unit amount, currency, sub-merchant relationship, and current status. Payment changes preserve lifecycle outcomes and amounts; notification deliveries preserve deduplication identity, event time, application result, and raw JSON for investigation. Database operations for these flows use stored procedures, and schema changes are delivered through the SQL project/DACPAC.

The daily summary groups by sub-merchant and currency, uses SAST day boundaries, and totals approved, declined, reversed, and refunded change events. A partial refund is recorded with the amount of that refund event, rather than replacing the original payment amount. The tests currently cover summary aggregation, not a live SQL deployment or the end-to-end stored-procedure result.

The local payment and callback paths use SQL as their source of truth; Redis, MassTransit, and the Blazor dashboard are existing supporting infrastructure rather than a distributed transaction or queue in the core Acme flow. The synchronous external call cannot share a database transaction. Persisting before submission, reusing the same Acme identifiers, and reconciling ambiguous results through RTTQ avoids losing the local intent, but it does not make SQL and Acme a single atomic system. A production evolution could add an outbox/worker and explicit retry state if asynchronous submission or higher throughput becomes necessary.

Schema evolution should be additive and backward compatible: deploy nullable columns/tables and procedures first, deploy code that can read both old/new shapes, backfill if needed, then enforce new constraints and remove obsolete columns in a later release. Notification event identity and payment-state rules should remain explicit rather than being inferred from mutable display fields. SQL migration/DACPAC promotion should be tested against a copy of production data and guarded against destructive changes.

## Local operation

Follow [`README.md`](README.md). `scripts/local-up.sh` starts SQL Server, Redis, and the Acme stub, deploys the DACPAC, and runs the API on the host; `scripts/local-up.sh --compose` runs the API in Docker. Local defaults are disposable placeholders and must not be reused outside development. Scenario selection is done with `ACME_STUB_SCENARIO`; the stub listens on host port 8082 by default and on `acme-pay-stub:8080` inside Compose.

## AWS deployment approach

I would keep the current container boundary and run the API as stateless ECS Fargate tasks behind an Application Load Balancer with TLS termination, private service/database subnets, and security groups limited to required paths. I would retain SQL Server semantics with Amazon RDS for SQL Server (Multi-AZ backups and point-in-time recovery) unless a separately scoped migration justified another engine. The stub is local/test infrastructure and would not be deployed as a production payment gateway.

The API has a Secrets Manager configuration hook; Compose disables it and supplies local environment values. In AWS, task roles should grant least-privilege access to the specific secret and configuration resources, and secrets should be injected at runtime rather than baked into images or committed. Non-secret feature/configuration values belong in managed configuration, with Secrets Manager for credentials and signing keys. Secret rotation should be coordinated with callback and gateway configuration.

Scale API tasks horizontally using request rate/latency and CPU/memory signals. Keep payment state in SQL and make callback processing idempotent so requests can land on any task. Scale SQL deliberately using query/lock/connection metrics and validated indexes; use a cache only for data whose staleness is acceptable. The existing Redis and MassTransit services should not be treated as payment durability guarantees.

For graceful shutdown, stop accepting new traffic during task draining, allow the load balancer and host to drain in-flight HTTP requests, and pass cancellation tokens through handlers and SQL calls. Do not acknowledge a callback before the SQL transaction commits. If shutdown interrupts an ambiguous Acme request, leave the durable row in `Processing` and reconcile it through RTTQ or a later notification; do not create a new transaction ID. A future worker/outbox should stop claiming work on shutdown and finish or release its current lease before the termination deadline.

Operational signals should include structured request/correlation IDs without card tokens or secrets; Acme submission counts, outcome/status-code counts and latency; timeout/RTTQ/reconciliation age; callback authentication failures, duplicate and out-of-order counts; SQL stored-procedure latency/failures and connection saturation; and pending-payment age. Readiness should include required SQL connectivity, while liveness should remain independent of transient gateway health. Export traces/metrics to CloudWatch/OpenTelemetry and alert on sustained gateway failure, rising unresolved payments, callback rejection, and database saturation.

This is a deployment design, not evidence of a deployed or load-tested AWS environment.

## AI-assisted workflow and validation

The repository rules in [`AGENTS.md`](AGENTS.md), [`.github/copilot-instructions.md`](.github/copilot-instructions.md), and [`.copilot/copilot-instructions.md`](.copilot/copilot-instructions.md) capture shared project constraints. During this remediation, GitHub Copilot Task Agent was used to inspect the specification and implementation, trace payment dependencies with the repository's Graphify materials, implement focused changes, and add tests. Impeccable was used when inspecting the dashboard surface; it did not replace the payment contract as the source of truth. The checklists and recording outline are in [`ai/`](ai/).

The work was divided by acceptance path: Acme request/response and RTTQ; durable submission and callbacks; SQL lifecycle/reporting; then local operation and deliverables. The agent compared the existing implementation against the exact Appendix A examples rather than relying on existing method names or the older compliance narrative. One concrete example of rejected behavior is the old client contract: a flat token/key request and top-level transaction ID could not satisfy the documented nested payment envelope and bearer-authenticated route. That path was replaced and contract-tested. The callback completion case was also corrected so `Complete`/`COMPL` is not silently turned into a decline. Existing documentation claims about AppConfig and Redis being the only mutable state were removed because the checked-in runtime does not support those claims.

Validation performed during this work:

- `dotnet build` of the Acme stub — passed with no warnings.
- Unit tests — 9 passed, including Appendix A serialization and unknown-field handling, RTTQ URL construction, rate-limit retries with fresh message IDs and stable transaction IDs, non-JSON gateway error status, Basic/HMAC verification, atomic stub idempotency/RRN collision handling, completed EFT state mapping, and summary aggregation.
- API compilation occurred as a project dependency of the unit-test run. Restore/build reports existing NU1603 package-resolution warnings and unrelated compiler/analyzer warnings.
- The Acme stub Docker image built and ran. A live local HTTP smoke verified health, bearer-protected payment submission, RTTQ, same-transaction replay, RRN-collision response, and unauthorized rejection. Callback delivery to an unavailable receiver was retried without failing the payment response.
- `docker compose config --quiet` and `bash -n scripts/local-up.sh` — passed.
- A live external Acme sandbox, full multi-container/SQL-backed callback-to-report flow, AWS deployment, and hosted walkthrough have not been verified in this environment. The remaining evidence gaps are itemized in [`docs/spec-compliance-evaluation.md`](docs/spec-compliance-evaluation.md).

If starting again, I would build the smallest exact-contract stub and API path before retaining or expanding ancillary infrastructure, then add a database-backed lifecycle test before writing broader architecture claims. The external 5–10 minute walkthrough still needs to be recorded and shared by the repository owner.
