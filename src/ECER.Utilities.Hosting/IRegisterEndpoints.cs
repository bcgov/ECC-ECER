using Microsoft.AspNetCore.Routing;

namespace ECER.Utilities.Hosting;

/// <summary>
/// Provider registration services for API endpoints
/// </summary>
public interface IRegisterEndpoints
{
    void Register(IEndpointRouteBuilder endpointRouteBuilder);

}
