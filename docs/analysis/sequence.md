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
