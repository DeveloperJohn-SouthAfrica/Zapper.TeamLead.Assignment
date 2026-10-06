using System.Collections.Generic;
using AcmeIntegreatedAssessement.Api;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.IntegrationTestWebAppFactory", Version = "1.0")]

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace AcmeIntegreatedAssessement.Application.IntegrationTests
{
    public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        public IntegrationTestWebAppFactory()
        {
        }

        Task IAsyncLifetime.InitializeAsync()
        {
            return Task.CompletedTask;
        }

        Task IAsyncLifetime.DisposeAsync()
        {
            return DisposeAsync().AsTask();
        }

        public override async ValueTask DisposeAsync()
        {
            await base.DisposeAsync();
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            var result = base.CreateHost(builder);
            return result;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder
                .UseEnvironment("Development")
                .ConfigureAppConfiguration((context, config)
                    => config.AddInMemoryCollection(GetTestSettings()))
                .ConfigureTestServices(services =>
                {
                });
        }

        private static IDictionary<string, string> GetTestSettings()
        {
            return new Dictionary<string, string>
            {
                ["SecretsManager:Enabled"] = "false",
                ["Infrastructure:EnableRedis"] = "false",
                ["AcmePay:BaseUrl"] = "https://example.com/",
                ["AcmePay:TimeoutSeconds"] = "60",
                ["ConnectionStrings:PaymentDatabase"] = "Server=localhost;Database=Payments;User Id=sa;Password=Password123!;TrustServerCertificate=True",
                ["PaymentDatabase:ConnectionStringName"] = "PaymentDatabase"
            };
        }
    }
}