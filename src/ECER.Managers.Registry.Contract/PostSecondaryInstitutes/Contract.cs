using ECER.Managers.Admin.Contract.Metadatas;
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

public record UpdatePostSecondaryInstitutionCommand(PostSecondaryInstitute Institute) : IRequest<string>;

public record PostSecondaryInstitute
{
  public string Id { get; set; } = null!;
  public string? Name { get; set; }
  public PsiInstitutionType? InstitutionType { get; set; }
  public PrivateAuspiceType? PrivateAuspiceType { get; set; }
  public string? PtiruInstitutionId { get; set; }
  public string? WebsiteUrl { get; set; }
  public string? Street1 { get; set; }
  public string? Street2 { get; set; }
  public string? Street3 { get; set; }
  public string? City { get; set; }
  public string? Province { get; set; }
  public string? Country { get; set; }
  public string? PostalCode { get; set; }
  public IEnumerable<Campus>? Campuses { get; set; }
};

public record Campus
{
  public string Id { get; set; } = null!;
  public string? Name { get; set; }
  public CampusStatus? Status { get; set; }
  public bool? IsSatelliteOrTemporaryLocation { get; set; }
  public string? Street1 { get; set; }
  public string? Street2 { get; set; }
  public string? Street3 { get; set; }
  public string? City { get; set; }
  public string? Province { get; set; }
  public string? PostalCode { get; set; }
}
public enum CampusStatus
{
  None,
  Active = 1,
  Inactive = 2
}

public enum PsiInstitutionType
{
  Private,
  Public,
  ContinuingEducation,
  PublicOOP,
}

public enum PrivateAuspiceType
{
  Theologicalinstitution,
  FirstNationsmandatedpostsecondaryinstitute,
  Other,
  Privatetraininginstitution,
  Indigenouscontrolledpostsecondaryinstitute,
}

