using ECER.Infrastructure.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace ECER.Utilities.Hosting;

public class StartupHealthCheck(IServiceProvider services, IConfiguration configuration, ILogger<StartupHealthCheck> logger) : IHealthCheck
{
  public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
  {
    var checkContext = new CheckContext(services, configuration);
    var assemblies = ReflectionExtensions.DiscoverLocalAessemblies();
    var configurations = assemblies.SelectMany(a => a.CreateInstancesOf<IPostConfigureChecker>());
    try
    {
      await Parallel.ForEachAsync(configurations, cancellationToken, async (config, ct) => await config.Check(checkContext, ct));
      return HealthCheckResult.Healthy();
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Error during startup healthcheck");
      return HealthCheckResult.Unhealthy(ex.Message);
    }
  }
}
