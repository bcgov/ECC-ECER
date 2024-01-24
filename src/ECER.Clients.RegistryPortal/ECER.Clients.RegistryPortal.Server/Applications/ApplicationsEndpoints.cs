using System.ComponentModel;
using AutoMapper;
using ECER.Managers.Registry;
using ECER.Utilities.Hosting;
using ECER.Utilities.Security;
using Microsoft.Extensions.Caching.Distributed;
using Wolverine;

namespace ECER.Clients.RegistryPortal.Server.Applications;

public class ApplicationsEndpoints : IRegisterEndpoints
{
    public void Register(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPut("/api/draftapplications/{id?}", async (string? id, DraftApplication draftApplication, HttpContext httpContext, IMessageBus messageBus, IMapper mapper, IDistributedCache cache) =>
            {
                var userContext = httpContext.User.GetUserContext();
                var application = mapper.Map<CertificationApplication>(draftApplication);
                application.RegistrantId = userContext!.UserId!;
                application.Id = id;
                var cmd = new SaveDraftCertificationApplicationCommand(application);
                var appId = await messageBus.InvokeAsync<string>(cmd);

                return TypedResults.Ok(new DraftApplicationResponse(appId));
            })
            .WithOpenApi("Handles  a new application submission to ECER", string.Empty, "draftapplication_put")
            .RequireAuthorization();

        endpointRouteBuilder.MapPost("/api/applications/{id}", async (string id, IMessageBus messageBus, IMapper mapper) =>
            {
                var cmd = new SubmitCertificationApplicationCommand(id);
                var appId = await messageBus.InvokeAsync<string>(cmd);

                return TypedResults.Ok(new DraftApplicationResponse(appId));
            })
            .WithOpenApi("Handles a new application submission to ECER", string.Empty, "application_post")
            .RequireAuthorization();

        endpointRouteBuilder.MapGet("/api/applications", async (IMessageBus messageBus) =>
            {
                var query = new CertificationApplicationsQuery();
                var results = await messageBus.InvokeAsync<ApplicationsQueryResults>(query);
                return TypedResults.Ok(new ApplicationQueryResponse(results.Items.Select(i => new Application
                {
                    Id = i.Id!,
                    RegistrantId = i.RegistrantId,
                    SubmittedOn = i.SubmittedOn
                })));
            })
            .WithOpenApi("Handles application queries", string.Empty, "application_get")
            .RequireAuthorization();
    }
}

/// <summary>
/// New application request
/// </summary>
public record DraftApplication
{
    public string? Id { get; set; }
    public IEnumerable<CertificationType> CertificationTypes { get; set; } = Array.Empty<CertificationType>();
}

public enum CertificationType
{
    [Description("One Year")]
    OneYear,

    [Description("Five Years")]
    FiveYear,
}

/// <summary>
/// New application response
/// </summary>
/// <param name="ApplicationId">The new application id</param>
public record DraftApplicationResponse(string ApplicationId);

/// <summary>
/// Application query response
/// </summary>
/// <param name="Items">The items in the response</param>
public record ApplicationQueryResponse(IEnumerable<Application> Items);

public record Application
{
    public string Id { get; set; } = null!;
    public DateTime SubmittedOn { get; set; }
    public string RegistrantId { get; set; } = null!;
}