using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AcmeIntegreatedAssessement.Application.Contracts
{
    public record SubmitPaymentRequestDto
    {
        public SubmitPaymentRequestDto()
        {
            AppPaymentId = null!;
            Currency = null!;
            ZapperMerchantId = null!;
            CustomerId = null!;
            SubMerchantRrn = null!;
            SubMerchantId = null!;
            StoredCardToken = null!;
            CurrencyCode = null!;
        }

        public string AppPaymentId { get; init; }
        public decimal Amount { get; init; }
        public string Currency { get; init; }
        public string ZapperMerchantId { get; init; }
        public string CustomerId { get; init; }
        public string SubMerchantRrn { get; init; }
        public string SubMerchantId { get; init; }
        public string StoredCardToken { get; init; }
        public string CurrencyCode { get; init; }
    }
}