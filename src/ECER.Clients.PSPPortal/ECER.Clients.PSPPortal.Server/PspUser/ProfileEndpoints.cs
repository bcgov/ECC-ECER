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
        async Task<Results<Ok, BadRequest<PspRegistrationErrorResponse>>> (
          RegisterPspUserRequest request, HttpContext ctx,
          CancellationToken ct, IMediator bus, IMapper mapper) =>
        {
          var user = ctx.User.GetUserContext()!;
          var result = await bus.Send(
            new RegisterPspUserCommand(
              request.Token,
              request.ProgramRepresentativeId,
              request.BceidBusinessId,
              mapper.Map<Managers.Registry.Contract.PspUsers.PspUserProfile>(request.Profile)!,
              user.Identity
            ),
            ctx.RequestAborted);

          if (!result.IsSuccess && result.Error.HasValue)
          {
            return TypedResults.BadRequest(new PspRegistrationErrorResponse
            {
              ErrorCode = result.Error.Value switch
              {
                RegisterPspUserError.PostSecondaryInstitutionNotFound => PspRegistrationError
                  .PostSecondaryInstitutionNotFound,
                RegisterPspUserError.PortalInvitationTokenInvalid => PspRegistrationError.PortalInvitationTokenInvalid,
                RegisterPspUserError.PortalInvitationWrongStatus => PspRegistrationError.PortalInvitationWrongStatus,
                RegisterPspUserError.BceidBusinessIdDoesNotMatch => PspRegistrationError.BceidBusinessIdDoesNotMatch,
                _ => PspRegistrationError.PostSecondaryInstitutionNotFound
              }
            });
          }

          return TypedResults.Ok();
        })
      .WithOpenApi("Update a psp user profile", string.Empty, "psp_user_register_post")
      .Produces<PspRegistrationErrorResponse>(StatusCodes.Status400BadRequest)
      .Produces(StatusCodes.Status200OK)
      .WithOpenApi()
      .RequireAuthorization("psp_new_user");
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

/// <summary>
/// Error codes for PSP user registration failures
/// </summary>
public enum PspRegistrationError
{
  /// <summary>The specified post-secondary institution was not found</summary>
  PostSecondaryInstitutionNotFound,
  /// <summary>The invitation token is invalid or expired</summary>
  PortalInvitationTokenInvalid,
  /// <summary>The invitation is not in correct status for registration</summary>
  PortalInvitationWrongStatus,
  /// <summary>The BCeID Business ID doesn't match expected value</summary>
  BceidBusinessIdDoesNotMatch
}

/// <summary>
/// Error response for PSP user registration failures. Returns only the error code for frontend handling.
/// </summary>
public record PspRegistrationErrorResponse
{
  /// <summary>
  /// The specific error code that occurred during registration.
  /// Frontend should handle localization and user messaging based on this code.
  /// </summary>
  /// <example>PostSecondaryInstitutionNotFound</example>
  public PspRegistrationError ErrorCode { get; set; }
}
