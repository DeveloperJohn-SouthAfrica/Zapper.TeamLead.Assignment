using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Hosting;
using Redis.OM;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Redis.Om.Repositories.Templates.IndexCreationService", Version = "1.0")]

namespace AcmeIntegreatedAssessement.Infrastructure.Services
{
    public class IndexCreationService : IHostedService
    {
        private readonly RedisConnectionProvider _provider;

        public IndexCreationService(RedisConnectionProvider provider)
        {
            _provider = provider;
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
        }

        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}