# Summary & Reporting Domain Classes

```mermaid
%%{init: {'theme': 'base', 'themeVariables': {'primaryColor': '#0f172a', 'edgeLabelBackground': '#e0f2fe', 'fontFamily': 'Inter, system-ui, sans-serif'}}}%%
classDiagram
    class DailySummaryService {
        +generateSummary(Date reportDate)
    }

    class SummaryQueryBuilder {
        +forDate(Date reportDate)
        +groupBySubMerchantAndCurrency()
    }

    class PaymentRepository {
        +queryPayments(StatusFilter filter)
    }

    class SummaryPresenter {
        +toApiResponse(SummaryData data)
        +toCsv(SummaryData data)
    }

    class SummaryScheduler {
        +enqueueSummaryJobs()
    }

    DailySummaryService --> SummaryQueryBuilder
    DailySummaryService --> PaymentRepository
    DailySummaryService --> SummaryPresenter
    SummaryScheduler --> DailySummaryService
    classDef service fill:#38bdf8,stroke:#0f172a,stroke-width:2px,color:#0c0e16,font-weight:600;
    classDef helper fill:#a5b4fc,stroke:#312e81,stroke-width:2px,color:#0c0e16,font-weight:600;
    class DailySummaryService service
    class SummaryQueryBuilder,SummaryScheduler helper
    class SummaryPresenter helper
```

## Notes

- Summaries are read-only operations that query the payment store without altering state, ensuring ACID writes remain unaffected.
- `DailySummaryService` aggregates by `TransactionStatus` categories defined by the ERD, meeting the business requirements (approved, declined, reversed, refunded).
- Presentation is abstracted so we can add REST, GraphQL, or downstream exports (DRY across output formats).
- The Mermaid styling keeps the scheduler, query builder, and presenter visually distinct so the reporting flow can be read at a glance.
