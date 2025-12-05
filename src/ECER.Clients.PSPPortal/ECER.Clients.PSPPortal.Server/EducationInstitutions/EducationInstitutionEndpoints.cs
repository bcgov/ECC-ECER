using AutoMapper;
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
        var results = await bus.Send(new UpdatePostSecondaryInstitutionCommand(mapper.Map<Managers.Registry.Contract.PostSecondaryInstitutes.PostSecondaryInstitute>(institute)!), ctx.RequestAborted);
        return TypedResults.Ok();
      })
    .WithOpenApi("Updates the education institution", string.Empty, "education_institution_put")
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
    public Auspice? Auspice { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? Street1 { get; set; }
    public string? Street2 { get; set; }
    public string? Street3 { get; set; }
    public string? City { get; set; }
    public string? Province { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
  };

  public enum Auspice
  {
    ContinuingEducation,
    PublicOOP,
    Private,
    Public
  }
  
  
