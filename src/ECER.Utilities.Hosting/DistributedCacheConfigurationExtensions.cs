using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace ECER.Utilities.Hosting;

public static class DistributedCacheConfigurationExtensions
{
  public static void ConfigureDistributedCache(this IServiceCollection services, DistributedCacheSettings? settings)
  {
    if (settings?.RedisConnectionString != null)
    {
      services.AddStackExchangeRedisCache(opts => { opts.ConfigurationOptions = ConfigurationOptions.Parse(settings.RedisConnectionString); });
    }
    else
    {
      services.AddDistributedMemoryCache();
    }
  }
}

public record DistributedCacheSettings
{
  public string? RedisConnectionString { get; set; }
}
