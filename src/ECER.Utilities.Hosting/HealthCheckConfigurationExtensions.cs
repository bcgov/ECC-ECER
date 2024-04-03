using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;

namespace ECER.Utilities.Hosting;

public static class HealthCheckConfigurationExtensions
{
  public static void ConfigureHealthChecks(this IServiceCollection services)
  {
    services.AddHealthChecks().AddCheck<StartupHealthCheck>("Startup", tags: ["startup"]);
  }

  public static void UseHealthChecks(this WebApplication webApplication)
  {
    webApplication.MapHealthChecks("/health/startup", new HealthCheckOptions
    {
      Predicate = healthCheck => healthCheck.Tags.Contains("startup")
    });

    webApplication.MapHealthChecks("/health/liveness", new HealthCheckOptions
    {
      Predicate = _ => false
    });
  }
}
