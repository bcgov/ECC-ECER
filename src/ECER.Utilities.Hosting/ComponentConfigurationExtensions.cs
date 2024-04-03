using ECER.Infrastructure.Common;
using Microsoft.AspNetCore.Builder;

namespace ECER.Utilities.Hosting;

public static class ComponentConfigurationExtensions
{
  public static void ConfigureComponents(this WebApplicationBuilder webApplicationBuilder)
  {
    var configurationContext = new ConfigurationContext(webApplicationBuilder.Services, webApplicationBuilder.Configuration);
    var assemblies = ReflectionExtensions.DiscoverLocalAessemblies();
    foreach (var config in assemblies.SelectMany(a => a.CreateInstancesOf<IConfigureComponents>()))
    {
      config.Configure(configurationContext);
    }
  }
}
