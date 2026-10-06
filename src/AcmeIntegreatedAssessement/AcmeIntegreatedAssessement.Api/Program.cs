using AcmeIntegreatedAssessement.Api.Configuration;
using AcmeIntegreatedAssessement.Api.Logging;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Program", Version = "1.0")]

namespace AcmeIntegreatedAssessement.Api
{
    public partial class Program
    {
        public static void Main(string[] args)
        {
            var logger = CreateBootstrapLogger();
            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (HostAbortedException)
            {
                // Excluding HostAbortedException from being logged, as this is an expected
                // exception when working with EF Core migrations (as per the .NET team on the below link)
                // https://github.com/dotnet/efcore/issues/29809#issuecomment-1344101370
            }
            catch (Exception ex)
            {
                logger.Write(LogEventLevel.Fatal, ex, "Unhandled exception");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    if (hostingContext.Configuration.GetValue<bool?>("SecretsManager:Enabled") == true)
                    {
                        config.ConfigureAwsSecretsManager(hostingContext.Configuration);
                    }
                })
                .UseSerilog(
                    (context, services, configuration) => configuration
                        .ReadFrom.Configuration(context.Configuration)
                        .ReadFrom.Services(services)
                        .Destructure.With(new BoundedLoggingDestructuringPolicy()),
                    writeToProviders: true)
                .ConfigureLogging((context, logBuilder) =>
                {
                    logBuilder.ClearProviders();
                    logBuilder.AddTelemetryConfiguration(context);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        }

        private static Serilog.ILogger CreateBootstrapLogger()
        {
            return new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateBootstrapLogger();
        }
    }
}
