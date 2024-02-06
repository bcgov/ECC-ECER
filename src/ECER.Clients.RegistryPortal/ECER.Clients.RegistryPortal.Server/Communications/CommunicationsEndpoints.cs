using ECER.Managers.Registry.Contract.Communications;
using ECER.Utilities.Hosting;
using ECER.Utilities.Security;
using Wolverine;

namespace ECER.Clients.RegistryPortal.Server.Communications;

public class CommunicationsEndpoints : IRegisterEndpoints
{
  public void Register(IEndpointRouteBuilder endpointRouteBuilder)
  {
    endpointRouteBuilder.MapGet("/api/messages", async (HttpContext httpContext, IMessageBus messageBus) =>
    {
      var userContext = httpContext.User.GetUserContext();
      var query = new CommunicationsQuery();
      query.ByIdentity = userContext!.Identity!;
      query.ByStatus = new CommunicationStatus[] { CommunicationStatus.NotifiedRecipient, CommunicationStatus.Acknowledged };
      var results = await messageBus.InvokeAsync<CommunicationsQueryResults>(query);

      return TypedResults.Ok(results.Items.Select(i => new Communication
      {
        Id = i.Id!,
        Subject = i.Subject,
        Text = i.Text
      }));
    })
     .WithOpenApi("Handles messages queries", string.Empty, "message_get")
     .RequireAuthorization();
  }
}

public record Communication
{
  public string Id { get; set; } = null!;
  public string Subject { get; set; } = null!;
  public string Text { get; set; } = null!;
}
