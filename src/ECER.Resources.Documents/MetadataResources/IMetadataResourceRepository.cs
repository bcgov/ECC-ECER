﻿namespace ECER.Resources.Documents.MetadataResources;

public interface IMetadataResourceRepository
{
  Task<IEnumerable<Province>> QueryProvinces(ProvincesQuery query, CancellationToken cancellationToken);

  Task<IEnumerable<SystemMessage>> QuerySystemMessages(SystemMessagesQuery query, CancellationToken cancellationToken);

  Task<IEnumerable<Country>> QueryCountries(CountriesQuery query, CancellationToken cancellationToken);

  Task<IEnumerable<PostSecondaryInstitution>> QueryPostSecondaryInstitutions(PostSecondaryInstitutionsQuery query, CancellationToken cancellationToken);

  Task<IEnumerable<IdentificationType>> QueryIdentificationTypes(IdentificationTypesQuery query, CancellationToken cancellationToken);

  Task<string> SetDownloadDate(string fileId, CancellationToken cancellationToken);
  Task<IEnumerable<CertificationComparison>> QueryCertificationComparisons(CertificationComparisonQuery query, CancellationToken cancellationToken);

}

public record Province(string ProvinceId, string ProvinceName, string ProvinceCode);
public record Country(string CountryId, string CountryName, string CountryCode);
public record IdentificationType(string Id, string Name, bool ForPrimary, bool ForSecondary);
public record PostSecondaryInstitution(string Id, string Name, string ProvinceId);
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

public record ProvincesQuery
{
  public string? ById { get; set; }
}

public record CertificationComparisonQuery
{
  public string? ById { get; set; }
  public string? ByProvinceId { get; set; }
}

public record SystemMessagesQuery
{
  public string? ById { get; set; }
}

public record CountriesQuery
{
  public string? ById { get; set; }
  public string? ByCode { get; set; }
  public string? ByName { get; set; }
}

public record PostSecondaryInstitutionsQuery
{
  public string? ById { get; set; }
  public string? ByProvinceId { get; set; }
  public string? ByName { get; set; }
}

public record IdentificationTypesQuery
{
  public string? ById { get; set; }
  public bool? ForPrimary { get; set; }
  public bool? ForSecondary { get; set; }
}

public record OutOfProvinceCertificationType(string Id)
{
  public string? CertificationType { get; set; }
}

public record CertificationComparison(string Id) 
{ 
  public string? BcCertificate { get; set; }
  public OutOfProvinceCertificationType? TransferringCertificate { get; set; }
}
