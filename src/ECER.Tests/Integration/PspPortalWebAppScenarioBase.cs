using ECER.Utilities.DataverseSdk.Model;
using ECER.Utilities.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xrm.Sdk.Client;
using System.Globalization;
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
  private ecer_Application inProgressTestApplication = null!;
  private ecer_PortalInvitation testPortalInvitationOne = null!;

  public Contact AuthenticatedBcscUser { get; set; } = null!;
  public IServiceProvider Services => serviceScope.ServiceProvider;
  public UserIdentity AuthenticatedBcscUserIdentity => AuthenticatedBcscUser.ecer_contact_ecer_authentication_455.Select(a => new UserIdentity(a.ecer_ExternalID, a.ecer_IdentityProvider)).First();
  public string AuthenticatedBcscUserId => AuthenticatedBcscUser.Id.ToString();
  public string inProgressApplicationId => inProgressTestApplication.Id.ToString();
  public Guid portalInvitationOneId => testPortalInvitationOne.ecer_PortalInvitationId ?? Guid.Empty;

  protected override void AddAuthorizationOptions(AuthorizationOptions opts)
  {
    ArgumentNullException.ThrowIfNull(opts);
    opts.AddPolicy("registry_verified_user", new AuthorizationPolicyBuilder(opts.GetPolicy("registry_verified_user")!).AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme).Build());
    opts.AddPolicy("registry_new_user", new AuthorizationPolicyBuilder(opts.GetPolicy("registry_new_user")!).AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme).Build());
    opts.AddPolicy("registry_unverified_user", new AuthorizationPolicyBuilder(opts.GetPolicy("registry_unverified_user")!).AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme).Build());
    opts.AddPolicy("registry_user", new AuthorizationPolicyBuilder(opts.GetPolicy("registry_user")!).AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme).Build());
    opts.DefaultPolicy = opts.GetPolicy("registry_verified_user")!;
  }

  public override async Task InitializeAsync()
  {
    Host = await CreateHost<Clients.RegistryPortal.Server.Program>();
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

    AuthenticatedBcscUser = GetOrAddApplicant(context, "bcsc", $"{TestRunId}_user1");
    inProgressTestApplication = GetOrAddApplication(context, AuthenticatedBcscUser, ecer_Application_StatusCode.InProgress);
    testPortalInvitationOne = GetOrAddPortalInvitation_PspProgramRepresentative(context, AuthenticatedBcscUser, "name1");

    context.SaveChanges();

    //load dependent properties
    context.Attach(AuthenticatedBcscUser);
    context.LoadProperty(AuthenticatedBcscUser, Contact.Fields.ecer_contact_ecer_authentication_455);

    context.SaveChanges();
  }

  private Contact GetOrAddApplicant(EcerContext context, string identityProvider, string userId)
  {
    var contact = (from a in context.ecer_AuthenticationSet
                   join c in context.ContactSet on a.ecer_contact_ecer_authentication_455.ContactId equals c.ContactId into contacts
                   from c in contacts.DefaultIfEmpty()
                   where a.ecer_IdentityProvider == identityProvider && a.ecer_ExternalID == userId && c.ecer_IsVerified == true
                   select c).SingleOrDefault();

    if (contact == null)
    {
      contact = new Contact
      {
        FirstName = "psp_test1",
        LastName = "psp_test1",
        MiddleName = "psp_test1",
        Address1_Telephone1 = "1234567890",
        EMailAddress1 = "test@test.com",
        ecer_IsVerified = true,
        StatusCode = Contact_StatusCode.Verified,
        BirthDate = DateTime.Parse("2000-03-15", CultureInfo.InvariantCulture),
      };

      var authentication = new ecer_Authentication
      {
        ecer_IdentityProvider = identityProvider,
        ecer_ExternalID = userId
      };
      context.AddObject(authentication);
      context.AddObject(contact);
      context.AddLink(authentication, ecer_Authentication.Fields.ecer_contact_ecer_authentication_455, contact);
    }

    return contact;
  }

  private ecer_Application GetOrAddApplication(EcerContext context, Contact applicant, ecer_Application_StatusCode status)
  {
    var application = (from a in context.ecer_ApplicationSet
                       where a.ecer_Applicantid.Id == applicant.Id && a.ecer_isECE5YR == true && a.StatusCode == status
                       select a).FirstOrDefault();

    if (application == null)
    {
      application = new ecer_Application
      {
        Id = Guid.NewGuid(),
        ecer_isECE5YR = true,
        StatusCode = status,
        StateCode = ecer_application_statecode.Active,
        ecer_CertificateType = "ECE 5 YR",
      };
      context.AddObject(application);
      context.AddLink(application, ecer_Application.Fields.ecer_application_Applicantid_contact, applicant);
    }
    return application;
  }

  private ecer_PortalInvitation GetOrAddPortalInvitation_PspProgramRepresentative(EcerContext context, Contact registrant, string name)
  {
    var portalInvitation = context.ecer_PortalInvitationSet.FirstOrDefault(p => p.ecer_ApplicantId != null &&
                                                                                p.ecer_ApplicationId != null &&
                                                                                p.ecer_Name == name &&
                                                                                p.ecer_CharacterReferenceId != null &&
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
        ecer_FirstName = "psp_autotest_charref_first",
        ecer_LastName = "psp_autotest_charref_last",
        ecer_EmailAddress = "reference_test@test.gov.bc.ca",
        StatusCode = ecer_PortalInvitation_StatusCode.Sent,
        ecer_Type = ecer_PortalInvitationTypes.PSIProgramRepresentative,
      };

      context.AddObject(portalInvitation);
      context.AddLink(portalInvitation, ecer_PortalInvitation.Fields.ecer_portalinvitation_ApplicantId, registrant);
      context.AddLink(portalInvitation, ecer_PortalInvitation.Fields.ecer_portalinvitation_ApplicationId, inProgressTestApplication);
    }

    return portalInvitation;
  }

}

