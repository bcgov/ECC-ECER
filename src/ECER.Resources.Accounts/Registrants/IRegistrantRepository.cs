﻿using ECER.Utilities.Security;

namespace ECER.Resources.Accounts.Registrants;

/// <summary>
/// Manages registrants
/// </summary>
public interface IRegistrantRepository
{
  /// <summary>
  /// Create a new registrant
  /// </summary>
  Task<string> Create(Registrant registrant);

  Task<IEnumerable<Registrant>> Query(RegistrantQuery query);
}

public record RegistrantQuery
{
  public string? ByUserId { get; set; }
  public UserIdentity? ByIdentity { get; set; }
}

public record Registrant
{
  public string Id { get; set; } = null!;
  public IEnumerable<UserIdentity> Identities { get; set; } = Array.Empty<UserIdentity>();
  public UserProfile Profile { get; set; } = null!;
}

public record UserProfile
{
  public string? FirstName { get; set; }

  public string? LastName { get; set; }
  public DateOnly? DateOfBirth { get; set; }
  public string? Email { get; set; }
  public string? Phone { get; set; }
  public Address? ResidentialAddress { get; set; }
  public Address? MailingAddress { get; set; }
}

public record Address(
    string Line1,
    string? Line2,
    string City,
    string PostalCode,
    string? Province,
    string Country
    );
