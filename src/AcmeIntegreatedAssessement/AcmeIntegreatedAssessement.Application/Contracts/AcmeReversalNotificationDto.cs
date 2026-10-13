using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AcmeIntegreatedAssessement.Application.Contracts
{
    public record AcmeReversalNotificationDto
    {
        public AcmeReversalNotificationDto()
        {
            EventType = null!;
            Rrn = null!;
            AppPaymentId = null!;
            TransactionUUID = null!;
            TransactionType = null!;
            CurrencyCode = null!;
            TxResultCode = null!;
        }

        public string EventType { get; init; }
        public bool IsApproved { get; init; }
        public Guid AcmeNotificationTransactionUuid { get; init; }
        public string Rrn { get; init; }
        public long Amount { get; init; }
        public DateTimeOffset OccurredAt { get; init; }
        public string AppPaymentId { get; init; }
        public string TransactionUUID { get; init; }
        public string TransactionType { get; init; }
        public DateTime TransactionDateTime { get; init; }
        public long TransactionAmount { get; init; }
        public string CurrencyCode { get; init; }
        public string TxResultCode { get; init; }
    }
}