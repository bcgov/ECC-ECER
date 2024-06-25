using MediatR;

namespace ECER.Managers.Registry.Contract.Communications;

/// <summary>
/// Invokes communication seen use case
/// </summary>
public record MarkCommunicationAsSeenCommand(string communicationId, string userId) : IRequest<string>;

public record UserCommunicationsStatusQuery : IRequest<CommunicationsStatusResults>
{
  public string ByRegistrantId { get; set; } = null!;
}

public record UserCommunicationQuery : IRequest<CommunicationsQueryResults>
{
  public string? ById { get; set; }
  public string? ByRegistrantId { get; set; }
  public string? ByParentId { get; set; }
  public IEnumerable<CommunicationStatus>? ByStatus { get; set; }
  public int PageNumber { get; set; }
  public int PageSize { get; set; }
}
public record CommunicationsQueryResults(IEnumerable<Communication> Items);

public record SendMessageCommand(Communication communication, string userId) : IRequest<SendMessageResult>;

public class SendMessageResult
{
  public string? CommunicationId { get; set; }
  public bool IsSuccess { get; set; }
  public string? ErrorMessage { get; set; }
}

public record Communication
{
  public string Id { get; set; } = null!;
  public string Subject { get; set; } = null!;
  public string Text { get; set; } = null!;
  public DateTime NotifiedOn { get; set; }
  public bool Acknowledged { get; set; }
  public CommunicationStatus Status { get; set; }
}

public enum CommunicationStatus
{
  Draft,
  NotifiedRecipient,
  Acknowledged,
  Inactive
}

public record CommunicationsStatusResults(CommunicationsStatus Status);
public record CommunicationsStatus
{
  public int Count { get; set; }
  public bool HasUnread { get; set; }
}
