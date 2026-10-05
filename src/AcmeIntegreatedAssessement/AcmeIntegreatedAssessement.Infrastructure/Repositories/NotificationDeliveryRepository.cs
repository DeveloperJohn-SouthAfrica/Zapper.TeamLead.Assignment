using System.Threading;
using System.Threading.Tasks;
using AcmeIntegreatedAssessement.Application.Common.Interfaces;
using AcmeIntegreatedAssessement.Domain.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using AcmeIntegreatedAssessement.Domain.Entities;

namespace AcmeIntegreatedAssessement.Infrastructure.Repositories
{
    public class NotificationDeliveryRepository : INotificationDeliveryRepository
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificationDeliveryRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> ExistsAsync(string notificationId, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.Set<NotificationDelivery>()
                .AnyAsync(n => n.NotificationId == notificationId, cancellationToken);
        }

        public async Task AddAsync(NotificationDelivery delivery, CancellationToken cancellationToken = default)
        {
            await _unitOfWork.Set<NotificationDelivery>().AddAsync(delivery, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
