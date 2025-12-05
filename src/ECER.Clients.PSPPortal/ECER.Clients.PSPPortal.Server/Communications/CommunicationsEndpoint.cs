using AutoMapper;
using ECER.Clients.PSPPortal.Server.Shared;
using ECER.Utilities.Hosting;
using ECER.Utilities.Security;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ECER.Managers.Registry.Contract.Communications;
using ECER.Managers.Registry.Contract.PspUsers;

namespace ECER.Clients.PSPPortal.Server.Communications;

public class CommunicationsEndpoint : IRegisterEndpoints
{
  public void Register(IEndpointRouteBuilder endpointRouteBuilder)
  { 
    endpointRouteBuilder.MapGet("/api/messages/{parentId?}", 
        async Task<Results<Ok<GetMessagesResponse>, BadRequest<ProblemDetails>, NotFound>> (string? parentId, HttpContext httpContext, 
          IMediator messageBus, IMapper mapper, CancellationToken ct, IOptions<PaginationSettings> paginationOptions) =>
      {
        var userContext = httpContext.User.GetUserContext()!;
        // Get users institute
        var currentRep = (await messageBus.Send<PspRepQueryResults>(new SearchPspRepQuery { ByUserIdentity = userContext.Identity }, ct)).Items.SingleOrDefault();
        if (currentRep == null || string.IsNullOrWhiteSpace(currentRep.PostSecondaryInstituteId))
        {
          return TypedResults.NotFound();
        }
        
        bool IdIsNotGuid = !Guid.TryParse(parentId, out _); if (IdIsNotGuid && parentId != null) { parentId = null; }
        // Get pagination parameters from the query string with default values
        var pageNumber = int.TryParse(httpContext.Request.Query[paginationOptions.Value.PageProperty], out var page) && page > 0 ? page : paginationOptions.Value.DefaultPageNumber;
        var pageSize = int.TryParse(httpContext.Request.Query[paginationOptions.Value.PageSizeProperty], out var size) ? size : paginationOptions.Value.DefaultPageSize;
        
        var query = new UserCommunicationQuery
        {
          //Pagination by users institute
          ByPostSecondaryInstituteId = currentRep.PostSecondaryInstituteId,
          ByParentId = parentId,
          ByStatus = [ECER.Managers.Registry.Contract.Communications.CommunicationStatus.NotifiedRecipient, 
            ECER.Managers.Registry.Contract.Communications.CommunicationStatus.Acknowledged], // will statuses be the same
          PageNumber = pageNumber,
          PageSize = pageSize
        };
        var results = await messageBus.Send<CommunicationsQueryResults>(query);

        return TypedResults.Ok(new GetMessagesResponse() { Communications = mapper.Map<IEnumerable<Communication>>(results.Items), TotalMessagesCount = results.TotalMessagesCount });
      })
      .WithOpenApi("Paginated endpoint to get all user messages", string.Empty, "get_all_messages")
      .RequireAuthorization("psp_user");
    
    endpointRouteBuilder.MapPost("/api/messages", 
        async Task<Results<Ok<SendMessageResponse>, BadRequest<ProblemDetails>, NotFound>> (SendMessageRequest request, HttpContext httpContext,
          CancellationToken ct, IMediator messageBus, IMapper mapper) =>
      {
        var userContext = httpContext.User.GetUserContext()!;
        
        // Get users institute
        // var currentRep = (await messageBus.Send<PspRepQueryResults>(new SearchPspRepQuery { ByUserIdentity = userContext.Identity }, ct)).Items.SingleOrDefault();
        // if (currentRep == null || string.IsNullOrWhiteSpace(currentRep.PostSecondaryInstituteId))
        // {
        //   return TypedResults.NotFound();
        // }
        
        bool IsNotGuid = !Guid.TryParse(request.Communication.Id, out _);
        if (IsNotGuid)
        {
          return TypedResults.BadRequest(new ProblemDetails() { Title = "Communication Id is not valid" });
        }

        var mappedCommunication = mapper.Map<Managers.Registry.Contract.Communications.Communication>(request.Communication);
        var cmd = new SendMessageCommand(mappedCommunication, userContext.UserId);
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
      .WithOpenApi("Endpoint to reply to an existing message", string.Empty, "post_message_reply")
      .RequireAuthorization("psp_user");
    
    endpointRouteBuilder.MapPut("/api/messages/{id}/seen",
      async Task<Results<Ok<CommunicationResponse>, BadRequest<string>>> (string? id,
        CommunicationSeenRequest request, HttpContext ctx, IMediator messageBus, CancellationToken ct) =>
      {
        await Task.CompletedTask;
        return TypedResults.BadRequest("not implemented");
      }).WithOpenApi("Handles messages status", string.Empty, "message_status_get")
        .RequireAuthorization("psp_user");
    
    endpointRouteBuilder.MapGet("/api/messages/status", async (HttpContext httpContext, IMediator messageBus) =>
      {
        await Task.CompletedTask;
        return TypedResults.BadRequest("not implemented");
      })
      .WithOpenApi("Handles messages status", string.Empty, "message_status_get")
      .RequireAuthorization("psp_user");
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

public record GetMessagesResponse
{
  public IEnumerable<Communication>? Communications { get; set; }
  public int TotalMessagesCount { get; set; }
}

public record Communication
{
  public string Id { get; set; } = null!;
  public string Subject { get; set; } = null!;
  public string Text { get; set; } = null!;
  public InitiatedFrom From { get; set; } 
  public bool Acknowledged { get; set; }
  public DateTime NotifiedOn { get; set; }
  public CommunicationStatus Status { get; set; }
  public bool DoNotReply { get; set; }
  public DateTime? LatestMessageNotifiedOn { get; set; }
  public bool? IsRead { get; set; }
  public string? ApplicationId { get; set; }
  public string? IcraEligibilityId { get; set; }
  public string? ProgramRepresentativeId { get; set; }
  public IEnumerable<CommunicationDocument> Documents { get; set; } = Array.Empty<CommunicationDocument>();
}

public record CommunicationDocument(string Id)
{
  public string Url { get; set; } = null!;
  public string Extention { get; set; } = null!;
  public string Name { get; set; } = null!;
  public string Size { get; set; } = null!;
}

public enum InitiatedFrom
{
  Investigation,
  PortalUser,
  Registry,
  ProgramRepresentative,
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
