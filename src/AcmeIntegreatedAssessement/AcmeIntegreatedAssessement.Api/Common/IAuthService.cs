using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Authentication.Templates.Server.AuthServiceInterfaceTemplate", Version = "1.0")]

namespace AcmeIntegreatedAssessement.Api.Common
{
    public interface IAuthService
    {
        Task Login(string email, string password, bool rememberMe, string returnUrl);
    }
}