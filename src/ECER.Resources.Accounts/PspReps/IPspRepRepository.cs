using ECER.Utilities.Security;

namespace ECER.Resources.Accounts.PspReps;

/// <summary>
/// Psp Rep resource access
/// </summary>
public interface IPspRepRepository
{
  /// <summary>
  /// Attach identity to a Psp representative
  /// </summary>
  Task<string> AttachIdentity(PspUser user, CancellationToken ct);
  
  /// <summary>
  /// Query Psp representatives
  /// </summary>
  /// <returns>Enumerable of Psp representatives</returns>
  Task<IEnumerable<PspUser>> Query(PspRepQuery query, CancellationToken ct);

  /// <summary>
  /// Saves a Psp representative's profile - Psp representative must exist
  /// </summary>
  Task Save(PspUser user, CancellationToken ct);

  /// <summary>
  /// Disables portal access for a Psp representative and removes authentication
  /// </summary>
  Task Deactivate(string pspUserId, CancellationToken ct);

  /// <summary>
  /// Sets the specified Psp representative as Primary and others in the same institution to Secondary
  /// </summary>
  Task SetPrimary(string pspUserId, CancellationToken ct);
}

public record PspRepQuery
{
  public string? ById { get; set; }
  public UserIdentity? ByIdentity { get; set; }
  public string? ByPostSecondaryInstituteId { get; set; }
}

public record PspUser
{
  public string Id { get; set; } = null!;
  
  public IEnumerable<UserIdentity> Identities { get; set; } = Array.Empty<UserIdentity>();
  public PspUserProfile Profile { get; set; } = null!;
  public PortalAccessStatus? AccessToPortal { get; set; }
  public string? PostSecondaryInstituteId { get; set; }
};

public record PspUserProfile
{
  public string? Id { get; set; }
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
