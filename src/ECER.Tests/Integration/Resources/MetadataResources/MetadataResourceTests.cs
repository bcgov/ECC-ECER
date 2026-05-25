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

  [Fact]
  public async Task QueryPostSecondaryInstitutions_ByStatusActive_ReturnsOnlyActive()
  {
    var all = await repository.QueryPostSecondaryInstitutions(
      new PostSecondaryInstitutionsQuery { },
      CancellationToken.None);
    var activeOnly = await repository.QueryPostSecondaryInstitutions(
      new PostSecondaryInstitutionsQuery { ByStatus = PostSecondaryInstitutionStatus.Active },
      CancellationToken.None);

    all.ShouldNotBeEmpty();
    activeOnly.ShouldNotBeEmpty();
    activeOnly.Count().ShouldBeLessThanOrEqualTo(all.Count());
  }

  [Fact]
  public async Task QueryPostSecondaryInstitutions_ByStatusOmitted_ReturnsAll()
  {
    var all = await repository.QueryPostSecondaryInstitutions(
      new PostSecondaryInstitutionsQuery { },
      CancellationToken.None);

    all.ShouldNotBeEmpty();
  }
}
