using ECER.Infrastructure.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Serilog;

namespace ECER.Utilities.Hosting;

public static class ComponentConfigurationExtensions
{
  public static void ConfigureComponents(this WebApplicationBuilder webApplicationBuilder, Serilog.ILogger logger)
  {
#pragma warning disable CA2000 // Dispose objects before losing scope
    using var loggerFactory = new LoggerFactory().AddSerilog(logger);
#pragma warning restore CA2000 // Dispose objects before losing scope
    var assemblies = ReflectionExtensions.DiscoverLocalAessemblies();
    foreach (var config in assemblies.SelectMany(a => a.CreateInstancesOf<IConfigureComponents>()))
    {
      var configurationContext = new ConfigurationContext(webApplicationBuilder.Services, webApplicationBuilder.Configuration, loggerFactory.CreateLogger(config.GetType()));
      config.Configure(configurationContext);
    }
  }
}
