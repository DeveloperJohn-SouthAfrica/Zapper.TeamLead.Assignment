using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly IPaymentChangeRepository _paymentChangeRepository;
        private readonly INotificationDeliveryRepository _notificationRepository;

        [IntentManaged(Mode.Merge)]
        public ProcessAcmeReversalNotificationCommandHandler(
            IPaymentRepository paymentRepository,
            IPaymentChangeRepository paymentChangeRepository,
            INotificationDeliveryRepository notificationRepository)
        {
            _paymentRepository = paymentRepository;
            _paymentChangeRepository = paymentChangeRepository;
            _notificationRepository = notificationRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Handle(ProcessAcmeReversalNotificationCommand request, CancellationToken cancellationToken)
        {
            if (await _notificationRepository.ExistsAsync(request.AcmeNotificationTransactionUuid.ToString(), cancellationToken))
            {
                return;
            }

            var payment = await _paymentRepository.GetByAppPaymentIdAsync(request.AppPaymentId, cancellationToken)
                ?? throw new InvalidOperationException($"Payment '{request.AppPaymentId}' not found.");

            var delivery = new NotificationDelivery
            {
                Id = Guid.NewGuid(),
                NotificationId = request.AcmeNotificationTransactionUuid.ToString(),
                AppPaymentId = request.AppPaymentId,
                PaymentId = payment.PaymentId,
                PaymentsId = payment.Id,
                NotificationType = request.EventType,
                OccurredAt = request.OccurredAt.UtcDateTime,
                IsApplied = true,
                RawPayloadJson = JsonSerializer.Serialize(request.Body)
            };
            await _notificationRepository.AddAsync(delivery, cancellationToken);

            var status = request.EventType;
            await _paymentRepository.UpdateStatusAsync(request.AppPaymentId, status, cancellationToken);

            var change = new PaymentChange
            {
                Id = Guid.NewGuid(),
                PaymentChangeId = Guid.NewGuid().ToString(),
                EventType = request.EventType,
                IsApproved = request.IsApproved,
                AcmeNotificationTransactionUuid = request.AcmeNotificationTransactionUuid.ToString(),
                Rrn = request.Rrn,
                Amount = request.Amount,
                OccurredAt = request.OccurredAt.UtcDateTime,
                PaymentId = payment.Id
            };
            await _paymentChangeRepository.AddAsync(change, cancellationToken);
        }
    }
}
