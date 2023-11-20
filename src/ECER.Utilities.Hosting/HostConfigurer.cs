using ECER.Infrastructure.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ECER.Utilities.Hosting;

/// <summary>
/// Helper class to add all configurable components to a hosted context
/// </summary>
public static class HostConfigurer
{
    /// <summary>
    /// Discover and configure all configurable components in the current executing directory
    /// </summary>
    /// <param name="services">The service collection context</param>
    /// <param name="configuration">The configuration context</param>
    public static void Configure(IServiceCollection services, IConfiguration configuration)
    {
        var configurationContext = new ConfigurationContext(services, configuration);
        var assemblies = ReflectionExtensions.DiscoverLocalAessemblies();
        var configurations = assemblies.SelectMany(a => a.CreateInstancesOf<IConfigureComponents>());
        foreach (var config in configurations)
        {
            config.Configure(configurationContext);
        }
    }
}
