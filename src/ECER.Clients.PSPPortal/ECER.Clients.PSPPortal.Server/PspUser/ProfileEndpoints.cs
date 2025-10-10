using AutoMapper;
using ECER.Managers.Registry.Contract.PspUsers;
using ECER.Utilities.Hosting;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ECER.Clients.PSPPortal.Server.Users;

public class ProfileEndpoints : IRegisterEndpoints
{
  public void Register(IEndpointRouteBuilder endpointRouteBuilder)
  {
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
      .WithOpenApi("Create Program Representative", string.Empty, "programRepresentative_post");
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
