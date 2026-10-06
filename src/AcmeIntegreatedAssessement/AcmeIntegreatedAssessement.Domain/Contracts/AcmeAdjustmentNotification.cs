using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace AcmeIntegreatedAssessement.Domain.Contracts
{
    public record AcmeAdjustmentNotification
    {
        public AcmeAdjustmentNotification(string transactionUUID,
            string transactionType,
            DateTime transactionDateTime,
            string rrn,
            long transactionAmount,
            string currencyCode,
            string txResultCode,
            bool isApproved)
        {
            TransactionUUID = transactionUUID;
            TransactionType = transactionType;
            TransactionDateTime = transactionDateTime;
            Rrn = rrn;
            TransactionAmount = transactionAmount;
            CurrencyCode = currencyCode;
            TxResultCode = txResultCode;
            IsApproved = isApproved;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Fully)]
        protected AcmeAdjustmentNotification()
        {
            TransactionUUID = null!;
            TransactionType = null!;
            Rrn = null!;
            CurrencyCode = null!;
            TxResultCode = null!;
        }

        public string TransactionUUID { get; init; }
        public string TransactionType { get; init; }
        public DateTime TransactionDateTime { get; init; }
        public string Rrn { get; init; }
        public long TransactionAmount { get; init; }
        public string CurrencyCode { get; init; }
        public string TxResultCode { get; init; }
        public bool IsApproved { get; init; }
    }
}