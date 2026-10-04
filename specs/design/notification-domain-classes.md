# Notification Domain Classes

```mermaid
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
```

## Notes

- Notification processing is split into validation, persistence, deduplication, and payment-state application to keep each class focused (SOLID).
- `PaymentAdviceHandler` (basic auth) and `TransactionStatusWebhookHandler` (HMAC signature verification via `SignatureVerifier`) share the `NotificationHandler` workflow once authentication/validation succeeds, ensuring identical downstream behavior for advice vs. internal switch events.
- `NotificationDeduplicator` keeps retry handling safe, allowing multiple identical callbacks without violating ACID constraints.
- `DiagnosticsPublisher` captures ISO/IEC 25010 maintainability signals by emitting structured traces for each state change, enabling later observability instrumentation.
- Mermaid `classDef` styling highlights how each handler, verifier, and repository contributes to the notification flow, keeping the diagram consistent with the rest of the analysis assets.
