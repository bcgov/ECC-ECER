using ECER.Managers.Registry;
using ECER.Utilities.Hosting;
using Wolverine;

namespace ECER.Clients.RegistryPortal.Server.Applications;

public class ApplicationsEndpoints : IRegisterEndpoints
{
    public void Register(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPost("/api/applications", async (NewApplicationRequest request, IMessageBus messageBus) =>
        {
            var cmd = new SubmitNewApplicationCommand();
            var appId = await messageBus.InvokeAsync<string>(cmd);

            return TypedResults.Ok(new NewApplicationResponse(appId));
        }).WithOpenApi(op =>
        {
            op.OperationId = "PostNewApplication";
            op.Summary = "New Application Submission";
            op.Description = "Handles  a new application submission to ECER";
            return op;
        });

        endpointRouteBuilder.MapGet("/api/applications", async (IMessageBus messageBus) =>
        {
            var query = new ApplicationsQuery();
            var results = await messageBus.InvokeAsync<ApplicationsQueryResults>(query);
            return TypedResults.Ok(new ApplicationQueryResponse(results.Items.Select(i => new Application
            {
                Id = i.Id,
                RegistrantId = i.RegistrantId,
                SubmittedOn = i.SubmittedOn
            })));
        }).WithOpenApi(op =>
        {
            op.OperationId = "GetApplications";
            op.Summary = "Query applications";
            op.Description = "Handles application queries";
            return op;
        });
    }
}

/// <summary>
/// New application request
/// </summary>
public record NewApplicationRequest();

/// <summary>
/// New application response
/// </summary>
/// <param name="ApplicationId">The new application id</param>
public record NewApplicationResponse(string ApplicationId);

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