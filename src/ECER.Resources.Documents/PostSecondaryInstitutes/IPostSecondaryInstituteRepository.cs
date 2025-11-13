namespace ECER.Resources.Documents.PostSecondaryInstitutes;

/// <summary>
/// Post Secondary Institute resource access
/// </summary>
public interface IPostSecondaryInstituteRepository
{
  /// <summary>
  /// Query Psp institutes
  /// </summary>
  /// <returns>Enumerable of post secondary institutes</returns>
  Task<IEnumerable<PostSecondaryInstitute>> Query(PostSecondaryInstituteQuery query, CancellationToken ct);

  /// <summary>
  /// Saves a post secondary institute - post secondary institute must exist
  /// </summary>
  Task Save(PostSecondaryInstitute institute, CancellationToken ct);
}

public record PostSecondaryInstituteQuery
{
  public string? ById { get; set; }
  public string? ByBceidBusinessId { get; set; }
  public string? ByProgramRepresentativeId { get; set; }
}

public record PostSecondaryInstitute
{
  public string Id { get; set; } = null!;
  public string? Name { get; set; }
  public string? BceidBusinessId { get; set; }
};

