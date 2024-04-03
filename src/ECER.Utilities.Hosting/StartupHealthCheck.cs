using ECER.Infrastructure.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ECER.Utilities.Hosting;

public class StartupHealthCheck(IServiceProvider services, IConfiguration configuration) : IHealthCheck
{
  public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
  {
    var checkContext = new CheckContext(services, configuration);
    var assemblies = ReflectionExtensions.DiscoverLocalAessemblies();
    var configurations = assemblies.SelectMany(a => a.CreateInstancesOf<IPostConfigureChecker>());
    try
    {
      foreach (var config in configurations)
      {
        await config.Check(checkContext, cancellationToken);
      }
      return HealthCheckResult.Healthy();
    }
    catch (Exception ex)
    {
      return HealthCheckResult.Unhealthy(ex.Message);
    }
  }
}
