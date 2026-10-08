using System.Text.Json;
using AcmeIntegreatedAssessement.Application.Common.Interfaces;
using AcmeIntegreatedAssessement.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AcmeIntegreatedAssessement.Application.Commands.ProcessAcmeApprovedNotification
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ProcessAcmeApprovedNotificationCommandHandler : IRequestHandler<ProcessAcmeApprovedNotificationCommand>
    {
        private readonly IPaymentRepository _paymentRepository;

        public ProcessAcmeApprovedNotificationCommandHandler(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(ProcessAcmeApprovedNotificationCommand request, CancellationToken cancellationToken)
        {
            var payment = await _paymentRepository.GetByAppPaymentIdAsync(request.AppPaymentId, cancellationToken)
                ?? throw new InvalidOperationException($"Payment '{request.AppPaymentId}' not found.");
            var isComplete = request.IsApproved &&
                             string.Equals(request.Body.TransactionStatus, "Complete", StringComparison.Ordinal) &&
                             string.Equals(request.Body.ResponseCode, "COMPL", StringComparison.Ordinal);
            var status = isComplete ? "Complete" : request.IsApproved ? "Success" : "Failure";
            var eventType = request.IsApproved ? "Approved" : "Declined";
            var occurredAt = request.OccurredAt.UtcDateTime;
            var notificationId = request.AcmeNotificationTransactionUuid.ToString("D");

            await _paymentRepository.ApplyNotificationAsync(
                new NotificationDelivery
                {
                    Id = Guid.NewGuid(),
                    NotificationId = notificationId,
                    AppPaymentId = request.AppPaymentId,
                    PaymentId = payment.PaymentId,
                    PaymentsId = payment.Id,
                    NotificationType = request.EventType,
                    OccurredAt = occurredAt,
                    IsApplied = true,
                    RawPayloadJson = string.IsNullOrEmpty(request.RawPayloadJson)
                        ? JsonSerializer.Serialize(request.Body)
                        : request.RawPayloadJson
                },
                new PaymentChange
                {
                    Id = Guid.NewGuid(),
                    PaymentChangeId = Guid.NewGuid().ToString("D"),
                    EventType = eventType,
                    IsApproved = request.IsApproved,
                    AcmeNotificationTransactionUuid = notificationId,
                    Rrn = request.Rrn,
                    Amount = request.Amount,
                    OccurredAt = occurredAt,
                    PaymentId = payment.Id
                },
                status,
                cancellationToken);
        }
    }
}
