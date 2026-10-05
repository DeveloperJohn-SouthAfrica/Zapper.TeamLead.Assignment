using System.Threading;
using System.Threading.Tasks;
using AcmeIntegreatedAssessement.Domain.Entities;

namespace AcmeIntegreatedAssessement.Application.Common.Interfaces
{
    public interface IPaymentChangeRepository
    {
        Task AddAsync(PaymentChange change, CancellationToken cancellationToken = default);
    }
}
