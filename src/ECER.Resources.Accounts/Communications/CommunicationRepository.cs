using AutoMapper;
using ECER.Utilities.DataverseSdk.Model;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System.Linq;

namespace ECER.Resources.Accounts.Communications;

internal class CommunicationRepository : ICommunicationRepository
{
  private readonly EcerContext context;
  private readonly IMapper mapper;

  public CommunicationRepository(EcerContext context, IMapper mapper)
  {
    this.context = context;
    this.mapper = mapper;
  }

  public async Task<CommunicationResult> Query(UserCommunicationQuery query)
  {
    await Task.CompletedTask;
    var communications = from c in context.ecer_CommunicationSet
                         join a in context.ecer_ApplicationSet on c.ecer_Applicationid.Id equals a.ecer_ApplicationId
                         select new { c, a };
    var UnreadMessagesCount = 0;
    IEnumerable<Communication> PortalComms = new List<Communication>();
    // Filtering by registrant ID
    if (query.ByRegistrantId != null) communications = communications.Where(r => r.c.ecer_Registrantid.Id == Guid.Parse(query.ByRegistrantId));

    // Filtering by status
    if (query.ByStatus != null)
    {
      var statuses = mapper.Map<IEnumerable<ecer_Communication_StatusCode>>(query.ByStatus)!.ToList();
      communications = communications.WhereIn(communication => communication.c.StatusCode!.Value, statuses);
    }

    // Filtering by ID
    if (query.ById != null) communications = communications.Where(r => r.c.ecer_CommunicationId == Guid.Parse(query.ById));
    else
    {
      // returning just child communications based on parent id
      if (query.ByParentId != null)
      {
        communications = communications.Where(r => r.c.ecer_ParentCommunicationid.Id == Guid.Parse(query.ByParentId));
        UnreadMessagesCount = communications.Where(comms => comms.c.ecer_InitiatedFrom == ecer_InitiatedFrom.Registry && comms.c.ecer_Acknowledged != true).ToList().Count;
      }
      // otherwise returning parent communications
      else
      {
        var communicationsList = communications.ToList();
        UnreadMessagesCount = communications.Where(comms => comms.c.ecer_InitiatedFrom == ecer_InitiatedFrom.Registry && comms.c.ecer_Acknowledged != true).ToList().Count;
        var latestMessageDateByParentId = communicationsList.GroupBy(ct => ct.c.ecer_ParentCommunicationid.Id) // Group by parent ID
                                                     .Select(group => new
                                                     {
                                                       ParentId = group.Key,
                                                       LatestMessageNotifiedDate = group.Max(ct => ct.c.ecer_DateNotified),
                                                       AreAllRead = !group.Any(r => r.c.ecer_InitiatedFrom == ecer_InitiatedFrom.Registry && r.c.ecer_Acknowledged != true),
                                                     });

        communications = communications.Where(c => c.c.ecer_IsRoot == true);

        PortalComms = mapper.Map<IEnumerable<Communication>>(communications.Select(comms => comms.c).ToList());

        foreach (var comm in PortalComms)
        {
          var matchingComm = latestMessageDateByParentId.SingleOrDefault(ld => ld.ParentId == Guid.Parse(comm.Id!));
          if (matchingComm != null)
          {
            comm.LatestMessageNotifiedOn = matchingComm.LatestMessageNotifiedDate;
            comm.IsRead = matchingComm.AreAllRead;
          }
        }
      }
    }
    if (PortalComms.ToList().Count == 0)
    {
      PortalComms = mapper.Map<IEnumerable<Communication>>(communications.Select(comms => comms.c).ToList());
    }
    if (query.PageNumber > 0)
    {
      PortalComms = PortalComms.OrderByDescending(c => c.LatestMessageNotifiedOn).Skip((query.PageNumber - 1) * query.PageSize).Take(query.PageSize);
    }
    else
    {
      PortalComms = PortalComms.OrderByDescending(c => c.LatestMessageNotifiedOn);
    }

    var result = new CommunicationResult
    {
      Communications = PortalComms,
      TotalMessagesCount = communications.ToList().Count,
      UnreadMessagesCount = UnreadMessagesCount
    };

    return result;
  }

  public async Task<string> MarkAsSeen(string communicationId, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;

    var communication =
      context.ecer_CommunicationSet.Single(c => c.ecer_CommunicationId == Guid.Parse(communicationId));

    if (communication == null) throw new InvalidOperationException($"Communication '{communicationId}' not found");

    communication.ecer_DateAcknowledged = DateTime.Now;
    communication.ecer_Acknowledged = true;
    communication.StatusCode = ecer_Communication_StatusCode.Acknowledged;
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
      var application = context.ecer_ApplicationSet.SingleOrDefault(a => a.ecer_ApplicationId == existingCommunication.ecer_Applicationid.Id && a.ecer_Applicantid.Id == Guid.Parse(userId));

      if (application == null)
      {
        throw new InvalidOperationException($"Application '{existingCommunication.ecer_Applicationid.Id}' not found");
      }
      var registrant = context.ContactSet.SingleOrDefault(r => r.ContactId == Guid.Parse(userId));
      if (registrant == null)
      {
        throw new InvalidOperationException($"Registrant '{userId}' not found");
      }
      var ecerCommunication = mapper.Map<ecer_Communication>(communication);

      ecerCommunication.ecer_CommunicationId = Guid.NewGuid();
      ecerCommunication.ecer_InitiatedFrom = ecer_InitiatedFrom.PortalUser;
      ecerCommunication.ecer_NotifyRecipient = true;
      context.AddObject(ecerCommunication);
      context.AddLink(registrant, ecer_Communication.Fields.ecer_contact_ecer_communication_122, ecerCommunication);
      var Referencingecer_communication_ParentCommunicationid = new Relationship(ecer_Communication.Fields.Referencingecer_communication_ParentCommunicationid)
      {
        PrimaryEntityRole = EntityRole.Referencing
      };
      context.AddLink(ecerCommunication, Referencingecer_communication_ParentCommunicationid, existingCommunication);
      context.AddLink(application, ecer_Communication.Fields.ecer_communication_Applicationid, ecerCommunication);
      context.SaveChanges();

      return ecerCommunication.ecer_CommunicationId.ToString()!;
    }
  }
}
