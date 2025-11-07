namespace ECER.Resources.Accounts.Communications;

public interface ICommunicationRepository
{
  Task<int> QueryStatus(string RegistrantId);

  Task<CommunicationResult> Query(UserCommunicationQuery query);

  Task<string> MarkAsSeen(string communicationId, CancellationToken cancellationToken);

  Task<string> SendMessage(Communication communication, string userId, CancellationToken cancellationToken);
}

public record UserCommunicationQuery
{
  public string? ById { get; set; }
  public IEnumerable<CommunicationStatus>? ByStatus { get; set; }
  public string? ByRegistrantId { get; set; }
  public string? ByParentId { get; set; }
  public int PageNumber { get; set; }
  public int PageSize { get; set; }
}

public record Communication(string? Id)
{
  public string Subject { get; set; } = string.Empty;
  public string Body { get; set; } = string.Empty;
  public InitiatedFrom From { get; set; }
  public DateTime NotifiedOn { get; set; }
  public bool Acknowledged { get; set; }
  public CommunicationStatus Status { get; set; }
  public bool DoNotReply { get; set; }
  public DateTime? LatestMessageNotifiedOn { get; set; }
  public string? ApplicationId { get; set; }
  public bool? IsRead { get; set; }
  public IEnumerable<CommunicationDocument> Documents { get; set; } = Array.Empty<CommunicationDocument>();
}

public record CommunicationDocument(string Id)
{
  public string? Url { get; set; } = string.Empty;
  public string? Extention { get; set; } = string.Empty;
  public string? Name { get; set; } = string.Empty;
  public string? Size { get; set; } = string.Empty;
}

public record CommunicationResult
{
  public IEnumerable<Communication>? Communications { get; set; }
  public int TotalMessagesCount { get; set; }
}

public enum CommunicationStatus
{
  Draft,
  NotifiedRecipient,
  Acknowledged,
  Inactive
}

public enum InitiatedFrom
{
  Investigation,
  PortalUser,
  Registry,
  ProgramRepresentative
}

public record CommunicationsStatus
{
  public int Count { get; set; }
  public bool HasUnread { get; set; }
}
