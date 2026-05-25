using ECER.Managers.Admin.Contract.Certifications;
using ECER.Utilities.Hosting;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ECER.Clients.Api.Certifications;

public class CertificationEndpoints : IRegisterEndpoints
{
  public void Register(IEndpointRouteBuilder endpointRouteBuilder)
  {
    endpointRouteBuilder.MapGet("/api/certifications/files/{id?}", async Task<Results<Ok<IEnumerable<CertificationSummary>>, BadRequest<string>>> (
      string? id,
      ICertificationMapper mapper,
      IMediator messageBus,
      HttpContext ctx,
      CancellationToken ct) =>
    {
      var results = await messageBus.Send<GetCertificationsCommandResponse>(new GetCertificationsCommand(id), ct);
      var items = mapper.MapCertificationSummaries(results.Items);
      return TypedResults.Ok(items);
    })
    .WithOpenApi("Returns certification summaries", string.Empty, "certification_get")
    .RequireAuthorization("ew_user")
    .WithParameterValidation();

    endpointRouteBuilder.MapGet("/api/certifications/file/download/{id}", async Task<Results<FileStreamHttpResult, BadRequest<string>, NotFound>> (
      string id,
      IMediator messageBus,
      HttpContext ctx,
      CancellationToken ct) =>
     {
       var certificationFiles = await messageBus.Send(new GetCertificationFileCommand(id, Utilities.ObjectStorage.Providers.EcerWebApplicationType.Registry), ct);

       var file = certificationFiles.Items.SingleOrDefault();
       if (file == null) return TypedResults.NotFound();

       return TypedResults.Stream(file.Content, file.ContentType, file.FileName);
     })
     .WithOpenApi("Downloads a certification summary file", string.Empty, "certificationFile_get")
     .RequireAuthorization("ew_user")
     .WithParameterValidation();
  }
}

public record CertificationSummary(string Id)
{
  public string? FileName { get; set; }
  public string? FileId { get; set; }
  public DateTime? CreatedOn { get; set; }
}
