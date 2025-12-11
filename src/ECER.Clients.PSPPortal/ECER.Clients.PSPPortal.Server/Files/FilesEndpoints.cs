using AutoMapper;
using ECER.Clients.PSPPortal.Server.Shared;
using ECER.Managers.Admin.Contract.Files;
using ECER.Managers.Registry.Contract.Communications;
using ECER.Utilities.Hosting;
using ECER.Utilities.Security;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text;

namespace ECER.Clients.PSPPortal.Server.Files;

public class FilesEndpoints : IRegisterEndpoints
{
  public void Register(IEndpointRouteBuilder endpointRouteBuilder)
  {
    endpointRouteBuilder.MapGet("/api/files/communication/{communicationId}/file/{fileId}", async Task<Results<FileStreamHttpResult, BadRequest<ProblemDetails>, NotFound>> (
        string communicationId,
        string fileId,
        IMediator messageBus,
        HttpContext httpContext,
        CancellationToken ct) =>
    {
      bool communicationIdIsNotGuid = !Guid.TryParse(communicationId, out _); if (communicationIdIsNotGuid) { return TypedResults.BadRequest(new ProblemDetails { Title = "Invalid GUID" }); }
      bool fileIdIsNotGuid = !Guid.TryParse(fileId, out _); if (fileIdIsNotGuid) { return TypedResults.BadRequest(new ProblemDetails { Title = "Invalid GUID" }); }

      var query = new UserCommunicationQuery
      {
        ById = communicationId
      };

      var communicationQueryResult = await messageBus.Send<CommunicationsQueryResults>(query, ct);
      if (communicationQueryResult.Items == null || !communicationQueryResult.Items.Any()) return TypedResults.NotFound();

      var communication = communicationQueryResult.Items.FirstOrDefault();
      var communicationFile = communication?.Documents.FirstOrDefault(d => d.Id == fileId);
      if (communicationFile == null) return TypedResults.NotFound();

      var results = await messageBus.Send(new FileQuery([new FileLocation(communicationFile.Id, communicationFile.Url ?? string.Empty)]), ct);
      var file = results.Items.SingleOrDefault();
      if (file == null) return TypedResults.NotFound();

      return TypedResults.Stream(file.Content, file.ContentType, file.FileName);
    })
      .WithOpenApi("Handles fetching files", string.Empty, "files_communication_get")
      .RequireAuthorization("psp_user")
      .WithParameterValidation();

    // This delete just works for temp folder...
    endpointRouteBuilder.MapDelete("/api/files/{fileId}", async Task<Results<Ok<FileResponse>, NotFound>> (
      string fileId,
      IMediator messageBus,
      HttpContext ctx,
      IOptions<UploaderSettings> uploaderOptions,
      CancellationToken ct) =>
  {
    var results = await messageBus.Send(new FileQuery([new FileLocation(fileId, uploaderOptions.Value.TempFolderName ?? string.Empty)], TrackDownload: false), ct);
    var file = results.Items.SingleOrDefault();
    if (file == null) return TypedResults.NotFound();
    await messageBus.Send(new DeleteFileCommand(file), ct);
    return TypedResults.Ok(new FileResponse(fileId));
  })
    .WithOpenApi("Handles delete uploaded file request", string.Empty, "delete_file")
    .RequireAuthorization("psp_user")
    .WithParameterValidation()
    .DisableAntiforgery();

    endpointRouteBuilder.MapPost("/api/files/{fileId}", async Task<Results<Ok<FileResponse>, BadRequest<ProblemDetails>, NotFound>> (string fileId,
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

      // Check if the file extension is allowed
      var fileExtension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
      if (string.IsNullOrEmpty(fileExtension) || !uploaderOptions.Value.AllowedFileTypes.Contains(fileExtension))
      {
        return TypedResults.BadRequest(new ProblemDetails { Title = "Unsupported file type", Detail = $"Supported file types: {string.Join(", ", uploaderOptions.Value.AllowedFileTypes)}" });
      }

      var fileProperties = new FileProperties();
      var sanitizedFilename = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(file.FileName));

      var files = httpContext.Request.Form.Files.Select(file => new FileData(new FileLocation(fileId, uploaderOptions.Value.TempFolderName ?? string.Empty), fileProperties, sanitizedFilename, file.ContentType, file.OpenReadStream())).ToList();
      if (files.Count == 0) return TypedResults.BadRequest(new ProblemDetails { Title = "No files were uploaded" });
      var response = await messageBus.Send(new SaveFileCommand(files), ct);
      var saveResult = response.Items.FirstOrDefault();
      if (saveResult == null || !saveResult.IsSuccessful)
      {
        var message = saveResult == null ? "Save Failed" : saveResult.Message;
        return TypedResults.BadRequest(new ProblemDetails { Title = "Save Failed", Detail = message });
      }
      return TypedResults.Ok(new FileResponse(fileId) { Url = saveResult.FileData.FileLocation.Folder + "/" + fileId });
    })
      .WithOpenApi("Handles upload file request", string.Empty, "upload_file")
      .RequireAuthorization("psp_user")
      .DisableAntiforgery();
  }
}

/// <summary>
/// file Response
/// </summary>
/// <param name="fileId"></param>
public record FileResponse(string fileId)
{
  public string Url { get; set; } = null!;
}
