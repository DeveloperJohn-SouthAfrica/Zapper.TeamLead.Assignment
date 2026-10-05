using AcmeIntegreatedAssessement.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace AcmeIntegreatedAssessement.Infrastructure.Persistence.Configurations
{
    public class StoredCardTokenConfiguration : IEntityTypeConfiguration<StoredCardToken>
    {
        public void Configure(EntityTypeBuilder<StoredCardToken> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.StoredCardTokenId)
                    .IsRequired();

            builder.Property(x => x.AcmeToken)
                    .IsRequired();

            builder.Property(x => x.CustomerId)
                    .IsRequired();

            builder.Property(x => x.CustomerEmail)
                    .IsRequired();

            builder.Property(x => x.PaymentId)
                    .IsRequired();

            builder.HasOne(x => x.Payment)
                .WithMany()
                .HasForeignKey(x => x.PaymentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}