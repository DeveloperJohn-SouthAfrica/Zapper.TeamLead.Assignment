# Specification compliance evaluation

**Review date:** 2026-10-08
**Source of truth:** [`Zapper - Team_Lead Assignment.md`](Zapper%20-%20Team_Lead%20Assignment.md), especially Appendix A.
**Review method:** implementation and artifact inspection plus the builds/tests listed below. This is not evidence of a live Acme sandbox, deployed AWS environment, or full database-backed lifecycle run.

## Executive finding

The main source-level gaps identified in the earlier review have been addressed: the payment/RTTQ client now follows the documented contract; the request is persisted before submission; notification endpoints authenticate the documented callback formats and return HTTP 200 after processing; notification state updates are deduplicated and event-time ordered in SQL; and reporting groups by sub-merchant and currency. The local stack now includes an Acme stub, and the repository has the required `README.md`, case-correct `SOLUTION.md`, AI configuration, and `ai/` artifacts.

**Do not represent the assignment as fully evidenced yet.** The hosted 5–10 minute walkthrough is not in this repository and must still be recorded/shared. The SQL-backed lifecycle, all eight required scenarios through the complete API/SQL path, a live Acme sandbox, and AWS deployment have not been verified here. The implementation is materially closer to the specification, but these are real evidence limits, not claims of successful integration.

## Compliance snapshot

| Requirement | Source status | Verification status |
|---|---|---|
| Task 1 — submit/query/notify payments | Appendix A contract, SQL-first submission, RTTQ, callback verification, and lifecycle handlers are implemented. | Focused contract/auth/idempotency unit tests pass. Full API + SQL scenario flow is not run. |
| Task 2 — schema and daily summary | SQL persists payments, changes, notifications, audit payloads; summary groups by sub-merchant/currency and SAST day. | Summary aggregation unit test passes. Stored procedures have not been exercised against a live database in this review. |
| Task 3 — local run and AWS readiness | .NET 8 API and stub are in Compose; README and `SOLUTION.md` cover local operation and an AWS approach. | Stub build, Compose configuration parsing, shell syntax, and API compilation as a test dependency pass. No complete Compose/database smoke test or AWS deployment. |
| Task 4 — AI workflow and artifacts | Root and tool-specific instructions exist; `SOLUTION.md` explains actual workflow/validation; `ai/` includes review notes and a walkthrough outline. | Files are present. The outline is not a hosted video. |
| Required hosted 5–10 minute walkthrough | Not delivered in this repository. | **Outstanding external action.** |

## Detailed assessment

### 1. Task 1 and Appendix A — payment integration

| Specification behavior | Current implementation | Evidence and remaining proof |
|---|---|---|
| Build a payment from customer, stored token, sub-merchant, amount, and app ID | `SubmitPaymentCommandHandler` loads the payment, SQL-backed sub-merchant and stored-token records; formats facilitator fields; stores `Processing` before sending. | `src/AcmeIntegreatedAssessement/AcmeIntegreatedAssessement.Application/Commands/SubmitPayment/SubmitPaymentCommandHandler.cs`; merchant/token SQL repository lookups under `Infrastructure/Repositories/`; stored procedures `GetSubMerchantByZapperMerchantId` and `GetStoredCardTokenByCustomerId`. |
| Exact payment URL, bearer auth, request/response envelope and fields | `AcmePayClient` sends `POST /api/payment`, bearer auth is configured by dependency injection, and the client constructs Appendix A fields including nested `Card.Token`, facilitator fields, `TransactionID`, `OrderNumber`, minor-unit amount and currency. It reads nested `PaymentResponse`; unknown JSON fields are ignored. Each 429 retry creates a fresh `MessageID` while keeping the Acme transaction/reference stable. | `Infrastructure/Services/AcmePayClient.cs`, `Infrastructure/DependencyInjection.cs`; `AcmeContractTests.SubmitPaymentAsync_SendsAppendixAEnvelopeAndIgnoresUnknownResponseFields` and `...RetriesRateLimitedRequestsWithNewMessageIdsAndStableTransactionId`. |
| Distinguish gateway transaction status from acquirer result | Initial responses use both `TransactionStatus` and `ResponseDetail.Code`; `00` maps to success, `COMPL` to completion, pending remains `Processing`, and other final outcomes map to failure. Advice maps approved/declined events; `Complete`/`COMPL` retains `Complete`. | `SubmitPaymentCommandHandler.cs`, `CommandsController.cs`, `ProcessAcmeApprovedNotificationCommandHandler.cs`; completed-EFT handler unit test. No exhaustive result-code matrix test yet. |
| Handle gateway errors and ambiguity | Rate limits use `Retry-After`; 429/5xx/final timeout paths query RTTQ with the same transaction ID; unresolved results remain `Processing`. Non-JSON error bodies preserve the HTTP status so a gateway 5xx still reaches reconciliation. | `AcmePayClient.cs`, `SubmitPaymentCommandHandler.cs`; RTTQ and non-JSON 503 unit tests. The local stub does not emit a 429 scenario; rate-limit behavior uses a synthetic test handler. |
| Duplicate app payment ID | The handler reuses an existing persisted payment instead of creating a new Acme transaction. SQL insert also checks the app ID; Acme and the stub use stable transaction/reference identifiers for idempotency. | `SubmitPaymentCommandHandler.cs`, `InsertPayment.sql`, `AcmePayStub/Program.cs`. Concurrent whole-API submissions are not database-integration-tested. |
| Acme stub and required scenarios | The stub validates its input contract, bearer token, transaction ID and RRN uniqueness; it offers `approved`, `declined`, `complete`, `processing`, `timeout`, `gateway-error`, `duplicate`, `out-of-order`, `reversal`, and `refund` modes with optional callback delivery. | `src/AcmeIntegreatedAssessement/AcmePayStub/Program.cs`, its Dockerfile, solution and Compose entries, plus README scenario instructions. Store atomicity is unit-tested; all named HTTP scenarios have not been run end-to-end. |

### 2. Appendix A — inbound notifications and lifecycle

| Requirement | Current implementation | Remaining proof |
|---|---|---|
| Payment advice uses HTTP Basic credentials | The approved-notification action checks `ACME_ADVICE_USERNAME`/`ACME_ADVICE_PASSWORD` before binding and processing the exact flat callback payload. | Basic-auth utility test passes; no API-hosted callback integration test. |
| Reversal/refund signature uses exact raw bytes | The webhook action reads a size-limited raw body, validates Base64 HMAC-SHA256 from `x-signature`, then deserializes the flat lower-camel payload. | HMAC utility test passes; no API-hosted signed callback integration test. |
| Acknowledge successful callbacks with HTTP 200 | Both successful callback actions return `Ok(...)` only after the mediator/SQL operation completes. | Controller code inspected; no full callback route test. |
| Avoid duplicate effects and stale state rollback | Advice identity is derived from stable notification fields; Switch webhook uses its event UUID. `ApplyPaymentNotification.sql` serializes dedupe/event insertion/state update in one SQL transaction. Older events are retained but cannot overwrite a newer status. | SQL procedure inspected; no live SQL concurrency/order test. |
| Map lifecycle status and report event | Advice becomes an `Approved` or `Declined` change; successful reversal/refund becomes `Reversed`/`Refunded`; unsuccessful adjustment is audited without changing payment status. `Complete` remains a valid payment status. | Handler unit test covers completed EFT mapping; reversal/refund/out-of-order SQL behavior remains integration-test work. |

The callbacks intentionally remain flat because Appendix A defines them that way; app and Acme payment/RTTQ request/response messages use the `MessageHeader` envelope.

### 3. Task 2 — persistence and daily reporting

The SQL database project contains payment, payment-change, notification-delivery, merchant, and stored-token tables plus stored procedures. Payment state writes, notification dedupe/audit, seed lookups, and summary retrieval use SQL stored procedures. `ApplyPaymentNotification` writes the callback and payment change atomically and guards status updates by event time. `GetDailyPaymentSummary` uses SAST-day boundaries and groups only by sub-merchant and currency, with approved, declined, reversed, and refunded event buckets. Amounts are `BIGINT` minor units.

The daily-summary handler test verifies grouped aggregation; it does not prove stored-procedure behavior, schema deployment, SQL seed availability, or report correctness after a full callback lifecycle. Those remain explicit database-backed verification tasks.

### 4. Task 3 — containerization and AWS readiness

- The API and Acme stub target .NET 8. Compose includes SQL Server, Redis, API, and stub; `scripts/local-up.sh` starts dependencies, deploys the DACPAC, and supports either a host API or Compose API.
- README prerequisites now match .NET 8, and local instructions include ports, scenario selection, test commands, and local-only credentials.
- `SOLUTION.md` describes an ECS/Fargate + load balancer + RDS for SQL Server approach, managed secrets, scaling, graceful shutdown, and diagnostics. It is a design proposal, not an AWS deployment.
- The existing Secrets Manager hook is configurable; local Compose disables it. No AWS AppConfig runtime integration is claimed.
- Compose YAML parsing and shell syntax passed. The stub image was built and run; local HTTP smoke checks verified health, authorized payment, RTTQ, transaction replay, RRN collision, and bearer rejection. The full API/SQL multi-container deployment, database readiness, certificates, and successful callback connectivity were not smoke-tested.

### 5. Task 4 and repository deliverables

| Deliverable | Status |
|---|---|
| `README.md` local run instructions | Present and updated for .NET 8, stub, environment variables, scenarios, and validation commands. |
| Exact `SOLUTION.md` filename | Present with the required case and includes design choices/trade-offs, schema evolution, AWS approach, AI workflow, and validation. |
| AI configuration | `AGENTS.md`, `.github/copilot-instructions.md`, and `.copilot/copilot-instructions.md` are present and point to `SOLUTION.md`. |
| `ai/` artifacts | Present: compliance remediation notes, workflow README, and walkthrough outline. |
| AI validation narrative | Present in `SOLUTION.md`; it distinguishes unit evidence from unrun SQL/sandbox/AWS verification. |
| Hosted 5–10 minute video | **Not present.** The outline is only a guide. The repository owner still needs to record and share the video. |

### 6. Required scenario evidence matrix

| Scenario | Current source support | Automated evidence in this run |
|---|---|---|
| Acquirer approval | Stub `approved`; maps `Success` + `00`. | Successful response contract tested, not full scenario. |
| Acquirer decline | Stub `declined`; maps non-00 acquirer code to failure/declined. | No full scenario test. |
| Same app ID resubmitted | Reuses SQL-persisted payment and Acme identifiers. | Not tested against SQL/API concurrency. |
| Duplicate callback | Stub replays the same advice event; SQL dedupes notification ID. | Stub store atomic idempotency tested; notification SQL not run. |
| Out-of-order callback | Stub sends reversal with later event time before older advice; SQL applies only non-stale status changes. | Not tested against SQL. |
| Reversal and refund | Stub emits HMAC-signed adjustments; API maps applied results to `Reversed`/`Refunded`. | HMAC utility tested; no full scenario test. |
| Acme timeout | Stub waits 35 seconds; API client timeout is 30 seconds; handler attempts RTTQ using the saved ID. | Client RTTQ lookup tested separately; no timed-out API+stub run. |

## Validation performed

On this review snapshot:

- Acme stub `dotnet build` passed with zero warnings.
- Unit suite passed **9 tests**: Appendix A request/response, unknown-field tolerance, RTTQ request, rate-limit retry identity, non-JSON gateway failure status, Basic/HMAC utilities, atomic stub idempotency/RRN conflict, EFT completion state mapping, and daily-summary aggregation.
- Stub container smoke passed: health, payment, RTTQ, transaction replay, RRN conflict, bearer rejection, and a payment response despite callback retries to an unavailable receiver.
- API source compiled as a referenced project during the unit-test run. Existing restore/compiler warnings remain, including NU1603 package resolution and unrelated API/application warnings; no warning cleanup unrelated to this task was attempted.
- `docker compose config --quiet` passed.
- `bash -n scripts/local-up.sh` passed.

## Remaining actions before claiming full acceptance

1. Record and share the required 5–10 minute hosted walkthrough.
2. Run the full local Compose stack, submit each required scenario through the app API, and verify the persisted state and summary in SQL.
3. Add/execute SQL-backed integration tests for duplicate and out-of-order callbacks, partial refunds, and app-ID concurrency.
4. If credentials/environment become available, verify a sandbox payment and callback round trip; do not substitute local stub evidence for that check.

No AWS deployment is required by the assignment; `SOLUTION.md` documents the proposed approach. The absent video and unrun external/database checks are the only remaining acceptance/evidence actions identified by this review.
