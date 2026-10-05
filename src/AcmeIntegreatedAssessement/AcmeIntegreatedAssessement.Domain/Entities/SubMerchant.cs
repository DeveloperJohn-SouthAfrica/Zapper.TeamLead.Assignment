using AcmeIntegreatedAssessement.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AcmeIntegreatedAssessement.Domain.Entities
{
    public class SubMerchant : IHasDomainEvent
    {

        public SubMerchant()
        {
            SubMerchantAltId = null!;
            ZapperMerchantId = null!;
            TradingName = null!;
            City = null!;
            Region = null!;
            Country = null!;
            MerchantCategoryCode = null!;
        }

        public Guid Id { get; set; }

        public string SubMerchantAltId { get; set; }

        public string ZapperMerchantId { get; set; }

        public string TradingName { get; set; }

        public string City { get; set; }

        public string Region { get; set; }

        public string Country { get; set; }

        public string MerchantCategoryCode { get; set; }

        public virtual ICollection<Payment>? Payments { get; set; } = [];

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}