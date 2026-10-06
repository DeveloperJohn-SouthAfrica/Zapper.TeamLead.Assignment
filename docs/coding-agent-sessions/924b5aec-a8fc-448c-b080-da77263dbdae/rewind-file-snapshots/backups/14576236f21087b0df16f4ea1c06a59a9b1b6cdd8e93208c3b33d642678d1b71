# Payment Domain Classes

```mermaid
%%{init: {'theme': 'base', 'themeVariables': {'primaryColor': '#0f172a', 'edgeLabelBackground': '#e0f2fe', 'fontFamily': 'Inter, system-ui, sans-serif'}}}%%
classDiagram
    class PaymentIntakeAPI {
        +submitPayment(PaymentRequest request)
    }

    class PaymentOrchestrator {
        +processPayment(PaymentRequest request)
        -validateRequest()
        -assembleAcmePayload()
    }

    class PaymentAggregate {
        -paymentId: string
        -status: TransactionStatus
        +markPending()
        +markApproved(ResponseDetail response)
        +markDeclined(ResponseDetail response)
        +markReversed()
        +markRefunded()
    }

    class PaymentRepository {
        +save(PaymentAggregate aggregate)
        +findByAppPaymentId(string appPaymentId)
        +beginTransaction()
    }

    class PaymentHistoryRepository {
        +appendHistory(string paymentId, string eventType, string details)
    }

    class AcmeGatewayClient {
        +submitPayment(AcmePaymentPayload payload)
        +configureTimeouts()
    }

    class TokenStore {
        +findToken(string customerId)
        +storeToken(string token, string customerId)
    }

    class RealTimeTransactionQueryService {
        +queryByTransactionId(string transactionId)
        +queryByOrderNumber(string orderNumber)
    }

    PaymentIntakeAPI --> PaymentOrchestrator
    PaymentOrchestrator --> PaymentAggregate
    PaymentOrchestrator --> PaymentRepository
    PaymentOrchestrator --> PaymentHistoryRepository
    PaymentOrchestrator --> AcmeGatewayClient
    PaymentAggregate --> TokenStore
    RealTimeTransactionQueryService --> PaymentRepository
    classDef controller fill:#fde047,stroke:#b45309,stroke-width:2px,color:#0c0e16,font-weight:600;
    classDef service fill:#38bdf8,stroke:#0f172a,stroke-width:2px,color:#0c0e16,font-weight:600;
    classDef repository fill:#a5b4fc,stroke:#312e81,stroke-width:2px,color:#0c0e16,font-weight:600;
    classDef support fill:#34d399,stroke:#065f46,stroke-width:2px,color:#0c0e16,font-weight:600;
    class PaymentIntakeAPI controller
    class PaymentOrchestrator service
    class PaymentAggregate service
    class PaymentRepository,PaymentHistoryRepository repository
    class AcmeGatewayClient,support repository
    class TokenStore support
    class RealTimeTransactionQueryService support
```

## Notes

- `PaymentIntakeAPI` is the entry controller; it is thin (Single Responsibility) and only marshals requests to the orchestrator.
- `PaymentOrchestrator` orchestrates validation, persistence, and gateway interaction while keeping `PaymentAggregate` pristine (DRY across status transitions).
- `PaymentRepository` and `PaymentHistoryRepository` pair in a single ACID transaction so the payment and audit trail move together, satisfying ISO/IEC 25010 reliability.
- `RealTimeTransactionQueryService` reuses the same repositories to answer `/apirttq/query` lookups and prevents duplicate submissions resulting from timed-out calls.
- `TokenStore` is an injected dependency that can be swapped for a caching tier or a shared SQL table; this keeps the design open for future scaling.
- Controllers, services, repositories, and support components are deliberately color-coded via Mermaid `classDef` styling so the diagrams remain legible when we export them for reviews or embed them in the final solution doc.
