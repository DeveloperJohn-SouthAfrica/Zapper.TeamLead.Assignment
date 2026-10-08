using System.Text.Json;
using AcmeIntegreatedAssessement.Application.Common.Interfaces;
using AcmeIntegreatedAssessement.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AcmeIntegreatedAssessement.Application.Commands.ProcessAcmeReversalNotification
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ProcessAcmeReversalNotificationCommandHandler : IRequestHandler<ProcessAcmeReversalNotificationCommand>
    {
        private readonly IPaymentRepository _paymentRepository;

        public ProcessAcmeReversalNotificationCommandHandler(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(ProcessAcmeReversalNotificationCommand request, CancellationToken cancellationToken)
        {
            var payment = await _paymentRepository.GetByAppPaymentIdAsync(request.AppPaymentId, cancellationToken)
                ?? throw new InvalidOperationException($"Payment '{request.AppPaymentId}' not found.");
            var isApplied = request.IsApproved && string.Equals(request.Body.TxResultCode, "00", StringComparison.Ordinal);
            var eventType = isApplied
                ? request.EventType.Equals("Refund", StringComparison.OrdinalIgnoreCase) ? "Refunded" : "Reversed"
                : $"{request.EventType}Failed";
            var status = isApplied ? eventType : null;
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
                    IsApproved = isApplied,
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
