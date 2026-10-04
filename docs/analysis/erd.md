# Entity Relationship Diagram (ERD)

```mermaid
%%{init: {'theme': 'base', 'themeVariables': {'primaryColor': '#0f172a', 'secondaryColor': '#fb7185', 'tertiaryColor': '#0ea5e9', 'edgeLabelBackground': '#e0f2fe', 'nodeBorder': '#94a3b8', 'fontFamily': 'Inter, system-ui, sans-serif'}}}%%
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
    classDef entity fill: #fcd34d,stroke:#be123c,stroke-width:2px,color:#0f172a,font-weight:600;
    classDef audit fill:#bae6fd,stroke:#1d4ed8,stroke-width:2px,color:#0f172a,font-weight:500;
    class SUBMERCHANT,CUSTOMER,STORED_CARD entity
    class PAYMENT entity
    class PAYMENT_HISTORY,NOTIFICATION audit
```

## Notes

- Every write to `PAYMENT` pairs with a `PAYMENT_HISTORY` entry to satisfy audit and ACID requirements; the colored class legend separates operational tables from auditing tables.
- Notifications live in their own table so retries accumulate without mutating prior rows, supporting the ISO/IEC 25010 reliability expectation.
- This schema remains intentionally minimal to keep the analysis phase stack-independent while still highlighting normalization and indexing for the next iterations.
