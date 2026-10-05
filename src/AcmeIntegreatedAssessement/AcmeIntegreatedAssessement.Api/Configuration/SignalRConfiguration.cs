using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.SignalR.SignalRConfiguration", Version = "1.0")]

namespace AcmeIntegreatedAssessement.Api.Configuration
{
    public static class SignalRConfiguration
    {
        public static IServiceCollection ConfigureSignalR(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSignalR();
            services.RegisterServices(configuration);
            return services;
        }

        public static IEndpointRouteBuilder MapHubs(this IEndpointRouteBuilder endpoints)
        {
            return endpoints;
        }

        private static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
        }
    }
}