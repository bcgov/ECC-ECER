using System.ComponentModel.DataAnnotations;
using ECER.Managers.Admin.Contract.Files;
using ECER.Utilities.Hosting;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Wolverine;

namespace ECER.Clients.Api.Files;

public class FileEndpoints : IRegisterEndpoints
{
  public void Register(IEndpointRouteBuilder endpointRouteBuilder)
  {
    endpointRouteBuilder.MapGet("/api/files/{fileId}", async Task<Results<FileStreamHttpResult, NotFound>> (
      string fileId,
      [FromHeader(Name = "file-folder")] string? folder,
      IMessageBus messageBus,
      CancellationToken ct) =>
    {
      var results = await messageBus.InvokeAsync<FIleQueryResults>(new FileQuery([new FileLocation(fileId, folder ?? string.Empty)]), ct);
      var file = results.Items.SingleOrDefault();
      if (file == null) return TypedResults.NotFound();
      return TypedResults.Stream(file.Content, file.ContentType, file.FileName);
    })
      .RequireAuthorization("api")
      .WithParameterValidation();

    endpointRouteBuilder.MapPost("/api/files/{fileId}", async (
      [FromForm] UploadFileRequest request,
      [FromRoute] string fileId,
      [FromHeader(Name = "file-classification")][Required] string classification,
      [FromHeader(Name = "file-tag")] string? tags,
      [FromHeader(Name = "file-folder")] string? folder,
      IMessageBus messageBus,
      CancellationToken ct) =>
      {
        var file = new FileData(new FileLocation(fileId, folder ?? string.Empty), request.File.FileName, request.File.ContentType, request.File.OpenReadStream());
        await messageBus.InvokeAsync(new SaveFileCommand([file]), ct);
        return TypedResults.Ok();
      })
      .WithOpenApi("Uploads a new file", string.Empty, "files_post")
      .RequireAuthorization("api")
      .WithParameterValidation();
  }
}

public record UploadFileRequest([Required] IFormFile File);
