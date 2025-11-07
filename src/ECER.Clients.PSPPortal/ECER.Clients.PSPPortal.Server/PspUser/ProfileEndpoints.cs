using System.ComponentModel.DataAnnotations;
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
      .RequireAuthorization("psp_new_user")
      .WithParameterValidation();

    endpointRouteBuilder.MapPut("/api/users/profile",
        async Task<Results<Ok, BadRequest<ProblemDetails>>> (PspUserProfile pspUserProfile, HttpContext ctx,
          CancellationToken ct, IMediator bus, IMapper mapper) =>
        {
          var pspUser = new PspUser(ctx.User.GetUserContext()!.UserId, mapper.Map<Managers.Registry.Contract.PspUsers.PspUserProfile>(pspUserProfile)!);
          await bus.Send(
            new UpdatePspRepProfileCommand(
              mapper.Map<PspUser>(pspUser)!),
            ctx.RequestAborted);
          return TypedResults.Ok();
        })
      .WithOpenApi("Update a psp user profile", string.Empty, "psp_user_profile_put")
      .RequireAuthorization("psp_new_user")
      .WithParameterValidation();

    endpointRouteBuilder.MapPost("/api/users/register",
        async Task<Results<Ok, BadRequest<ProblemDetails>>> (RegisterPspUserRequest request, HttpContext ctx,
          CancellationToken ct, IMediator bus, IMapper mapper) =>
        {
          var user = ctx.User.GetUserContext()!;
          await bus.Send(
            new RegisterPspUserCommand(request.Token, request.ProgramRepresentativeId, request.BceidBusinessId,
              mapper.Map<Managers.Registry.Contract.PspUsers.PspUserProfile>(request.Profile)!, user.Identity),
            ctx.RequestAborted);
          return TypedResults.Ok();
        })
      .WithOpenApi("Register new Psp Program Representative", string.Empty, "psp_user_register_post")
      .RequireAuthorization("psp_new_user")
      .WithParameterValidation();

  }
}

/// <summary>
/// Request to register a new psp user
/// </summary>
public record RegisterPspUserRequest([Required] string Token, [Required] string ProgramRepresentativeId, [Required] string BceidBusinessId)
{
  [Required]
  public PspUserProfile Profile { get; set; } = null!;
};

/// <summary>
/// User profile information
/// </summary>
public record PspUserProfile
{
  public string? FirstName { get; set; }
  public string? LastName { get; set; }
  public string? Email { get; set; } = null!;
};
