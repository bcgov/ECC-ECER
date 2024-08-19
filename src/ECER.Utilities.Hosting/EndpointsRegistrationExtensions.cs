using ECER.Infrastructure.Common;
using Microsoft.AspNetCore.Routing;
using System.Reflection;

namespace ECER.Utilities.Hosting;

public static class EndpointsRegistrationExtensions
{
  public static void RegisterApiEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
  {
    var assemblies = new[] { Assembly.GetCallingAssembly() };
    foreach (var config in assemblies.SelectMany(a => a.CreateInstancesOf<IRegisterEndpoints>()))
    {
      config.Register(endpointRouteBuilder);
    }
  }
}
