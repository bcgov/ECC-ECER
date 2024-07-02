using ECER.Utilities.Security;

namespace ECER.Resources.Accounts.Registrants;

/// <summary>
/// Registrant resouce access
/// </summary>
public interface IRegistrantRepository
{
  /// <summary>
  /// Create a new registrant
  /// </summary>
  Task<string> Create(Registrant registrant, CancellationToken ct);

  /// <summary>
  /// Query registrants
  /// </summary>
  /// <returns>Enumerable of registrants</returns>
  Task<IEnumerable<Registrant>> Query(RegistrantQuery query, CancellationToken ct);

  /// <summary>
  /// Saves a registrant's profile - registrant must exist
  /// </summary>
  Task Save(Registrant registrant, CancellationToken ct);
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
  public IEnumerable<PreviousName> PreviousNames { get; set; } = Array.Empty<PreviousName>();
};

public record PreviousName (string? FirstName, string? LastName)
{
  public string? Id { get; set; }
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
    string? Line2,
    string City,
    string PostalCode,
    string? Province,
    string Country
    );
