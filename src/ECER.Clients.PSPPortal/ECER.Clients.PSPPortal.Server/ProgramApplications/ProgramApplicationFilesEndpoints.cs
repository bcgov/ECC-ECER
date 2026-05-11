using AutoMapper;
using ECER.Clients.PSPPortal.Server.Shared;
using ECER.Infrastructure.Common.Validators;
using ECER.Managers.Registry.Contract.ProgramApplications;
using ECER.Managers.Registry.Contract.PspUsers;
using ECER.Utilities.Hosting;
using ECER.Utilities.Security;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text;
using ContractProgramApplicationQuery = ECER.Managers.Registry.Contract.ProgramApplications.ProgramApplicationQuery;

namespace ECER.Clients.PSPPortal.Server.ProgramApplications;

public class ProgramApplicationFilesEndpoints : IRegisterEndpoints
{
  public void Register(IEndpointRouteBuilder endpointRouteBuilder)
  {
    const string policyName = "psp_user";

    endpointRouteBuilder.MapGet(
      "/api/programApplications/{programApplicationId}/files",
      async Task<Results<Ok<IEnumerable<ApplicationFileInfo>>, NotFound>> (
        string programApplicationId,
        HttpContext ctx,
        IMediator messageBus,
        IMapper mapper,
        CancellationToken ct) =>
      {
        var userContext = ctx.User.GetUserContext()!;
        var programRep = (await messageBus.Send<PspRepQueryResults>(new SearchPspRepQuery { ByUserIdentity = userContext.Identity }, ct)).Items.SingleOrDefault();
        if (programRep == null || string.IsNullOrWhiteSpace(programRep.PostSecondaryInstituteId)) return TypedResults.NotFound();

        var existing = await messageBus.Send(new ContractProgramApplicationQuery { ById = programApplicationId, ByPostSecondaryInstituteId = programRep.PostSecondaryInstituteId }, ct);
        if (!existing.Items.Any()) return TypedResults.NotFound();

        var files = await messageBus.Send(new ProgramApplicationFilesQuery(programApplicationId), ct);
        return TypedResults.Ok(mapper.Map<IEnumerable<ApplicationFileInfo>>(files));
      })
      .WithOpenApi("Get all document files for a program application", string.Empty, "program_application_files_get")
      .RequireAuthorization(policyName)
      .AddGuidValidation("programApplicationId")
      .WithParameterValidation();

    endpointRouteBuilder.MapGet(
      "/api/programApplications/{programApplicationId}/documentUrls",
      async Task<Results<Ok<IEnumerable<ApplicationFileInfo>>, NotFound>> (
        string programApplicationId,
        HttpContext ctx,
        IMediator messageBus,
        IMapper mapper,
        CancellationToken ct) =>
      {
        var userContext = ctx.User.GetUserContext()!;
        var programRep = (await messageBus.Send<PspRepQueryResults>(new SearchPspRepQuery { ByUserIdentity = userContext.Identity }, ct)).Items.SingleOrDefault();
        if (programRep == null || string.IsNullOrWhiteSpace(programRep.PostSecondaryInstituteId)) return TypedResults.NotFound();

        var existing = await messageBus.Send(new ContractProgramApplicationQuery { ById = programApplicationId, ByPostSecondaryInstituteId = programRep.PostSecondaryInstituteId }, ct);
        if (!existing.Items.Any()) return TypedResults.NotFound();

        var files = await messageBus.Send(new ProgramApplicationDocumentUrlsQuery(programApplicationId), ct);
        return TypedResults.Ok(mapper.Map<IEnumerable<ApplicationFileInfo>>(files));
      })
      .WithOpenApi("Get all document URLs for a program application", string.Empty, "program_application_document_urls_get")
      .RequireAuthorization(policyName)
      .AddGuidValidation("programApplicationId")
      .WithParameterValidation();

    endpointRouteBuilder.MapPost(
      "/api/programApplications/{programApplicationId}/componentGroups/{componentGroupId}/components/{componentId}/files/{fileId}",
      async Task<Results<Ok<ApplicationFileInfo>, BadRequest<ProblemDetails>, NotFound>> (
        string programApplicationId,
        string componentGroupId,
        string componentId,
        string fileId,
        [FromForm(Name = "file")] IFormFile file,
        HttpContext httpContext,
        IMediator messageBus,
        IMapper mapper,
        IOptions<UploaderSettings> uploaderOptions,
        CancellationToken ct) =>
      {
        if (!Guid.TryParse(fileId, out _))
          return TypedResults.BadRequest(new ProblemDetails { Title = "Invalid file ID" });

        if (file == null || file.Length == 0)
          return TypedResults.BadRequest(new ProblemDetails { Title = "No file uploaded or file is empty" });

        var fileExtension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
        if (string.IsNullOrEmpty(fileExtension) || !uploaderOptions.Value.AllowedFileTypes.Contains(fileExtension))
          return TypedResults.BadRequest(new ProblemDetails { Title = "Unsupported file type", Detail = $"Supported file types: {string.Join(", ", uploaderOptions.Value.AllowedFileTypes)}" });

        var userContext = httpContext.User.GetUserContext()!;
        var programRep = (await messageBus.Send<PspRepQueryResults>(new SearchPspRepQuery { ByUserIdentity = userContext.Identity }, ct)).Items.SingleOrDefault();
        if (programRep == null || string.IsNullOrWhiteSpace(programRep.PostSecondaryInstituteId)) return TypedResults.NotFound();

        var existing = await messageBus.Send(new ContractProgramApplicationQuery { ById = programApplicationId, ByPostSecondaryInstituteId = programRep.PostSecondaryInstituteId }, ct);
        if (!existing.Items.Any()) return TypedResults.NotFound();

        // Duplicate name check across all existing files in the application
        var existingFiles = await messageBus.Send(new ProgramApplicationFilesQuery(programApplicationId), ct);
        if (existingFiles.Any(f => string.Equals(f.FileName, file.FileName, StringComparison.OrdinalIgnoreCase)))
          return TypedResults.BadRequest(new ProblemDetails { Title = "Duplicate file name", Detail = "A file with this name has already been uploaded to this application." });

        var sanitizedFilename = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(file.FileName));
        var command = new UploadProgramApplicationFileCommand(
          fileId, programApplicationId, componentGroupId, componentId,
          programRep.PostSecondaryInstituteId,
          sanitizedFilename, file.ContentType, file.Length, file.OpenReadStream());

        var result = await messageBus.Send(command, ct);
        return TypedResults.Ok(mapper.Map<ApplicationFileInfo>(result));
      })
      .WithOpenApi("Upload a file immediately for a program application component", string.Empty, "program_application_file_upload")
      .RequireAuthorization(policyName)
      .DisableAntiforgery()
      .AddGuidValidation("programApplicationId").AddGuidValidation("componentGroupId").AddGuidValidation("componentId");

    endpointRouteBuilder.MapPost(
      "/api/programApplications/{programApplicationId}/componentGroups/{componentGroupId}/components/{componentId}/files/{documentUrlId}/share",
      async Task<Results<Ok<ApplicationFileInfo>, BadRequest<ProblemDetails>, NotFound>> (
        string programApplicationId,
        string componentGroupId,
        string componentId,
        string documentUrlId,
        HttpContext ctx,
        IMediator messageBus,
        IMapper mapper,
        CancellationToken ct) =>
      {
        var userContext = ctx.User.GetUserContext()!;
        var programRep = (await messageBus.Send<PspRepQueryResults>(new SearchPspRepQuery { ByUserIdentity = userContext.Identity }, ct)).Items.SingleOrDefault();
        if (programRep == null || string.IsNullOrWhiteSpace(programRep.PostSecondaryInstituteId)) return TypedResults.NotFound();

        var existing = await messageBus.Send(new ContractProgramApplicationQuery { ById = programApplicationId, ByPostSecondaryInstituteId = programRep.PostSecondaryInstituteId }, ct);
        if (!existing.Items.Any()) return TypedResults.NotFound();

        var command = new ShareExistingDocumentCommand(documentUrlId, programApplicationId, componentGroupId, componentId);
        var result = await messageBus.Send(command, ct);
        return TypedResults.Ok(mapper.Map<ApplicationFileInfo>(result));
      })
      .WithOpenApi("Share an existing document URL to a program application component", string.Empty, "program_application_file_share")
      .RequireAuthorization(policyName)
      .AddGuidValidation("programApplicationId").AddGuidValidation("componentGroupId").AddGuidValidation("componentId").AddGuidValidation("documentUrlId")
      .WithParameterValidation();

    endpointRouteBuilder.MapDelete(
      "/api/programApplications/{programApplicationId}/files/{shareDocumentUrlId}",
      async Task<Results<Ok, NotFound>> (
        string programApplicationId,
        string shareDocumentUrlId,
        HttpContext ctx,
        IMediator messageBus,
        CancellationToken ct) =>
      {
        var userContext = ctx.User.GetUserContext()!;
        var programRep = (await messageBus.Send<PspRepQueryResults>(new SearchPspRepQuery { ByUserIdentity = userContext.Identity }, ct)).Items.SingleOrDefault();
        if (programRep == null || string.IsNullOrWhiteSpace(programRep.PostSecondaryInstituteId)) return TypedResults.NotFound();

        var existing = await messageBus.Send(new ContractProgramApplicationQuery { ById = programApplicationId, ByPostSecondaryInstituteId = programRep.PostSecondaryInstituteId }, ct);
        if (!existing.Items.Any()) return TypedResults.NotFound();

        await messageBus.Send(new DeleteProgramApplicationFileCommand(shareDocumentUrlId), ct);
        return TypedResults.Ok();
      })
      .WithOpenApi("Delete a shared document URL (and conditionally the underlying file)", string.Empty, "program_application_file_delete")
      .RequireAuthorization(policyName)
      .AddGuidValidation("programApplicationId").AddGuidValidation("shareDocumentUrlId")
      .WithParameterValidation()
      .DisableAntiforgery();
  }
}
