using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

namespace ECER.Utilities.Hosting;

public static class DataProtectionConfigurationExtensions
{
  public static void ConfigureDataProtection(this IServiceCollection services, DataProtectionSettings? dataProtectionSettings)
  {
    var dpbuilder = services.AddDataProtection();

    if (dataProtectionSettings?.ApplicationName != null) dpbuilder.SetApplicationName(dataProtectionSettings.ApplicationName);

    if (!string.IsNullOrEmpty(dataProtectionSettings?.RedisConnectionString))
    {
      var redis = ConnectionMultiplexer.Connect(dataProtectionSettings.RedisConnectionString);
      dpbuilder.PersistKeysToStackExchangeRedis(redis);
    }
    else if (!string.IsNullOrEmpty(dataProtectionSettings?.DirectoryPath) && Directory.Exists(dataProtectionSettings.DirectoryPath))
    {
      dpbuilder.PersistKeysToFileSystem(new DirectoryInfo(dataProtectionSettings.DirectoryPath));
    }
    else if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")!.Equals("Development", StringComparison.OrdinalIgnoreCase))
    {
      dpbuilder.UseEphemeralDataProtectionProvider();
    }
    else
    {
      throw new InvalidOperationException("Data protection is not configured correctly");
    }
  }
}

public record DataProtectionSettings
{
  public string? DirectoryPath { get; set; }
  public string? RedisConnectionString { get; set; }
  public string? ApplicationName { get; set; }
}
