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

  public async Task<CommunicationsStatus> GetCommunicationsCountAndNewIndicator(string userId)
  {
    await Task.CompletedTask;
    var communications = from a in context.ecer_CommunicationSet
                         join b in context.ecer_ApplicationSet on a.ecer_Applicationid.Id equals b.ecer_ApplicationId
                         select new { a, b };
    communications = communications.Where(r => r.b.ecer_Applicantid.Id == Guid.Parse(userId));

    var unreadCount= communications.Where(communication => communication.a.ecer_Acknowledged == false).ToList().Count; // it does not support Any

    var hasUnread = unreadCount > 0;

    return mapper.Map<CommunicationsStatus>(new CommunicationsStatus() { HasUnread=hasUnread, Count= unreadCount });
  }

  public async Task<IEnumerable<Communication>> Query(CommunicationQuery query)
  {
    await Task.CompletedTask;
    var communications = from a in context.ecer_CommunicationSet
                         join b in context.ecer_ApplicationSet on a.ecer_Applicationid.Id equals b.ecer_ApplicationId
                         select new { a, b };

    if (query.ByStatus != null)
    {
      var statuses = mapper.Map<IEnumerable<ecer_Communication_StatusCode>>(query.ByStatus).ToList();
      communications = communications.WhereIn(communication => communication.a.StatusCode!.Value, statuses);
    }

    if (query.ById != null) communications = communications.Where(r => r.a.ecer_CommunicationId == Guid.Parse(query.ById));
    if (query.ByApplicantId != null) communications = communications.Where(r => r.b.ecer_Applicantid.Id == Guid.Parse(query.ByApplicantId));
    var data = communications.Select(r => r.a).ToList();
    var result = mapper.Map<IEnumerable<Communication>>(data);
    return result;
  }
}
