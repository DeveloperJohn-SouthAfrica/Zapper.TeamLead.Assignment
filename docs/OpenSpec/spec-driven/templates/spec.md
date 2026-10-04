# Data Flow Diagrams (DFD)

## Level 0 - Payment acceptance system

```mermaid
flowchart LR
    subgraph External Actors
        App["Zapper\nApp"]:::actor
        AcmeStub["Acme Pay\nStub"]:::actor
        AcmeAdvice["Acme Payment\nAdvice"]:::actor
        AcmeSwitch["Acme Transaction\nStatus"]:::actor
    end

    subgraph Core System
        Intake["Payment Intake\nAPI"]:::core
        Orchestrator["Payment\nOrchestrator"]:::core
        NotificationHandler["Notification\nEndpoints"]:::core
        SummaryService["Daily Summary\nReporter"]:::core
        RealTimeQuery["Real Time\nTransaction Query"]:::core
        PaymentAdvice["Payment Advice\nEndpoint"]:::core
        TransactionStatus["Transaction Status\nWebhook"]:::core
    end

    subgraph Persistence
        PaymentDB["SQL Store\n(Payments, History,\nNotifications)"]:::data
        TokenStore["Token / Idempotency\nStore"]:::data
    end

    App -->|Payment Request| Intake
    Intake -->|Validate + Context| Orchestrator
    Orchestrator --> PaymentDB
    Orchestrator -->|Submit| AcmeStub
    AcmeStub -->|Callbacks| NotificationHandler
    AcmeAdvice -->|"Notify (Auth)"| PaymentAdvice
    AcmeSwitch -->|"Webhook (HMAC)"| TransactionStatus
    PaymentAdvice --> NotificationHandler
    TransactionStatus --> NotificationHandler
    NotificationHandler --> PaymentDB
    NotificationHandler --> TokenStore
    RealTimeQuery --> PaymentDB
    App -->|RTTQ Query| RealTimeQuery
    SummaryService --> PaymentDB
    TokenStore --> Orchestrator
    SummaryService -->|Reports| App
```

## Notes

- **Data stores** keep audit records and tokens separate to maintain ACID guarantees (transactions update `Payment` + `PaymentHistory` atomically); the color legend differentiates actors, system responsibilities, and persistence surfaces.
- **Notification flow** writes to a ledger-style record before emitting state transitions so retries stay idempotent; the Payment Advice and Transaction Status endpoints share the same handler after authenticating via basic auth or HMAC, respectively.
- **Real Time Transaction Query** lets the App reconcile timed-out submissions without resubmitting, ensuring append-only histories remain consistent while the service reports deterministic outcomes.
- **Summary service** reads the shared stores in a read-only fashion, keeping ISO/IEC 25010 reliability and performance expectations in focus.
# Entity Relationship Diagram (ERD)

```mermaid
erDiagram
    SUBMERCHANT {
        string SubMerchantID PK "Zapper identifier"
        string TradingName
        string City
        string Region
        string Country
        string MerchantCategoryCode
        string CardAcceptorName
    }

    CUSTOMER {
        string CustomerID PK
        string Email
    }

    STORED_CARD {
        string Token PK
        string CustomerID FK
        string Email
        string CardOnFileScenario
    }

    PAYMENT {
        string PaymentID PK
        string AppPaymentID
        string CustomerID FK
        string SubMerchantID FK
        long Amount
        string CurrencyCode
        string TransactionStatus
        string Channel
        datetime CreatedAt
        datetime UpdatedAt
    }

    PAYMENT_HISTORY {
        string HistoryID PK
        string PaymentID FK
        string EventType
        string Details
        datetime OccurredAt
    }

    NOTIFICATION {
        string NotificationID PK
        string PaymentID FK
        string AcmeMessageID
        string TransactionStatus
        string ResponseCode
        string RawPayload
        datetime ReceivedAt
    }

    PAYMENT ||--|{ PAYMENT_HISTORY : records
    PAYMENT ||--|{ NOTIFICATION : influenced_by
    CUSTOMER ||--|{ STORED_CARD : owns
    CUSTOMER ||--|{ PAYMENT : initiates
    SUBMERCHANT ||--|{ PAYMENT : receives
    STORED_CARD }|..|| CUSTOMER : "references"
```

## Notes

- Every write to `PAYMENT` pairs with a `PAYMENT_HISTORY` entry to satisfy audit and ACID requirements; the colored class legend separates operational tables from auditing tables.
- Notifications live in their own table so retries accumulate without mutating prior rows, supporting the ISO/IEC 25010 reliability expectation.
- This schema remains intentionally minimal to keep the analysis phase stack-independent while still highlighting normalization and indexing for the next iterations.
# Sequence Diagrams

## 1. Payment submission flow

```mermaid
sequenceDiagram
    participant App as Zapper<br>App
    participant API as Payment<br>Intake
    participant Orchestrator
    participant Acme as Acme<br>Stub
    participant DB as SQL<br>Store

    App->>API: Submit payment request (customer, sub-merchant, amount, app payment ID)
    API->>Orchestrator: Validate, augment with metadata
    Orchestrator->>DB: Begin transaction, create Payment + History
    Orchestrator->>Acme: POST /api/payment (tokenized card, headers)
    Acme-->>Orchestrator: Response (status, ReconID, codes)
    Orchestrator->>DB: Commit payment update, append history
    Orchestrator-->>App: Return correlation info + pending status
```

## 2. Notification dedup/reordering

```mermaid
sequenceDiagram
    participant Acme as Acme<br>Stub
    participant Notify as Notification<br>Handler
    participant DB as SQL<br>Store
    participant Orchestrator

    Acme->>Notify: Callback (status update)
    Notify->>DB: Insert notification record (idempotent insert)
    DB-->>Notify: Ack
    Notify->>Orchestrator: Map to payment, determine new status
    Orchestrator->>DB: Update payment state + history transactionally
    Notify-->>Acme: 200 OK
```

## 3. Daily summary generation

```mermaid
sequenceDiagram
    participant Scheduler
    participant Summary as Summary<br>Service
    participant DB as SQL<br>Store
    participant Client as Reporting<br>Consumer

    Scheduler->>Summary: Trigger aggregated summary for date, merchant, currency
    Summary->>DB: Query payments grouped by status (approved, declined, reversed, refunded)
    DB-->>Summary: Return aggregates
    Summary->>Client: Provide daily report (counts + totals)
```

## 4. Payment Advice notification

```mermaid
sequenceDiagram
    participant AcmeAdvice as Acme Payment<br>Advice
    participant AdviceEndpoint as Payment Advice<br>Endpoint
    participant NotificationSystem
    participant DB as SQL<br>Store
    participant Orchestrator

    AcmeAdvice->>AdviceEndpoint: POST notification (basic auth, payload)
    AdviceEndpoint->>NotificationSystem: validate auth + envelope
    NotificationSystem->>DB: insert notification record
    DB-->>NotificationSystem: ack
    NotificationSystem->>Orchestrator: apply to PaymentAggregate
    Orchestrator->>DB: update payment + history
    AdviceEndpoint-->>AcmeAdvice: 200 OK
```

## 5. Transaction Status webhook (Reversals/Refunds)

```mermaid
sequenceDiagram
    participant AcmeSwitch as Acme Transaction<br>Status
    participant StatusWebhook as Transaction Status<br>Webhook
    participant SignatureVerifier
    participant NotificationSystem
    participant DB as SQL <br> Store

    AcmeSwitch->>StatusWebhook: POST reversal/refund (x-signature HMAC)
    StatusWebhook->>SignatureVerifier: validate x-signature using secret
    SignatureVerifier-->>StatusWebhook: valid / invalid
    StatusWebhook->>NotificationSystem: process if valid
    NotificationSystem->>DB: append notification + history
    StatusWebhook-->>AcmeSwitch: 200 OK (or 401 on failure)
```

## 6. Real-time transaction query

```mermaid
sequenceDiagram
    participant App as Zapper<br>App
    participant RTTQ as Real Time<br>Transaction Query
    participant DB as SQL<br>Store

    App->>RTTQ: GET /apirttq/query/Merchant/{MerchantID}?TransactionID=xxx
    RTTQ->>DB: locate latest Payment or Notification per TransactionID/OrderNumber
    DB-->>RTTQ: return recorded state (Processing/Success/Failure)
    RTTQ-->>App: respond with PaymentResponse-shaped payload
```

## Notes

- Each diagram is kept small so sub-components can later author their own focused diagrams when we proceed with OO design.
- State transitions rely on idempotent inserts and transactional histories, satisfying ACID and ensuring maintainable, traceable behavior per ISO/IEC 25010.
- Real-time queries reuse the same persistent models to avoid resubmitting timed-out requests and present consistent TransactionStatus.
- All sequence diagrams share the same base theme (Inter font, muted edges, deliberate accent colors) so the flow of time appears cohesive across payment, notification, and reporting scenarios.
# Structured Systems Analysis & Design Phase

This phase remains deliberately stack-agnostic. We are exploring the problem space through data flows, entities, and interactions before selecting languages, frameworks, or an architectural stance. The goal is a modular, high-level design made of independent components that can later be mapped to OO or adaptive patterns.

## Phase deliverables

- **DFD** describing how payments, notification callbacks (payment advice + transaction status), real-time transaction queries, and summaries move through the system and its supporting data stores. _(See [DFD](./dfd.md))_
- **ERD** capturing the persistent model for merchants, cards, payments, and history. _(See [ERD](./erd.md))_
- **Sequence diagrams** that show the lifecycle of a payment request, notifications (including the payment advice webhook and the transaction status webhook), real-time transaction queries, and the daily summary generation. _(See [Sequence diagrams](./sequence.md))_

## High-level component overview

1. **External interfaces**
   - **Payment Intake API**: accepts customer, sub-merchant, and amount. Responsible for request correlation and validation.
   - **Notification endpoints**: publicly reachable handlers that authenticate Acme callbacks, deduplicate, and emit state transitions.
   - **Summary/reporting surface**: exposes per-merchant daily summaries either via API or query.
2. **Core coordination**
   - **Payment orchestration**: builds Acme request, records intent, updates payment status, and surfaces diagnostics.
   - **Acme stub**: imitates the payment gateway with configurable delays, reordering, and idempotency safeguards.
   - **Durable persistence**: applies ACID principles, stores audit trails, and ensures notifications land in a consistent, append-only ledger.
   - **Real Time Transaction Query service**: exposes the `/apirttq/query` pattern so the service can reconcile timed-out submissions and keep outcomes consistent.

3. **Supporting services**
   - **Idempotency/token store** for ensuring retries are safe.
   - **Diagnostics/logging module** that captures correlation IDs, message headers, and environment context for troubleshooting.

## Compliance & guiding constraints

This analysis already keeps ISO/IEC 25010 quality attributes top-of-mind (functional suitability, reliability, performance efficiency, security, maintainability, etc.). The resulting architecture must be extendable into SOLID/DRY responsibilities once we choose a stack, and remain compatible with 12-Factor deployment targets.

The Notification subsystem also covers both **Payment Advice** (basic auth) and **Transaction Status webhooks** (HMAC signatures + retries) so replayed or out-of-order updates stay idempotent. The diagrams now use a coherent style guide derived from Context7-sourced Mermaid class definitions so colors, fonts, and accents reinforce the roles and data paths.

For the next phase we will do Object Oriented analysis and design, referencing these artifacts to derive classes, interfaces, and adapters before validating with an adaptive design approach. The working area for that transition is documented in `docs/design/README.md`.
