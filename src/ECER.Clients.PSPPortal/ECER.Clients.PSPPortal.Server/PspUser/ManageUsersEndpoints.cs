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
