using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AcmeIntegreatedAssessement.Application.Contracts
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class SubmitPaymentRequestDtoValidator : AbstractValidator<SubmitPaymentRequestDto>
    {
        [IntentManaged(Mode.Merge)]
        public SubmitPaymentRequestDtoValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Merge)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.AppPaymentId)
                .NotNull();

            RuleFor(v => v.Currency)
                .NotNull();

            RuleFor(v => v.ZapperMerchantId)
                .NotNull();

            RuleFor(v => v.CustomerId)
                .NotNull();

            RuleFor(v => v.SubMerchantRrn)
                .NotNull();

            RuleFor(v => v.SubMerchantId)
                .NotNull();

            RuleFor(v => v.StoredCardToken)
                .NotNull();

            RuleFor(v => v.CurrencyCode)
                .NotNull();
        }
    }
}