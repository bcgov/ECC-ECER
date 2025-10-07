using AutoMapper;
using ECER.Managers.Registry.Contract.Registrants;
using ECER.Utilities.Hosting;
using ECER.Utilities.Security;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ECER.Clients.PSPPortal.Server.Users;

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

    endpointRouteBuilder.MapPost("/api/profile/verificationIds", async Task<Results<Ok, BadRequest<ProblemDetails>>> (ProfileIdentification profileIdentification, HttpContext ctx, CancellationToken ct, IMediator bus, IMapper mapper) =>
    {
      profileIdentification.RegistrantId = ctx.User.GetUserContext()!.UserId;

      await bus.Send(new UpdateRegistrantProfileIdentificationCommand(mapper.Map<Managers.Registry.Contract.Registrants.ProfileIdentification>(profileIdentification)!), ctx.RequestAborted);
      return TypedResults.Ok();
    })
  .WithOpenApi("Sets user verification Ids", string.Empty, "profileVerification_post")
  .RequireAuthorization("registry_unverified_user");
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
  public IEnumerable<IdentityDocument> Documents { get; set; } = Array.Empty<IdentityDocument>();
}

public enum PreviousNameStage
{
  Archived,
  PendingforDocuments,
  ReadyforVerification,
  Unverified,
  Verified,
}

public enum PreviousNameSources
{
  NameLog,
  Profile,
  Transcript,
  OutofProvinceCertificate
}

public record IdentityDocument(string Id)
{
  public string Url { get; set; } = null!;
  public string Extention { get; set; } = null!;
  public string Name { get; set; } = null!;
  public string Size { get; set; } = null!;
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

public record ProfileIdentification()
{
  public string RegistrantId { get; set; } = null!;
  public string PrimaryIdTypeObjectId { get; set; } = null!;
  public IEnumerable<IdentityDocument> PrimaryIds { get; set; } = Array.Empty<IdentityDocument>();
  public string SecondaryIdTypeObjectId { get; set; } = null!;
  public IEnumerable<IdentityDocument> SecondaryIds { get; set; } = Array.Empty<IdentityDocument>();
}
