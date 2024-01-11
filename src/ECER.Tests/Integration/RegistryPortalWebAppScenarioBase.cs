using Xunit.Abstractions;

namespace ECER.Tests.Integration;

[CollectionDefinition("RegistryPortalWebAppScenario")]
public class WebAppScenarioCollectionFixture : ICollectionFixture<RegistryPortalWebAppFixture>;

[Collection("RegistryPortalWebAppScenario")]
public abstract class RegistryPortalWebAppScenarioBase : WebAppScenarioBase
{
    protected RegistryPortalWebAppScenarioBase(ITestOutputHelper output, RegistryPortalWebAppFixture fixture) : base(output, fixture)
    {
    }
}

public class RegistryPortalWebAppFixture : WebAppFixtureBase
{
    public override async Task InitializeAsync()
    {
        Host = await CreateHost<Clients.RegistryPortal.Server.Program>();
    }
}