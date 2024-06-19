using AutoMapper;
using ECER.Utilities.DataverseSdk.Model;
using Microsoft.Xrm.Sdk.Client;
using System.Drawing.Printing;

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

    // Filtering by status
    if (query.ByStatus != null)
    {
      var statuses = mapper.Map<IEnumerable<ecer_Communication_StatusCode>>(query.ByStatus)!.ToList();
      communications = communications.WhereIn(communication => communication.c.StatusCode!.Value, statuses);
    }

    // Filtering by ID
    if (query.ById != null) communications = communications.Where(r => r.c.ecer_CommunicationId == Guid.Parse(query.ById));

    // Filtering by registrant ID
    if (query.ByRegistrantId != null) communications = communications.Where(r => r.c.ecer_Registrantid.Id == Guid.Parse(query.ByRegistrantId));

    if (query.PageNumber > 0)
    {
      communications = communications.OrderByDescending(r => r.c.ecer_DateNotified).Skip((query.PageNumber - 1) * query.PageSize).Take(query.PageSize);
    }
    else
    {
      communications = communications.OrderByDescending(r => r.c.ecer_DateNotified);
    }
    return mapper.Map<IEnumerable<Communication>>(communications.Select(r => r.c).ToList());
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
}
