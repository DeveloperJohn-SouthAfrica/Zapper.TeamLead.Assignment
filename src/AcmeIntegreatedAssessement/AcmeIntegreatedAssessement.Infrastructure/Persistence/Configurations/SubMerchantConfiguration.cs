using AcmeIntegreatedAssessement.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace AcmeIntegreatedAssessement.Infrastructure.Persistence.Configurations
{
    public class SubMerchantConfiguration : IEntityTypeConfiguration<SubMerchant>
    {
        public void Configure(EntityTypeBuilder<SubMerchant> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.SubMerchantAltId)
                    .IsRequired();

            builder.Property(x => x.ZapperMerchantId)
                    .IsRequired();

            builder.Property(x => x.TradingName)
                    .IsRequired();

            builder.Property(x => x.City)
                    .IsRequired();

            builder.Property(x => x.Region)
                    .IsRequired();

            builder.Property(x => x.Country)
                    .IsRequired();

            builder.Property(x => x.MerchantCategoryCode)
                    .IsRequired();

            builder.HasMany(x => x.Payments)
                .WithOne()
                .HasForeignKey(x => x.SubMerchantId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}