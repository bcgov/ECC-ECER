using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace ECER.Utilities.Hosting;

public static class DistributedCacheConfigurationExtensions
{
  public static IServiceCollection ConfigureDistributedCache(this IServiceCollection services, DistributedCacheSettings? settings)
  {
    if (settings?.RedisConnectionString != null)
    {
      var configOptions = ConfigurationOptions.Parse(settings.RedisConnectionString);
      var instanceName = settings.InstanceName;
      services.AddStackExchangeRedisCache(opts =>
      {
        opts.ConfigurationOptions = configOptions;
        opts.InstanceName = instanceName;
      });
      services.AddStackExchangeRedisOutputCache(opts =>
      {
        opts.ConfigurationOptions = configOptions;
        opts.InstanceName = instanceName;
      });
    }
    else
    {
      services.AddDistributedMemoryCache();
      services.AddOutputCache();
    }
    return services;
  }
}

public record DistributedCacheSettings
{
  public string? RedisConnectionString { get; set; }
  public string? InstanceName { get; set; }
}
