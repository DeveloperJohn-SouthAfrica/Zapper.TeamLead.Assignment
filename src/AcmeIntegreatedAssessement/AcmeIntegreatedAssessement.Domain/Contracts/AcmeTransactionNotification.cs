using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace AcmeIntegreatedAssessement.Domain.Contracts
{
    public record AcmeTransactionNotification
    {
        public AcmeTransactionNotification(string requestType,
            string merchantID,
            string transactionID,
            DateTime transactionDateTime,
            string orderNumber,
            string reconID,
            string retrievalReferenceNumber,
            string systemsTraceAuditNumber,
            long amount,
            string currencyCode,
            string paymentService,
            string transactionStatus,
            string responseSource,
            string responseCode,
            string responseDescription,
            string authCode,
            string maskedCardNumber,
            string commProtocol)
        {
            RequestType = requestType;
            MerchantID = merchantID;
            TransactionID = transactionID;
            TransactionDateTime = transactionDateTime;
            OrderNumber = orderNumber;
            ReconID = reconID;
            RetrievalReferenceNumber = retrievalReferenceNumber;
            SystemsTraceAuditNumber = systemsTraceAuditNumber;
            Amount = amount;
            CurrencyCode = currencyCode;
            PaymentService = paymentService;
            TransactionStatus = transactionStatus;
            ResponseSource = responseSource;
            ResponseCode = responseCode;
            ResponseDescription = responseDescription;
            AuthCode = authCode;
            MaskedCardNumber = maskedCardNumber;
            CommProtocol = commProtocol;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Fully)]
        protected AcmeTransactionNotification()
        {
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

        public string RequestType { get; init; }
        public string MerchantID { get; init; }
        public string TransactionID { get; init; }
        public DateTime TransactionDateTime { get; init; }
        public string OrderNumber { get; init; }
        public string ReconID { get; init; }
        public string RetrievalReferenceNumber { get; init; }
        public string SystemsTraceAuditNumber { get; init; }
        public long Amount { get; init; }
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