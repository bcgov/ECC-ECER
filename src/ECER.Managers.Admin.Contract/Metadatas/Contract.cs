using MediatR;

namespace ECER.Managers.Admin.Contract.Metadatas;

/// <summary>
/// Invokes provinces use case
/// </summary>
public record ProvincesQuery : IRequest<ProvincesQueryResults>
{
  public string? ById { get; set; }
}

public record CertificationComparisonQuery : IRequest<CertificationComparisonQueryResults>
{
  public string? ById { get; set; }
  public string? ByProvinceId { get; set; }
}
public record DefaultContent
{
  public string? Name { get; set; }
  public string? SingleText { get; set; }
  public string? MultiText { get; set; }
}
public record CountriesQuery : IRequest<CountriesQueryResults>
{
  public string? ById { get; set; }
  public string? ByCode { get; set; }
  public string? ByName { get; set; }
  public bool? ByICRA { get; set; }

}

public record AreaOfInstructionsQuery : IRequest<AreaOfInstructionsQueryResults>
{
  public string? ById { get; set; }
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
public record DefaultContentsQuery : IRequest<DefaultContentsQueryResults> { }
public record DynamicsConfigQuery : IRequest<DynamicsConfigQueryResults> { };
public record SystemMessagesQueryResults(IEnumerable<SystemMessage> Items);
public record IdentificationTypesQueryResults(IEnumerable<IdentificationType> Items);
public record DefaultContentsQueryResults(IEnumerable<DefaultContent> Items);
public record CertificationComparisonQueryResults(IEnumerable<ComparisonRecord> Items);
public record ProvincesQueryResults(IEnumerable<Province> Items);
public record CountriesQueryResults(IEnumerable<Country> Items);
public record AreaOfInstructionsQueryResults(IEnumerable<AreaOfInstruction> Items);
public record PostSecondaryInstitutionsQueryResults(IEnumerable<PostSecondaryInstitution> Items);
public record DynamicsConfigQueryResults(DynamicsConfig config);
public record Province(string ProvinceId, string ProvinceName, string ProvinceCode);
public record Country(string CountryId, string CountryName, string CountryCode, bool IsICRA);
public record AreaOfInstruction(string Id, string Name, IEnumerable<string> ProgramTypes, int? MinimumHours);
public record PostSecondaryInstitution(string Id, string Name, string ProvinceId);
public record IdentificationType(string Id, string Name, bool ForPrimary, bool ForSecondary);
public record DynamicsConfig(bool ICRAFeatureEnabled);
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

public record OutOfProvinceCertificationType(string Id)
{
  public string? CertificationType { get; set; }
}

public record CertificationComparison(string Id)
{
  public string? BcCertificate { get; set; }
}

public record ComparisonRecord()
{
  public OutOfProvinceCertificationType? TransferringCertificate { get; set; }
  public IEnumerable<CertificationComparison> Options { get; set; } = Array.Empty<CertificationComparison>();
}
