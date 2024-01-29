using AutoMapper;
using ECER.Utilities.Hosting;
using Microsoft.AspNetCore.Http.HttpResults;
using Wolverine;

namespace ECER.Clients.RegistryPortal.Server.Users;

public class ProfileEndpoints : IRegisterEndpoints
{
  public void Register(IEndpointRouteBuilder endpointRouteBuilder)
  {
    endpointRouteBuilder.MapGet("/api/profile", async Task<UserProfile> (HttpContext ctx, CancellationToken ct, IMessageBus bus, IMapper mapper) =>
    {
      await Task.CompletedTask;
      return new UserProfile("first", "last", new DateOnly(200, 1, 1), "email@test.gov.bc.ca", "123-123-1234", null, null);
    })
      .WithOpenApi("Gets the current user profile", string.Empty, "profile_get")
      .RequireAuthorization();

    endpointRouteBuilder.MapPut("/api/profile", async Task<Ok> (HttpContext ctx, CancellationToken ct, IMessageBus bus, IMapper mapper) =>
    {
      await Task.CompletedTask;
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
