using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ECER.Resources.Accounts.Documents;

public interface IDocumentRepository
{
  Task<IEnumerable<Document>> Query(UserDocumentQuery query);
}

public record UserDocumentQuery
{
  public string? ById { get; set; }
  public string? ByRegistrantId { get; set; }
  public string? ByCommunicationId { get; set; }
}

public record Document(string? Id)
{
  public string Name { get; set; } = string.Empty;
  public string Size { get; set; } = string.Empty;
}
