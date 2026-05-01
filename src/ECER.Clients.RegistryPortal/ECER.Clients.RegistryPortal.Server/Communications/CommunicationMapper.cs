using ECER.Utilities.ObjectStorage.Providers;
using Riok.Mapperly.Abstractions;
using ContractCommunications = ECER.Managers.Registry.Contract.Communications;

namespace ECER.Clients.RegistryPortal.Server.Communications;

internal interface ICommunicationMapper
{
  IEnumerable<Communication> MapCommunications(IEnumerable<ContractCommunications.Communication> source);
  ContractCommunications.Communication MapCommunication(Communication source);
}

[Mapper]
internal partial class CommunicationMapper : ICommunicationMapper
{
  public IEnumerable<Communication> MapCommunications(IEnumerable<ContractCommunications.Communication> source) => source.Select(MapCommunication).ToList();

  public ContractCommunications.Communication MapCommunication(Communication source) => new()
  {
    Id = source.Id,
    Subject = source.Subject,
    Text = source.Text,
    From = MapInitiatedFrom(source.From),
    Acknowledged = source.Acknowledged,
    NotifiedOn = source.NotifiedOn,
    Status = MapCommunicationStatus(source.Status),
    DoNotReply = source.DoNotReply,
    LatestMessageNotifiedOn = source.LatestMessageNotifiedOn,
    IsRead = source.IsRead,
    ApplicationId = source.ApplicationId,
    IcraEligibilityId = source.IcraEligibilityId,
    Documents = source.Documents.Select(MapCommunicationDocument).ToList(),
  };

  private Communication MapCommunication(ContractCommunications.Communication source) => new()
  {
    Id = source.Id ?? string.Empty,
    Subject = source.Subject,
    Text = source.Text,
    From = MapInitiatedFrom(source.From),
    Acknowledged = source.Acknowledged,
    NotifiedOn = source.NotifiedOn,
    Status = MapCommunicationStatus(source.Status),
    DoNotReply = source.DoNotReply,
    LatestMessageNotifiedOn = source.LatestMessageNotifiedOn,
    IsRead = source.IsRead,
    ApplicationId = source.ApplicationId,
    IcraEligibilityId = source.IcraEligibilityId,
    Documents = source.Documents.Select(MapCommunicationDocument).ToList(),
  };

  private static CommunicationDocument MapCommunicationDocument(ContractCommunications.CommunicationDocument source) => new(source.Id)
  {
    Url = source.Url,
    Extention = source.Extention,
    Name = source.Name,
    Size = source.Size,
    EcerWebapplicationType = source.EcerWebApplicationType,
  };

  private static ContractCommunications.CommunicationDocument MapCommunicationDocument(CommunicationDocument source) => new(source.Id)
  {
    Url = source.Url,
    Extention = source.Extention,
    Name = source.Name,
    Size = source.Size,
    EcerWebApplicationType = EcerWebApplicationType.Registry,
  };

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial InitiatedFrom MapInitiatedFrom(ContractCommunications.InitiatedFrom source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractCommunications.InitiatedFrom MapInitiatedFrom(InitiatedFrom source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial CommunicationStatus MapCommunicationStatus(ContractCommunications.CommunicationStatus source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractCommunications.CommunicationStatus MapCommunicationStatus(CommunicationStatus source);
}
