using ECER.Utilities.Security;
using MediatR;

namespace ECER.Managers.Registry.Contract.PspUsers;

/// <summary>
/// Invokes a psp rep query use case
/// </summary>
public record SearchPspRepQuery : IRequest<PspRepQueryResults>
{
  public UserIdentity? ByUserIdentity { get; set; }
}

/// <summary>
/// Container for <see cref="SearchPspRepQuery"/> results
/// </summary>
public record PspRepQueryResults(IEnumerable<PspUser> Items);

public record PspUser(string Id, PspUserProfile Profile);

public record PspUserProfile
{
  public string? FirstName { get; set; }
  public string? LastName { get; set; }
  public string? Email { get; set; } = null!;
  public string? BceidBusinessId { get; set; }
  public string? ProgramRepresentativeId { get; set; }
};

public record RegisterPspUserCommand(PspUserProfile Profile) : IRequest<RegisterPspUserResult>;

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
  BceidBusinessIdDoesNotMatch,
}

public record UpdatePspRepProfileCommand(PspUser User) : IRequest<string>;
