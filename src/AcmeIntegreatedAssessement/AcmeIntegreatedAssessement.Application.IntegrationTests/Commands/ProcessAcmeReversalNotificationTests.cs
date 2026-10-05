using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AcmeIntegreatedAssessement.Application.IntegrationTests.Commands
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class ProcessAcmeReversalNotificationTests : BaseIntegrationTest
    {
        public ProcessAcmeReversalNotificationTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }
    }
}