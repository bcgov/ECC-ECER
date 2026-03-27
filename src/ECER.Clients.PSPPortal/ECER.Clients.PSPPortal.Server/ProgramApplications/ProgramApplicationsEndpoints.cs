using System.ComponentModel.DataAnnotations;
using AutoMapper;
using ECER.Clients.PSPPortal.Server.Shared;
using ECER.Infrastructure.Common;
using ECER.Infrastructure.Common.Validators;
using ECER.Managers.Registry.Contract.ProgramApplications;
using ECER.Managers.Registry.Contract.Programs;
using ECER.Managers.Registry.Contract.PspUsers;
using ECER.Utilities.Hosting;
using ECER.Utilities.Security;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using ContractApplicationStatus = ECER.Managers.Registry.Contract.ProgramApplications.ApplicationStatus;
using CreateProgramApplicationCommand = ECER.Managers.Registry.Contract.ProgramApplications.CreateProgramApplicationCommand;
using ContractProgramApplicationQuery = ECER.Managers.Registry.Contract.ProgramApplications.ProgramApplicationQuery;
using ContractSubmitProgramApplicationCommand = ECER.Managers.Registry.Contract.ProgramApplications.SubmitProgramApplicationCommand;
using ECER.Utilities.ObjectStorage.Providers;

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

      if (request.ProgramApplicationType == ApplicationType.NewCampusatRecognizedPrivateInstitution)
      {
        if (request.ProgramProfileId == null)
        {
          return TypedResults.BadRequest(new ProblemDetails { Title = "Program profile must not be null" });
        }
        
        if (request.CampusId == null)
        {
          return TypedResults.BadRequest(new ProblemDetails { Title = "Campus must not be null" });
        }
      }
      
      if (request.ProgramProfileId != null)
      {
        var existingProgramProfile = await messageBus.Send(new ProgramsQuery
        {
          ById = request.ProgramProfileId,
        }, ct);
        if (!existingProgramProfile.Items.Any()) return TypedResults.NotFound();
      }
      
      var programApplication = new ProgramApplication
      {
        PostSecondaryInstituteId = programRep.PostSecondaryInstituteId,
        ProgramApplicationName = request.ProgramApplicationName,
        ProgramApplicationType = request.ProgramApplicationType,
        ProgramTypes = request.ProgramTypes,
        DeliveryType = request.DeliveryType,
        Status = ApplicationStatus.Draft,
        ProgramProfileId = request.ProgramProfileId
      };
      
      if (request.CampusId != null)
      {
        programApplication.ProgramCampuses = new[]
        {
          new ProgramCampus
          {
            CampusId = request.CampusId,
            Id = null
          }
        };
      }
      
      var contractApplication = mapper.Map<Managers.Registry.Contract.ProgramApplications.ProgramApplication>(programApplication);
      var created = await messageBus.Send(new CreateProgramApplicationCommand(contractApplication), ct);
      if (created == null) return TypedResults.BadRequest(new ProblemDetails { Title = "Failed to create program application" });

      return TypedResults.Ok(new CreateProgramApplicationResponse(mapper.Map<ProgramApplication>(created)));
    })
    .WithOpenApi("Create a draft program application", string.Empty, "program_application_post")
    .RequireAuthorization(policyNames)
    .WithParameterValidation();

    endpointRouteBuilder.MapGet("/api/programApplications/{id?}", async Task<Results<Ok<GetProgramApplicationResponse>, NotFound>> (
      string? id, ApplicationStatus[]? byStatus, string? campusId, HttpContext ctx, IMediator messageBus, IMapper mapper, CancellationToken ct, IOptions<PaginationSettings> paginationOptions) =>
    {
      bool IdIsNotGuid = !Guid.TryParse(id, out _); if (IdIsNotGuid) { id = null; }

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
        ByCampusId = campusId,
        PageNumber = pageNumber,
        PageSize = pageSize
      };

      var results = await messageBus.Send(query, ct);

      return TypedResults.Ok(new GetProgramApplicationResponse() { Applications = mapper.Map<IEnumerable<ProgramApplication>>(results.Items), Count = results.Count });
    })
    .WithOpenApi("Handles program application queries", string.Empty, "program_application_get")
    .RequireAuthorization(policyNames)
    .AddGuidValidationQueryParams(["campusId"], isRequired: false)
    .WithParameterValidation();
    
  endpointRouteBuilder.MapPut("/api/programApplications/{id}", async Task<Results<Ok<string>, BadRequest<string>, NotFound>> (string id, ProgramApplication request, HttpContext ctx, CancellationToken ct, IMediator messageBus, IMapper mapper) =>
    {
      if (string.IsNullOrWhiteSpace(id)) return TypedResults.BadRequest("program application id cannot be null or whitespace");
      bool IdIsNotGuid = !Guid.TryParse(id, out _);
      if (IdIsNotGuid) return TypedResults.BadRequest("invalid program application id");
  
      if (request.Id != id) return TypedResults.BadRequest("resource id and payload id do not match");
  
      var userContext = ctx.User.GetUserContext()!;
      var programRep = (await messageBus.Send<PspRepQueryResults>(new SearchPspRepQuery { ByUserIdentity = userContext.Identity }, ct)).Items.SingleOrDefault();
      if (programRep == null || string.IsNullOrWhiteSpace(programRep.PostSecondaryInstituteId)) return TypedResults.NotFound();
  
      var existing = await messageBus.Send(new ContractProgramApplicationQuery
      {
        ById = id,
        ByPostSecondaryInstituteId = programRep.PostSecondaryInstituteId,
        ByStatus = new[] { ContractApplicationStatus.Draft }
      }, ct);
  
      if (!existing.Items.Any()) return TypedResults.NotFound();
  
      var programId = await messageBus.Send(new UpdateProgramApplicationCommand(mapper.Map<Managers.Registry.Contract.ProgramApplications.ProgramApplication>(request)), ct);
      return TypedResults.Ok(programId);
    })
    .WithOpenApi("Update program application", string.Empty, "program_application_put")
    .RequireAuthorization(policyNames)
    .WithParameterValidation();
  
  endpointRouteBuilder.MapGet("/api/programApplications/{id}/components", async Task<Results<Ok<IEnumerable<NavigationMetadata>>, BadRequest<string>, NotFound>> (string id, HttpContext ctx, IMediator messageBus, IMapper mapper, CancellationToken ct) =>
    {
      if (string.IsNullOrWhiteSpace(id)) return TypedResults.BadRequest("program application id cannot be null or whitespace");
      bool IdIsNotGuid = !Guid.TryParse(id, out _);
      if (IdIsNotGuid) return TypedResults.BadRequest("invalid program application id");
  
      var userContext = ctx.User.GetUserContext()!;
      var programRep = (await messageBus.Send<PspRepQueryResults>(new SearchPspRepQuery { ByUserIdentity = userContext.Identity }, ct)).Items.SingleOrDefault();
      if (programRep == null || string.IsNullOrWhiteSpace(programRep.PostSecondaryInstituteId)) return TypedResults.NotFound();
  
      var existing = await messageBus.Send(new ContractProgramApplicationQuery
      {
        ById = id,
        ByPostSecondaryInstituteId = programRep.PostSecondaryInstituteId
      }, ct);
  
      if (!existing.Items.Any()) return TypedResults.NotFound();
      
      var componentGroups = await messageBus.Send(new ComponentGroupQuery
      {
        ByProgramApplicationId = id,
      }, ct);

      return TypedResults.Ok(mapper.Map<IEnumerable<NavigationMetadata>>(componentGroups));
    })
    .WithOpenApi("Gets component groups", string.Empty, "program_application_components_get")
    .RequireAuthorization(policyNames)
    .WithParameterValidation();

  endpointRouteBuilder.MapGet("/api/programApplications/{id}/componentGroups/{componentGroupId?}", async Task<Results<Ok<IEnumerable<ComponentGroupWithComponents>>, BadRequest<string>, NotFound>> (string id, string? componentGroupId, HttpContext ctx, IMediator messageBus, IMapper mapper, CancellationToken ct) =>
    {
      var userContext = ctx.User.GetUserContext()!;
      var programRep = (await messageBus.Send<PspRepQueryResults>(new SearchPspRepQuery { ByUserIdentity = userContext.Identity }, ct)).Items.SingleOrDefault();
      if (programRep == null || string.IsNullOrWhiteSpace(programRep.PostSecondaryInstituteId)) return TypedResults.NotFound();

      var existing = await messageBus.Send(new ContractProgramApplicationQuery
      {
        ById = id,
        ByPostSecondaryInstituteId = programRep.PostSecondaryInstituteId
      }, ct);
      if (!existing.Items.Any()) return TypedResults.NotFound();

      var result = await messageBus.Send(new ComponentGroupWithComponentsQuery { ByProgramApplicationId = id, ByComponentGroupId = componentGroupId }, ct);
      return TypedResults.Ok(mapper.Map<IEnumerable<ComponentGroupWithComponents>>(result));
    })
    .WithOpenApi("Gets program application components by component group id", string.Empty, "program_application_component_group_components_get")
    .RequireAuthorization(policyNames)
    .AddGuidValidation("id").AddGuidValidation("componentGroupId", false)
    .WithParameterValidation();
  
  endpointRouteBuilder.MapPut("/api/programApplications/{id}/componentGroups/{componentGroupId}", async Task<Results<Ok<string>, BadRequest<string>, NotFound>> (string id, string componentGroupId, ComponentGroupWithComponents request, HttpContext ctx, CancellationToken ct, IMediator messageBus, IMapper mapper) =>
    {
      if (request.Id != componentGroupId) return TypedResults.BadRequest("resource id and payload id do not match");

      var userContext = ctx.User.GetUserContext()!;
      var programRep = (await messageBus.Send<PspRepQueryResults>(new SearchPspRepQuery { ByUserIdentity = userContext.Identity }, ct)).Items.SingleOrDefault();
      if (programRep == null || string.IsNullOrWhiteSpace(programRep.PostSecondaryInstituteId)) return TypedResults.NotFound();

      var existing = await messageBus.Send(new ContractProgramApplicationQuery
      {
        ById = id,
        ByPostSecondaryInstituteId = programRep.PostSecondaryInstituteId
      }, ct);
      if (!existing.Items.Any()) return TypedResults.NotFound();

      var result = await messageBus.Send(new UpdateComponentGroupCommand(mapper.Map<Managers.Registry.Contract.ProgramApplications.ComponentGroupWithComponents>(request), id, programRep.PostSecondaryInstituteId), ct);
      return TypedResults.Ok(result);
    })
    .WithOpenApi("Update program application component group", string.Empty, "program_application_component_group_put")
    .RequireAuthorization(policyNames)
    .AddGuidValidation("id").AddGuidValidation("componentGroupId")
    .WithParameterValidation();

  endpointRouteBuilder.MapPost("/api/programApplications/{id}/submit", async Task<Results<Ok<SubmitProgramApplicationResponse>, BadRequest<string>, BadRequest<SubmitProgramApplicationValidationError>, NotFound>> (
    string id, SubmitProgramApplicationRequest request, HttpContext ctx, IMediator messageBus, CancellationToken ct) =>
    {
      if (string.IsNullOrWhiteSpace(id)) return TypedResults.BadRequest("program application id cannot be null or whitespace");
      if (!Guid.TryParse(id, out _)) return TypedResults.BadRequest("invalid program application id");

      var userContext = ctx.User.GetUserContext()!;
      var programRep = (await messageBus.Send<PspRepQueryResults>(new SearchPspRepQuery { ByUserIdentity = userContext.Identity }, ct)).Items.SingleOrDefault();
      if (programRep == null || string.IsNullOrWhiteSpace(programRep.PostSecondaryInstituteId)) return TypedResults.NotFound();

      var existing = await messageBus.Send(new ContractProgramApplicationQuery
      {
        ById = id,
        ByPostSecondaryInstituteId = programRep.PostSecondaryInstituteId,
        ByStatus = new[] { ContractApplicationStatus.Draft, ContractApplicationStatus.ReviewAnalysis }
      }, ct);
      var application = existing.Items.SingleOrDefault();
      if (application == null) return TypedResults.NotFound();
      
      
      var command = new ContractSubmitProgramApplicationCommand(
        ProgramApplication: application,
        ProgramRepresentativeId: programRep.Id,
        Declaration: request.Declaration);

      var result = await messageBus.Send(command, ct);

      if (result.Error != null)
      {
        return TypedResults.BadRequest(new SubmitProgramApplicationValidationError(result.Error.ToString()!, result.ValidationErrors));
      }

      return TypedResults.Ok(new SubmitProgramApplicationResponse(result.ProgramApplicationId!));
    })
    .WithOpenApi("Submit a program application", string.Empty, "program_application_submit_post")
    .RequireAuthorization(policyNames)
    .AddGuidValidation("id")
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
  public ApplicationStatusReasonDetail? StatusReasonDetail { get; set; }
  public IEnumerable<ProgramCertificationType>? ProgramTypes { get; set; }
  public DeliveryType? DeliveryType { get; set; }
  public bool? ComponentsGenerationCompleted { get; set; }
  public string? ProgramRepresentativeId { get; set; }
  public string? ProgramLength { get; set; }
  public IEnumerable<MethodofInstruction>? OnlineMethodOfInstruction { get; set; }
  public IEnumerable<DeliveryMethodforInstructor>? DeliveryMethod { get; set; }
  public IEnumerable<WorkHoursType>? EnrollmentOptions { get; set; }
  public IEnumerable<AdmissionOptions>? AdmissionOptions { get; set; }
  public string? MinimumEnrollment { get; set; }
  public string? MaximumEnrollment { get; set; }
  public IEnumerable<ProgramCampus>? ProgramCampuses { get; set; }
  public string? OtherAdmissionOptions  { get; set; }
  public string? InstituteInfoEntryProgress { get; set; }
  public DateTime? DeclarationDate { get; set; }
  public bool? DeclarationAccepted { get; set; }
  public string? DeclarantName { get; set; }
  public string? ProgramProfileId { get; set; }
  public string? ProgramProfileName { get; set; }
  public string? DeclarationText { get; set; }
}

public record ProgramCampus
{ 
  public string? Id { get; set; }
  public string? CampusId { get; set; }
  public string? Name { get; set; }
}

public enum MethodofInstruction
{
  Asynchronous,
  Synchronous,
}

public enum DeliveryMethodforInstructor
{
  Inpersonsitevisits,
  Virtualsitevisits,
}
public enum AdmissionOptions
{
  Allcoursesrestrictedtoearlychildhoodeducationstudents,
  Cohortenrollmentstudentsstarttogetherandgraduatetogether,
  Continuousenrollmentstudentscanenrolatanytime,
  Oneormorecoursesopentoanystudentsintheinstitution,
  Other,
}
public enum WorkHoursType
{
  FullTime,
  PartTime,
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
  public IEnumerable<ProgramCertificationType>? ProgramTypes { get; set; }
  public DeliveryType? DeliveryType { get; set; }
  public string? ProgramProfileId { get; set; }
  public string? CampusId { get; set; }
}

public record CreateProgramApplicationResponse(ProgramApplication ProgramApplication);

public record SubmitProgramApplicationRequest
{
  public bool Declaration { get; set; }
}

public record SubmitProgramApplicationResponse(string ProgramApplicationId);
public record SubmitProgramApplicationValidationError(string Error, IEnumerable<string> ValidationErrors);
public record NavigationMetadata(string Id, string Name, string Status, string CategoryName, int DisplayOrder, NavigationType NavigationType, bool? RfaiRequired);
public record ComponentGroupWithComponents(string Id, string Name, string? Instruction, string Status, string CategoryName, int DisplayOrder, IEnumerable<ProgramApplicationComponent> Components);
public record ProgramApplicationComponent(string Id, string Name, string? Question, int DisplayOrder, string? Answer, IEnumerable<FileInfo>? Files, bool? RfaiRequired)
{
  public IEnumerable<FileInfo> NewFiles { get; set; } = Array.Empty<FileInfo>();
  public IEnumerable<FileInfo> DeletedFiles { get; set; } = Array.Empty<FileInfo>();
};

public enum NavigationType
{
  Component,
  Other,
}

public record FileInfo(string Id)
{
  public string? Name { get; set; }
  public string? Url { get; set; }
  public string? Size { get; set; }
  public string? Extension { get; set; }
  public EcerWebApplicationType? EcerWebApplicationType { get; set; }
}

public enum ApplicationStatus
{
  Approved,
  Archived,
  Denied,
  Draft,
  Inactive,
  InterimRecognition,
  OnGoingRecognition,
  RefusetoApprove,
  ReviewAnalysis,
  Submitted,
  Withdrawn
}

public enum ApplicationStatusReasonDetail
{
  Pendingdecision,
  Recognitionevaluationmeeting,
  RFAIreceived,
  RFAIrequested,
}

public enum ApplicationType
{
  AddOnlineorHybridDeliveryMethod,
  CurriculumRevisionsatRecognizedInstitution,
  NewBasicECEPostBasicProgram,
  NewCampusatRecognizedPrivateInstitution,
  SatelliteProgram,
  WorkIntegratedLearningProgram,
}

public enum DeliveryType
{
  Hybrid,
  Inperson,
  Online,
}

public enum ProvincialCertificationTypeOffered
{
  ECEBasic,
  ITE,
  ITESNE,
  SNE
}

public enum ProgramCertificationType
{
  Basic,
  ITE,
  SNE
}
