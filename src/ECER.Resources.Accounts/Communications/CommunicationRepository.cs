using AutoMapper;
using ECER.Utilities.DataverseSdk.Model;

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

  public async Task<IEnumerable<Communication>> Query(UserCommunicationQuery query)
  {
    await Task.CompletedTask;
    var communications = from c in context.ecer_CommunicationSet
                         join a in context.ecer_ApplicationSet on c.ecer_Applicationid.Id equals a.ecer_ApplicationId
                         select new { c, a };

    if (query.ByStatus != null)
    {
      var statuses = mapper.Map<IEnumerable<ecer_Communication_StatusCode>>(query.ByStatus)!.ToList();
      communications = communications.WhereIn(communication => communication.c.StatusCode!.Value, statuses);
    }

    if (query.ById != null) communications = communications.Where(r => r.c.ecer_CommunicationId == Guid.Parse(query.ById));
    if (query.ByRegistrantId != null) communications = communications.Where(r => r.c.ecer_Registrantid.Id == Guid.Parse(query.ByRegistrantId));
    var data = communications.Select(r => r.c).ToList();
    var result = mapper.Map<IEnumerable<Communication>>(data);
    return result!;
  }
}
