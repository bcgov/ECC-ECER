using Alba;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace ECER.Tests.Integration;

public class WebAppFixture : IAsyncLifetime
{
    private readonly ServiceCollection testRelatedServices = new();

    public IServiceCollection Services => testRelatedServices;

    public string TestRunId { get; } = $"autotest_{Guid.NewGuid().ToString().Substring(0, 4)}_";

    public virtual async Task<IAlbaHost> CreateHost<TProgram>(ITestOutputHelper output, KeyValuePair<string, string?>[]? configurationSettings = null, IEnumerable<IAlbaExtension>? extensions = null)
        where TProgram : class
    {
        return await AlbaHost.For<TProgram>(
            builder =>
            {
                builder.ConfigureServices(
                    (_, services) =>
                    {
                        services.AddLogging(loggingBuilder => loggingBuilder.AddXUnit(output));
                    });

                Dictionary<string, string?> configOverrides = new(configurationSettings ?? Enumerable.Empty<KeyValuePair<string, string?>>()) { };
                builder.ConfigureAppConfiguration(
                    (_, configBuilder) =>
                    {
                        configBuilder.AddInMemoryCollection(configOverrides);
                    });
            },
            extensions?.ToArray() ?? Array.Empty<IAlbaExtension>());
    }

    public async Task InitializeAsync()
    {
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await Task.CompletedTask;
    }
}