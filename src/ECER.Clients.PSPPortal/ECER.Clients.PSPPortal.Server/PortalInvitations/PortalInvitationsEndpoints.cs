using AutoMapper;
using ECER.Managers.Registry.Contract.PortalInvitations;
using ECER.Utilities.Hosting;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ECER.Clients.PSPPortal.Server.PortalInvitations;

public class PortalInvitationsEndpoints : IRegisterEndpoints
{
  public void Register(IEndpointRouteBuilder endpointRouteBuilder)
  {
    endpointRouteBuilder.MapGet("/api/PortalInvitations/{token?}", async Task<Results<Ok<PortalInvitationQueryResult>, BadRequest<ProblemDetails>>> (string? token, IMediator messageBus, HttpContext httpContext, IMapper mapper, CancellationToken ct) =>
    {
      if (string.IsNullOrWhiteSpace(token))
      {
        return TypedResults.BadRequest(new ProblemDetails { Status = StatusCodes.Status400BadRequest, Detail = "Provided token is not valid" });
      }
      var result = await messageBus.Send(new PortalInvitationVerificationQuery(token), ct);

      if (!result.IsSuccess)
      {
        return TypedResults.BadRequest(new ProblemDetails { Status = StatusCodes.Status400BadRequest, Detail = result.ErrorMessage });
      }

      return TypedResults.Ok(new PortalInvitationQueryResult(mapper.Map<PortalInvitation>(result.Invitation)));
    }).WithOpenApi("Handles portal invitation queries", string.Empty, "portal_invitation_get").WithParameterValidation();
  }
}

public record PortalInvitationQueryResult(PortalInvitation PortalInvitation);

public record PortalInvitation(string? Id)
{
  public string? PspProgramRepresentativeId { get; set; }
  public InviteType? InviteType { get; set; }
  public string? BceidBusinessName { get; set; }
  public bool IsLinked { get; set; }
}

public enum InviteType
{
  PSIProgramRepresentative,
}
