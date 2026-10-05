using System.Threading;
using System.Threading.Tasks;
using AcmeIntegreatedAssessement.Domain.Entities;

namespace AcmeIntegreatedAssessement.Application.Common.Interfaces
{
    public interface INotificationDeliveryRepository
    {
        Task<bool> ExistsAsync(string notificationId, CancellationToken cancellationToken = default);
        Task AddAsync(NotificationDelivery delivery, CancellationToken cancellationToken = default);
    }
}
