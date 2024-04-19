using AutoMapper;
using ECER.Managers.Registry.Contract.MetadataResources;
using ECER.Utilities.Hosting;
using MediatR;

namespace ECER.Clients.RegistryPortal.Server.MetadataResources;

public class MetadataResourcesEndpoints : IRegisterEndpoints
{
  public void Register(IEndpointRouteBuilder endpointRouteBuilder)
  {
    endpointRouteBuilder.MapGet("/api/provincelist", async (HttpContext ctx, IMediator messageBus, IMapper mapper, CancellationToken ct) =>
    {
      var results = await messageBus.Send(new ProvincesQuery(), ct);
      return TypedResults.Ok(mapper.Map<IEnumerable<Province>>(results.Items));
    })
.WithOpenApi("Handles province queries", string.Empty, "province_get")
.RequireAuthorization()
.WithParameterValidation();
  } 

}

public record Province(string ProvinceId, string ProvinceName);
