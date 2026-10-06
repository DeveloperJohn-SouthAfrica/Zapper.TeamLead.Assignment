using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AcmeIntegreatedAssessement.Application.IntegrationTests.Queries
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetPaymentStateTests : BaseIntegrationTest
    {
        public GetPaymentStateTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }
    }
}