﻿using ECER.Utilities.DataverseSdk.Model;
using ECER.Utilities.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xrm.Sdk.Client;
using System.Globalization;
using Xunit.Abstractions;

namespace ECER.Tests.Integration;

[CollectionDefinition("RegistryPortalWebAppScenario")]
public class WebAppScenarioCollectionFixture : ICollectionFixture<RegistryPortalWebAppFixture>;

[Collection("RegistryPortalWebAppScenario")]
public abstract class RegistryPortalWebAppScenarioBase : WebAppScenarioBase
{
  protected new RegistryPortalWebAppFixture Fixture => (RegistryPortalWebAppFixture)base.Fixture;

  protected RegistryPortalWebAppScenarioBase(ITestOutputHelper output, RegistryPortalWebAppFixture fixture) : base(output, fixture)
  {
  }
}

public class RegistryPortalWebAppFixture : WebAppFixtureBase
{
  private IServiceScope serviceScope = null!;
  private Contact authenticatedBcscUser = null!;
  private ecer_Application testApplication = null!;
  private ecer_Communication testCommunicationOne = null!;
  private ecer_Communication testCommunicationTwo = null!;
  private ecer_Communication testCommunicationThree = null!;
  private ecer_PortalInvitation testPortalInvitationOne = null!;
  private ecer_PortalInvitation testPortalInvitationCharacterReference = null!;
  private ecer_PortalInvitation testPortalInvitationWorkExperienceReference = null!;

  private Contact authenticatedBcscUser2 = null!;
  private ecer_Application testApplication2 = null!;

  public IServiceProvider Services => serviceScope.ServiceProvider;
  public UserIdentity AuthenticatedBcscUserIdentity => authenticatedBcscUser.ecer_contact_ecer_authentication_455.Select(a => new UserIdentity(a.ecer_ExternalID, a.ecer_IdentityProvider)).First();
  public string AuthenticatedBcscUserId => authenticatedBcscUser.Id.ToString();
  public string communicationOneId => testCommunicationOne.Id.ToString();
  public string communicationTwoId => testCommunicationTwo.Id.ToString();
  public string communicationThreeId => testCommunicationThree.Id.ToString();
  public string applicationId => testApplication.Id.ToString();
  public Guid portalInvitationOneId => testPortalInvitationOne.ecer_PortalInvitationId ?? Guid.Empty;
  public Guid portalInvitationCharacterReferenceId => testPortalInvitationCharacterReference.ecer_PortalInvitationId ?? Guid.Empty;
  public Guid portalInvitationWorkExperienceReferenceId => testPortalInvitationWorkExperienceReference.ecer_PortalInvitationId ?? Guid.Empty;

  public UserIdentity AuthenticatedBcscUserIdentity2 => authenticatedBcscUser2.ecer_contact_ecer_authentication_455.Select(a => new UserIdentity(a.ecer_ExternalID, a.ecer_IdentityProvider)).First();
  public string AuthenticatedBcscUserId2 => authenticatedBcscUser2.Id.ToString();
  public string applicationId2 => testApplication2.Id.ToString();

  protected override void AddAuthorizationOptions(AuthorizationOptions opts)
  {
    ArgumentNullException.ThrowIfNull(opts);
    opts.AddPolicy("registry_user", new AuthorizationPolicyBuilder(opts.GetPolicy("registry_user")!).AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme).Build());
    opts.AddPolicy("registry_new_user", new AuthorizationPolicyBuilder(opts.GetPolicy("registry_new_user")!).AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme).Build());
    opts.DefaultPolicy = opts.GetPolicy("registry_user")!;
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
    serviceScope.Dispose();
  }

  private async Task InitializeDataverseTestData(EcerContext context)
  {
    await Task.CompletedTask;

    authenticatedBcscUser = GetOrAddApplicant(context, "bcsc", $"{TestRunId}_user1");
    testApplication = GetOrAddApplication(context, authenticatedBcscUser);
    testCommunicationOne = GetOrAddCommunication(context, testApplication, authenticatedBcscUser, "comm1");
    testCommunicationTwo = GetOrAddCommunication(context, testApplication, authenticatedBcscUser, "comm2");
    testCommunicationThree = GetOrAddCommunication(context, testApplication, authenticatedBcscUser, "comm3");
    testPortalInvitationOne = GetOrAddPortalInvitation_CharacterReference(context, authenticatedBcscUser, "name1");
    testPortalInvitationCharacterReference = GetOrAddPortalInvitation_CharacterReference(context, authenticatedBcscUser, "name2");
    testPortalInvitationWorkExperienceReference = GetOrAddPortalInvitation_WorkExperienceReference(context, authenticatedBcscUser, "name3");
    context.SaveChanges();

    //load dependent properties
    context.Attach(authenticatedBcscUser);
    context.LoadProperty(authenticatedBcscUser, Contact.Fields.ecer_contact_ecer_authentication_455);

    //load user 2
    authenticatedBcscUser2 = GetOrAddApplicant(context, "bcsc", $"{TestRunId}_user2");
    testApplication2 = GetOrAddApplication(context, authenticatedBcscUser2);

    context.SaveChanges();

    //load dependent properties
    context.Attach(authenticatedBcscUser2);
    context.LoadProperty(authenticatedBcscUser2, Contact.Fields.ecer_contact_ecer_authentication_455);
  }

  private Contact GetOrAddApplicant(EcerContext context, string identityProvider, string userId)
  {
    var contact = (from a in context.ecer_AuthenticationSet
                   join c in context.ContactSet on a.ecer_contact_ecer_authentication_455.ContactId equals c.ContactId into contacts
                   from c in contacts.DefaultIfEmpty()
                   where a.ecer_IdentityProvider == identityProvider && a.ecer_ExternalID == userId
                   select c).SingleOrDefault();

    if (contact == null)
    {
      contact = new Contact
      {
        FirstName = "test1",
        LastName = "test1",
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

  private ecer_Application GetOrAddApplication(EcerContext context, Contact applicant)
  {
    var application = (from a in context.ecer_ApplicationSet
                       where a.ecer_Applicantid.Id == applicant.Id && a.ecer_isECE5YR == true && a.StatusCode == ecer_Application_StatusCode.InProgress
                       select a).FirstOrDefault();

    if (application == null)
    {
      application = new ecer_Application
      {
        Id = Guid.NewGuid(),
        ecer_isECE5YR = true,
        StatusCode = ecer_Application_StatusCode.InProgress,
        StateCode = ecer_application_statecode.Active,
        ecer_CertificateType = "ECE 5 YR",
      };
      context.AddObject(application);
      context.AddLink(application, ecer_Application.Fields.ecer_application_Applicantid_contact, applicant);
    }
    return application;
  }

  private ecer_Communication GetOrAddCommunication(EcerContext context, ecer_Application application, Contact registrant, string name)
  {
    var communication = (from a in context.ecer_CommunicationSet
                         where a.ecer_Applicationid.Id == application.Id
                         & a.ecer_Registrantid.Id == registrant.Id
                         & a.ecer_Name == name
                         select a).FirstOrDefault();

    if (communication == null)
    {
      communication = new ecer_Communication
      {
        Id = Guid.NewGuid(),
        ecer_Message = "Test message",
        ecer_Acknowledged = false,
        StatusCode = ecer_Communication_StatusCode.NotifiedRecipient,
        ecer_Name = name
      };

      context.AddObject(communication);
      context.AddLink(communication, ecer_Communication.Fields.ecer_communication_Applicationid, application);
      context.AddLink(registrant, ecer_Communication.Fields.ecer_contact_ecer_communication_122, communication);
    }
    else
    {
      // Reset to notified (not yet marked as seen)
      communication.ecer_Acknowledged = false;
      communication.StatusCode = ecer_Communication_StatusCode.NotifiedRecipient;
      context.UpdateObject(communication);
    }

    return communication;
  }

  private ecer_PortalInvitation GetOrAddPortalInvitation_CharacterReference(EcerContext context, Contact registrant, string name)
  {
    var portalInvitation = context.ecer_PortalInvitationSet.FirstOrDefault(p => p.ecer_ApplicantId != null &&
                                                                                p.ecer_ApplicationId != null &&
                                                                                p.ecer_Name == name &&
                                                                                p.ecer_CharacterReferenceId != null &&
                                                                                p.StatusCode == ecer_PortalInvitation_StatusCode.Sent);

    if (portalInvitation == null)
    {
      var charGuid = Guid.NewGuid();

      var characterReference = new ecer_CharacterReference
      {
        Id = charGuid,
        ecer_CharacterReferenceId = charGuid,
        ecer_Name = "Reference Test name",
        ecer_FirstName = "Reference Test firstname",
        ecer_LastName = "Reference Test lastname",
        ecer_EmailAddress = "reference_test@test.com"
      };

      var guid = Guid.NewGuid();
      portalInvitation = new ecer_PortalInvitation
      {
        Id = guid,
        ecer_PortalInvitationId = guid,
        ecer_Name = name,
        ecer_FirstName = "Test firstname",
        ecer_LastName = "Test lastname",
        ecer_EmailAddress = "test@email.com",
        StatusCode = ecer_PortalInvitation_StatusCode.Sent,
      };

      context.AddObject(characterReference);
      context.AddObject(portalInvitation);
      context.AddLink(portalInvitation, ecer_PortalInvitation.Fields.ecer_portalinvitation_CharacterReferenceId, characterReference);
      context.AddLink(portalInvitation, ecer_PortalInvitation.Fields.ecer_portalinvitation_ApplicantId, registrant);
      context.AddLink(portalInvitation, ecer_PortalInvitation.Fields.ecer_portalinvitation_ApplicationId, testApplication);
    }

    return portalInvitation;
  }

  private ecer_PortalInvitation GetOrAddPortalInvitation_WorkExperienceReference(EcerContext context, Contact registrant, string name)
  {
    var portalInvitation = context.ecer_PortalInvitationSet.FirstOrDefault(p => p.ecer_ApplicantId != null &&
                                                                                p.ecer_ApplicationId != null &&
                                                                                p.ecer_Name == name &&
                                                                                p.ecer_WorkExperienceReferenceId != null &&
                                                                                p.StatusCode == ecer_PortalInvitation_StatusCode.Sent);

    if (portalInvitation == null)
    {
      var wpGuid = Guid.NewGuid();

      var workexperienceReference = new ecer_WorkExperienceRef
      {
        Id = wpGuid,
        ecer_WorkExperienceRefId = wpGuid,
        ecer_Name = "Reference Test name",
        ecer_FirstName = "Reference Test firstname",
        ecer_LastName = "Reference Test lastname",
        ecer_EmailAddress = "reference_test@test.com"
      };

      var guid = Guid.NewGuid();
      portalInvitation = new ecer_PortalInvitation
      {
        Id = guid,
        ecer_PortalInvitationId = guid,
        ecer_Name = name,
        ecer_FirstName = "Test firstname",
        ecer_LastName = "Test lastname",
        ecer_EmailAddress = "test@email.com",
        StatusCode = ecer_PortalInvitation_StatusCode.Sent,
      };

      context.AddObject(workexperienceReference);
      context.AddObject(portalInvitation);
      context.AddLink(portalInvitation, ecer_PortalInvitation.Fields.ecer_portalinvitation_ApplicantId, registrant);
      context.AddLink(portalInvitation, ecer_PortalInvitation.Fields.ecer_portalinvitation_ApplicationId, testApplication);
      context.AddLink(portalInvitation, ecer_PortalInvitation.Fields.ecer_portalinvitation_WorkExperienceRefId, workexperienceReference);
    }

    return portalInvitation;
  }
}
