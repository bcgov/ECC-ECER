using AutoMapper;
using ECER.Managers.Registry.Contract.Communications;
using ECER.Managers.Registry.Contract.Registrants;
using ECER.Utilities.Hosting;
using ECER.Utilities.Security;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel.DataAnnotations;

namespace ECER.Clients.RegistryPortal.Server.Users;

public class UserInfoEndpoints : IRegisterEndpoints
{
  public void Register(IEndpointRouteBuilder endpointRouteBuilder)
  {
    endpointRouteBuilder.MapGet("api/userinfo", async Task<Results<Ok<UserInfo>, NotFound>> (HttpContext ctx, CancellationToken ct, IMediator bus, IMapper mapper) =>
        {
          var user = ctx.User.GetUserContext()!;
          var results = await bus.Send<RegistrantQueryResults>(new SearchRegistrantQuery { ByUserIdentity = user.Identity }, ct);

          var registrant = results.Items.SingleOrDefault();
          if (registrant == null) return TypedResults.NotFound();

          var query = new UserCommunicationsStatusQuery();
          query.ByRegistrantId = registrant.UserId;
          var communicationsStatus = await bus.Send<CommunicationsStatusResults>(query);

          var userInfo = mapper.Map<UserInfo>(registrant.Profile);
          userInfo!.UnreadMessagesCount = communicationsStatus.Status.Count;
          return TypedResults.Ok(userInfo);
        })
        .WithOpenApi("Gets the currently logged in user profile or NotFound if no profile found", string.Empty, "userinfo_get")
        .RequireAuthorization("registry_new_user")
        .WithParameterValidation();

    endpointRouteBuilder.MapPost("api/userinfo", async Task<Ok> (UserInfo userInfo, HttpContext ctx, CancellationToken ct, IMediator bus, IMapper mapper) =>
        {
          var user = ctx.User.GetUserContext()!;
          userInfo.MiddleName = ECER.Infrastructure.Common.Utility.GetMiddleName(userInfo.FirstName!, userInfo.GivenName!);
          await bus.Send(new RegisterNewUserCommand(mapper.Map<Managers.Registry.Contract.Registrants.UserProfile>(userInfo)!, user.Identity), ct);
          return TypedResults.Ok();
        })
        .WithOpenApi("Creates or updates the currently logged on user's profile", string.Empty, "userinfo_post")
        .RequireAuthorization("registry_new_user")
        .WithParameterValidation();
  }
}

public record UserInfo([Required] string FirstName, [Required] string LastName, [Required] string GivenName, [Required] DateOnly DateOfBirth, [Required] string Email, [Required] string Phone)
{
  public string? MiddleName { get; set; }
  public string? RegistrationNumber { get; set; }
  public bool IsVerified { get; set; }
  public int UnreadMessagesCount { get; set; }

  public bool IsRegistrant { get; set; }
}
