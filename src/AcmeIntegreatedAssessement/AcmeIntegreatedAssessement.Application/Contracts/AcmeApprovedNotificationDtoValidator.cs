using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AcmeIntegreatedAssessement.Application.Contracts
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class AcmeApprovedNotificationDtoValidator : AbstractValidator<AcmeApprovedNotificationDto>
    {
        [IntentManaged(Mode.Merge)]
        public AcmeApprovedNotificationDtoValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Merge)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.EventType)
                .NotNull();

            RuleFor(v => v.Rrn)
                .NotNull();

            RuleFor(v => v.AppPaymentId)
                .NotNull();

            RuleFor(v => v.RequestType)
                .NotNull();

            RuleFor(v => v.MerchantID)
                .NotNull();

            RuleFor(v => v.TransactionID)
                .NotNull();

            RuleFor(v => v.OrderNumber)
                .NotNull();

            RuleFor(v => v.ReconID)
                .NotNull();

            RuleFor(v => v.RetrievalReferenceNumber)
                .NotNull();

            RuleFor(v => v.SystemsTraceAuditNumber)
                .NotNull();

            RuleFor(v => v.CurrencyCode)
                .NotNull();

            RuleFor(v => v.PaymentService)
                .NotNull();

            RuleFor(v => v.TransactionStatus)
                .NotNull();

            RuleFor(v => v.ResponseSource)
                .NotNull();

            RuleFor(v => v.ResponseCode)
                .NotNull();

            RuleFor(v => v.ResponseDescription)
                .NotNull();

            RuleFor(v => v.AuthCode)
                .NotNull();

            RuleFor(v => v.MaskedCardNumber)
                .NotNull();

            RuleFor(v => v.CommProtocol)
                .NotNull();
        }
    }
}