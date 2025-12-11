using AutoMapper;
using ECER.Infrastructure.Common;
using ECER.Managers.Registry.Contract.Communications;
using ECER.Resources.Accounts.Communications;
using MediatR;

namespace ECER.Managers.Registry;

public class CommunicationHandlers(ICommunicationRepository communicationRepository, IMapper mapper)
  : IRequestHandler<Contract.Communications.UserCommunicationsStatusQuery, CommunicationsStatusResults>,
    IRequestHandler<Contract.Communications.UserCommunicationQuery, CommunicationsQueryResults>,
    IRequestHandler<MarkCommunicationAsSeenCommand, string>,
    IRequestHandler<SendMessageCommand, SendMessageResult>
{
  public async Task<CommunicationsStatusResults> Handle(Contract.Communications.UserCommunicationsStatusQuery request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    var unreadMessagesCount = await communicationRepository.QueryStatus(new Resources.Accounts.Communications.UserCommunicationsStatusQuery
    {
      ByRegistrantId = request.ByRegistrantId,
      ByPostSecondaryInstituteId = request.ByPostSecondaryInstituteId,
    });
    var communicationsStatus = new Contract.Communications.CommunicationsStatus() { HasUnread = unreadMessagesCount > 0, Count = unreadMessagesCount };
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
      ByPostSecondaryInstituteId =  request.ByPostSecondaryInstituteId,
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
      Resources.Accounts.Communications.CommunicationStatus.NotifiedRecipient,
      Resources.Accounts.Communications.CommunicationStatus.Acknowledged
    };

    var communications = await communicationRepository.Query(
      request.IsPspUser == true ? 
        new Resources.Accounts.Communications.UserCommunicationQuery
        {
          ById = request.CommunicationId,
          ByPostSecondaryInstituteId = request.PostSecondaryInstituteId,
          ByStatus = statuses
        }
      :
        new Resources.Accounts.Communications.UserCommunicationQuery
        {
          ById = request.CommunicationId,
          ByRegistrantId = request.UserId,
          ByStatus = statuses
        });

    if (!communications.Communications!.Any())
    {
      throw new InvalidOperationException($"Communication '{request.CommunicationId}' not found");
    }

    var seenCommunicationId = await communicationRepository.MarkAsSeen(request.CommunicationId, cancellationToken);

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
