﻿using AutoMapper;
using ECER.Utilities.DataverseSdk.Model;
using Microsoft.Xrm.Sdk.Client;

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

    //Sort by notifiedOn in descending order
    var sortedCommunications = communications.OrderByDescending(r => r.c.ecer_DateNotified);
    
    var data = sortedCommunications.Select(r => r.c).ToList();
    var result = mapper.Map<IEnumerable<Communication>>(data);
    return result!;
  }
  
  public async Task<string> Seen(string communicationId, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;

    var communication =
      context.ecer_CommunicationSet.Single(c => c.ecer_CommunicationId == Guid.Parse(communicationId));
    
    if (communication == null) throw new InvalidOperationException($"Application '{communicationId}' not found");
      
    communication.ecer_DateAcknowledged = DateTime.Now;
    communication.ecer_Acknowledged = true;
    communication.StatusCode = ecer_Communication_StatusCode.Acknowledged;
    context.UpdateObject(communication);
    
    context.SaveChanges();
    return communication.Id.ToString();
  }
}
