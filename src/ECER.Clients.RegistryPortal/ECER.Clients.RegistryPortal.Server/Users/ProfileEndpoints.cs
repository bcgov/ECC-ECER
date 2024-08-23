using System.ComponentModel.DataAnnotations;
using AutoMapper;
using ECER.Managers.Registry.Contract.Registrants;
using ECER.Utilities.Hosting;
using ECER.Utilities.Security;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ECER.Clients.RegistryPortal.Server.Users;

public class ProfileEndpoints : IRegisterEndpoints
{
  public void Register(IEndpointRouteBuilder endpointRouteBuilder)
  {
    endpointRouteBuilder.MapGet("/api/profile", async Task<Results<Ok<UserProfile>, NotFound>> (HttpContext ctx, CancellationToken ct, IMediator bus, IMapper mapper) =>
    {
      var profile = (await bus.Send(new SearchRegistrantQuery { ByUserIdentity = ctx.User.GetUserContext()!.Identity }, ctx.RequestAborted)).Items.SingleOrDefault();
      if (profile == null) return TypedResults.NotFound();
      return TypedResults.Ok(mapper.Map<UserProfile>(profile.Profile));
    })
      .WithOpenApi("Gets the current user profile", string.Empty, "profile_get")
      .RequireAuthorization();

    endpointRouteBuilder.MapPut("/api/profile", async Task<Ok> (UserProfile profile, HttpContext ctx, CancellationToken ct, IMediator bus, IMapper mapper) =>
    {
      var registrant = new Registrant(ctx.User.GetUserContext()!.UserId, mapper.Map<Managers.Registry.Contract.Registrants.UserProfile>(profile)!);
      await bus.Send(new UpdateRegistrantProfileCommand(registrant), ctx.RequestAborted);
      return TypedResults.Ok();
    })
  .WithOpenApi("Gets the current user profile", string.Empty, "profile_put")
  .RequireAuthorization();
  }
}

/// <summary>
/// User profile information
/// </summary>
public record UserProfile
{
  public string? FirstName { get; set; }
  public string? LastName { get; set; }
  public string? MiddleName { get; set; }
  public string? PreferredName { get; set; }
  public string? AlternateContactPhone { get; set; }
  public DateOnly? DateOfBirth { get; set; }
  public string Email { get; set; } = null!;
  public string Phone { get; set; } = null!;
  public Address? ResidentialAddress { get; set; }
  public Address? MailingAddress { get; set; }
  public IEnumerable<PreviousName> PreviousNames { get; set; } = Array.Empty<PreviousName>();
};

/// <summary>
/// Previous Name
/// </summary>
public record PreviousName([Required] string FirstName, [Required] string LastName)
{
  public string? Id { get; set; }
  public string? MiddleName { get; set; }
  public string? PreferredName { get; set; }
  public PreviousNameStage? Status { get; set; }
  public PreviousNameSources? Source { get; set; }
}

public enum PreviousNameStage
{
  Unverified,
  ReadyforVerification,
  Verified,
  Archived,
}

public enum PreviousNameSources
{
  NameLog,
  Profile,
  Transcript,
}

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
