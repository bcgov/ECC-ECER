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

public record UserProfile(
    string FirstName,
    string LastName,
    DateOnly? DateOfBirth,
    string Email,
    string Phone,
    Address? ResidentialAddress,
    Address? MailingAddress,
    string? PreferredName,
    string? MiddleName,
    string? AlternateContactPhone
    );

public record Address(
    string Line1,
    string Line2,
    string City,
    string PostalCode,
    string? Province,
    string Country
    );
