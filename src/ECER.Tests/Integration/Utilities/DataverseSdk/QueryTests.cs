using ECER.Utilities.DataverseSdk.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Shouldly;
using Xunit.Categories;

namespace ECER.Tests.Integration.Utilities.DataverseSdk;

[IntegrationTest]
public class QueryTests : IAsyncLifetime
{
    private readonly IConfigurationRoot configuration;
    private ServiceClient serviceClient = null!;
    private EcerContext dataverseContext = null!;

    public QueryTests()
    {
        var configBuilder = new ConfigurationBuilder().AddUserSecrets(typeof(Clients.RegistryPortal.Server.Program).Assembly);
        configuration = configBuilder.Build();
    }

    [Fact]
    public void WhereIn_Filtered()
    {
        var statuses = new[] { ecer_Application_StatusCode.Draft, ecer_Application_StatusCode.Complete };
        var query = dataverseContext.ecer_ApplicationSet.WhereIn(a => a.StatusCode!.Value, statuses).ToList();
        query.ShouldNotBeEmpty();
        query.ShouldAllBe(a => statuses.Contains(a.StatusCode!.Value));
    }

    [Fact]
    public void WhereNotIn_Filtered()
    {
        var statuses = new[] { ecer_Application_StatusCode.Draft, ecer_Application_StatusCode.Complete };
        var query = dataverseContext.ecer_ApplicationSet.WhereNotIn(a => a.StatusCode!.Value, statuses).ToList();
        query.ShouldNotBeEmpty();
        query.ShouldAllBe(a => !statuses.Contains(a.StatusCode!.Value));
    }

    public async Task InitializeAsync()
    {
        await Task.CompletedTask;
        serviceClient = new ServiceClient(configuration.GetValue<string>("Dataverse:ConnectionString"));
        dataverseContext = new EcerContext(serviceClient);
    }

    public async Task DisposeAsync()
    {
        await Task.CompletedTask;
        dataverseContext.Dispose();
        serviceClient.Dispose();
    }
}