using AcmeIntegreatedAssessement.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace AcmeIntegreatedAssessement.Infrastructure.Persistence.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.PaymentId)
                    .IsRequired();

            builder.Property(x => x.AppPaymentId)
                    .IsRequired();

            builder.Property(x => x.Amount)
                    .IsRequired();

            builder.Property(x => x.Currency)
                    .IsRequired();

            builder.Property(x => x.SubMerchantRrn)
                    .IsRequired();

            builder.Property(x => x.Status)
                    .IsRequired();

            builder.Property(x => x.AcmeTransactionUuid)
                    .IsRequired();

            builder.Property(x => x.SubMerchantId)
                    .IsRequired();

            builder.OwnsMany(x => x.PaymentChanges, ConfigurePaymentChanges);

            builder.OwnsMany(x => x.NotificationDeliveries, ConfigureNotificationDeliveries);

            builder.Ignore(e => e.DomainEvents);
        }

        public static void ConfigurePaymentChanges(OwnedNavigationBuilder<Payment, PaymentChange> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.PaymentId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PaymentChangeId)
                    .IsRequired();

            builder.Property(x => x.EventType)
                    .IsRequired();

            builder.Property(x => x.IsApproved)
                    .IsRequired();

            builder.Property(x => x.AcmeNotificationTransactionUuid)
                    .IsRequired();

            builder.Property(x => x.Rrn)
                    .IsRequired();

            builder.Property(x => x.Amount)
                    .IsRequired();

            builder.Property(x => x.OccurredAt)
                    .IsRequired();

            builder.Property(x => x.PaymentId)
                    .IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }

        public static void ConfigureNotificationDeliveries(OwnedNavigationBuilder<Payment, NotificationDelivery> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.PaymentsId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.NotificationId)
                    .IsRequired();

            builder.Property(x => x.AppPaymentId)
                    .IsRequired();

            builder.Property(x => x.PaymentId)
                    .IsRequired();

            builder.Property(x => x.NotificationType)
                    .IsRequired();

            builder.Property(x => x.OccurredAt)
                    .IsRequired();

            builder.Property(x => x.IsApplied)
                    .IsRequired();

            builder.Property(x => x.RawPayloadJson)
                    .IsRequired();

            builder.Property(x => x.PaymentsId)
                    .IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }
    }
}