using System;
using System.Threading;
using System.Threading.Tasks;
using AcmeIntegreatedAssessement.Application.Common.Interfaces;
using AcmeIntegreatedAssessement.Domain.Entities;
using AcmeIntegreatedAssessement.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;
[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AcmeIntegreatedAssessement.Application.Commands.SubmitPayment
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class SubmitPaymentCommandHandler : IRequestHandler<SubmitPaymentCommand>
    {
        private readonly ISubMerchantRepository _subMerchantRepository;
        private readonly IStoredCardTokenRepository _tokenRepository;
        private readonly IAcmePayClient _acmeClient;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPaymentChangeRepository _paymentChangeRepository;

        [IntentManaged(Mode.Merge)]
        public SubmitPaymentCommandHandler(
            ISubMerchantRepository subMerchantRepository,
            IStoredCardTokenRepository tokenRepository,
            IAcmePayClient acmeClient,
            IPaymentRepository paymentRepository,
            IPaymentChangeRepository paymentChangeRepository)
        {
            _subMerchantRepository = subMerchantRepository;
            _tokenRepository = tokenRepository;
            _acmeClient = acmeClient;
            _paymentRepository = paymentRepository;
            _paymentChangeRepository = paymentChangeRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(SubmitPaymentCommand request, CancellationToken cancellationToken)
        {
            if (await _paymentRepository.GetByAppPaymentIdAsync(request.AppPaymentId, cancellationToken) != null)
            {
                return;
            }

            var subMerchant = await _subMerchantRepository.FindFirstAsync(
                sm => sm.ZapperMerchantId == request.ZapperMerchantId, cancellationToken)
                ?? throw new InvalidOperationException($"SubMerchant '{request.ZapperMerchantId}' not found.");

            if (await _tokenRepository.FindFirstAsync(
                    t => t.CustomerId == request.Request.StoredCardToken, cancellationToken) is null)
            {
                throw new InvalidOperationException($"StoredCardToken '{request.Request.StoredCardToken}' not found.");
            }

            var transactionId = await _acmeClient.SubmitPaymentAsync(
                request.ZapperMerchantId,
                request.SubMerchantRrn,
                request.Request.StoredCardToken,
                request.Request.Amount,
                request.Request.CurrencyCode,
                request.AppPaymentId,
                cancellationToken);

            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                PaymentId = Guid.NewGuid().ToString(),
                AppPaymentId = request.AppPaymentId,
                Amount = request.Request.Amount,
                Currency = request.Request.CurrencyCode,
                SubMerchantRrn = request.SubMerchantRrn,
                Status = "Pending",
                AcmeTransactionUuid = transactionId,
                SubMerchantId = subMerchant.Id
            };
            await _paymentRepository.AddAsync(payment, cancellationToken);

            var change = new PaymentChange
            {
                Id = Guid.NewGuid(),
                PaymentChangeId = Guid.NewGuid().ToString(),
                EventType = "Submitted",
                IsApproved = false,
                AcmeNotificationTransactionUuid = string.Empty,
                Rrn = string.Empty,
                Amount = payment.Amount,
                OccurredAt = DateTimeOffset.UtcNow.UtcDateTime,
                PaymentId = payment.Id
            };
            await _paymentChangeRepository.AddAsync(change, cancellationToken);
        }
    }
}
