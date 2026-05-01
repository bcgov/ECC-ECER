using Riok.Mapperly.Abstractions;
using ContractMetadatas = ECER.Managers.Admin.Contract.Metadatas;
using ResourceMetadata = ECER.Resources.Documents.MetadataResources;

namespace ECER.Managers.Admin;

public interface IMetadataMapper
{
  ResourceMetadata.PostSecondaryInstitutionsQuery MapPostSecondaryInstitutionsQuery(ContractMetadatas.PostSecondaryInstitutionsQuery source);
  IEnumerable<ContractMetadatas.Province> MapProvinces(IEnumerable<ResourceMetadata.Province> source);
  IEnumerable<ContractMetadatas.Country> MapCountries(IEnumerable<ResourceMetadata.Country> source);
  IEnumerable<ContractMetadatas.AreaOfInstruction> MapAreaOfInstructions(IEnumerable<ResourceMetadata.AreaOfInstruction> source);
  IEnumerable<ContractMetadatas.PostSecondaryInstitution> MapPostSecondaryInstitutions(IEnumerable<ResourceMetadata.PostSecondaryInstitution> source);
  IEnumerable<ContractMetadatas.SystemMessage> MapSystemMessages(IEnumerable<ResourceMetadata.SystemMessage> source);
  IEnumerable<ContractMetadatas.DefaultContent> MapDefaultContents(IEnumerable<ResourceMetadata.DefaultContent> source);
  IEnumerable<ContractMetadatas.IdentificationType> MapIdentificationTypes(IEnumerable<ResourceMetadata.IdentificationType> source);
  ContractMetadatas.DynamicsConfig MapDynamicsConfig(IEnumerable<ResourceMetadata.DynamicsConfig> source);
}

[Mapper]
internal partial class MetadataMapper : IMetadataMapper
{
  public IEnumerable<ContractMetadatas.Province> MapProvinces(IEnumerable<ResourceMetadata.Province> source) => source.Select(MapProvince).ToList();

  public IEnumerable<ContractMetadatas.Country> MapCountries(IEnumerable<ResourceMetadata.Country> source) => source.Select(MapCountry).ToList();

  public IEnumerable<ContractMetadatas.AreaOfInstruction> MapAreaOfInstructions(IEnumerable<ResourceMetadata.AreaOfInstruction> source) => source.Select(MapAreaOfInstruction).ToList();

  public IEnumerable<ContractMetadatas.PostSecondaryInstitution> MapPostSecondaryInstitutions(IEnumerable<ResourceMetadata.PostSecondaryInstitution> source) => source.Select(MapPostSecondaryInstitution).ToList();

  public IEnumerable<ContractMetadatas.SystemMessage> MapSystemMessages(IEnumerable<ResourceMetadata.SystemMessage> source) => source.Select(MapSystemMessage).ToList();

  public IEnumerable<ContractMetadatas.DefaultContent> MapDefaultContents(IEnumerable<ResourceMetadata.DefaultContent> source) => source.Select(MapDefaultContent).ToList();

  public IEnumerable<ContractMetadatas.IdentificationType> MapIdentificationTypes(IEnumerable<ResourceMetadata.IdentificationType> source) => source.Select(MapIdentificationType).ToList();

  public ContractMetadatas.DynamicsConfig MapDynamicsConfig(IEnumerable<ResourceMetadata.DynamicsConfig> source) => new(FeatureEnabled(source, "ICRA Feature"));

  private ContractMetadatas.SystemMessage MapSystemMessage(ResourceMetadata.SystemMessage source) => new(source.Name, source.Subject, source.Message)
  {
    StartDate = source.StartDate,
    EndDate = source.EndDate,
    PortalTags = source.PortalTags.Select(MapPortalTag).ToArray(),
  };

  private static bool FeatureEnabled(IEnumerable<ResourceMetadata.DynamicsConfig> source, string key)
  {
    var feature = source.FirstOrDefault(item => item.Key == key);
    return feature != null && feature.Value?.ToUpper() == "ON";
  }

  public partial ResourceMetadata.PostSecondaryInstitutionsQuery MapPostSecondaryInstitutionsQuery(ContractMetadatas.PostSecondaryInstitutionsQuery source);

  private partial ContractMetadatas.Province MapProvince(ResourceMetadata.Province source);

  private partial ContractMetadatas.Country MapCountry(ResourceMetadata.Country source);

  private partial ContractMetadatas.AreaOfInstruction MapAreaOfInstruction(ResourceMetadata.AreaOfInstruction source);

  private partial ContractMetadatas.PostSecondaryInstitution MapPostSecondaryInstitution(ResourceMetadata.PostSecondaryInstitution source);

  private partial ContractMetadatas.DefaultContent MapDefaultContent(ResourceMetadata.DefaultContent source);

  private partial ContractMetadatas.IdentificationType MapIdentificationType(ResourceMetadata.IdentificationType source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractMetadatas.PortalTags MapPortalTag(ResourceMetadata.PortalTags source);
}
