using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AcmeIntegreatedAssessement.Application.Contracts
{
    public record AcmeApprovedNotificationDto
    {
        public AcmeApprovedNotificationDto()
        {
            EventType = null!;
            Rrn = null!;
            AppPaymentId = null!;
            RequestType = null!;
            MerchantID = null!;
            TransactionID = null!;
            OrderNumber = null!;
            ReconID = null!;
            RetrievalReferenceNumber = null!;
            SystemsTraceAuditNumber = null!;
            CurrencyCode = null!;
            PaymentService = null!;
            TransactionStatus = null!;
            ResponseSource = null!;
            ResponseCode = null!;
            ResponseDescription = null!;
            AuthCode = null!;
            MaskedCardNumber = null!;
            CommProtocol = null!;
        }

        public string EventType { get; init; }
        public bool IsApproved { get; init; }
        public Guid AcmeNotificationTransactionUuid { get; init; }
        public string Rrn { get; init; }
        public long Amount { get; init; }
        public DateTimeOffset OccurredAt { get; init; }
        public string AppPaymentId { get; init; }
        public string RequestType { get; init; }
        public string MerchantID { get; init; }
        public string TransactionID { get; init; }
        public DateTime TransactionDateTime { get; init; }
        public string OrderNumber { get; init; }
        public string ReconID { get; init; }
        public string RetrievalReferenceNumber { get; init; }
        public string SystemsTraceAuditNumber { get; init; }
        public string CurrencyCode { get; init; }
        public string PaymentService { get; init; }
        public string TransactionStatus { get; init; }
        public string ResponseSource { get; init; }
        public string ResponseCode { get; init; }
        public string ResponseDescription { get; init; }
        public string AuthCode { get; init; }
        public string MaskedCardNumber { get; init; }
        public string CommProtocol { get; init; }
    }
}