using ECER.Utilities.Security;
using MediatR;

namespace ECER.Managers.Registry.Contract.Registrants;

/// <summary>
/// Invokes a new registrant registration use case
/// </summary>
public record RegisterNewUserCommand(UserProfile Profile, UserIdentity Identity) : IRequest<string>;

/// <summary>
/// Invokes updating a registrant's profile use case
/// </summary>
/// <param name="Registrant"></param>
public record UpdateRegistrantProfileCommand(Registrant Registrant) : IRequest<string>;

/// <summary>
/// Invokes a registrant query use case
/// </summary>
public record SearchRegistrantQuery : IRequest<RegistrantQueryResults>
{
  public UserIdentity? ByUserIdentity { get; set; }
}

/// <summary>
/// Container for <see cref="SearchRegistrantQuery"/> results
/// </summary>
public record RegistrantQueryResults(IEnumerable<Registrant> Items);

public record Registrant(string UserId, UserProfile Profile);

public record UserProfile {
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
  public IEnumerable<PreviousName>? PreviousNames { get; set; } = Array.Empty<PreviousName>();
};

public record PreviousName (string? FirstName, string? LastName)
{
  public string? MiddleName { get; set; }
  public string? PreferredName { get; set; }
  public PreviousNameStage? Status { get; set; }
}

public enum PreviousNameStage
{
  Unverified,
  ReadyforVerification,
  Verified,
  Archived,
}

public record Address(
    string Line1,
    string Line2,
    string City,
    string PostalCode,
    string? Province,
    string Country
    );
