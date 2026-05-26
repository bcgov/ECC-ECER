using Alba;
using Alba.Security;
using Bogus;
using ECER.Utilities.FileScanner;
using ECER.Utilities.ObjectStorage.Providers;
using MartinCostello.Logging.XUnit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
using Xunit.Categories;

namespace ECER.Tests.Integration;

[IntegrationTest]
public abstract class WebAppScenarioBase : IAsyncLifetime
{
  protected WebAppFixtureBase Fixture { get; }

  public IAlbaHost Host => Fixture.Host;

  protected Faker Faker { get; } = new Faker("en_CA");

  protected WebAppScenarioBase(ITestOutputHelper output, WebAppFixtureBase fixture)
  {
    this.Fixture = fixture;
    this.Fixture.OutputHelper = output;
  }

  public virtual async Task InitializeAsync()
  {
    await Task.CompletedTask;
  }

  public virtual async Task DisposeAsync()
  {
    await Task.CompletedTask;
  }
}

public abstract class WebAppFixtureBase : IAsyncLifetime, ITestOutputHelperAccessor
{
  public string TestOwnerScope { get; } = BuildTestOwnerScope();
  public string TestRunId { get; }
  public IAlbaHost Host { get; protected set; } = null!;
  public ITestOutputHelper? OutputHelper { get; set; }

  protected WebAppFixtureBase()
  {
    TestRunId = $"{TestOwnerScope}_{Guid.NewGuid().ToString("N")[..4]}_";
  }

  protected virtual async Task<IAlbaHost> CreateHost<TProgram>(KeyValuePair<string, string?>[]? configurationSettings = null, IEnumerable<IAlbaExtension>? extensions = null)
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
                    services.AddLogging(loggingBuilder => loggingBuilder.ClearProviders().AddXUnit(this));
                    // Configure test authentication and policy - Alba expects JwtBearerDefaults.AuthenticationScheme to automatically configure this
                    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts =>
                        {
                          opts.Authority = "https://test_server";
                          opts.Audience = "test_client";
                        });

                    services.AddTransient<IFileScannerProvider, TestFileScannerProvider>();
                    services.AddSingleton<IObjectStorageProviderResolver, TestObjectStorageProviderResolver>();
                    services.AddAuthorization(AddAuthorizationOptions);
                  });
          var configOverrides = new Dictionary<string, string?>(configurationSettings ?? Enumerable.Empty<KeyValuePair<string, string?>>());
          builder.ConfigureAppConfiguration(
                  (ctx, configBuilder) =>
                  {
                    var secretsFile = Environment.GetEnvironmentVariable("SECRETS_FILE_PATH");
                    if (secretsFile != null && File.Exists(secretsFile))
                    {
                      configBuilder.AddJsonFile(secretsFile, false);
                    }
                    else
                    {
                      configBuilder
                        .AddUserSecrets<Clients.Api.Program>(optional: true)
                        .AddUserSecrets<Clients.RegistryPortal.Server.Program>(optional: true)
                        .AddUserSecrets<Clients.PSPPortal.Server.Program>(optional: true);
                    }
                    configBuilder.AddInMemoryCollection(configOverrides);
                  });
        },
        extensions.ToArray());
  }

  protected virtual void AddAuthorizationOptions(AuthorizationOptions opts)
  {
  }

  public abstract Task InitializeAsync();

  public abstract Task DisposeAsync();

  private static string BuildTestOwnerScope()
  {
    static string Normalize(string value) => new string(value.Where(char.IsLetterOrDigit).Select(char.ToLowerInvariant).ToArray());

    var userName = Normalize(Environment.UserName);
    var machineName = Normalize(Environment.MachineName);
    return $"autotest_{userName}_{machineName}";
  }
}
