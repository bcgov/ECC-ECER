using AutoMapper;
using ECER.Managers.Registry.Contract.Registrants;
using ECER.Utilities.Hosting;
using ECER.Utilities.Security;
using Microsoft.AspNetCore.Http.HttpResults;
using Wolverine;

namespace ECER.Clients.RegistryPortal.Server.Users;

public class ProfileEndpoints : IRegisterEndpoints
{
  public void Register(IEndpointRouteBuilder endpointRouteBuilder)
  {
    endpointRouteBuilder.MapGet("/api/profile", async Task<Results<Ok<UserProfile>, NotFound>> (HttpContext ctx, CancellationToken ct, IMessageBus bus, IMapper mapper) =>
    {
      var profile = (await bus.InvokeAsync<RegistrantQueryResults>(new RegistrantQuery { ByUserIdentity = ctx.User.GetUserContext()!.Identity }, ctx.RequestAborted)).Items.SingleOrDefault();
      if (profile == null) return TypedResults.NotFound();
      return TypedResults.Ok(mapper.Map<UserProfile>(profile.Profile));
    })
      .WithOpenApi("Gets the current user profile", string.Empty, "profile_get")
      .RequireAuthorization();

    endpointRouteBuilder.MapPut("/api/profile", async Task<Ok> (UserProfile profile, HttpContext ctx, CancellationToken ct, IMessageBus bus, IMapper mapper) =>
    {
      var registrant = new Registrant(ctx.User.GetUserContext()!.UserId, mapper.Map<Managers.Registry.Contract.Registrants.UserProfile>(profile));
      await bus.InvokeAsync<string>(new UpdateRegistrantProfileCommand(registrant), ctx.RequestAborted);
      return TypedResults.Ok();
    })
  .WithOpenApi("Gets the current user profile", string.Empty, "profile_put")
  .RequireAuthorization();
  }
}

/// <summary>
/// User profile information
/// </summary>
public record UserProfile(
    string? FirstName,
    string? LastName,
    DateOnly? DateOfBirth,
    string Email,
    string Phone,
    Address? ResidentialAddress,
    Address? MailingAddress
    );

/// <summary>
/// Address
/// </summary>
public record Address(
    string Line1,
    string? Line2,
    string City,
    string PostalCode,
    string? Province,
    string Country
    );
