﻿using ECER.Managers.Admin.Contract.Files;
using ECER.Utilities.Hosting;
using MediatR;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ECER.Clients.Api.Files;

public class FileEndpoints : IRegisterEndpoints
{
  private const int FILE_MAX_SIZE = 100_000_000;

  public void Register(IEndpointRouteBuilder endpointRouteBuilder)
  {
    endpointRouteBuilder.MapGet("/api/files/{fileId}", async Task<Results<FileStreamHttpResult, NotFound>> (
        string fileId,
        [FromHeader(Name = "file-folder")] string? folder,
        IMediator messageBus,
        HttpContext ctx,
        CancellationToken ct) =>
      {
        var results = await messageBus.Send(new FileQuery([new FileLocation(fileId, folder ?? string.Empty)]), ct);
        var file = results.Items.SingleOrDefault();
        if (file == null) return TypedResults.NotFound();

        ctx.Response.Headers.Append("file-folder", file.FileLocation.Folder);
        ctx.Response.Headers.Append("file-tag", file.FileProperties.Tags);
        ctx.Response.Headers.Append("file-classification", file.FileProperties.Classification);
        return TypedResults.Stream(file.Content, file.ContentType, file.FileName);
      })
      .RequireAuthorization()
      .WithParameterValidation();

    endpointRouteBuilder.MapPost("/api/files/{fileId}", async Task<Results<Ok<FileResponse>, BadRequest<ProblemDetails>, NotFound>> (
        [FromRoute] string fileId,
        [FromHeader(Name = "file-classification")][Required] string classification,
        [FromHeader(Name = "file-tag")] string? tags,
        [FromHeader(Name = "file-folder")] string? folder,
        HttpContext httpContext,
        IMediator messageBus,
        CancellationToken ct) =>
      {
        //modify the max request body size for this requeset
        var feature = httpContext.Features.Get<IHttpMaxRequestBodySizeFeature>();
        if (feature != null) feature.MaxRequestBodySize = FILE_MAX_SIZE;

        var fileProperties = new FileProperties() { Classification = classification, Tags = tags };
        var files = httpContext.Request.Form.Files.Select(file => new FileData(new FileLocation(fileId, folder ?? string.Empty), fileProperties, file.FileName, file.ContentType, file.OpenReadStream())).ToList();
        if (files.Count == 0) return TypedResults.BadRequest(new ProblemDetails { Detail = "No files were uploaded" });
        var response = await messageBus.Send(new SaveFileCommand(files), ct);
        var saveResult = response.Items.FirstOrDefault();
        if (saveResult == null || !saveResult.IsSuccessful)
        {
          var message = saveResult == null ? "Save Failed" : saveResult.Message;
          return TypedResults.BadRequest(new ProblemDetails { Title = "Save Failed", Detail = message });
        }
        return TypedResults.Ok(new FileResponse(fileId));
      })
      .WithOpenApi("Uploads a new file", string.Empty, "files_post")
      .DisableAntiforgery()
      .RequireAuthorization()
      .WithParameterValidation();
  }
}

/// <summary>
/// file Response
/// </summary>
/// <param name="fileId"></param>
public record FileResponse(string fileId);
