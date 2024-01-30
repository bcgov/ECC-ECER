using AutoMapper;
using ECER.Managers.Registry.Contract.Registrants;
using ECER.Utilities.Hosting;
using ECER.Utilities.Security;
using Microsoft.AspNetCore.Http.HttpResults;
using Wolverine;

namespace ECER.Clients.RegistryPortal.Server.Users;

public class UserInfoEndpoints : IRegisterEndpoints
{
  public void Register(IEndpointRouteBuilder endpointRouteBuilder)
  {
    endpointRouteBuilder.MapGet("api/userinfo", async Task<Results<Ok<UserInfo>, NotFound>> (HttpContext ctx, CancellationToken ct, IMessageBus bus, IMapper mapper) =>
        {
          var user = ctx.User.GetUserContext()!;

          var results = await bus.InvokeAsync<RegistrantQueryResults>(new RegistrantQuery { ByUserIdentity = user.Identity }, ct);

          var registrant = results.Items.SingleOrDefault();
          if (registrant == null) return TypedResults.NotFound();
          return TypedResults.Ok(mapper.Map<UserInfo>(registrant.Profile));
        })
        .WithOpenApi("Gets the currently logged in user profile or NotFound if no profile found", string.Empty, "userinfo_get")
        .RequireAuthorization("registry_new_user");

    endpointRouteBuilder.MapPost("api/userinfo", async Task<Ok> (UserInfo userInfo, HttpContext ctx, CancellationToken ct, IMessageBus bus, IMapper mapper) =>
        {
          var user = ctx.User.GetUserContext()!;

          await bus.InvokeAsync<string>(new RegisterNewUserCommand(mapper.Map<Managers.Registry.Contract.Registrants.UserProfile>(userInfo), user.Identity), ct);

          return TypedResults.Ok();
        })
        .WithOpenApi("Creates or updates the currently logged on user's profile", string.Empty, "userinfo_post")
        .RequireAuthorization("registry_new_user");
  }
}

public record UserInfo(string FirstName, string LastName, DateOnly DateOfBirth, string Email, string Phone);
