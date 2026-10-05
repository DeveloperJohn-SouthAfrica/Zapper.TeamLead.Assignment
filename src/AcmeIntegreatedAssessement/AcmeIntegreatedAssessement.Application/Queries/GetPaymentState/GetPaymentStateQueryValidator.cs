using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AcmeIntegreatedAssessement.Application.Queries.GetPaymentState
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetPaymentStateQueryValidator : AbstractValidator<GetPaymentStateQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetPaymentStateQueryValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Merge)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.AppPaymentId)
                .NotNull();
        }
    }
}