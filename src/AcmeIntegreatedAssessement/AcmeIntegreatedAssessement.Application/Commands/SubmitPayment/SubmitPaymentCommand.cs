using AcmeIntegreatedAssessement.Application.Common.Interfaces;
using AcmeIntegreatedAssessement.Application.Contracts;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AcmeIntegreatedAssessement.Application.Commands.SubmitPayment
{
    public class SubmitPaymentCommand : IRequest, ICommand
    {
        public SubmitPaymentCommand(string appPaymentId,
            decimal amount,
            string currency,
            string zapperMerchantId,
            string customerId,
            string subMerchantRrn, SubmitPaymentRequestDto request)
        {
            AppPaymentId = appPaymentId;
            Amount = amount;
            Currency = currency;
            ZapperMerchantId = zapperMerchantId;
            CustomerId = customerId;
            SubMerchantRrn = subMerchantRrn;
            Request = request;
        }

        public string AppPaymentId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string ZapperMerchantId { get; set; }
        public string CustomerId { get; set; }
        public string SubMerchantRrn { get; set; }
        public SubmitPaymentRequestDto Request { get; set; }
    }
}