using AcmeIntegreatedAssessement.Application.Common.Interfaces;
using AcmeIntegreatedAssessement.Application.Contracts;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AcmeIntegreatedAssessement.Application.Commands.ProcessAcmeApprovedNotification
{
    public class ProcessAcmeApprovedNotificationCommand : IRequest, ICommand
    {
        public ProcessAcmeApprovedNotificationCommand(string eventType,
            bool isApproved,
            Guid acmeNotificationTransactionUuid,
            string rrn,
            decimal amount,
            DateTimeOffset occurredAt,
            string appPaymentId, AcmeApprovedNotificationDto body)
        {
            EventType = eventType;
            IsApproved = isApproved;
            AcmeNotificationTransactionUuid = acmeNotificationTransactionUuid;
            Rrn = rrn;
            Amount = amount;
            OccurredAt = occurredAt;
            AppPaymentId = appPaymentId;
            Body = body;
        }

        public string EventType { get; set; }
        public bool IsApproved { get; set; }
        public Guid AcmeNotificationTransactionUuid { get; set; }
        public string Rrn { get; set; }
        public decimal Amount { get; set; }
        public DateTimeOffset OccurredAt { get; set; }
        public string AppPaymentId { get; set; }
        public AcmeApprovedNotificationDto Body { get; set; }
    }
}