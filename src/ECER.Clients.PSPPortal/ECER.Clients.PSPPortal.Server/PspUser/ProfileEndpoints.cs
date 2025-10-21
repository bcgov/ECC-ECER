using AutoMapper;
using ECER.Managers.Registry.Contract.Communications;
using ECER.Managers.Registry.Contract.PspUsers;
using ECER.Utilities.Hosting;
using ECER.Utilities.Security;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ECER.Clients.PSPPortal.Server.Users;

public class ProfileEndpoints : IRegisterEndpoints
{
  public void Register(IEndpointRouteBuilder endpointRouteBuilder)
  {
    endpointRouteBuilder.MapGet("api/users/profile", async Task<Results<Ok<PspUserProfile>, NotFound>> (HttpContext ctx, CancellationToken ct, IMediator bus, IMapper mapper) =>
      {
        var user = ctx.User.GetUserContext()!;
        var results = await bus.Send<PspRepQueryResults>(new SearchPspRepQuery() { ByUserIdentity = user.Identity }, ct);

        var pspUser = results.Items.SingleOrDefault();
        if (pspUser == null) return TypedResults.NotFound();

        var query = new UserCommunicationsStatusQuery();
        query.ByRegistrantId = pspUser.Id;

        var pspUserProfile = mapper.Map<PspUserProfile>(pspUser.Profile);
        return TypedResults.Ok(pspUserProfile);
      })
      .WithOpenApi("Gets the currently logged in user profile or NotFound if no profile found", string.Empty, "psp_user_profile_get")
      .RequireAuthorization("registry_new_user")
      .WithParameterValidation();
    
    endpointRouteBuilder.MapPost("/api/users/register",
        async Task<Results<Ok, BadRequest<ProblemDetails>>> (PspUserProfile pspUserProfile, HttpContext ctx,
          CancellationToken ct, IMediator bus, IMapper mapper) =>
        {
          await bus.Send(
            new RegisterPspUserCommand(
              mapper.Map<Managers.Registry.Contract.PspUsers.PspUserProfile>(pspUserProfile)!),
            ctx.RequestAborted);
          return TypedResults.Ok();
        })
      .WithOpenApi("Create Program Representative", string.Empty, "psp_user_profile_post");
  }
}

/// <summary>
/// User profile information
/// </summary>
public record PspUserProfile
{
  public string Id { get; set; } = null!;
  public string? FirstName { get; set; }
  public string? LastName { get; set; }
  public string? Email { get; set; } = null!;
  public string? BceidBusinessId { get; set; }
  public string? ProgramRepresentativeId { get; set; }
};
