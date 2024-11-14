using AutoMapper;
using ECER.Managers.Admin.Contract.Certifications;
using ECER.Managers.Admin.Contract.Files;
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
      var items = mapper.Map<IEnumerable<CertificationSummary>>(results.Items);
      return TypedResults.Ok(items);
    })
    .WithOpenApi("Returns certification summaries", string.Empty, "certification_get")
    .RequireAuthorization()
    .WithParameterValidation();

    endpointRouteBuilder.MapGet("/api/certifications/file/download/{id?}", async Task<Results<FileStreamHttpResult, BadRequest<string>, NotFound>> (
      string? id,
      IMapper mapper,
      IMediator messageBus,
      HttpContext ctx,
      CancellationToken ct) =>
     {
       var certifications = await messageBus.Send<GetCertificationsCommandResponse>(new GetCertificationsCommand(id), ct);
       var certificate = certifications.Items.SingleOrDefault();

       if (certificate == null || certificate.FileId == null) return TypedResults.NotFound();
       var files = await messageBus.Send(new FileQuery([new FileLocation(certificate!.FileId!, certificate.FilePath ?? string.Empty)]), ct);
       var file = files.Items.SingleOrDefault();
       if (file == null) return TypedResults.NotFound();

       return TypedResults.Stream(file.Content, file.ContentType, file.FileName);
     })
     .WithOpenApi("Downloads a certification summary file", string.Empty, "certificationFile_get")
     .RequireAuthorization()
     .WithParameterValidation();
  }
}

public record CertificationSummary(string Id)
{
  public string? FileName { get; set; }
  public string? FileId { get; set; }
}
