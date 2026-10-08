using AcmeIntegreatedAssessement.Api.Configuration;
using AcmeIntegreatedAssessement.Api.Components;
using AcmeIntegreatedAssessement.Api.Components.Account;
using AcmeIntegreatedAssessement.Api.Components.Services;
using AcmeIntegreatedAssessement.Api.Filters;
using AcmeIntegreatedAssessement.Api.Logging;
using AcmeIntegreatedAssessement.Application;
using AcmeIntegreatedAssessement.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using Serilog;

namespace AcmeIntegreatedAssessement.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(opt => opt.Filters.Add<ExceptionFilter>())
                .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
            services.AddApplication(Configuration);
            services.ConfigureApplicationSecurity(Configuration);
            services.ConfigureHealthChecks(Configuration);
            services.ConfigureProblemDetails();
            services.ConfigureSignalR(Configuration);
            services.ConfigureBugsnag(Configuration);
            services.AddInfrastructure(Configuration);
            services.AddTelemetryConfiguration(Configuration);
            services.ConfigureSwagger(Configuration);
            services.ConfigureMultiTenancy(Configuration);
            services.AddCascadingAuthenticationState();
            services.AddHttpContextAccessor();
            services.AddScoped<IdentityRedirectManager>();
            services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
            services.AddAuthorization();
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie();

            services.AddRazorComponents()
                .AddInteractiveServerComponents();

            services.AddMudServices();
            services.AddScoped<ThemeService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSerilogRequestLogging();
            app.UseHttpsRedirection();

            if (!env.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseMultiTenancy();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseAntiforgery();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultHealthChecks();
                endpoints.MapControllers();
                endpoints.MapRazorComponents<App>().AddInteractiveServerRenderMode();
                endpoints.MapHubs();
            });

            app.UseSwashbuckle(Configuration);
        }
    }
}
