using AutoMapper;
using ECER.Managers.Admin.Contract.Files;
using ECER.Utilities.Hosting;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace ECER.Clients.RegistryPortal.Server.Files;

public class FilesEndpoints : IRegisterEndpoints
{
  public void Register(IEndpointRouteBuilder endpointRouteBuilder)
  {
    endpointRouteBuilder.MapDelete("/api/files/{fileId}", async Task<Results<Ok<FileResponse>, NotFound>> (
        string fileId,
        IMediator messageBus,
        HttpContext ctx,
        IOptions<UploaderSettings> uploaderOptions,
        CancellationToken ct) =>
    {
      var results = await messageBus.Send(new FileQuery([new FileLocation(fileId, uploaderOptions.Value.TempFolderName ?? string.Empty)]), ct);
      var file = results.Items.SingleOrDefault();
      if (file == null) return TypedResults.NotFound();
      await messageBus.Send(new DeleteFileCommand(file), ct);
      return TypedResults.Ok(new FileResponse(fileId));
    })
      .WithOpenApi("Handles delete uploaded file request", string.Empty, "delete_file")
      .RequireAuthorization()
      .WithParameterValidation()
      .DisableAntiforgery();

    endpointRouteBuilder.MapPost("/api/files/{fileId}", async Task<Results<Ok<FileResponse>, BadRequest<ProblemDetails>, NotFound>> (string fileId,
        [FromHeader(Name = "file-classification")][Required] string classification,
        [FromHeader(Name = "file-tag")] string? tags,
        [FromForm(Name = "file")] IFormFile file, HttpContext httpContext, CancellationToken ct, IMediator messageBus, IMapper mapper, IOptions<UploaderSettings> uploaderOptions) =>
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

      var files = httpContext.Request.Form.Files.Select(file => new FileData(new FileLocation(fileId, uploaderOptions.Value.TempFolderName ?? string.Empty), fileProperties, file.FileName, file.ContentType, file.OpenReadStream())).ToList();
      if (files.Count == 0) return TypedResults.BadRequest(new ProblemDetails { Title = "No files were uploaded" });
      var response = await messageBus.Send(new SaveFileCommand(files), ct);
      var saveResult = response.Items.FirstOrDefault();
      if (saveResult == null || !saveResult.IsSuccessful)
      {
        return TypedResults.BadRequest(new ProblemDetails { Title = "Save Failed" });
      }
      return TypedResults.Ok(new FileResponse(fileId));
    })
      .WithOpenApi("Handles upload file request", string.Empty, "upload_file")
      .RequireAuthorization()
      .DisableAntiforgery();
  }
}

/// <summary>
/// file Response
/// </summary>
/// <param name="fileId"></param>
public record FileResponse(string fileId);
