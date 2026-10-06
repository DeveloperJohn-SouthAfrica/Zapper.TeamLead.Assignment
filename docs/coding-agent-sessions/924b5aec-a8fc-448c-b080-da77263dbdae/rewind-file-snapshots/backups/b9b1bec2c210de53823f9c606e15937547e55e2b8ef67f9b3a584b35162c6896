# Notification Domain Classes

```mermaid
%%{init: {'theme': 'base', 'themeVariables': {'primaryColor': '#0f172a', 'edgeLabelBackground': '#e0f2fe', 'fontFamily': 'Inter, system-ui, sans-serif'}}}%%
classDiagram
    class NotificationHandler {
        +handleNotification(NotificationEnvelope envelope)
        -validateSignature()
    }

    class NotificationRepository {
        +insertNotification(NotificationRecord record)
        +findByAcmeMessageId(string messageId)
    }

    class NotificationDeduplicator {
        +isDuplicate(NotificationRecord record)
    }

    class PaymentAdviceHandler {
        +handleAdvice(NotificationEnvelope envelope)
        -ensureBasicAuth()
    }

    class TransactionStatusWebhookHandler {
        +handleWebhook(TransactionStatusPayload payload)
        -ensureSignature()
    }

    class SignatureVerifier {
        +verify(payload, signature)
    }

    class PaymentStateTransitionService {
        +applyNotification(NotificationRecord record)
    }

    class PaymentRepository {
        +findByTransactionId(string transactionId)
        +save(PaymentAggregate aggregate)
    }

    class DiagnosticsPublisher {
        +publish(Event event, TraceContext context)
    }

    NotificationHandler --> NotificationRepository
    NotificationHandler --> NotificationDeduplicator
    NotificationHandler --> PaymentStateTransitionService
    PaymentAdviceHandler --> NotificationHandler
    TransactionStatusWebhookHandler --> SignatureVerifier
    TransactionStatusWebhookHandler --> NotificationHandler
    SignatureVerifier --> NotificationHandler
    PaymentStateTransitionService --> PaymentRepository
    PaymentStateTransitionService --> DiagnosticsPublisher
    NotificationRepository --> NotificationDeduplicator
    classDef handler fill:#fde68a,stroke:#92400e,stroke-width:2px,color:#0c0e16,font-weight:600;
    classDef auth fill:#a5b4fc,stroke:#312e81,stroke-width:2px,color:#0c0e16,font-weight:600;
    classDef verify fill:#34d399,stroke:#065f46,stroke-width:2px,color:#0c0e16,font-weight:600;
    classDef service fill:#38bdf8,stroke:#0f172a,stroke-width:2px,color:#0c0e16,font-weight:600;
    class NotificationHandler,PaymentAdviceHandler,TransactionStatusWebhookHandler handler
    class NotificationRepository,PaymentStateTransitionService service
    class NotificationDeduplicator service
    class SignatureVerifier verify
    class DiagnosticsPublisher auth
```

## Notes

- Notification processing is split into validation, persistence, deduplication, and payment-state application to keep each class focused (SOLID).
- `PaymentAdviceHandler` (basic auth) and `TransactionStatusWebhookHandler` (HMAC signature verification via `SignatureVerifier`) share the `NotificationHandler` workflow once authentication/validation succeeds, ensuring identical downstream behavior for advice vs. internal switch events.
- `NotificationDeduplicator` keeps retry handling safe, allowing multiple identical callbacks without violating ACID constraints.
- `DiagnosticsPublisher` captures ISO/IEC 25010 maintainability signals by emitting structured traces for each state change, enabling later observability instrumentation.
- Mermaid `classDef` styling highlights how each handler, verifier, and repository contributes to the notification flow, keeping the diagram consistent with the rest of the analysis assets.
