using ECER.Managers.Registry.Contract.Communications;
using ECER.Utilities.Hosting;
using ECER.Utilities.Security;
using MediatR;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;

namespace ECER.Clients.RegistryPortal.Server.Communications;

public class CommunicationsEndpoints : IRegisterEndpoints
{
  public void Register(IEndpointRouteBuilder endpointRouteBuilder)
  {
    endpointRouteBuilder.MapGet("/api/messages/{parentId?}", async (string? parentId, HttpContext httpContext, IMediator messageBus, IMapper mapper, IOptions<PaginationSettings> paginationOptions) =>
    {
      var userContext = httpContext.User.GetUserContext();
      bool IdIsNotGuid = !Guid.TryParse(parentId, out _); if (IdIsNotGuid && parentId != null) { parentId = null; }
      // Get pagination parameters from the query string with default values
      var pageNumber = int.TryParse(httpContext.Request.Query[paginationOptions.Value.PageProperty], out var page) ? page : paginationOptions.Value.DefaultPageNumber;
      var pageSize = int.TryParse(httpContext.Request.Query[paginationOptions.Value.PageSizeProperty], out var size) ? size : paginationOptions.Value.DefaultPageSize;

      var query = new UserCommunicationQuery
      {
        ByRegistrantId = userContext!.UserId,
        ByParentId = parentId,
        ByStatus = [ECER.Managers.Registry.Contract.Communications.CommunicationStatus.NotifiedRecipient, ECER.Managers.Registry.Contract.Communications.CommunicationStatus.Acknowledged],
        PageNumber = pageNumber,
        PageSize = pageSize
      };
      var results = await messageBus.Send<CommunicationsQueryResults>(query);

      return TypedResults.Ok(mapper.Map<IEnumerable<Communication>>(results.Items));
    })
     .WithOpenApi("Handles messages queries", string.Empty, "message_get")
     .RequireAuthorization();

    endpointRouteBuilder.MapPost("/api/messages", async Task<Results<Ok<SendMessageResponse>, BadRequest<ProblemDetails>, NotFound>> (SendMessageRequest request, HttpContext ctx, CancellationToken ct, IMediator messageBus, IMapper mapper) =>
    {
      var userId = ctx.User.GetUserContext()?.UserId;
      bool IsNotGuid = !Guid.TryParse(request.Communication.Id, out _);
      if (IsNotGuid)
      {
        return TypedResults.BadRequest(new ProblemDetails() { Title = "Communication Id is not valid" });
      }

      var mappedCommunication = mapper.Map<Managers.Registry.Contract.Communications.Communication>(request.Communication);
      var cmd = new SendMessageCommand(mappedCommunication, userId!);
      var result = await messageBus.Send(cmd, ct);

      if (!result.IsSuccess)
      {
        var problemDetails = new ProblemDetails
        {
          Status = StatusCodes.Status400BadRequest,
          Title = "Failed to send message.",
          Extensions = { ["errors"] = result.ErrorMessage }
        };
        return TypedResults.BadRequest(problemDetails);
      }
      return TypedResults.Ok(new SendMessageResponse(result.CommunicationId!));
    })
      .WithOpenApi("Handles message send request", string.Empty, "message_post")
      .RequireAuthorization();

    endpointRouteBuilder.MapPut("/api/messages/{id}/seen",
        async Task<Results<Ok<CommunicationResponse>, BadRequest<string>>> (string? id,
          CommunicationSeenRequest request, HttpContext ctx, IMediator messageBus, CancellationToken ct) =>
        {
          var userId = ctx.User.GetUserContext()?.UserId;

          bool IdIsNotGuid = !Guid.TryParse(id, out _);
          bool CommunicationIdIsNotGuid = !Guid.TryParse(request.CommunicationId, out _);

          if (IdIsNotGuid || CommunicationIdIsNotGuid) return TypedResults.BadRequest("invalid id");
          if (request.CommunicationId != id) return TypedResults.BadRequest("resource id and payload id do not match");

          var communicationId =
            await messageBus.Send(new MarkCommunicationAsSeenCommand(request.CommunicationId, userId!), ct);
          return TypedResults.Ok(new CommunicationResponse(communicationId));
        })
      .WithOpenApi("Marks a communication as seen", string.Empty, "communication_put")
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

/// <summary>
/// Save communication response
/// </summary>
/// <param name="CommunicationId">The communication id</param>
public record CommunicationResponse(string CommunicationId);

/// <summary>
/// Send Message Request
/// </summary>
/// <param name="Communication"></param>
public record SendMessageRequest(Communication Communication);

/// <summary>
/// Send Message Response
/// </summary>
/// <param name="CommunicationId"></param>
public record SendMessageResponse(string CommunicationId);

/// <summary>
/// Communication seen request
/// </summary>
/// <param name="CommunicationId">The communication ID</param>
public record CommunicationSeenRequest(string CommunicationId);

public record Communication
{
  public string Id { get; set; } = null!;
  public string Subject { get; set; } = null!;
  public string Text { get; set; } = null!;
  public InitiatedFrom From { get; set; }
  public bool Acknowledged { get; set; }
  public DateTime NotifiedOn { get; set; }
  public CommunicationStatus Status { get; set; }
}

public enum InitiatedFrom
{
  Investigation,
  Registrant,
  Registry,
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
