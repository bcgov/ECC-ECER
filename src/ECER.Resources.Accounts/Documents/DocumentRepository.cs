using AutoMapper;
using ECER.Resources.Accounts.Documents;
using ECER.Utilities.DataverseSdk.Model;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System.Linq;

namespace ECER.Resources.Accounts.Communications;

internal class DocumentRepository : IDocumentRepository
{
  private readonly EcerContext context;
  private readonly IMapper mapper;

  public DocumentRepository(EcerContext context, IMapper mapper)
  {
    this.context = context;
    this.mapper = mapper;
  }

  public async Task<IEnumerable<Document>> Query(UserDocumentQuery query)
  {
    await Task.CompletedTask;
    var documents = from d in context.bcgov_DocumentUrlSet
                    join c in context.ContactSet on d.bcgov_contact_bcgov_documenturl.ContactId equals c.ContactId
                    select new { d, c };

    if (query.ById != null) documents = documents.Where(r => r.d.bcgov_DocumentUrlId == Guid.Parse(query.ById));
    if (query.ByRegistrantId != null) documents = documents.Where(r => r.c.ContactId == Guid.Parse(query.ByRegistrantId));

    if (query.ByCommunicationId != null) documents = documents.Where(r => r.d.ecer_CommunicationId.Id == Guid.Parse(query.ByCommunicationId));

    return mapper.Map<IEnumerable<Document>>(documents.Select(item => item.d))!.ToList();
  }
}
