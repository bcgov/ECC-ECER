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

  /// <summary>
  /// Saves a registrant's profile - registrant must exist
  /// </summary>
  Task SaveIdentityIds(Registrant registrant, ProfileIdentification profileIdentification, CancellationToken ct);
}

public record RegistrantQuery
{
  public string? ByUserId { get; set; }
  public string? ByRegistrationNumber { get; set; }
  public string? ByLastName { get; set; }
  public DateOnly? ByDateOfBirth { get; set; }
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
  public string? MiddleName { get; set; }
  public string? PreferredName { get; set; }
  public string? AlternateContactPhone { get; set; }
  public DateOnly? DateOfBirth { get; set; }
  public string Email { get; set; } = null!;
  public string Phone { get; set; } = null!;
  public Address? ResidentialAddress { get; set; }
  public Address? MailingAddress { get; set; }
  public bool IsVerified { get; set; }
  public string? RegistrationNumber { get; set; }
  public IEnumerable<PreviousName> PreviousNames { get; set; } = Array.Empty<PreviousName>();
  public bool IsRegistrant { get; set; }
  public StatusCode Status { get; set; }
  public IDVerificationDecision? IDVerificationDecision { get; set; }
};

public record PreviousName(string FirstName, string LastName)
{
  public string? Id { get; set; }
  public string? MiddleName { get; set; }
  public string? PreferredName { get; set; }
  public PreviousNameStage? Status { get; set; }
  public PreviousNameSources? Source { get; set; }
  public IEnumerable<IdentityDocument> Documents { get; set; } = Array.Empty<IdentityDocument>();
}

public enum PreviousNameStage
{
  Archived,
  PendingforDocuments,
  ReadyforVerification,
  Unverified,
  Verified,
}

public enum PreviousNameSources
{
  NameLog,
  Profile,
  Transcript,
  OutofProvinceCertificate
}

public enum IDVerificationDecision
{
  Approve,
  Reject
}

public enum StatusCode
{
  Inactive,
  PendingforDocuments,
  ReadyforIDVerification,
  ReadyforRegistrantMatch,
  Unverified,
  Verified,
}

public record IdentityDocument(string Id)
{
  public string Url { get; set; } = null!;
  public string Extention { get; set; } = null!;
  public string Name { get; set; } = null!;
  public string Size { get; set; } = null!;
}

public record Address(
    string Line1,
    string? Line2,
    string City,
    string PostalCode,
    string? Province,
    string Country
    );
public record ProfileIdentification()
{
  public string PrimaryIdTypeObjectId { get; set; } = null!;
  public IEnumerable<IdentityDocument> PrimaryIds { get; set; } = Array.Empty<IdentityDocument>();
  public string SecondaryIdTypeObjectId { get; set; } = null!;
  public IEnumerable<IdentityDocument> SecondaryIds { get; set; } = Array.Empty<IdentityDocument>();
}
