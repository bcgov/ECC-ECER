using AutoMapper;
using ECER.Utilities.DataverseSdk.Model;
using System.ServiceModel.Channels;

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

  public async Task<CommunicationsStatus> NotificationStatus(string userId)
  {
    await Task.CompletedTask;
    var communications = from a in context.ecer_CommunicationSet
                         join b in context.ecer_ApplicationSet on a.ecer_Applicationid.Id equals b.ecer_ApplicationId
                         join c in context.ContactSet on b.ecer_Applicantid.Id equals c.ContactId
                         select new { a, b, c };
    communications = communications.Where(r => r.b.ecer_Applicantid.Id == Guid.Parse(userId));

    var unreadCount= communications.Where(communication => communication.a.ecer_Acknowledged == false).ToList().Count; // it does not support Any

    var hasUnread = unreadCount > 0;
    var statuses= new List<ecer_Communication_StatusCode> { ecer_Communication_StatusCode.NotifiedRecipient, ecer_Communication_StatusCode.Acknowledged };
    var count = communications.WhereIn(communication => communication.a.StatusCode!.Value, statuses).ToList().Count;
    return mapper.Map<CommunicationsStatus>(new CommunicationsStatus() { HasUnread=hasUnread, Count=count});
  }

  public async Task<IEnumerable<Communication>> Query(CommunicationQuery query)
  {
    await Task.CompletedTask;
    var communications = from a in context.ecer_CommunicationSet
                         join b in context.ecer_ApplicationSet on a.ecer_Applicationid.Id equals b.ecer_ApplicationId
                         join c in context.ContactSet on b.ecer_Applicantid.Id equals c.ContactId
                         select new { a, b, c };

    if (query.ByStatus != null)
    {
      var statuses = mapper.Map<IEnumerable<ecer_Communication_StatusCode>>(query.ByStatus).ToList();
      communications = communications.WhereIn(communication => communication.a.StatusCode!.Value, statuses);
    }

    if (query.ById != null) communications = communications.Where(r => r.a.ecer_CommunicationId == Guid.Parse(query.ById));
    if (query.ByApplicantId != null) communications = communications.Where(r => r.b.ecer_Applicantid.Id == Guid.Parse(query.ByApplicantId));

    return mapper.Map<IEnumerable<Communication>>(communications.Select(r => r.a).ToList());
  }
}
