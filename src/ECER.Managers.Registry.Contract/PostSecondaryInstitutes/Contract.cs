using ECER.Utilities.Security;
using MediatR;

namespace ECER.Managers.Registry.Contract.PostSecondaryInstitutes;

/// <summary>
/// Invokes a post secondary institutions query use case
/// </summary>
public record SearchPostSecondaryInstitutionQuery : IRequest<PostSecondaryInstitutionsQueryResults>
{
  public string? ByProgramRepresentativeId { get; set; }
}

/// <summary>
/// Container for <see cref="SearchPostSecondaryInstitutionQuery"/> results
/// </summary>
public record PostSecondaryInstitutionsQueryResults(IEnumerable<PostSecondaryInstitute> Items);

public record PostSecondaryInstitute
{
  public string Id { get; set; } = null!;
  public string? Name { get; set; }
  public Auspice? Auspice { get; set; }
  public string? WebsiteUrl { get; set; }
  public string? Street1 { get; set; }
  public string? Street2 { get; set; }
  public string? Street3 { get; set; }
  public string? City { get; set; }
  public string? Province { get; set; }
  public string? Country { get; set; }
  public string? PostalCode { get; set; }
};

public enum Auspice
{
  ContinuingEducation,
  PublicOOP,
  Private,
  Public
}

