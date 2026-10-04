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
