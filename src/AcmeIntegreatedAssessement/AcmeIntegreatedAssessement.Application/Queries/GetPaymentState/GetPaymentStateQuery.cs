using AcmeIntegreatedAssessement.Application.Common.Interfaces;
using AcmeIntegreatedAssessement.Application.Contracts;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AcmeIntegreatedAssessement.Application.Queries.GetPaymentState
{
    public class GetPaymentStateQuery : IRequest<PaymentStateResponseDto>, IQuery
    {
        public GetPaymentStateQuery(string appPaymentId)
        {
            AppPaymentId = appPaymentId;
        }

        public string AppPaymentId { get; set; }
    }
}