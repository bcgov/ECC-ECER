using AutoMapper;
using ECER.Managers.Admin.Contract.Files;
using ECER.Utilities.Hosting;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ECER.Clients.RegistryPortal.Server.Files;

public class FilesEndpoints : IRegisterEndpoints
{
  public void Register(IEndpointRouteBuilder endpointRouteBuilder)
  {
    endpointRouteBuilder.MapPost("/api/files/{fileId}", async Task<Results<Ok<UploadFileResponse>, BadRequest<ProblemDetails>, NotFound>> (string fileId,
        [FromHeader(Name = "file-classification")][Required] string classification,
        [FromHeader(Name = "file-tag")] string? tags,
        [FromForm(Name = "file")] IFormFile file, HttpContext httpContext, CancellationToken ct, IMediator messageBus, IMapper mapper) =>
    {
      if (!Guid.TryParse(fileId, out _))
      {
        return TypedResults.BadRequest(new ProblemDetails { Title = "Invalid GUID" });
      }

      if (file == null || file.Length == 0)
      {
        return TypedResults.BadRequest(new ProblemDetails { Title = "No file uploaded or file is empty" });
      }
      var fileProperties = new FileProperties() { Classification = classification, Tags = tags };
      var folder = "tempfolder";
      var files = httpContext.Request.Form.Files.Select(file => new FileData(new FileLocation(fileId, folder ?? string.Empty), fileProperties, file.FileName, file.ContentType, file.OpenReadStream())).ToList();
      if (files.Count == 0) return TypedResults.BadRequest(new ProblemDetails { Title = "No files were uploaded" });
      await messageBus.Send(new SaveFileCommand(files), ct);
      return TypedResults.Ok(new UploadFileResponse(string.Empty));
    })
      .WithOpenApi("Handles upload file request", string.Empty, "upload_file")
      .RequireAuthorization()
      .DisableAntiforgery();
  }
}

/// <summary>
/// upload file Response
/// </summary>
/// <param name="fileId"></param>
public record UploadFileResponse(string fileId);
