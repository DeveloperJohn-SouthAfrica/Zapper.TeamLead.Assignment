using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AcmeIntegreatedAssessement.Application.Contracts
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class AcmeReversalNotificationDtoValidator : AbstractValidator<AcmeReversalNotificationDto>
    {
        [IntentManaged(Mode.Merge)]
        public AcmeReversalNotificationDtoValidator()
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

            RuleFor(v => v.TransactionUUID)
                .NotNull();

            RuleFor(v => v.TransactionType)
                .NotNull();

            RuleFor(v => v.CurrencyCode)
                .NotNull();

            RuleFor(v => v.TxResultCode)
                .NotNull();
        }
    }
}