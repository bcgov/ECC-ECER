using AutoMapper;
using ECER.Managers.Admin.Contract.Certifications;
using ECER.Utilities.Hosting;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ECER.Clients.Api.Certifications;

public class CertificationEndpoints : IRegisterEndpoints
{
  public void Register(IEndpointRouteBuilder endpointRouteBuilder)
  {
    endpointRouteBuilder.MapGet("/api/certifications/files/{id?}", async Task<Results<Ok<IEnumerable<CertificationSummary>>, BadRequest<string>>> (
      string? id,
      IMapper mapper,
      IMediator messageBus,
    HttpContext ctx,
    CancellationToken ct) =>
    {
      var results = await messageBus.Send<GetCertificationsCommandResponse>(new GetCertificationsCommand(id), ct);
      return TypedResults.Ok(mapper.Map<IEnumerable<CertificationSummary>>(results.Items));
    })
    .RequireAuthorization()
    .WithParameterValidation();
  }
}

public record CertificationSummary(string Id)
{
  public string? FileName { get; set; }
  public string? FilePath { get; set; }
}
