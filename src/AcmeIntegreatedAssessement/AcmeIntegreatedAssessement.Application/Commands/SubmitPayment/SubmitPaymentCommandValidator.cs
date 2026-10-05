using AcmeIntegreatedAssessement.Application.Common.Validation;
using AcmeIntegreatedAssessement.Application.Contracts;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AcmeIntegreatedAssessement.Application.Commands.SubmitPayment
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class SubmitPaymentCommandValidator : AbstractValidator<SubmitPaymentCommand>
    {
        [IntentManaged(Mode.Merge)]
        public SubmitPaymentCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        [IntentManaged(Mode.Merge)]
        private void ConfigureValidationRules(IValidatorProvider provider)
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

            RuleFor(v => v.Request)
                .NotNull()
                .SetValidator(provider.GetValidator<SubmitPaymentRequestDto>()!);
        }
    }
}