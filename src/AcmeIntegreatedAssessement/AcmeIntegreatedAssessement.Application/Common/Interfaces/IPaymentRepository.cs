using System.Threading;
using System.Threading.Tasks;
using AcmeIntegreatedAssessement.Domain.Entities;

namespace AcmeIntegreatedAssessement.Application.Common.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetByAppPaymentIdAsync(string appPaymentId, CancellationToken cancellationToken = default);
        Task<Payment?> GetByAcmeTransactionUuidAsync(Guid transactionId, CancellationToken cancellationToken = default);
        Task<Payment?> GetBySubMerchantRrnAsync(string rrn, CancellationToken cancellationToken = default);
        Task AddAsync(Payment payment, CancellationToken cancellationToken = default);
        Task ApplyNotificationAsync(NotificationDelivery delivery, PaymentChange change, string? paymentStatus, CancellationToken cancellationToken = default);
        Task UpdateStatusAsync(string appPaymentId, string status, CancellationToken cancellationToken = default);
    }
}
