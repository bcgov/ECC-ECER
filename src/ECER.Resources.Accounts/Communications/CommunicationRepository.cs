using AutoMapper;
using ECER.Utilities.DataverseSdk.Model;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using ECER.Utilities.ObjectStorage.Providers.S3;
using Microsoft.Extensions.Configuration;
using ECER.Utilities.ObjectStorage.Providers;

namespace ECER.Resources.Accounts.Communications;

internal class CommunicationRepository : ICommunicationRepository
{
  private readonly EcerContext context;
  private readonly IMapper mapper;
  private readonly IObjecStorageProvider objectStorageProvider;
  private readonly IConfiguration configuration;

  public CommunicationRepository(EcerContext context, IMapper mapper, IObjecStorageProvider objecStorageProvider, IConfiguration configuration)
  {
    this.context = context;
    this.mapper = mapper;
    this.objectStorageProvider = objecStorageProvider;
    this.configuration = configuration;
  }

  public async Task<int> QueryStatus(string RegistrantId)
  {
    await Task.CompletedTask;
    var unseenCommunications = context.ecer_CommunicationSet.Where(item => item.ecer_Registrantid.Id == Guid.Parse(RegistrantId) &&
                                                                     item.ecer_InitiatedFrom == ecer_InitiatedFrom.Registry &&
                                                                     item.StatusCode == ecer_Communication_StatusCode.NotifiedRecipient &&
                                                                     item.StateCode == ecer_communication_statecode.Active &&
                                                                     item.ecer_Acknowledged != true
                                                                     ).Select(item => new { item.Id, item.ecer_IsRoot, item.ecer_ParentCommunicationid }).ToList().
                                                                     Where(item => item.ecer_IsRoot ?? false || item.ecer_ParentCommunicationid != null).ToList(); // SDK does not support including this condition inside query
    return unseenCommunications.Count;
  }

  public async Task<CommunicationResult> Query(UserCommunicationQuery query)
  {
    await Task.CompletedTask;
    var communications = context.ecer_CommunicationSet;

    // Filtering by registrant ID
    if (query.ByRegistrantId != null) communications = communications.Where(item => item.ecer_Registrantid.Id == Guid.Parse(query.ByRegistrantId));

    // Filtering by status
    if (query.ByStatus != null)
    {
      var statuses = mapper.Map<IEnumerable<ecer_Communication_StatusCode>>(query.ByStatus)!.ToList();
      communications = communications.WhereIn(item => item.StatusCode!.Value, statuses);
    }

    // Filtering by ID
    if (query.ById != null) communications = communications.Where(item => item.ecer_CommunicationId == Guid.Parse(query.ById));

    // otherwise if parent id provided returning just child communications based on parent id
    else if (query.ByParentId != null)
    {
      communications = communications.Where(item => item.ecer_ParentCommunicationid.Id == Guid.Parse(query.ByParentId));
    }
    // otherwise if its not a single query and parent id not provided returning all parent communications
    else
    {
      communications = communications.Where(item => item.ecer_IsRoot == true);
    }
    int paginatedTotalCommunicationCount = 0;
    if (query.PageNumber > 0)
    {
      paginatedTotalCommunicationCount = communications.Select(item => item.Id).ToList().Count;
      communications = communications.OrderByDescending(item => item.ecer_LatestMessageNotifiedDate).Skip((query.PageNumber - 1) * query.PageSize).Take(query.PageSize);
    }
    else
    {
      communications = communications.OrderByDescending(item => item.ecer_DateNotified);
    }

    context.LoadProperties(communications, ecer_Communication.Fields.ecer_bcgov_documenturl_CommunicationId_ecer_communication);

    var finalCommunications = communications.ToList();
    return new CommunicationResult
    {
      Communications = mapper.Map<IEnumerable<Communication>>(finalCommunications),
      TotalMessagesCount = query.PageNumber > 0 ? paginatedTotalCommunicationCount : finalCommunications.Count,
    };
  }

  public async Task<string> MarkAsSeen(string communicationId, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;

    var communication =
      context.ecer_CommunicationSet.Single(c => c.ecer_CommunicationId == Guid.Parse(communicationId));

    if (communication == null) throw new InvalidOperationException($"Communication '{communicationId}' not found");

    communication.ecer_DateAcknowledged = DateTime.UtcNow;
    communication.ecer_Acknowledged = true;
    communication.ecer_AreAllRead = true;
    communication.StatusCode = ecer_Communication_StatusCode.Acknowledged;

    if (communication.ecer_ParentCommunicationid != null)
    {
      var parent = context.ecer_CommunicationSet.Single(c => c.ecer_CommunicationId == communication.ecer_ParentCommunicationid.Id);
      parent.ecer_AreAllRead = true;
      parent.ecer_Acknowledged = true;
      parent.StatusCode = ecer_Communication_StatusCode.Acknowledged;
      context.UpdateObject(parent);
    }

    context.UpdateObject(communication);
    context.SaveChanges();
    return communication.Id.ToString();
  }

  public async Task<string> SendMessage(Communication communication, string userId, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;
    var existingCommunication = context.ecer_CommunicationSet.SingleOrDefault(d => d.ecer_CommunicationId == Guid.Parse(communication.Id!));
    if (existingCommunication == null)
    {
      throw new InvalidOperationException($"Communication '{communication.Id}' not found");
    }
    else
    {
      var registrant = context.ContactSet.SingleOrDefault(r => r.ContactId == Guid.Parse(userId));
      if (registrant == null)
      {
        throw new InvalidOperationException($"Registrant '{userId}' not found");
      }

      existingCommunication.ecer_LatestMessageNotifiedDate = DateTime.UtcNow;
      context.UpdateObject(existingCommunication);

      var ecerCommunication = mapper.Map<ecer_Communication>(communication);

      ecerCommunication.ecer_CommunicationId = Guid.NewGuid();
      ecerCommunication.ecer_InitiatedFrom = ecer_InitiatedFrom.PortalUser;
      ecerCommunication.StatusCode = ecer_Communication_StatusCode.Acknowledged;
      ecerCommunication.ecer_NotifyRecipient = true;
      ecerCommunication.ecer_DateNotified = DateTime.UtcNow;
      ecerCommunication.ecer_LatestMessageNotifiedDate = DateTime.UtcNow;

      context.AddObject(ecerCommunication);
      context.AddLink(registrant, ecer_Communication.Fields.ecer_contact_ecer_communication_122, ecerCommunication);
      var Referencingecer_communication_ParentCommunicationid = new Relationship(ecer_Communication.Fields.Referencingecer_communication_ParentCommunicationid)
      {
        PrimaryEntityRole = EntityRole.Referencing
      };
      context.AddLink(ecerCommunication, Referencingecer_communication_ParentCommunicationid, existingCommunication);

      foreach (var document in communication.Documents)
      {
        if (string.IsNullOrEmpty(document.Id))
        {
          throw new InvalidOperationException($"Document '{document.Id}' is not valid");
        }

        var sourceFolder = "tempfolder";
        var destinationFolder = "ecer_communication/" + ecerCommunication.ecer_CommunicationId;
        var fileId = document.Id;
        await objectStorageProvider.MoveAsync(new S3Descriptor(GetBucketName(configuration), fileId, sourceFolder), new S3Descriptor(GetBucketName(configuration), fileId, destinationFolder), cancellationToken);

        var documenturl = new bcgov_DocumentUrl()
        {
          bcgov_DocumentUrlId = Guid.Parse(fileId),
          bcgov_Url = destinationFolder,
          bcgov_FileName = document.Name,
          bcgov_FileSize = document.Size,
          bcgov_FileExtension = document.Extention,
          StatusCode = bcgov_DocumentUrl_StatusCode.Active,
          StateCode = bcgov_documenturl_statecode.Active
        };

        context.AddObject(documenturl);
        context.AddLink(documenturl, bcgov_DocumentUrl.Fields.ecer_bcgov_documenturl_CommunicationId_ecer_communication, ecerCommunication);
      }

      context.SaveChanges();
      return ecerCommunication.ecer_CommunicationId.ToString()!;
    }
  }

  private static string GetBucketName(IConfiguration configuration) =>
  configuration.GetValue<string>("objectStorage:bucketName") ?? throw new InvalidOperationException("objectStorage:bucketName is not set");
}
