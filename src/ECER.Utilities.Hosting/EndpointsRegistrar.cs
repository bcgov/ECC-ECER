using ECER.Infrastructure.Common;
using Microsoft.AspNetCore.Routing;

namespace ECER.Utilities.Hosting;

public static class EndpointsRegistrar
{

    public static void RegisterAll(IEndpointRouteBuilder endpointRouteBuilder){
        var assemblies = ReflectionExtensions.DiscoverLocalAessemblies();
        var configurations = assemblies.SelectMany(a => a.CreateInstancesOf<IRegisterEndpoints>());
        foreach (var config in configurations)
        {
            config.Register(endpointRouteBuilder);
        }
    }
}
