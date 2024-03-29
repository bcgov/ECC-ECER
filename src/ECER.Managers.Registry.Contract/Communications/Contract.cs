﻿using ECER.Managers.Registry.Contract.Applications;
using ECER.Utilities.Security;

namespace ECER.Managers.Registry.Contract.Communications;

public record UserCommunicationsStatusQuery
{
  public string ByRegistrantId { get; set; } = null!;
}

public record UserCommunicationQuery
{
  public string? ById { get; set; }
  public string? ByRegistrantId { get; set; }
  public IEnumerable<CommunicationStatus>? ByStatus { get; set; }
}
public record CommunicationsQueryResults(IEnumerable<Communication> Items);

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
