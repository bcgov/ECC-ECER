using AutoMapper;
using ECER.Clients.RegistryPortal.Server.Applications;
using ECER.Managers.Registry;
using ECER.Utilities.Hosting;
using ECER.Utilities.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Wolverine;

namespace ECER.Clients.RegistryPortal.Server.Communications;

public class CommunicationsEndpoints : IRegisterEndpoints
{

  public void Register(IEndpointRouteBuilder endpointRouteBuilder)
  {
    endpointRouteBuilder.MapGet("/api/messages", (HttpContext httpContext, IMessageBus messageBus) =>
    {
      var userContext = httpContext.User.GetUserContext();
      //var query = new MessagesQuery();
      //query.ByUserId = userContext!.UserId!;
      //var results = await messageBus.InvokeAsync<MessagesQueryResponse>(query);

      var results = new List<Communication>()
      {
        new Communication
{
    Id = "001",
    Subject = "Welcome to the Team!",
    Text = "Hello! We are thrilled to have you on board. Your expertise and skills are a fantastic addition to our team. Looking forward to great accomplishments together. Best, The Team"
}
      , new Communication
{
    Id = "002",
    Subject = "Project Update Meeting",
    Text = "Dear Team, Please be reminded of our project update meeting scheduled for tomorrow at 10:00 AM. We'll discuss the current progress and next steps. Kindly ensure you have reviewed the latest project documents before the meeting. Regards, Project Manager"
}
      };

      return TypedResults.Ok(new CommunicationsQueryResponse(results.Select(i => new Communication
      {
        Id = i.Id!,
        Subject = i.Subject,
        Text = i.Text
      })));
    })
     .WithOpenApi("Handles messages queries", string.Empty, "message_get")
     .RequireAuthorization();


    endpointRouteBuilder.MapGet("/api/messages/status", (HttpContext httpContext, IMessageBus messageBus) =>
    {
      var userContext = httpContext.User.GetUserContext();
      //var query = new MessagesQuery();
      //query.ByUserId = userContext!.UserId!;
      //var result = await messageBus.InvokeAsync<MessagesStatusResponse>(query);

      var result = new CommunicationsStatusResponse(new CommunicationsStatus() { Count = 2, HasUnread = true });
      return TypedResults.Ok(new CommunicationsStatusResponse(result.Status));
    })
       .WithOpenApi("Handles messages status", string.Empty, "message_status_get")
       .RequireAuthorization();
  }
}

public record CommunicationsQueryResponse(IEnumerable<Communication> Items);
public record CommunicationsStatusResponse(CommunicationsStatus Status);


public record Communication
{
  public string Id { get; set; } = null!;
  public string Subject { get; set; } = null!;
  public string Text { get; set; } = null!;
}

public record CommunicationsStatus
{
  public int Count { get; set; }
  public bool HasUnread { get; set; }
}
