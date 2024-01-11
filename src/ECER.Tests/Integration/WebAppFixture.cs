using Alba;
using Alba.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace ECER.Tests.Integration;

public class WebAppFixture : IAsyncLifetime
{
    public IServiceCollection Services { get; } = new ServiceCollection();

    public string TestRunId { get; } = $"autotest_{Guid.NewGuid().ToString().Substring(0, 4)}_";

    public virtual async Task<IAlbaHost> CreateHost<TProgram>(ITestOutputHelper output, KeyValuePair<string, string?>[]? configurationSettings = null, IEnumerable<IAlbaExtension>? extensions = null)
        where TProgram : class
    {
        if (extensions == null) extensions = Array.Empty<IAlbaExtension>();

        // add jwt authentication stub
#pragma warning disable CA2000 // Dispose objects before losing scope
        extensions = extensions.Append(new JwtSecurityStub());
#pragma warning restore CA2000 // Dispose objects before losing scope

        return await AlbaHost.For<TProgram>(
            builder =>
            {
                builder.ConfigureServices(
                    (_, services) =>
                    {
                        services.AddLogging(loggingBuilder => loggingBuilder.AddXUnit(output));
                        // Configure test authentication and policy - Alba expects JwtBearerDefaults.AuthenticationScheme to automatically configure this
                        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts =>
                        {
                            opts.Authority = "https://test_server";
                            opts.Audience = "test_client";
                        });
                        services.AddAuthorizationBuilder().AddDefaultPolicy(JwtBearerDefaults.AuthenticationScheme, opts =>
                        {
                            opts.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme).RequireAuthenticatedUser();
                        });
                    });
                var configOverrides = new Dictionary<string, string?>(configurationSettings ?? Enumerable.Empty<KeyValuePair<string, string?>>());
                builder.ConfigureAppConfiguration(
                    (_, configBuilder) =>
                    {
                        configBuilder.AddInMemoryCollection(configOverrides);
                    });
            },
            extensions.ToArray());
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