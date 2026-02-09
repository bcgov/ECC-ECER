using System.ComponentModel.DataAnnotations;
using AutoMapper;
using ECER.Clients.PSPPortal.Server.Shared;
using ECER.Infrastructure.Common;
using ECER.Managers.Registry.Contract.PspUsers;
using ECER.Managers.Registry.Contract.ProgramApplications;
using ECER.Utilities.Hosting;
using ECER.Utilities.Security;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using ContractApplicationStatus = ECER.Managers.Registry.Contract.ProgramApplications.ApplicationStatus;
using CreateProgramApplicationCommand = ECER.Managers.Registry.Contract.ProgramApplications.CreateProgramApplicationCommand;
using ContractProgramApplicationQuery = ECER.Managers.Registry.Contract.ProgramApplications.ProgramApplicationQuery;

namespace ECER.Clients.PSPPortal.Server.ProgramApplications;

public class ProgramApplicationsEndpoints : IRegisterEndpoints
{
  public void Register(IEndpointRouteBuilder endpointRouteBuilder)
  {
    const string policyNames = "psp_user";

    endpointRouteBuilder.MapPost("/api/programApplications", async Task<Results<Ok<CreateProgramApplicationResponse>, BadRequest<ProblemDetails>, NotFound>> (
      CreateProgramApplicationRequest request, HttpContext ctx, IMediator messageBus, IMapper mapper, CancellationToken ct) =>
    {
      var userContext = ctx.User.GetUserContext()!;
      var programRep = (await messageBus.Send<PspRepQueryResults>(new SearchPspRepQuery { ByUserIdentity = userContext.Identity }, ct)).Items.SingleOrDefault();
      if (programRep == null || string.IsNullOrWhiteSpace(programRep.PostSecondaryInstituteId)) return TypedResults.NotFound();

      var programApplication = new ProgramApplication(null, programRep.PostSecondaryInstituteId)
      {
        ProgramApplicationName = request.ProgramApplicationName,
        ProgramApplicationType = request.ProgramApplicationType,
        ProgramType = request.ProgramType,
        DeliveryType = request.DeliveryType,
        Status = ApplicationStatus.Draft
      };

      var created = await messageBus.Send(new CreateProgramApplicationCommand(programApplication), ct);
      if (created == null) return TypedResults.BadRequest(new ProblemDetails { Title = "Failed to create program application" });

      return TypedResults.Ok(new CreateProgramApplicationResponse(mapper.Map<ProgramApplication>(created)));
    })
    .WithOpenApi("Create a draft program application", string.Empty, "program_application_post")
    .RequireAuthorization(policyNames)
    .WithParameterValidation();

    endpointRouteBuilder.MapGet("/api/programApplications/{id?}", async Task<Results<Ok<GetProgramApplicationResponse>, NotFound>> (
      string? id, ApplicationStatus[]? byStatus, HttpContext ctx, IMediator messageBus, IMapper mapper, CancellationToken ct, IOptions<PaginationSettings> paginationOptions) =>
    {bool IdIsNotGuid = !Guid.TryParse(id, out _); if (IdIsNotGuid) { id = null; }

      // Get pagination parameters from the query string with default values
      var pageNumber = int.TryParse(ctx.Request.Query[paginationOptions.Value.PageProperty], out var page) && page > 0 ? page : paginationOptions.Value.DefaultPageNumber;
      var pageSize = int.TryParse(ctx.Request.Query[paginationOptions.Value.PageSizeProperty], out var size) ? size : paginationOptions.Value.DefaultPageSize;

      var userContext = ctx.User.GetUserContext()!;
      var programRep = (await messageBus.Send<PspRepQueryResults>(new SearchPspRepQuery { ByUserIdentity = userContext.Identity }, ct)).Items.SingleOrDefault();
      if (programRep == null || string.IsNullOrWhiteSpace(programRep.PostSecondaryInstituteId)) return TypedResults.NotFound();

      var statusFilter = (byStatus != null && byStatus.Length > 0)
        ? byStatus.Convert<ApplicationStatus, ContractApplicationStatus>()
        : null;

      var query = new ContractProgramApplicationQuery
      {
        ById = id,
        ByPostSecondaryInstituteId = programRep.PostSecondaryInstituteId,
        ByStatus = statusFilter,
        PageNumber = pageNumber,
        PageSize = pageSize
      };

      var results = await messageBus.Send(query, ct);

      return TypedResults.Ok(new GetProgramApplicationResponse() { Applications = mapper.Map<IEnumerable<ProgramApplication>>(results.Items), Count = results.Count });
    })
    .WithOpenApi("Handles program application queries", string.Empty, "program_application_get")
    .RequireAuthorization(policyNames)
    .WithParameterValidation();
  }
}

public record ProgramApplication
{
  public string? Id { get; set; }
  [Required]
  public string PostSecondaryInstituteId { get; set; } = null!;
  public string? ProgramApplicationName { get; set; }
  public ApplicationType? ProgramApplicationType { get; set; }
  public ApplicationStatus? Status { get; set; }
  public ProvincialCertificationTypeOffered? ProgramType { get; set; }
  public DeliveryType? DeliveryType { get; set; }
}

public record GetProgramApplicationResponse
{
  public IEnumerable<ProgramApplication>? Applications { get; set; }
  public int Count { get; set; }
}

public record CreateProgramApplicationRequest
{
  public string? ProgramApplicationName { get; set; }
  public ApplicationType? ProgramApplicationType { get; set; }
  public ProvincialCertificationTypeOffered? ProgramType { get; set; }
  public DeliveryType? DeliveryType { get; set; }
}

public record CreateProgramApplicationResponse(ProgramApplication ProgramApplication);

public enum ApplicationStatus
{
  Draft,
  InterimRecognition,
  OnGoingRecognition,
  PendingReview,
  RefusetoApprove,
  ReviewAnalysis,
  RFAI,
  Submitted,
  Withdrawn
}

public enum ApplicationType
{
  NewBasicPostBasicProgramHybridOnline,
  NewBasicPostBasicProgramInperson,
  NewDeliveryMethod,
  PrivateNewCampusLocation,
  SatelliteProgram,
}

public enum DeliveryType
{
  Hybrid,
  Inperson,
  Online,
  Satellite,
  WorkIntegratedLearning
}

public enum ProvincialCertificationTypeOffered
{
  ECEBasic,
  ITE,
  ITESNE,
  SNE
}
