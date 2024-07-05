﻿using AutoMapper;
using ECER.Infrastructure.Common;
using ECER.Managers.Registry.Contract.Communications;
using ECER.Resources.Accounts.Communications;
using MediatR;

namespace ECER.Managers.Registry;

public class CommunicationHandlers(ICommunicationRepository communicationRepository, IMapper mapper)
  : IRequestHandler<UserCommunicationsStatusQuery, CommunicationsStatusResults>,
    IRequestHandler<Contract.Communications.UserCommunicationQuery, CommunicationsQueryResults>,
    IRequestHandler<MarkCommunicationAsSeenCommand, string>,
    IRequestHandler<SendMessageCommand, SendMessageResult>
{
  public async Task<CommunicationsStatusResults> Handle(UserCommunicationsStatusQuery request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var statuses = new List<Resources.Accounts.Communications.CommunicationStatus>();
    statuses.Add(Resources.Accounts.Communications.CommunicationStatus.NotifiedRecipient);
    var communications = await communicationRepository.Query(new Resources.Accounts.Communications.UserCommunicationQuery
    {
      ByRegistrantId = request.ByRegistrantId,
      ByStatus = statuses,
    });

    var unreadCount = communications.Communications!.Where(c => !c.Acknowledged).ToList().Count; // it does not support Any
    var hasUnread = unreadCount > 0;

    var communicationsStatus = new Contract.Communications.CommunicationsStatus() { HasUnread = hasUnread, Count = unreadCount };
    return new CommunicationsStatusResults(communicationsStatus!);
  }

  public async Task<CommunicationsQueryResults> Handle(Contract.Communications.UserCommunicationQuery request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(communicationRepository);
    ArgumentNullException.ThrowIfNull(mapper);
    ArgumentNullException.ThrowIfNull(request);

    var communication = await communicationRepository.Query(new Resources.Accounts.Communications.UserCommunicationQuery
    {
      ById = request.ById,
      ByRegistrantId = request.ByRegistrantId,
      ByParentId = request.ByParentId,
      ByStatus = request.ByStatus?.Convert<Contract.Communications.CommunicationStatus, Resources.Accounts.Communications.CommunicationStatus>(),
      PageNumber = request.PageNumber,
      PageSize = request.PageSize,
    });

    return new CommunicationsQueryResults(mapper.Map<IEnumerable<Contract.Communications.Communication>>(communication.Communications)!)
    { TotalMessagesCount = communication.TotalMessagesCount };
  }

  /// <summary>
  /// Handles marking a communication as seen use case
  /// </summary>
  /// <param name="request">The command</param>
  /// <param name="cancellationToken">cancellation token</param>
  /// <returns></returns>
  public async Task<string> Handle(MarkCommunicationAsSeenCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var statuses = new List<Resources.Accounts.Communications.CommunicationStatus>
    {
      Resources.Accounts.Communications.CommunicationStatus.NotifiedRecipient
    };

    var communications = await communicationRepository.Query(new Resources.Accounts.Communications.UserCommunicationQuery
    {
      ById = request.communicationId,
      ByRegistrantId = request.userId,
      ByStatus = statuses
    });

    if (!communications.Communications!.Any())
    {
      throw new InvalidOperationException($"Communication '{request.communicationId}' not found");
    }

    var seenCommunicationId = await communicationRepository.MarkAsSeen(request.communicationId, cancellationToken);

    return seenCommunicationId;
  }

  public async Task<SendMessageResult> Handle(SendMessageCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    var Communication = mapper.Map<Resources.Accounts.Communications.Communication>(request.communication);
    var CommunicationId = await communicationRepository.SendMessage(Communication, request.userId, cancellationToken);
    return new SendMessageResult() { CommunicationId = CommunicationId, IsSuccess = true };
  }
}
