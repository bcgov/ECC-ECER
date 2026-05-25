using ECER.Utilities.DataverseSdk.Model;
using Riok.Mapperly.Abstractions;

namespace ECER.Resources.Documents.MetadataResources;

internal interface IMetadataResourceRepositoryMapper
{
  List<Country> MapCountries(IEnumerable<ecer_Country> source);
  List<AreaOfInstruction> MapAreasOfInstruction(IEnumerable<ecer_ProvincialRequirement> source);
  List<PostSecondaryInstitution> MapPostSecondaryInstitutions(IEnumerable<ecer_PostSecondaryInstitute> source);
  List<IdentificationType> MapIdentificationTypes(IEnumerable<ecer_identificationtype> source);
  List<Province> MapProvinces(IEnumerable<ecer_Province> source);
  List<CertificationComparison> MapCertificationComparisons(IEnumerable<ecer_certificationcomparison> source);
  List<SystemMessage> MapSystemMessages(IEnumerable<ecer_SystemMessage> source);
  List<DefaultContent> MapDefaultContents(IEnumerable<ecer_DefaultContents> source);
  List<DynamicsConfig> MapDynamicsConfigurations(IEnumerable<bcgov_config> source);
}

[Mapper]
internal partial class MetadataResourceRepositoryMapper : IMetadataResourceRepositoryMapper
{
  public List<Country> MapCountries(IEnumerable<ecer_Country> source) => source.Select(MapCountry).ToList();

  public List<AreaOfInstruction> MapAreasOfInstruction(IEnumerable<ecer_ProvincialRequirement> source) => source.Select(MapAreaOfInstruction).ToList();

  public List<PostSecondaryInstitution> MapPostSecondaryInstitutions(IEnumerable<ecer_PostSecondaryInstitute> source) => source.Select(MapPostSecondaryInstitution).ToList();

  public List<IdentificationType> MapIdentificationTypes(IEnumerable<ecer_identificationtype> source) => source.Select(MapIdentificationType).ToList();

  public List<Province> MapProvinces(IEnumerable<ecer_Province> source) => source.Select(MapProvince).ToList();

  public List<CertificationComparison> MapCertificationComparisons(IEnumerable<ecer_certificationcomparison> source) => source.Select(MapCertificationComparison).ToList();

  public List<SystemMessage> MapSystemMessages(IEnumerable<ecer_SystemMessage> source) => source.Select(MapSystemMessage).ToList();

  public List<DefaultContent> MapDefaultContents(IEnumerable<ecer_DefaultContents> source) => source.Select(MapDefaultContent).ToList();

  public List<DynamicsConfig> MapDynamicsConfigurations(IEnumerable<bcgov_config> source) => source.Select(MapDynamicsConfiguration).ToList();

  private static Province MapProvince(ecer_Province source) => new(
    source.ecer_ProvinceId?.ToString() ?? string.Empty,
    source.ecer_Name ?? string.Empty,
    source.ecer_Abbreviation ?? string.Empty);

  private SystemMessage MapSystemMessage(ecer_SystemMessage source) => new(
    source.ecer_name ?? string.Empty,
    source.ecer_subject ?? string.Empty,
    source.ecer_message ?? string.Empty)
  {
    StartDate = source.ecer_startdate.GetValueOrDefault(),
    EndDate = source.ecer_enddate.GetValueOrDefault(),
    PortalTags = source.ecer_PortalTags?.Select(MapPortalTag).ToArray() ?? Array.Empty<PortalTags>(),
  };

  private static Country MapCountry(ecer_Country source) => new(
    source.ecer_CountryId?.ToString() ?? string.Empty,
    source.ecer_Name ?? string.Empty,
    source.ecer_ShortName ?? string.Empty,
    source.ecer_EligibleforICRA.GetValueOrDefault());

  private static AreaOfInstruction MapAreaOfInstruction(ecer_ProvincialRequirement source) => new(
    source.ecer_ProvincialRequirementId?.ToString() ?? string.Empty,
    source.ecer_Name ?? string.Empty,
    source.ecer_CertificateLevels?.Select(level => level.ToString()).ToArray() ?? Array.Empty<string>(),
    source.ecer_MinimumHours,
    source.ecer_DisplayOrder,
    source.ecer_ParentAreaofInstructionId?.Id.ToString());

  private static PostSecondaryInstitution MapPostSecondaryInstitution(ecer_PostSecondaryInstitute source) => new(
    source.ecer_PostSecondaryInstituteId?.ToString() ?? string.Empty,
    source.ecer_Name ?? string.Empty,
    source.ecer_ProvinceId?.Id.ToString() ?? string.Empty);

  private static IdentificationType MapIdentificationType(ecer_identificationtype source) => new(
    source.ecer_identificationtypeId?.ToString() ?? string.Empty,
    source.ecer_Name ?? string.Empty,
    source.ecer_ForPrimary.GetValueOrDefault(),
    source.ecer_ForSecondary.GetValueOrDefault());

  private OutOfProvinceCertificationType MapOutOfProvinceCertificationType(ecer_outofprovincecertificationtype source) => new(source.Id.ToString())
  {
    CertificationType = source.ecer_certificationtype,
  };

  private CertificationComparison MapCertificationComparison(ecer_certificationcomparison source) => new(source.Id.ToString())
  {
    BcCertificate = source.ecer_bccertificate,
    TransferringCertificate = source.ecer_certificationcomparisontransferringcertificate == null
      ? null
      : MapOutOfProvinceCertificationType(source.ecer_certificationcomparisontransferringcertificate),
  };

  private DefaultContent MapDefaultContent(ecer_DefaultContents source) => new()
  {
    Name = source.ecer_Name,
    SingleText = source.ecer_SingleLineofText,
    MultiText = source.ecer_MultipleLineofText,
  };

  private static DynamicsConfig MapDynamicsConfiguration(bcgov_config source) => new(
    source.bcgov_Key ?? string.Empty,
    source.bcgov_Value ?? string.Empty);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial PortalTags MapPortalTag(ecer_PortalTags source);
}
