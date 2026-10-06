using System.Threading;
using System.Threading.Tasks;
using AcmeIntegreatedAssessement.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Redis.Om.Repositories.Templates.RedisOmUnitOfWork", Version = "1.0")]

namespace AcmeIntegreatedAssessement.Infrastructure.Persistence
{
    internal class NoopRedisOmUnitOfWork : IRedisOmUnitOfWork
    {
        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
