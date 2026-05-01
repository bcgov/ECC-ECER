using Riok.Mapperly.Abstractions;
using ContractCommunications = ECER.Managers.Registry.Contract.Communications;
using ResourceCommunications = ECER.Resources.Accounts.Communications;

namespace ECER.Managers.Registry;

public interface ICommunicationMapper
{
  IEnumerable<ContractCommunications.Communication> MapCommunications(IEnumerable<ResourceCommunications.Communication> source);
  ResourceCommunications.Communication MapCommunication(ContractCommunications.Communication source);
}

[Mapper]
internal partial class CommunicationMapper : ICommunicationMapper
{
  public IEnumerable<ContractCommunications.Communication> MapCommunications(IEnumerable<ResourceCommunications.Communication> source) => source.Select(MapCommunication).ToList();

  public ResourceCommunications.Communication MapCommunication(ContractCommunications.Communication source) => new(source.Id)
  {
    Category = MapCommunicationCategory(source.Category),
    Subject = source.Subject,
    Body = source.Text,
    From = MapInitiatedFrom(source.From),
    NotifiedOn = source.NotifiedOn,
    Acknowledged = source.Acknowledged,
    Status = MapCommunicationStatus(source.Status),
    DoNotReply = source.DoNotReply,
    LatestMessageNotifiedOn = source.LatestMessageNotifiedOn,
    IsRead = source.IsRead,
    ApplicationId = source.ApplicationId,
    IcraEligibilityId = source.IcraEligibilityId,
    ProgramRepresentativeId = source.ProgramRepresentativeId,
    EducationInstituteName = source.EducationInstituteName,
    IsPspUser = source.IsPspUser,
    Documents = MapCommunicationDocuments(source.Documents),
  };

  private ContractCommunications.Communication MapCommunication(ResourceCommunications.Communication source) => new()
  {
    Id = source.Id ?? string.Empty,
    Category = MapCommunicationCategory(source.Category),
    Subject = source.Subject,
    Text = source.Body,
    From = MapInitiatedFrom(source.From),
    NotifiedOn = source.NotifiedOn,
    Acknowledged = source.Acknowledged,
    Status = MapCommunicationStatus(source.Status),
    DoNotReply = source.DoNotReply,
    LatestMessageNotifiedOn = source.LatestMessageNotifiedOn,
    IsRead = source.IsRead,
    ApplicationId = source.ApplicationId,
    IcraEligibilityId = source.IcraEligibilityId,
    ProgramRepresentativeId = source.ProgramRepresentativeId,
    EducationInstituteName = source.EducationInstituteName,
    IsPspUser = source.IsPspUser,
    Documents = MapCommunicationDocuments(source.Documents),
  };

  private static List<ContractCommunications.CommunicationDocument> MapCommunicationDocuments(IEnumerable<ResourceCommunications.CommunicationDocument> source) =>
    source.Select(MapCommunicationDocument).ToList();

  private static List<ResourceCommunications.CommunicationDocument> MapCommunicationDocuments(IEnumerable<ContractCommunications.CommunicationDocument> source) =>
    source.Select(MapCommunicationDocument).ToList();

  private static ContractCommunications.CommunicationDocument MapCommunicationDocument(ResourceCommunications.CommunicationDocument source) => new(source.Id)
  {
    Url = source.Url ?? string.Empty,
    Extention = source.Extention ?? string.Empty,
    Name = source.Name ?? string.Empty,
    Size = source.Size ?? string.Empty,
    EcerWebApplicationType = source.EcerWebApplicationType,
  };

  private static ResourceCommunications.CommunicationDocument MapCommunicationDocument(ContractCommunications.CommunicationDocument source) => new(source.Id)
  {
    Url = source.Url,
    Extention = source.Extention,
    Name = source.Name,
    Size = source.Size,
    EcerWebApplicationType = source.EcerWebApplicationType,
  };

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractCommunications.CommunicationCategory MapCommunicationCategory(ResourceCommunications.CommunicationCategory source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourceCommunications.CommunicationCategory MapCommunicationCategory(ContractCommunications.CommunicationCategory source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractCommunications.CommunicationStatus MapCommunicationStatus(ResourceCommunications.CommunicationStatus source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourceCommunications.CommunicationStatus MapCommunicationStatus(ContractCommunications.CommunicationStatus source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractCommunications.InitiatedFrom MapInitiatedFrom(ResourceCommunications.InitiatedFrom source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourceCommunications.InitiatedFrom MapInitiatedFrom(ContractCommunications.InitiatedFrom source);

  private ContractCommunications.CommunicationCategory? MapCommunicationCategory(ResourceCommunications.CommunicationCategory? source) => source.HasValue ? MapCommunicationCategory(source.Value) : null;

  private ResourceCommunications.CommunicationCategory? MapCommunicationCategory(ContractCommunications.CommunicationCategory? source) => source.HasValue ? MapCommunicationCategory(source.Value) : null;
}
