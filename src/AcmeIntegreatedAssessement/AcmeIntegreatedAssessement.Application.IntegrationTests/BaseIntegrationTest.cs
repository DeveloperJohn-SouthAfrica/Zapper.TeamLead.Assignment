using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.BaseIntegrationTest", Version = "1.0")]

namespace AcmeIntegreatedAssessement.Application.IntegrationTests
{
    public class BaseIntegrationTest
    {
        public BaseIntegrationTest(IntegrationTestWebAppFactory webAppFactory)
        {
            WebAppFactory = webAppFactory;
        }

        public IntegrationTestWebAppFactory WebAppFactory { get; }

        protected HttpClient CreateClient()
        {
            var client = WebAppFactory.CreateClient();
            const string tenantHeaderName = "X-Tenant-Identifier";
            if (client.DefaultRequestHeaders.Contains(tenantHeaderName))
            {
                client.DefaultRequestHeaders.Remove(tenantHeaderName);
            }
            client.DefaultRequestHeaders.Add(tenantHeaderName, "tenant1");
            return client;
        }
    }
}