using AcmeIntegreatedAssessement.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AcmeIntegreatedAssessement.Domain.Entities
{
    public class StoredCardToken : IHasDomainEvent
    {

        public StoredCardToken()
        {
            StoredCardTokenId = null!;
            AcmeToken = null!;
            CustomerId = null!;
            CustomerEmail = null!;
            Payment = null!;
        }

        public Guid Id { get; set; }

        public string StoredCardTokenId { get; set; }

        public string AcmeToken { get; set; }

        public string CustomerId { get; set; }

        public string CustomerEmail { get; set; }

        public Guid PaymentId { get; set; }

        public virtual Payment Payment { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}