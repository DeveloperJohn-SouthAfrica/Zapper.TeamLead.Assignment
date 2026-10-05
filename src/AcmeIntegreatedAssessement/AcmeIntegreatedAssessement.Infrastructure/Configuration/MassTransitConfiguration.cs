using System.Reflection;
using AcmeIntegreatedAssessement.Application.Common.Eventing;
using AcmeIntegreatedAssessement.Infrastructure.Eventing;
using Intent.RoslynWeaver.Attributes;
using MassTransit;
using MassTransit.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.MassTransitConfiguration", Version = "1.0")]

namespace AcmeIntegreatedAssessement.Infrastructure.Configuration
{
    public static class MassTransitConfiguration
    {
        public static IServiceCollection AddMassTransitConfiguration(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddScoped<MassTransitMessageBus>();
            services.AddScoped<IMessageBus>(provider => provider.GetRequiredService<MassTransitMessageBus>());

            services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();
                x.AddConsumers();
                x.UsingInMemory((context, cfg) =>
                {
                    cfg.UseMessageRetry(r => r.Interval(
                        configuration.GetValue<int?>("MassTransit:RetryInterval:RetryCount") ?? 10,
                        configuration.GetValue<TimeSpan?>("MassTransit:RetryInterval:Interval") ?? TimeSpan.FromSeconds(5)));

                    cfg.ConfigureEndpoints(context);
                    cfg.UseMessageScope(context);
                    cfg.UseInMemoryOutbox(context);
                    cfg.UsePublishFilter(typeof(FinbucklePublishingFilter<>), context);
                    cfg.UseConsumeFilter(typeof(FinbuckleConsumingFilter<>), context);
                    cfg.UseSendFilter(typeof(FinbuckleSendingFilter<>), context);
                });
                x.AddInMemoryInboxOutbox();
            });
            return services;
        }

        private static void AddConsumers(this IRegistrationConfigurator cfg)
        {
        }
    }
}