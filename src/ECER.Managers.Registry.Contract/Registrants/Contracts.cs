﻿using ECER.Utilities.Security;

namespace ECER.Managers.Registry.Contract.Registrants;

/// <summary>
/// Invokes a new registrant registration use case
/// </summary>
public record RegisterNewUserCommand(UserProfile Profile, UserIdentity Identity);

/// <summary>
/// Invokes updating a registrant's profile use case
/// </summary>
/// <param name="Registrant"></param>
public record UpdateRegistrantProfileCommand(Registrant Registrant);

/// <summary>
/// Invokes a registrant query use case
/// </summary>
public record RegistrantQuery
{
  public UserIdentity? ByUserIdentity { get; set; }
}

/// <summary>
/// contains <see cref="RegistrantQuery"/> results
/// </summary>
public record RegistrantQueryResults(IEnumerable<Registrant> Items);

public record Registrant(string UserId, UserProfile Profile);

public record UserProfile(
    string FirstName,
    string LastName,
    string? PreferredName,
    string? MiddleName,
    DateOnly? DateOfBirth,
    string Email,
    string Phone,
    string? AlternateContactPhone,
    Address? ResidentialAddress,
    Address? MailingAddress
    );

public record Address(
    string Line1,
    string Line2,
    string City,
    string PostalCode,
    string? Province,
    string Country
    );
