using ECER.Resources.Accounts.PortalInvitations;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit.Abstractions;
using Xunit.Categories;

namespace ECER.Tests.Integration.Resources.Accounts.PortalInvitations;

[IntegrationTest]
public class PortalInvitationRepositoryTests : RegistryPortalWebAppScenarioBase
{
  private readonly IPortalInvitationRepository repository;

  public PortalInvitationRepositoryTests(ITestOutputHelper output, RegistryPortalWebAppFixture fixture) : base(output, fixture)
  {
    repository = Fixture.Services.GetRequiredService<IPortalInvitationRepository>();
  }

  [Fact]
  public async Task QueryPortalInvitations_ById_Found()
  {
    // Arrange
    var portalInvitationId = Fixture.portalInvitationId;

    // Act
    var portalInvitations = await repository.Query(new PortalInvitationQuery(portalInvitationId));

    // Assert
    portalInvitations.ShouldHaveSingleItem();
  }
}
