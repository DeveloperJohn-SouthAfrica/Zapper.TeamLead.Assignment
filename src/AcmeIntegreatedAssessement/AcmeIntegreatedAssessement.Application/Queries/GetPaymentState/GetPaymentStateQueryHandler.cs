using System;
using System.Threading;
using System.Threading.Tasks;
using AcmeIntegreatedAssessement.Application.Common.Interfaces;
using AcmeIntegreatedAssessement.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AcmeIntegreatedAssessement.Application.Queries.GetPaymentState
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetPaymentStateQueryHandler : IRequestHandler<GetPaymentStateQuery, int>
    {
        private readonly IPaymentRepository _paymentRepository;

        [IntentManaged(Mode.Merge)]
        public GetPaymentStateQueryHandler(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<int> Handle(GetPaymentStateQuery request, CancellationToken cancellationToken)
        {
            var payment = await _paymentRepository.GetByAppPaymentIdAsync(request.AppPaymentId, cancellationToken)
                ?? throw new InvalidOperationException($"Payment '{request.AppPaymentId}' not found.");

            return payment.Status switch
            {
                "Pending" => 0,
                "Approved" => 1,
                "Declined" => 2,
                "Reversed" => 3,
                "Refunded" => 4,
                _ => throw new InvalidOperationException($"Unknown payment status '{payment.Status}'")
            };
        }
    }
}
