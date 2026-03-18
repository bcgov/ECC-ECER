using ECER.Utilities.DataverseSdk.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace ECER.Tests.Integration;

[CollectionDefinition("ApiWebAppScenario")]
public class ApiWebAppScenarioCollectionFixture : ICollectionFixture<ApiWebAppFixture>;

[Collection("ApiWebAppScenario")]
public abstract class ApiWebAppScenarioBase : WebAppScenarioBase
{
  protected new ApiWebAppFixture Fixture => (ApiWebAppFixture)base.Fixture;

  protected ApiWebAppScenarioBase(ITestOutputHelper output, ApiWebAppFixture fixture) : base(output, fixture)
  {
  }
}

public class ApiWebAppFixture : WebAppFixtureBase
{
  private IServiceScope serviceScope = null!;
  public ecer_PortalInvitation testPortalInvitationForLinkGeneration { get; set; } = null!;

  public IServiceProvider Services => serviceScope.ServiceProvider;

  protected override void AddAuthorizationOptions(AuthorizationOptions opts)
  {
    ArgumentNullException.ThrowIfNull(opts);
    opts.AddPolicy("api_user", new AuthorizationPolicyBuilder(opts.GetPolicy("api_user")!).AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme).Build());
    opts.AddPolicy("ew_user", new AuthorizationPolicyBuilder(opts.GetPolicy("ew_user")!).AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme).Build());

    opts.DefaultPolicy = opts.GetPolicy("api_user")!;
  }

  public override async Task InitializeAsync()
  {
    Host = await CreateHost<Clients.Api.Program>();
    serviceScope = Host.Services.CreateScope();
    var context = serviceScope.ServiceProvider.GetRequiredService<EcerContext>();
    await InitializeDataverseTestData(context);
  }

  public override async Task DisposeAsync()
  {
    await Task.CompletedTask;
    serviceScope?.Dispose();
  }

  private async Task InitializeDataverseTestData(EcerContext context)
  {
    await Task.CompletedTask;
    testPortalInvitationForLinkGeneration = GetOrAddPortalInvitation_No_Links(context, "autotest_for_api_unit_testing_1");
    context.SaveChanges();
  }

  //this will create a portal invitation without any linkages to character / work reference. Should not exist in real world scenarios. Just needed for testing link generation
  private static ecer_PortalInvitation GetOrAddPortalInvitation_No_Links(EcerContext context, string name)
  {
    var portalInvitation = context.ecer_PortalInvitationSet.FirstOrDefault(p => p.ecer_ApplicantId != null &&
                                                                                p.ecer_ApplicationId != null &&
                                                                                p.ecer_Name == name &&
                                                                                p.ecer_CharacterReferenceId != null &&
                                                                                p.StatusCode == ecer_PortalInvitation_StatusCode.Sent);

    if (portalInvitation == null)
    {
      var guid = Guid.NewGuid();
      portalInvitation = new ecer_PortalInvitation
      {
        Id = guid,
        ecer_PortalInvitationId = guid,
        ecer_Name = name,
        ecer_FirstName = "autotest_first_for_api_no_links",
        ecer_LastName = "autotest_last_for_api_no_links",
        ecer_EmailAddress = "reference_test@test.gov.bc.ca",
        StatusCode = ecer_PortalInvitation_StatusCode.Sent,
      };

      context.AddObject(portalInvitation);
    }

    return portalInvitation;
  }
}
