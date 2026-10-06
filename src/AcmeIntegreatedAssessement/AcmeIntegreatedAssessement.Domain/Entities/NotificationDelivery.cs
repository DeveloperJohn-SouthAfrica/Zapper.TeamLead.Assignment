using AcmeIntegreatedAssessement.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AcmeIntegreatedAssessement.Domain.Entities
{
    /// <summary>
    /// Tracks idempotent processing of Acme notifications and audit data.
    /// </summary>
    public class NotificationDelivery : IHasDomainEvent
    {
        public NotificationDelivery()
        {
            NotificationId = null!;
            AppPaymentId = null!;
            PaymentId = null!;
            NotificationType = null!;
            RawPayloadJson = null!;
        }

        public Guid Id { get; set; }

        public string NotificationId { get; set; }

        public string AppPaymentId { get; set; }

        public string PaymentId { get; set; }

        public string NotificationType { get; set; }

        public DateTime OccurredAt { get; set; }

        public bool IsApplied { get; set; }

        public string RawPayloadJson { get; set; }

        public Guid PaymentsId { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}