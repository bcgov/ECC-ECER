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
}

public record PspRepQuery
{
  public string? ById { get; set; }
  public UserIdentity? ByIdentity { get; set; }
}

public record PspUser
{
  public string Id { get; set; } = null!;
  
  public IEnumerable<UserIdentity> Identities { get; set; } = Array.Empty<UserIdentity>();
  public PspUserProfile Profile { get; set; } = null!;
};

public record PspUserProfile
{
  public string? FirstName { get; set; }
  public string? LastName { get; set; }
  public string? Email { get; set; } = null!;
};
