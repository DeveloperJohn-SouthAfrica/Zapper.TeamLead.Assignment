using AcmeIntegreatedAssessement.Application.Common.Validation;
using AcmeIntegreatedAssessement.Application.Contracts;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AcmeIntegreatedAssessement.Application.Commands.ProcessAcmeReversalNotification
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ProcessAcmeReversalNotificationCommandValidator : AbstractValidator<ProcessAcmeReversalNotificationCommand>
    {
        [IntentManaged(Mode.Merge)]
        public ProcessAcmeReversalNotificationCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        [IntentManaged(Mode.Merge)]
        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.EventType)
                .NotNull();

            RuleFor(v => v.Rrn)
                .NotNull();

            RuleFor(v => v.AppPaymentId)
                .NotNull();

            RuleFor(v => v.Body)
                .NotNull()
                .SetValidator(provider.GetValidator<AcmeReversalNotificationDto>()!);
        }
    }
}