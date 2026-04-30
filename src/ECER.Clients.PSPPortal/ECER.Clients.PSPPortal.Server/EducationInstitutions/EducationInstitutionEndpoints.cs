using AutoMapper;
using ECER.Infrastructure.Common.Validators;
using ECER.Managers.Registry.Contract.PostSecondaryInstitutes;
using ECER.Utilities.Hosting;
using ECER.Utilities.Security;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
namespace ECER.Clients.PSPPortal.Server.EducationInstitutions;

public class EducationInstitutionEndpoints : IRegisterEndpoints
{
  public void Register(IEndpointRouteBuilder endpointRouteBuilder)
  {
    endpointRouteBuilder.MapGet("api/education-institution",
      async Task<Results<Ok<EducationInstitution>, NotFound>> (HttpContext ctx, CancellationToken ct, IMediator bus,
        IMapper mapper) =>
      {
        var user = ctx.User.GetUserContext()!;
        var results = await bus.Send<PostSecondaryInstitutionsQueryResults>(
          new SearchPostSecondaryInstitutionQuery() { ByProgramRepresentativeId = user.UserId }, ct);
        var institution = results.Items.SingleOrDefault();
        if (institution == null) return TypedResults.NotFound();

        var educationInstitution = mapper.Map<EducationInstitution>(institution);
        return TypedResults.Ok(educationInstitution);
      }).WithOpenApi("Get users education institution", string.Empty, "education_institution_get")
        .WithOpenApi()
        .RequireAuthorization("psp_user");

    endpointRouteBuilder.MapPut("/api/education-institution",
      async Task<Results<Ok, BadRequest<ProblemDetails>>> (EducationInstitution institute, HttpContext ctx, CancellationToken ct, IMediator bus, IMapper mapper) =>
      {
        await bus.Send(new UpdatePostSecondaryInstitutionCommand(mapper.Map<Managers.Registry.Contract.PostSecondaryInstitutes.PostSecondaryInstitute>(institute)!), ctx.RequestAborted);
        return TypedResults.Ok();
      })
    .WithOpenApi("Updates the education institution", string.Empty, "education_institution_put")
    .RequireAuthorization("psp_user");

    endpointRouteBuilder.MapPost("/api/education-institution/campus",
      async Task<Results<Ok<string>, BadRequest<ProblemDetails>>> (CreateCampusRequest request, HttpContext ctx, CancellationToken ct, IMediator bus, IMapper mapper) =>
      {
        var user = ctx.User.GetUserContext()!;
        var campus = mapper.Map<Managers.Registry.Contract.PostSecondaryInstitutes.Campus>(request);
        var isSatellite = request.IsSatelliteOrTemporaryLocation == true;
        try
        {
          var newCampusId = await bus.Send(new CreateCampusCommand(user.UserId, campus, isSatellite, isSatellite ? null : request.ProgramIds), ct);
          return TypedResults.Ok(newCampusId);
        }
        catch (InvalidOperationException e)
        {
          return TypedResults.BadRequest(new ProblemDetails { Detail = e.Message });
        }
      })
    .WithOpenApi("Creates a new campus or satellite location for the user's institution", string.Empty, "campus_post")
    .WithParameterValidation()
    .RequireAuthorization("psp_user");

    endpointRouteBuilder.MapPut("/api/education-institution/campus/{campusId}",
      async Task<Results<Ok, NotFound, BadRequest<ProblemDetails>>> (string campusId, UpdateCampusRequest request, HttpContext ctx, CancellationToken ct, IMediator bus, IMapper mapper) =>
      {
        var user = ctx.User.GetUserContext()!;
        var campus = mapper.Map<Managers.Registry.Contract.PostSecondaryInstitutes.Campus>(request);
        campus = campus with { Id = campusId };
        var result = await bus.Send(new UpdateCampusCommand(user.UserId, campus), ct);
        
        if (result.Error == UpdateCampusError.InstitutionNotFound)
        {
          return TypedResults.BadRequest(new ProblemDetails() { Title = $"No institution found for program representative {user.UserId}" });
        }

        if (result.Error == UpdateCampusError.DuplicateCampusName)
        {
          return TypedResults.BadRequest(new ProblemDetails() { Title = $"This campus name already exists." });
        }
        
        if (result.Error == UpdateCampusError.InvalidCampus)
        {
          return TypedResults.BadRequest(new ProblemDetails() { Title = $"Campus {request.Name} does not belong to the user's institution" });
        }
        return TypedResults.Ok();
      })
    .WithOpenApi("Updates an existing campus", string.Empty, "campus_put")
    .AddGuidValidation("campusId")
    .WithParameterValidation()
    .RequireAuthorization("psp_user");
  }
}

/// <summary>
/// Save Post Secondary Institute request
/// </summary>
/// <param name="Institute">The Post Secondary Institute</param>
public record SavePostSecondaryInstituteRequest(PostSecondaryInstitute Institute);

public record EducationInstitution
{
    public string Id { get; set; } = null!;
    public string? Name { get; set; }
    public PsiInstitutionType? InstitutionType { get; set; }
    public PrivateAuspiceType? PrivateAuspiceType { get; set; }
    public string? PtiruInstitutionId { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? Street1 { get; set; }
    public string? Street2 { get; set; }
    public string? Street3 { get; set; }
    public string? City { get; set; }
    public string? Province { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public IEnumerable<Campus>? Campuses { get; set; }
  };
  public record Campus
  {
    public string Id { get; set; } = null!;
    public string? Name { get; set; }
    public string? GeneratedName { get; set; }
    public CampusStatus? Status { get; set; }
    public bool? IsSatelliteOrTemporaryLocation { get; set; }
    public string? Street1 { get; set; }
    public string? Street2 { get; set; }
    public string? Street3 { get; set; }
    public string? City { get; set; }
    public string? Province { get; set; }
    public string? PostalCode { get; set; }
    public string? KeyCampusContactId { get; set; }
    public string? KeyCampusContactName { get; set; }
    public string? OtherCampusContactName { get; set; }
  }
  public enum CampusStatus
  {
    None,
    Active = 1,
    Inactive = 2
  }
  public enum PsiInstitutionType
  {
    Private,
    Public,
    ContinuingEducation,
    PublicOOP,
  }

  public enum PrivateAuspiceType
  {
    Theologicalinstitution,
    FirstNationsmandatedpostsecondaryinstitute,
    Other,
    Privatetraininginstitution,
    Indigenouscontrolledpostsecondaryinstitute,
  }

  public record CreateCampusRequest
  {
    public string? Name { get; set; }
    public bool? IsSatelliteOrTemporaryLocation { get; set; }
    public string? Street1 { get; set; }
    public string? Street2 { get; set; }
    public string? Street3 { get; set; }
    public string? City { get; set; }
    public string? Province { get; set; }
    public string? PostalCode { get; set; }
    public string? KeyCampusContactId { get; set; }
    public string? OtherCampusContactName { get; set; }
    public IEnumerable<string>? ProgramIds { get; set; }
  }

  public record UpdateCampusRequest
  {
    public string? Name { get; set; }
    public string? Street1 { get; set; }
    public string? Street2 { get; set; }
    public string? Street3 { get; set; }
    public string? City { get; set; }
    public string? Province { get; set; }
    public string? PostalCode { get; set; }
    public string? KeyCampusContactId { get; set; }
    public string? OtherCampusContactName { get; set; }
  }


