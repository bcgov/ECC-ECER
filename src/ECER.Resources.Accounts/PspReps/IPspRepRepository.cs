using ECER.Utilities.Security;

namespace ECER.Resources.Accounts.PspReps;

/// <summary>
/// Psp Rep resource access
/// </summary>
public interface IPspRepRepository
{
  /// <summary>
  /// Query Psp representatives
  /// </summary>
  /// <returns>Enumerable of Psp representatives</returns>
  Task<IEnumerable<PspRep>> Query(PspRepQuery query, CancellationToken ct);

  /// <summary>
  /// Saves a Psp representative's profile - Psp representative must exist
  /// </summary>
  Task Save(PspRep pspRep, CancellationToken ct);
}

public record PspRepQuery
{
  public string? ById { get; set; }
  public UserIdentity? ByIdentity { get; set; }
}

public record PspRep
{
  public string Id { get; set; } = null!;
  public string? FirstName { get; set; }
  public string? LastName { get; set; }
  public string? Email { get; set; } = null!;
  public string? BceidBusinessId { get; set; }
};
