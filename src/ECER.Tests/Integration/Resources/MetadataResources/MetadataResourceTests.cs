using ECER.Resources.Documents.MetadataResources;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit.Abstractions;
using Xunit.Categories;

namespace ECER.Tests.Integration.Resources.MetadataResources;

[IntegrationTest]
public class MetadataResourceTests : RegistryPortalWebAppScenarioBase
{
  private readonly IMetadataResourceRepository repository;

  public MetadataResourceTests(ITestOutputHelper output, RegistryPortalWebAppFixture fixture) : base(output, fixture)
  {
    repository = Fixture.Services.GetRequiredService<IMetadataResourceRepository>();
  }

  [Fact]
  public async Task QueryDynamicsConfig_Found()
  {
    // Act
    var config = await repository.QueryDynamicsConfiguration(new DynamicsConfigQuery { }, CancellationToken.None);

    // Assert
    config.ShouldNotBeEmpty();
  }
}
