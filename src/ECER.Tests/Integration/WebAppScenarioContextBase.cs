using Alba;
using Bogus;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace ECER.Tests.Integration;

[CollectionDefinition("WebAppScenario")]
public class WebAppScenarioCollectionFixture : ICollectionFixture<WebAppFixture>;

[Collection("WebAppScenario")]
public abstract class WebAppScenarioBase<TProgram> : IAsyncLifetime
    where TProgram : class
{
    protected WebAppFixture Fixture { get; }
    private IServiceScope testServicesScope = null!;

    public IAlbaHost Host { get; private set; } = null!;

    public IServiceProvider TestServices => testServicesScope.ServiceProvider;

    protected ITestOutputHelper Output { get; }
    protected Faker Faker { get; } = new Faker("en_CA");

    protected WebAppScenarioBase(ITestOutputHelper output, WebAppFixture fixture)
    {
        Output = output;
        this.Fixture = fixture;
    }

    public virtual async Task InitializeAsync()
    {
        Host = await Fixture.CreateHost<TProgram>(Output, extensions: Array.Empty<IAlbaExtension>());
        testServicesScope = Host.Services.CreateScope();
    }

    public virtual async Task DisposeAsync()
    {
        testServicesScope.Dispose();
        await Host.DisposeAsync();
    }
}