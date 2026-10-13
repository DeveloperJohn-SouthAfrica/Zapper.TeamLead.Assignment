using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AcmeIntegreatedAssessement.Application.Contracts
{
    public record PaymentStateResponseDto
    {
        public PaymentStateResponseDto()
        {
            AppPaymentId = null!;
            Status = null!;
            Currency = null!;
        }

        public string AppPaymentId { get; init; }
        public string Status { get; init; }
        public long Amount { get; init; }
        public string Currency { get; init; }
    }
}