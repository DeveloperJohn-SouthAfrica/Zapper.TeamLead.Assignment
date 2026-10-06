using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace AcmeIntegreatedAssessement.Application.IntegrationTests
{
    public class HomePageTests : BaseIntegrationTest, IClassFixture<IntegrationTestWebAppFactory>
    {
        public HomePageTests(IntegrationTestWebAppFactory webAppFactory) : base(webAppFactory)
        {
        }

        [Fact]
        public async Task Home_page_returns_content()
        {
            var client = CreateClient();
            using var response = await client.GetAsync("/");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("ux-drawer-open", content, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("_framework/blazor.web.js", content, StringComparison.OrdinalIgnoreCase);
        }
    }
}
