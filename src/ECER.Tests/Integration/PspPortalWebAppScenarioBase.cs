using ECER.Utilities.DataverseSdk.Model;
using ECER.Utilities.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xrm.Sdk;
using Xunit.Abstractions;

namespace ECER.Tests.Integration.PspApi;

[CollectionDefinition("PspPortalWebAppScenario")]
public class WebAppScenarioCollectionFixture : ICollectionFixture<PspPortalWebAppFixture>;

[Collection("PspPortalWebAppScenario")]
public abstract class PspPortalWebAppScenarioBase : WebAppScenarioBase
{
  protected new PspPortalWebAppFixture Fixture => (PspPortalWebAppFixture)base.Fixture;

  protected PspPortalWebAppScenarioBase(ITestOutputHelper output, PspPortalWebAppFixture fixture) : base(output, fixture)
  {
  }
}

public class PspPortalWebAppFixture : WebAppFixtureBase
{
  private IServiceScope serviceScope = null!;
  private ecer_PostSecondaryInstitute testPostSecondaryInstitute = null!;
  private ecer_ECEProgramRepresentative testProgramRepresentative = null!;
  private ecer_PortalInvitation testPortalInvitationOne = null!;
  private UserIdentity testPspIdentity = null!;

  public IServiceProvider Services => serviceScope.ServiceProvider;
  public UserIdentity AuthenticatedPspUserIdentity => testPspIdentity;
  public string AuthenticatedPspUserId => testProgramRepresentative.Id.ToString();
  public string PostSecondaryInstituteId => testPostSecondaryInstitute.ecer_PostSecondaryInstituteId?.ToString() ?? string.Empty;
  public Guid portalInvitationOneId => testPortalInvitationOne.ecer_PortalInvitationId ?? Guid.Empty;

  protected override void AddAuthorizationOptions(AuthorizationOptions opts)
  {
    ArgumentNullException.ThrowIfNull(opts);
    opts.AddPolicy("psp_user", new AuthorizationPolicyBuilder(opts.GetPolicy("psp_user")!).AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme).Build());
    opts.AddPolicy("psp_user_has_accepted_terms", new AuthorizationPolicyBuilder(opts.GetPolicy("psp_user_has_accepted_terms")!).AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme).Build());
    opts.DefaultPolicy = opts.GetPolicy("psp_user")!;
  }

  public override async Task InitializeAsync()
  {
    Host = await CreateHost<Clients.PSPPortal.Server.Program>();
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

    testPostSecondaryInstitute = GetOrAddPostSecondaryInstitute(context);
    testProgramRepresentative = GetOrAddProgramRepresentative(context, testPostSecondaryInstitute);
    testPspIdentity = GetOrAddProgramRepresentativeIdentity(context, testProgramRepresentative);
    testPortalInvitationOne = GetOrAddPortalInvitation_PspProgramRepresentative(context, testProgramRepresentative, $"{TestRunId}psp_invite1");

    context.SaveChanges();

    //load dependent properties
    context.Attach(testProgramRepresentative);
    context.LoadProperty(testProgramRepresentative, ecer_ECEProgramRepresentative.Fields.ecer_authentication_eceprogramrepresentative);

    context.SaveChanges();
  }

  private ecer_PostSecondaryInstitute GetOrAddPostSecondaryInstitute(EcerContext context)
  {
    var instituteName = $"{TestRunId}psp_institute";
    var institute = context.ecer_PostSecondaryInstituteSet.FirstOrDefault(i => i.ecer_Name == instituteName);

    if (institute == null)
    {
      var instituteId = Guid.NewGuid();
      institute = new ecer_PostSecondaryInstitute
      {
        Id = instituteId,
        ecer_PostSecondaryInstituteId = instituteId,
        ecer_Name = instituteName,
        ecer_City = "Victoria"
      };

      context.AddObject(institute);
    }

    return institute;
  }

  private ecer_ECEProgramRepresentative GetOrAddProgramRepresentative(EcerContext context, ecer_PostSecondaryInstitute institute)
  {
    var repName = $"{TestRunId}psp_rep";
    var representative = context.ecer_ECEProgramRepresentativeSet.FirstOrDefault(r => r.ecer_FirstName == repName);

    if (representative == null)
    {
      var repId = Guid.NewGuid();
      representative = new ecer_ECEProgramRepresentative
      {
        Id = repId,
        ecer_ECEProgramRepresentativeId = repId,
        ecer_Name = $"{repName}_name",
        ecer_FirstName = repName,
        ecer_LastName = "autotest",
        ecer_PreferredFirstName = repName,
        ecer_EmailAddress = "psp_rep@test.gov.bc.ca",
        ecer_PhoneNumber = "5555555555",
        ecer_PhoneExtension = "123",
        ecer_Role = "Program Representative",
        ecer_RepresentativeRole = ecer_RepresentativeRole.Primary,
        ecer_HasAcceptedTermsofUse = true,
        StateCode = ecer_eceprogramrepresentative_statecode.Active,
        StatusCode = ecer_ECEProgramRepresentative_StatusCode.Active,
        ecer_PostSecondaryInstitute = new EntityReference(ecer_PostSecondaryInstitute.EntityLogicalName, institute.Id)
      };
      context.AddObject(representative);
      context.AddLink(representative, new Relationship(ecer_ECEProgramRepresentative.Fields.ecer_eceprogramrepresentative_PostSecondaryIns), institute);
    }

    return representative;
  }

  private UserIdentity GetOrAddProgramRepresentativeIdentity(EcerContext context, ecer_ECEProgramRepresentative representative)
  {
    var identity = new UserIdentity($"{TestRunId}psp_user1", "bceidbusiness");
    var authentication = context.ecer_AuthenticationSet.FirstOrDefault(a => a.ecer_IdentityProvider == identity.IdentityProvider && a.ecer_ExternalID == identity.UserId);

    if (authentication == null)
    {
      authentication = new ecer_Authentication
      {
        Id = Guid.NewGuid(),
        ecer_IdentityProvider = identity.IdentityProvider,
        ecer_ExternalID = identity.UserId
      };
      context.AddObject(authentication);
      context.AddLink(authentication, new Relationship(ecer_Authentication.Fields.ecer_authentication_eceprogramrepresentative), representative);
    }

    return identity;
  }

  private ecer_PortalInvitation GetOrAddPortalInvitation_PspProgramRepresentative(EcerContext context, ecer_ECEProgramRepresentative representative, string name)
  {
    var portalInvitation = context.ecer_PortalInvitationSet.FirstOrDefault(p => p.ecer_Name == name &&
                                                                                p.StatusCode == ecer_PortalInvitation_StatusCode.Sent &&
                                                                                p.ecer_Type == ecer_PortalInvitationTypes.PSIProgramRepresentative);

    if (portalInvitation == null)
    {
      var guid = Guid.NewGuid();
      portalInvitation = new ecer_PortalInvitation
      {
        Id = guid,
        ecer_PortalInvitationId = guid,
        ecer_Name = name,
        ecer_FirstName = representative.ecer_FirstName,
        ecer_LastName = representative.ecer_LastName,
        ecer_EmailAddress = representative.ecer_EmailAddress,
        StatusCode = ecer_PortalInvitation_StatusCode.Sent,
        ecer_Type = ecer_PortalInvitationTypes.PSIProgramRepresentative,
      };

      context.AddObject(portalInvitation);
      context.AddLink(portalInvitation, new Relationship(ecer_PortalInvitation.Fields.ecer_portalinvitation_psiprogramrepresentativeid), representative);
    }

    return portalInvitation;
  }

}
