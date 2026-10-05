using AcmeIntegreatedAssessement.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AcmeIntegreatedAssessement.Domain.Entities
{
    public class PaymentChange : IHasDomainEvent
    {

        public PaymentChange()
        {
            PaymentChangeId = null!;
            EventType = null!;
            AcmeNotificationTransactionUuid = null!;
            Rrn = null!;
        }

        public Guid Id { get; set; }

        public string PaymentChangeId { get; set; }

        public string EventType { get; set; }

        public bool IsApproved { get; set; }

        public string AcmeNotificationTransactionUuid { get; set; }

        public string Rrn { get; set; }

        public decimal Amount { get; set; }

        public DateTime OccurredAt { get; set; }

        public Guid PaymentId { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}