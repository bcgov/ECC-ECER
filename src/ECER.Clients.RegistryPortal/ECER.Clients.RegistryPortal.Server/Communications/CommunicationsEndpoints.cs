using ECER.Managers.Registry.Contract.Communications;
using ECER.Utilities.Hosting;
using ECER.Utilities.Security;
using MediatR;
using AutoMapper;

namespace ECER.Clients.RegistryPortal.Server.Communications;

public class CommunicationsEndpoints : IRegisterEndpoints
{
  public void Register(IEndpointRouteBuilder endpointRouteBuilder)
  {
    endpointRouteBuilder.MapGet("/api/messages", async (HttpContext httpContext, IMediator messageBus, IMapper mapper) =>
    {
      var userContext = httpContext.User.GetUserContext();
      var query = new UserCommunicationQuery
      {
        ByRegistrantId = userContext!.UserId,
        ByStatus = [ECER.Managers.Registry.Contract.Communications.CommunicationStatus.NotifiedRecipient, ECER.Managers.Registry.Contract.Communications.CommunicationStatus.Acknowledged]
      };
      var results = await messageBus.Send<CommunicationsQueryResults>(query);

      return TypedResults.Ok(mapper.Map<IEnumerable<Communication>>(results.Items));
    })
     .WithOpenApi("Handles messages queries", string.Empty, "message_get")
     .RequireAuthorization();

    endpointRouteBuilder.MapGet("/api/messages/status", async (HttpContext httpContext, IMediator messageBus) =>
    {
      var userContext = httpContext.User.GetUserContext();
      var query = new UserCommunicationsStatusQuery();
      query.ByRegistrantId = userContext!.UserId;
      var result = await messageBus.Send(query);
      return TypedResults.Ok(new CommunicationsStatusResults(result.Status));
    })
     .WithOpenApi("Handles messages status", string.Empty, "message_status_get")
     .RequireAuthorization();
  }
}

public record Communication
{
  public string Id { get; set; } = null!;
  public string Subject { get; set; } = null!;
  public string Text { get; set; } = null!;
  public bool Acknowledged { get; set; }
  public DateTime NotifiedOn { get; set; }
  public CommunicationStatus Status { get; set; }
}

public enum CommunicationStatus
{
  Draft,
  NotifiedRecipient,
  Acknowledged,
  Inactive
}

public record CommunicationsStatus
{
  public int Count { get; set; }
  public bool HasUnread { get; set; }
}
