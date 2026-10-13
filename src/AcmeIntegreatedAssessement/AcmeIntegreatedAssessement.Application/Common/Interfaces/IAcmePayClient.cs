using System.Threading;
using System.Threading.Tasks;
using AcmeIntegreatedAssessement.Application.Contracts;

namespace AcmeIntegreatedAssessement.Application.Common.Interfaces
{
    public interface IAcmePayClient
    {
        Task<AcmePaymentResult> SubmitPaymentAsync(AcmePaymentSubmission payment, CancellationToken cancellationToken = default);
        Task<AcmePaymentResult?> QueryPaymentAsync(Guid transactionId, CancellationToken cancellationToken = default);
    }
}
