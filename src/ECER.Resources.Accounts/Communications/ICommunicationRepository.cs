using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ECER.Resources.Accounts.Communications;

public interface ICommunicationRepository
{
  Task<IEnumerable<Communication>> Query(CommunicationQuery query);
  Task<CommunicationsStatus> GetCommunicationsCountAndNewIndicator(string userId);
}

public record CommunicationQuery
{
  public string? ById { get; set; }
  public IEnumerable<CommunicationStatus>? ByStatus { get; set; }
  public string? ByApplicantId { get; set; }
}

public abstract record Communication(string? Id)
{
  public string Subject { get; set; } = string.Empty;
  public string Body { get; set; } = string.Empty;
  public DateTime NotifiedOn { get; set; }
  public bool Acknowledged { get; set; }
  public CommunicationStatus Status { get; set; }
}

public enum CommunicationStatus
{
  Draft,
  NotifiedRecipient,
  Acknowledged,
  InActive
}

public record CommunicationsStatus
{
  public int Count { get; set; }
  public bool HasUnread { get; set; }
}

