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
                .NotEmpty();

            RuleFor(v => v.Amount)
                .GreaterThan(0);

            RuleFor(v => v.CardOnFileScenario)
                .Must(value => value is "CIT" or "MIT");

            RuleFor(v => v.CurrencyCode)
                .Must(value => string.IsNullOrWhiteSpace(value) || value.Length == 3 && value.All(char.IsLetter));
        }
    }
}