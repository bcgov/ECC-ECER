using ECER.Managers.Registry;

namespace ECER.Clients.RegistryPortal.Server;

/// <summary>
/// Registry API endpoint map
/// </summary>
public static class ApplicationsEndpoints
{
    internal static void Map(WebApplication app)
    {
        app.MapPost("/api/applications", async (NewApplicationRequest request, HttpContext ctx) =>
        {
            var handlers = ctx.RequestServices.GetRequiredService<ApplicationHandlers>();
            var cmd = new SubmitNewApplicationCommand();
            var appId = await handlers.Handle(cmd);

            return TypedResults.Ok(new NewApplicationResponse(appId));
        }).WithOpenApi(op =>
        {
            op.OperationId = "PostNewApplication";
            op.Summary = "New Application Submission";
            op.Description = "Handles  a new application submission to ECER";
            return op;
        });

        app.MapGet("/api/applications", async (HttpContext ctx) =>
        {
            var handlers = ctx.RequestServices.GetRequiredService<ApplicationHandlers>();
            var query = new ApplicationsQuery();
            var results = await handlers.Handle(query);
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