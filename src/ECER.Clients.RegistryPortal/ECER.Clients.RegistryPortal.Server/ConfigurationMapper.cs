using Riok.Mapperly.Abstractions;
using ContractMetadatas = ECER.Managers.Admin.Contract.Metadatas;

namespace ECER.Clients.RegistryPortal.Server;

internal interface IConfigurationMapper
{
  IEnumerable<Province> MapProvinces(IEnumerable<ContractMetadatas.Province> source);
  IEnumerable<Country> MapCountries(IEnumerable<ContractMetadatas.Country> source);
  IEnumerable<SystemMessage> MapSystemMessages(IEnumerable<ContractMetadatas.SystemMessage> source);
  IEnumerable<IdentificationType> MapIdentificationTypes(IEnumerable<ContractMetadatas.IdentificationType> source);
  IEnumerable<PostSecondaryInstitution> MapPostSecondaryInstitutions(IEnumerable<ContractMetadatas.PostSecondaryInstitution> source);
  IEnumerable<ComparisonRecord> MapComparisonRecords(IEnumerable<ContractMetadatas.ComparisonRecord> source);
  IEnumerable<DefaultContent> MapDefaultContents(IEnumerable<ContractMetadatas.DefaultContent> source);
}

[Mapper]
internal partial class ConfigurationMapper : IConfigurationMapper
{
  public IEnumerable<Province> MapProvinces(IEnumerable<ContractMetadatas.Province> source) => source.Select(MapProvince).ToList();

  public IEnumerable<Country> MapCountries(IEnumerable<ContractMetadatas.Country> source) => source.Select(MapCountry).ToList();

  public IEnumerable<SystemMessage> MapSystemMessages(IEnumerable<ContractMetadatas.SystemMessage> source) => source.Select(MapSystemMessage).ToList();

  public IEnumerable<IdentificationType> MapIdentificationTypes(IEnumerable<ContractMetadatas.IdentificationType> source) => source.Select(MapIdentificationType).ToList();

  public IEnumerable<PostSecondaryInstitution> MapPostSecondaryInstitutions(IEnumerable<ContractMetadatas.PostSecondaryInstitution> source) => source.Select(MapPostSecondaryInstitution).ToList();

  public IEnumerable<ComparisonRecord> MapComparisonRecords(IEnumerable<ContractMetadatas.ComparisonRecord> source) => source.Select(MapComparisonRecord).ToList();

  public IEnumerable<DefaultContent> MapDefaultContents(IEnumerable<ContractMetadatas.DefaultContent> source) => source.Select(MapDefaultContent).ToList();

  private SystemMessage MapSystemMessage(ContractMetadatas.SystemMessage source) => new(source.Name, source.Subject, source.Message)
  {
    StartDate = source.StartDate,
    EndDate = source.EndDate,
    PortalTags = source.PortalTags.Select(MapPortalTag).ToArray(),
  };

  private ComparisonRecord MapComparisonRecord(ContractMetadatas.ComparisonRecord source) => new()
  {
    TransferringCertificate = source.TransferringCertificate == null ? null : MapOutOfProvinceCertificationType(source.TransferringCertificate),
    Options = source.Options.Select(MapCertificationComparison).ToList(),
  };

  private partial Province MapProvince(ContractMetadatas.Province source);

  private partial Country MapCountry(ContractMetadatas.Country source);

  private partial IdentificationType MapIdentificationType(ContractMetadatas.IdentificationType source);

  private partial PostSecondaryInstitution MapPostSecondaryInstitution(ContractMetadatas.PostSecondaryInstitution source);

  private partial OutOfProvinceCertificationType MapOutOfProvinceCertificationType(ContractMetadatas.OutOfProvinceCertificationType source);

  private partial CertificationComparison MapCertificationComparison(ContractMetadatas.CertificationComparison source);

  private partial DefaultContent MapDefaultContent(ContractMetadatas.DefaultContent source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial PortalTags MapPortalTag(ContractMetadatas.PortalTags source);
}
