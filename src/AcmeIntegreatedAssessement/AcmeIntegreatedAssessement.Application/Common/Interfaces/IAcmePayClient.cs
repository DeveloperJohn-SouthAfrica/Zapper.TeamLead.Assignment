using System.Threading;
using System.Threading.Tasks;

namespace AcmeIntegreatedAssessement.Application.Common.Interfaces
{
    public interface IAcmePayClient
    {
        /// <summary>
        /// Submits a payment request to the Acme Pay stub.
        /// </summary>
        Task<string> SubmitPaymentAsync(string zapperMerchantId,
                                        string subMerchantRrn,
                                        string customerId,
                                        decimal amount,
                                        string currency,
                                        string appPaymentId,
                                        CancellationToken cancellationToken = default);
    }
}
