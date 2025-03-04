using MediatR;

namespace ECER.Managers.Admin.Contract.Metadatas;

/// <summary>
/// Invokes provinces use case
/// </summary>
public record ProvincesQuery : IRequest<ProvincesQueryResults>
{
  public string? ById { get; set; }
}
public record CountriesQuery : IRequest<CountriesQueryResults>
{
  public string? ById { get; set; }
  public string? ByCode { get; set; }
  public string? ByName { get; set; }
}

public record PostSecondaryInstitutionsQuery : IRequest<PostSecondaryInstitutionsQueryResults>
{
  public string? ById { get; set; }
  public string? ByProvinceId { get; set; }
  public string? ByName { get; set; }
}

public record SystemMessagesQuery : IRequest<SystemMessagesQueryResults>
{
  public string? ById { get; set; }
}

public record IdentificationTypesQuery : IRequest<IdentificationTypesQueryResults>
{
  public string? ById { get; set; }
  public bool? ForPrimary { get; set; }
  public bool? ForSecondary { get; set; }
}
public record SystemMessagesQueryResults(IEnumerable<SystemMessage> Items);
public record IdentificationTypesQueryResults(IEnumerable<IdentificationType> Items);
public record ProvincesQueryResults(IEnumerable<Province> Items);
public record CountriesQueryResults(IEnumerable<Country> Items);
public record PostSecondaryInstitutionsQueryResults(IEnumerable<PostSecondaryInstitution> Items);
public record Province(string ProvinceId, string ProvinceName, string ProvinceCode);
public record Country(string CountryId, string CountryName, string CountryCode);
public record PostSecondaryInstitution(string Id, string Name, string ProvinceId);
public record IdentificationType(string Id, string Name, bool ForPrimary, bool ForSecondary);
public record SystemMessage(string Name, string Subject, string Message)
{
  public DateTime StartDate { get; set; }
  public DateTime EndDate { get; set; }
  public IEnumerable<PortalTags> PortalTags { get; set; } = Array.Empty<PortalTags>();
}

public enum PortalTags
{
  LOGIN,
  LOOKUP,
  REFERENCES
}
