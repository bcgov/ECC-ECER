using ECER.Infrastructure.Common;
using Microsoft.AspNetCore.Routing;

namespace ECER.Utilities.Hosting;

public static class EndpointsRegistrationExtensions
{
  public static void RegisterApiEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
  {
    var assemblies = ReflectionExtensions.DiscoverLocalAessemblies();
    foreach (var config in assemblies.SelectMany(a => a.CreateInstancesOf<IRegisterEndpoints>()))
    {
      config.Register(endpointRouteBuilder);
    }
  }
}
