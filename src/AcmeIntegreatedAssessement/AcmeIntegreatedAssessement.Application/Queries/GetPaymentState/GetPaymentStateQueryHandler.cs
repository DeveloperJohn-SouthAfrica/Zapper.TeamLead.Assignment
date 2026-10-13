using System;
using System.Threading;
using System.Threading.Tasks;
using AcmeIntegreatedAssessement.Application.Common.Interfaces;
using AcmeIntegreatedAssessement.Application.Contracts;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AcmeIntegreatedAssessement.Application.Queries.GetPaymentState
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetPaymentStateQueryHandler : IRequestHandler<GetPaymentStateQuery, PaymentStateResponseDto>
    {
        private readonly IPaymentRepository _paymentRepository;

        [IntentManaged(Mode.Merge)]
        public GetPaymentStateQueryHandler(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<PaymentStateResponseDto> Handle(GetPaymentStateQuery request, CancellationToken cancellationToken)
        {
            var payment = await _paymentRepository.GetByAppPaymentIdAsync(request.AppPaymentId, cancellationToken)
                ?? throw new InvalidOperationException($"Payment '{request.AppPaymentId}' not found.");

            return new PaymentStateResponseDto
            {
                AppPaymentId = payment.AppPaymentId,
                Status = payment.Status,
                Amount = payment.Amount,
                Currency = payment.Currency
            };
        }
    }
}
