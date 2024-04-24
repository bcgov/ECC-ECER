using AutoMapper;
using ECER.Clients.RegistryPortal.Server.Applications;
using ECER.Managers.Registry.Contract.PortalInvitations;
using ECER.Utilities.Hosting;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ECER.Clients.RegistryPortal.Server.References;

public class PortalInvitationsEndpoints : IRegisterEndpoints
{
  public void Register(IEndpointRouteBuilder endpointRouteBuilder)
  {
    endpointRouteBuilder.MapGet("/api/PortalInvitations/{token?}", async Task<Results<Ok<PortalInvitationQueryResult>, BadRequest<ProblemDetails>>> (string? token, IMediator messageBus, HttpContext httpContext, IMapper mapper, CancellationToken ct) =>
    {
      if (token == null)
      {
        return TypedResults.BadRequest(new ProblemDetails { Status = StatusCodes.Status400BadRequest, Detail = "Provided token is not valid" });
      }
      var result = await messageBus.Send(new PortalInvitationVerificationQuery(token), ct);

      if (!result.IsSuccess)
      {
        return TypedResults.BadRequest(new ProblemDetails { Status = StatusCodes.Status400BadRequest, Detail = result.ErrorMessage });
      }
      return TypedResults.Ok(new PortalInvitationQueryResult(mapper.Map<PortalInvitation>(result.Invitation)));
    }).WithOpenApi("Handles references queries", string.Empty, "references_get").WithParameterValidation();
  }
}

public record PortalInvitationQueryResult(PortalInvitation PortalInvitation);

public record PortalInvitation(string? Id, string Name, string ReferenceFirstName, string ReferenceLastName, string ReferenceEmailAddress)
{
  public string? ApplicantFirstName { get; set; }
  public string? ApplicantLastName { get; set; }
  public string? ApplicationId { get; set; }
  public IEnumerable<CertificationType>? CertificationTypes { get; set; }
  public string? WorkexperienceReferenceId { get; set; }
  public string? CharacterReferenceId { get; set; }
  public InviteType? InviteType { get; set; }
}

public enum InviteType
{
  CharacterReference,
  WorkExperienceReference
}
