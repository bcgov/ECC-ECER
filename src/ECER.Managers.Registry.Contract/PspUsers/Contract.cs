using ECER.Utilities.Security;
using MediatR;

namespace ECER.Managers.Registry.Contract.PspUsers;

/// <summary>
/// Invokes a new psp user registration use case
/// </summary>
public record RegisterNewPspUserCommand(string Id, PspUserProfile Profile, UserIdentity Identity) : IRequest<string>;


/// <summary>
/// Invokes a psp rep query use case
/// </summary>
public record SearchPspRepQuery : IRequest<PspRepQueryResults>
{
  public UserIdentity? ByUserIdentity { get; set; }
  public string? ByPostSecondaryInstituteId { get; set; }
}

/// <summary>
/// Container for <see cref="SearchPspRepQuery"/> results
/// </summary>
public record PspRepQueryResults(IEnumerable<PspUser> Items);

public record PspUser(string Id, PspUserProfile Profile)
{
  public PortalAccessStatus? AccessToPortal { get; init; }
  public string? PostSecondaryInstituteId { get; init; }
}

public record PspUserProfile
{
  public string? FirstName { get; set; }
  public string? LastName { get; set; }
  public string? PreferredName { get; set; }
  public string? Phone { get; set; }
  public string? PhoneExtension { get; set; }
  public string? JobTitle { get; set; }
  public PspUserRole? Role { get; set; }
  public string? Email { get; set; } = null!;
  public bool? HasAcceptedTermsOfUse { get; set; }
};

public enum PspUserRole
{
  /// <summary>Primary (for email/communications</summary>
  Primary,
  /// <summary>Secondary</summary>
  Secondary
}

public enum PortalAccessStatus
{
  Invited,
  Active,
  Disabled
}

/// <summary>
/// Request to register a new psp user
/// </summary>
public record RegisterPspUserCommand(string Token, string ProgramRepresentativeId, string BceidBusinessId, PspUserProfile Profile, UserIdentity Identity) : IRequest<RegisterPspUserResult>;

public class RegisterPspUserResult
{
  public bool IsSuccess { get; init; }
  public RegisterPspUserError? Error { get; init; }

  public static RegisterPspUserResult Success() => new()
  {
    IsSuccess = true
  };

  public static RegisterPspUserResult Failure(RegisterPspUserError error) => new()
  {
    IsSuccess = false,
    Error = error
  };
}

public enum RegisterPspUserError
{
  PostSecondaryInstitutionNotFound,
  PortalInvitationTokenInvalid,
  PortalInvitationWrongStatus,
  BceidBusinessIdDoesNotMatch,
}

public record UpdatePspRepProfileCommand(PspUser User) : IRequest<string>;
