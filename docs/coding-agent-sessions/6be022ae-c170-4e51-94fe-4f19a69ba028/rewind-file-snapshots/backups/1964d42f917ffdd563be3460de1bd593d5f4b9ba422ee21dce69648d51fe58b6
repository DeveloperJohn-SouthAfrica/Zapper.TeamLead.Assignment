using AcmeIntegreatedAssessement.Api.Components;
using AcmeIntegreatedAssessement.Api.Components.Account;
using AcmeIntegreatedAssessement.Api.Components.Services;
using AcmeIntegreatedAssessement.Api.Configuration;
using AcmeIntegreatedAssessement.Api.Filters;
using AcmeIntegreatedAssessement.Api.Logging;
using AcmeIntegreatedAssessement.Application;
using AcmeIntegreatedAssessement.Infrastructure;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using MudBlazor.Services;
using Serilog;
using Serilog.Events;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Program", Version = "1.0")]

namespace AcmeIntegreatedAssessement.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using var logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateBootstrapLogger();

            try
            {
                var builder = WebApplication.CreateBuilder(args);

                // Add services to the container.
                if (builder.Configuration.GetValue<bool?>("SecretsManager:Enabled") == true)
                {
                    builder.Configuration.ConfigureAwsSecretsManager(builder.Configuration);
                }

                builder.Host.UseSerilog((context, services, configuration) => configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Destructure.With(new BoundedLoggingDestructuringPolicy()), writeToProviders: true);

                builder.Host.ConfigureLogging((context, logBuilder) =>
                {
                    logBuilder.ClearProviders();
                    logBuilder.AddTelemetryConfiguration(context);
                });

                builder.Services.AddControllers(
                    opt =>
                    {
                        opt.Filters.Add<ExceptionFilter>();
                    });
                builder.Services.AddApplication(builder.Configuration);
                builder.Services.ConfigureApplicationSecurity(builder.Configuration);
                builder.Services.ConfigureHealthChecks(builder.Configuration);
                builder.Services.ConfigureProblemDetails();
                builder.Services.ConfigureSignalR(builder.Configuration);
                builder.Services.ConfigureBugsnag(builder.Configuration);
                builder.Services.AddInfrastructure(builder.Configuration);
                builder.Services.AddTelemetryConfiguration(builder.Configuration);
                builder.Services.ConfigureSwagger(builder.Configuration);
                builder.Services.ConfigureMultiTenancy(builder.Configuration);
                builder.Services.AddCascadingAuthenticationState();
                builder.Services.AddHttpContextAccessor();
                builder.Services.AddScoped<IdentityRedirectManager>();
                builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
                builder.Services.AddAuthorization();
                builder.Services.AddAuthentication(options =>
                                            {
                                                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                                            }).AddCookie();

                builder.Services.AddRazorComponents()
                    .AddInteractiveServerComponents();

                builder.Services.AddMudServices();
                builder.Services.AddScoped<ThemeService>();

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                app.UseSerilogRequestLogging();
                app.UseHttpsRedirection();

                if (!app.Environment.IsDevelopment())
                {
                    app.UseExceptionHandler("/Error");
                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                    app.UseHsts();
                }
                app.UseStaticFiles();
                app.UseRouting();
                app.UseMultiTenancy();
                app.UseAuthentication();
                app.UseAuthorization();
                app.UseAntiforgery();
                app.MapDefaultHealthChecks();
                app.MapControllers();

                app.MapRazorComponents<App>()
                    .AddInteractiveServerRenderMode();
                app.MapHubs();
                app.UseSwashbuckle(builder.Configuration);

                logger.Write(LogEventLevel.Information, "Starting web host");

                app.Run();
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
        }
    }
}