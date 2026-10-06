# Implementation Plan — zapper-team-lead-assignment

## Tasks

- [x] T1. Prerequisites & bootstrapping
  - [x] T1.1 [app] (satisfies: Task 0) — Ensure application `AcmeIntegreatedAssessement` exists and is configured
    - Create/confirm the application output area so later [code] tasks can write to the correct repo paths.

- [ ] T2. Payment processing & notifications (Domain + Services)
  - [x] T2.1 [model] (satisfies: Task 1) — Model `SubMerchant`
    - In `Domain` designer: create `SubMerchant` entity with fields: `ZapperMerchantId`, `TradingName`, `City`, `Region`, `Country`, `MerchantCategoryCode`.
  - [x] T2.2 [model] (satisfies: Task 1) — Model `StoredCardToken`
    - In `Domain` designer: create `StoredCardToken` entity with fields: `AcmeToken`, `CustomerId`, `CustomerEmail`.
  - [x] T2.3 [model] (satisfies: Task 1) — Model `Payment`
    - In `Domain` designer: create `Payment` entity with fields: `AppPaymentId`, `Amount`, `Currency`, `SubMerchantRrn`, `Status`, `AcmeTransactionUuid`.
    - Associations: `SubMerchant (1) -> Payment (0..*)` and `StoredCardToken (0..*) -> Payment (1)`.
  - [x] T2.4 [model] (satisfies: Task 1) — Model `PaymentChange`
    - In `Domain` designer: create `PaymentChange` entity with fields: `EventType`, `IsApproved`, `AcmeNotificationTransactionUuid`, `Rrn`, `Amount`, `OccurredAt`.
    - Association: `Payment (1) -> PaymentChange (0..*)`.
  - [x] T2.5 [model] (satisfies: Task 1) — Model notification payload DTOs + idempotency/audit support
    - In `Services` designer: create/define DTOs for Acme notification payloads (approved/declined and adjustment) plus any signature/verification inputs needed by operations.
    - Ensure modeled operations capture: message identifiers, RRN/transaction identifiers, approval flags, amounts, and occurred timestamps (as available in Appendix A).
  - [x] T2.6 [model] (satisfies: Task 1) — Add Payments endpoints (request, query, notifications)
    - In `Services` designer (CQRS style): model operations for
      - POST `/api/payments` (submit payment request)
      - GET `/api/payments/{appPaymentId}` (query current state)
      - POST Acme lifecycle notifications endpoint(s)
        - Approved/declined transaction endpoint
        - Reversal/refund adjustment endpoint
  - [ ] T2.7 [code] (satisfies: Task 1) — Implement submit payment request handler(s)
    - Implement operation body for POST `/api/payments`:
      - Lookup `StoredCardToken` and `SubMerchant`.
      - Construct Acme PaymentRequest from model data.
      - Call Acme stub.
      - Persist `Payment` record.
      - Persist initial `PaymentChange` (if the model uses it).
      - Handle Acme behaviours: slow responses via timeout policy.
      - Handle duplicate submission (same `appPaymentId`) idempotently.
  - [ ] T2.8 [code] (satisfies: Task 1) — Implement query payment state
    - Implement operation body for GET `/api/payments/{appPaymentId}`:
      - Retrieve `Payment` + current status.
      - Return DTO response.
  - [ ] T2.9 [code] (satisfies: Task 1) — Implement notification processing (approved/declined)
    - Implement operation body for POST lifecycle notification endpoint:
      - Verify signature/authentication per Appendix A.
      - Idempotently upsert notification effects (avoid duplicate/conflicting records).
      - Apply out-of-order notification handling by using timestamps/occurredAt ordering rules in persistence layer.
      - Update `Payment` status and create `PaymentChange`.
  - [ ] T2.10 [code] (satisfies: Task 1) — Implement notification processing (reversal/refund)
    - Implement operation body for POST reversal/refund endpoint:
      - Verify signature/authentication per Appendix A.
      - Idempotently upsert event effects.
      - Update `Payment` status (reversed/refunded semantics as modelled) and create `PaymentChange`.

  - [ ] T2.11 [code] (satisfies: Task 1) — Write unit/integration tests for Task 1 behaviours
    - Cover required scenarios:
      - approved payment
      - declined payment
      - duplicate payment request (same appPaymentId)
      - duplicate notifications
      - out-of-order notifications
      - reversal after approval
      - refund after approval
      - Acme timeout before response
      - include enough assertions to verify no conflicting duplicates in DB records

- [ ] T3. Data model & daily reporting (SQL + endpoint)
  - [ ] T3.1 [model] (satisfies: Task 2) — Model daily summary query/export contract
    - In `Services` designer: add a daily summary operation/endpoint contract:
      - GET `/api/reports/daily-payments?date=...`
      - response grouped by sub-merchant and currency with counts and totals for approved/declined/reversals/refunds.
  - [ ] T3.2 [code] (satisfies: Task 2) — Implement daily summary query/report
    - Implement operation body for daily summary:
      - Use SQL projection or LINQ-to-Entities strategy.
      - Compute counts and total values for each category.
      - Group by sub-merchant + currency.
  - [ ] T3.3 [code] (satisfies: Task 2) — Write tests for daily summary grouping
    - Unit/integration tests validating:
      - grouping by sub-merchant and currency
      - totals for each payment status bucket
      - date filter correctness

- [ ] T4. AWS readiness / SOLUTION.md content (documentation)
  - [ ] T4.1 [code] (satisfies: Task 3) — Write SOLUTION.md AWS/runbook reasoning
    - Add content to `SOLUTION.md` covering AWS deploy approach, config/secrets, scaling, graceful shutdown, and diagnostics.

- [ ] T5. AI-assisted development documentation (documentation)
  - [ ] T5.1 [code] (satisfies: Task 4) — Write SOLUTION.md AI workflow details
    - Add content about AI tools used, how context was prepared, what was delegated vs retained, review/iteration steps.

## Task Dependency Graph

```json
{
  "waves": [
    { "id": 1, "tasks": ["T1.1", "T2.1", "T2.2", "T2.3", "T2.4", "T2.5", "T2.6"], "label": "Model payments domain + endpoints" },
    { "id": 2, "tasks": ["T2.7", "T2.8", "T2.9", "T2.10", "T2.11"], "label": "Implement payments + notifications" },
    { "id": 3, "tasks": ["T3.1", "T3.2", "T3.3"], "label": "Daily summary" },
    { "id": 4, "tasks": ["T4.1", "T5.1"], "label": "Docs only" }
  ]
}
```
