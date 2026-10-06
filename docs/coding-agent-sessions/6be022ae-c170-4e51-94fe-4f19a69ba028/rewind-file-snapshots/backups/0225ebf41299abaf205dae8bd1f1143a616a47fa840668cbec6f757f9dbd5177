using System;
using System.Threading;
using System.Threading.Tasks;
using AcmeIntegreatedAssessement.Application.Common.Interfaces;
using AcmeIntegreatedAssessement.Domain.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using AcmeIntegreatedAssessement.Domain.Entities;

namespace AcmeIntegreatedAssessement.Infrastructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly IUnitOfWork _unitOfWork;

        public PaymentRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Payment?> GetByAppPaymentIdAsync(string appPaymentId, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.Set<Payment>()
                .FirstOrDefaultAsync(p => p.AppPaymentId == appPaymentId, cancellationToken);
        }

        public async Task AddAsync(Payment payment, CancellationToken cancellationToken = default)
        {
            await _unitOfWork.Set<Payment>().AddAsync(payment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateStatusAsync(string appPaymentId, string status, CancellationToken cancellationToken = default)
        {
            var payment = await GetByAppPaymentIdAsync(appPaymentId, cancellationToken)
                ?? throw new InvalidOperationException($"Payment '{appPaymentId}' not found.");
            payment.Status = status;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
