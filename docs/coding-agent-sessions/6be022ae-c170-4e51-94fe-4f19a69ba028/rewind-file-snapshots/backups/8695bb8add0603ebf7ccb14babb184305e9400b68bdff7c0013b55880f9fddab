using System.Threading;
using System.Threading.Tasks;
using AcmeIntegreatedAssessement.Application.Common.Interfaces;
using AcmeIntegreatedAssessement.Domain.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using AcmeIntegreatedAssessement.Domain.Entities;

namespace AcmeIntegreatedAssessement.Infrastructure.Repositories
{
    public class PaymentChangeRepository : IPaymentChangeRepository
    {
        private readonly IUnitOfWork _unitOfWork;

        public PaymentChangeRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddAsync(PaymentChange change, CancellationToken cancellationToken = default)
        {
            await _unitOfWork.Set<PaymentChange>().AddAsync(change, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
