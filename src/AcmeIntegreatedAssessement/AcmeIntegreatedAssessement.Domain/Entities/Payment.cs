using AcmeIntegreatedAssessement.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AcmeIntegreatedAssessement.Domain.Entities
{
    public class Payment : IHasDomainEvent
    {

        public Payment()
        {
            PaymentId = null!;
            AppPaymentId = null!;
            Currency = null!;
            SubMerchantRrn = null!;
            Status = null!;
            AcmeTransactionUuid = null!;
        }

        public Guid Id { get; set; }

        public string PaymentId { get; set; }

        public string AppPaymentId { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; }

        public string SubMerchantRrn { get; set; }

        public string Status { get; set; }

        public string AcmeTransactionUuid { get; set; }

        public Guid SubMerchantId { get; set; }

        public virtual ICollection<PaymentChange>? PaymentChanges { get; set; } = [];

        public virtual ICollection<NotificationDelivery>? NotificationDeliveries { get; set; } = [];

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}