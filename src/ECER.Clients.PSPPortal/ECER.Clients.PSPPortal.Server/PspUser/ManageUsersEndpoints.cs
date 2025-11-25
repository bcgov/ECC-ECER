using System.Linq;
using AutoMapper;
using ECER.Managers.Registry.Contract.PspUsers;
using ECER.Utilities.Hosting;
using ECER.Utilities.Security;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ECER.Clients.PSPPortal.Server.Users;

public class ManageUsersEndpoints : IRegisterEndpoints
{
  public void Register(IEndpointRouteBuilder endpointRouteBuilder)
  {
    endpointRouteBuilder.MapGet("api/users/manage",
      async Task<Results<Ok<IEnumerable<PspUserListItem>>, NotFound>> (HttpContext ctx, CancellationToken ct, IMediator bus, IMapper mapper) =>
      {
        var user = ctx.User.GetUserContext()!;
        var currentRep = (await bus.Send<PspRepQueryResults>(new SearchPspRepQuery { ByUserIdentity = user.Identity }, ct)).Items.SingleOrDefault();
        if (currentRep == null || string.IsNullOrWhiteSpace(currentRep.PostSecondaryInstituteId))
        {
          return TypedResults.NotFound();
        }

        var reps = await bus.Send<PspRepQueryResults>(new SearchPspRepQuery
        {
          ByPostSecondaryInstituteId = currentRep.PostSecondaryInstituteId
        }, ct);

        var result = mapper.Map<IEnumerable<PspUserListItem>>(reps.Items);
        return TypedResults.Ok(result);
      })
      .WithOpenApi("Gets PSP representatives for the current user's institution", string.Empty, "psp_user_manage_get")
      .RequireAuthorization("psp_user")
      .WithParameterValidation();

    endpointRouteBuilder.MapPost("api/users/manage/{programRepId}/deactivate",
      async Task<Results<Ok, NotFound, BadRequest<string>>> (string programRepId, HttpContext ctx, CancellationToken ct, IMediator bus) =>
      {
        if (!Guid.TryParse(programRepId, out _))
        {
          return TypedResults.NotFound();
        }

        var user = ctx.User.GetUserContext()!;
        var currentRep = (await bus.Send<PspRepQueryResults>(new SearchPspRepQuery { ByUserIdentity = user.Identity }, ct)).Items.SingleOrDefault();
        if (currentRep == null || string.IsNullOrWhiteSpace(currentRep.PostSecondaryInstituteId))
        {
          return TypedResults.NotFound();
        }

        var targetRep = (await bus.Send<PspRepQueryResults>(new SearchPspRepQuery { ById = programRepId }, ct)).Items.SingleOrDefault();
        if (targetRep == null || targetRep.PostSecondaryInstituteId != currentRep.PostSecondaryInstituteId)
        {
          return TypedResults.NotFound();
        }
        
        if (targetRep.Profile.Role == (Managers.Registry.Contract.PspUsers.PspUserRole?)PspUserRole.Primary)
        {
          return TypedResults.BadRequest("Cannot deactivate the primary representative. Please set another representative as primary before deactivating this user.");
        }
        
        if (user.UserId == programRepId)
        {
          return TypedResults.BadRequest("Cannot deactivate your own account");
        }

        await bus.Send(new DeactivatePspRepCommand(programRepId), ct);
        return TypedResults.Ok();
      })
      .WithOpenApi("Deactivates a PSP representative for the current user's institution", string.Empty, "psp_user_manage_deactivate_post")
      .RequireAuthorization("psp_user")
      .WithParameterValidation();

    endpointRouteBuilder.MapPost("api/users/manage/{programRepId}/set-primary",
      async Task<Results<Ok, NotFound, BadRequest<string>>> (string programRepId, HttpContext ctx, CancellationToken ct, IMediator bus) =>
      {
        if (!Guid.TryParse(programRepId, out _))
        {
          return TypedResults.NotFound();
        }

        var user = ctx.User.GetUserContext()!;
        var currentRep = (await bus.Send<PspRepQueryResults>(new SearchPspRepQuery { ByUserIdentity = user.Identity }, ct)).Items.SingleOrDefault();
        if (currentRep == null || string.IsNullOrWhiteSpace(currentRep.PostSecondaryInstituteId))
        {
          return TypedResults.NotFound();
        }

        var targetRep = (await bus.Send<PspRepQueryResults>(new SearchPspRepQuery { ById = programRepId }, ct)).Items.SingleOrDefault();
        if (targetRep == null || targetRep.PostSecondaryInstituteId != currentRep.PostSecondaryInstituteId)
        {
          return TypedResults.NotFound();
        }
        
        if(targetRep.AccessToPortal != (Managers.Registry.Contract.PspUsers.PortalAccessStatus?)PortalAccessStatus.Active)
        {
          return TypedResults.BadRequest("Cannot set a representative who does not have active portal access as primary.");
        }

        await bus.Send(new SetPrimaryPspRepCommand(programRepId), ct);
        return TypedResults.Ok();
      })
      .WithOpenApi("Sets the specified PSP representative as Primary for the current user's institution", string.Empty, "psp_user_manage_set_primary_post")
      .RequireAuthorization("psp_user")
      .WithParameterValidation();
  }
}

public record PspUserListItem
{
  public string Id { get; set; } = null!;
  public PspUserProfile Profile { get; set; } = null!;
  public PortalAccessStatus? AccessToPortal { get; set; }
  public string? PostSecondaryInstituteId { get; set; }
}

public enum PortalAccessStatus
{
  Invited,
  Active,
  Disabled
}
