using ECER.Utilities.DataverseSdk.Model;
using ECER.Utilities.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xrm.Sdk;
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

  private ecer_Application inProgressTestApplication = null!;
  private ecer_Application draftTestApplication = null!;

  private ecer_Communication testCommunication1 = null!;
  private ecer_Communication testCommunication2 = null!;
  private ecer_Communication testCommunication3 = null!;
  private ecer_Communication testCommunication4 = null!;

  private ecer_PortalInvitation testPortalInvitationOne = null!;
  private ecer_PortalInvitation testPortalInvitationCharacterReferenceSubmit = null!;
  private ecer_PortalInvitation testPortalInvitationWorkExperienceReferenceSubmit = null!;
  private ecer_PortalInvitation testPortalInvitationCharacterReferenceOptout = null!;
  private ecer_PortalInvitation testPortalInvitationWorkExperienceReferenceOptout = null!;
  private ecer_PortalInvitation testPortalInvitationWorkExperienceReferenceCompleted = null!;
  private Contact authenticatedBcscUser2 = null!;

  public IServiceProvider Services => serviceScope.ServiceProvider;
  public UserIdentity AuthenticatedBcscUserIdentity => authenticatedBcscUser.ecer_contact_ecer_authentication_455.Select(a => new UserIdentity(a.ecer_ExternalID, a.ecer_IdentityProvider)).First();
  public string AuthenticatedBcscUserId => authenticatedBcscUser.Id.ToString();

  public string communicationOneId => testCommunication1.Id.ToString();
  public string communicationTwoId => testCommunication2.Id.ToString();
  public string communicationThreeId => testCommunication3.Id.ToString();
  public string communicationFourId => testCommunication4.Id.ToString();
  public string inProgressApplicationId => inProgressTestApplication.Id.ToString();
  public string draftTestApplicationId => draftTestApplication.Id.ToString();

  public Guid portalInvitationOneId => testPortalInvitationOne.ecer_PortalInvitationId ?? Guid.Empty;
  public Guid portalInvitationCharacterReferenceIdSubmit => testPortalInvitationCharacterReferenceSubmit.ecer_PortalInvitationId ?? Guid.Empty;
  public Guid portalInvitationWorkExperienceReferenceIdSubmit => testPortalInvitationWorkExperienceReferenceSubmit.ecer_PortalInvitationId ?? Guid.Empty;
  public Guid portalInvitationCharacterReferenceIdOptout => testPortalInvitationCharacterReferenceOptout.ecer_PortalInvitationId ?? Guid.Empty;
  public Guid portalInvitationWorkExperienceReferenceIdOptout => testPortalInvitationWorkExperienceReferenceOptout.ecer_PortalInvitationId ?? Guid.Empty;
  public Guid portalInvitationWorkExperienceReferenceIdCompleted => testPortalInvitationWorkExperienceReferenceCompleted.ecer_PortalInvitationId ?? Guid.Empty;
  public UserIdentity AuthenticatedBcscUserIdentity2 => authenticatedBcscUser2.ecer_contact_ecer_authentication_455.Select(a => new UserIdentity(a.ecer_ExternalID, a.ecer_IdentityProvider)).First();
  public string AuthenticatedBcscUserId2 => authenticatedBcscUser2.Id.ToString();
  private ecer_Application inProgressTestApplication2 = null!;
  private ecer_Application draftTestApplication2 = null!;
  private ecer_Application draftTestApplication3 = null!;
  private ecer_Application submittedTestApplication = null!;
  private ecer_Application submittedTestApplication2 = null!;
  private ecer_Application submittedTestApplication3 = null!;
  private ecer_Application submittedTestApplication4 = null!;
  private ecer_WorkExperienceRef submittedTestApplicationWorkExperienceRef = null!;
  private ecer_WorkExperienceRef submittedTestApplicationWorkExperienceRef2 = null!;
  private ecer_CharacterReference submittedTestApplicationCharacterRef = null!;
  public string inprogressTestApplicationId2 => inProgressTestApplication2.Id.ToString();
  public string draftTestApplicationId2 => draftTestApplication2.Id.ToString();
  public string draftTestApplicationId3 => draftTestApplication3.Id.ToString();
  public string submittedTestApplicationId => submittedTestApplication.Id.ToString();
  public string submittedTestApplicationId2 => submittedTestApplication2.Id.ToString();
  public string submittedTestApplicationId3 => submittedTestApplication3.Id.ToString();
  public string submittedTestApplicationId4 => submittedTestApplication4.Id.ToString();
  public string submittedTestApplicationWorkExperienceRefId => submittedTestApplicationWorkExperienceRef.Id.ToString();
  public string submittedTestApplicationWorkExperienceRefId2 => submittedTestApplicationWorkExperienceRef2.Id.ToString();

  public string submittedTestApplicationCharacterRefId => submittedTestApplicationCharacterRef.Id.ToString();

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

    inProgressTestApplication = GetOrAddApplication(context, authenticatedBcscUser, ecer_Application_StatusCode.InProgress);
    inProgressTestApplication2 = GetOrAddApplication(context, authenticatedBcscUser, ecer_Application_StatusCode.InProgress);
    draftTestApplication = GetOrAddApplication(context, authenticatedBcscUser, ecer_Application_StatusCode.Draft);
    draftTestApplication2 = GetOrAddApplication(context, authenticatedBcscUser, ecer_Application_StatusCode.Draft);
    draftTestApplication3 = GetOrAddApplication(context, authenticatedBcscUser, ecer_Application_StatusCode.Draft);
    submittedTestApplication = GetOrAddApplication(context, authenticatedBcscUser, ecer_Application_StatusCode.Submitted);
    submittedTestApplication2 = GetOrAddApplication(context, authenticatedBcscUser, ecer_Application_StatusCode.Submitted);
    submittedTestApplication3 = GetOrAddApplication(context, authenticatedBcscUser, ecer_Application_StatusCode.Submitted);
    submittedTestApplication4 = GetOrAddApplication(context, authenticatedBcscUser, ecer_Application_StatusCode.Submitted);
    submittedTestApplicationWorkExperienceRef = AddWorkExperienceReferenceToApplication(context, submittedTestApplication);
    submittedTestApplicationWorkExperienceRef2 = AddWorkExperienceReferenceToApplication(context, submittedTestApplication2);
    submittedTestApplicationCharacterRef = AddCharacterReferenceToApplication(context, submittedTestApplication3);
    testCommunication1 = GetOrAddCommunication(context, inProgressTestApplication, "comm1", null);
    testCommunication2 = GetOrAddCommunication(context, inProgressTestApplication, "comm2", null);
    testCommunication3 = GetOrAddCommunication(context, inProgressTestApplication, "comm3", null);
    testCommunication4 = GetOrAddCommunication(context, inProgressTestApplication, "comm4", null);

    testPortalInvitationOne = GetOrAddPortalInvitation_CharacterReference(context, authenticatedBcscUser, "name1");
    testPortalInvitationCharacterReferenceSubmit = GetOrAddPortalInvitation_CharacterReference(context, authenticatedBcscUser, "name2");
    testPortalInvitationWorkExperienceReferenceSubmit = GetOrAddPortalInvitation_WorkExperienceReference(context, authenticatedBcscUser, "name3");
    testPortalInvitationCharacterReferenceOptout = GetOrAddPortalInvitation_CharacterReference(context, authenticatedBcscUser, "name4");
    testPortalInvitationWorkExperienceReferenceOptout = GetOrAddPortalInvitation_WorkExperienceReference(context, authenticatedBcscUser, "name5");
    testPortalInvitationWorkExperienceReferenceCompleted = GetOrAddPortalInvitation_WorkExperienceReference(context, authenticatedBcscUser, "name6");

    context.SaveChanges();

    CompletePortalInvitation_WorkExperienceReference(context, "name6");

    //load dependent properties
    context.Attach(authenticatedBcscUser);
    context.LoadProperty(authenticatedBcscUser, Contact.Fields.ecer_contact_ecer_authentication_455);

    //load user 2
    authenticatedBcscUser2 = GetOrAddApplicant(context, "bcsc", $"{TestRunId}_user2");
    inProgressTestApplication2 = GetOrAddApplication(context, authenticatedBcscUser2, ecer_Application_StatusCode.InProgress);
    draftTestApplication2 = GetOrAddApplication(context, authenticatedBcscUser2, ecer_Application_StatusCode.Draft);

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

  private ecer_WorkExperienceRef AddWorkExperienceReferenceToApplication(EcerContext context, ecer_Application application)
  {
    var wpGuid = Guid.NewGuid();

    var workexperienceReference = new ecer_WorkExperienceRef
    {
      Id = wpGuid,
      ecer_WorkExperienceRefId = wpGuid,
      ecer_Name = "Test name",
      ecer_FirstName = "Test firstname",
      ecer_LastName = "Test lastname",
      ecer_EmailAddress = "Work_Experience_Reference@example.com",
      ecer_PhoneNumber = "9999999999",
      ecer_StartDate = DateTime.Now,
      ecer_EndDate = DateTime.Now,
    };

    context.AddObject(workexperienceReference);
    context.AddLink(workexperienceReference, ecer_WorkExperienceRef.Fields.ecer_workexperienceref_Applicationid_ecer, application);

    return workexperienceReference;
  }

  private ecer_CharacterReference AddCharacterReferenceToApplication(EcerContext context, ecer_Application application)
  {
    var wpGuid = Guid.NewGuid();

    var characterReference = new ecer_CharacterReference
    {
      Id = wpGuid,
      ecer_CharacterReferenceId = wpGuid,
      ecer_Name = "Test name",
      ecer_FirstName = "Test firstname",
      ecer_LastName = "Test lastname",
      ecer_EmailAddress = "Character_Reference@example.com",
      ecer_PhoneNumber = "9999999999"
    };

    context.AddObject(characterReference);
    context.AddLink(characterReference, ecer_CharacterReference.Fields.ecer_characterreference_Applicationid, application);

    return characterReference;
  }

  private ecer_Communication GetOrAddCommunication(EcerContext context, ecer_Application application, string message, Guid? parentCommunicationId)
  {
    var communication = context.ecer_CommunicationSet.FirstOrDefault(c => c.ecer_Applicationid.Id == application.Id && c.ecer_Registrantid.Id == authenticatedBcscUser.Id && c.ecer_Message == message && c.StatusCode == ecer_Communication_StatusCode.NotifiedRecipient);

    if (communication == null)
    {
      communication = new ecer_Communication
      {
        Id = Guid.NewGuid(),
        ecer_Message = message,
        ecer_Acknowledged = false,
        StatusCode = ecer_Communication_StatusCode.NotifiedRecipient,
      };
      if (parentCommunicationId == null)
      {
        communication.ecer_IsRoot = true;
      }
      context.AddObject(communication);

      if (parentCommunicationId != null)
      {
        var parent = context.ecer_CommunicationSet.SingleOrDefault(d => d.ecer_CommunicationId == parentCommunicationId);
        var Referencingecer_communication_ParentCommunicationid = new Relationship(ecer_Communication.Fields.Referencingecer_communication_ParentCommunicationid)
        {
          PrimaryEntityRole = EntityRole.Referencing
        };
        context.AddLink(communication, Referencingecer_communication_ParentCommunicationid, parent);
      }
      context.AddLink(communication, ecer_Communication.Fields.ecer_communication_Applicationid, inProgressTestApplication);

#pragma warning disable S125 // Sections of code should not be commented out
      // Adding this statement causes duplicate key issue for all tests "Cannot insert duplicate key"
      //context.AddLink(authenticatedBcscUser, ecer_Communication.Fields.ecer_contact_ecer_communication_122, communication);
#pragma warning restore S125 // Sections of code should not be commented out
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
        ecer_EmailAddress = "reference_test@example.com"
      };

      var guid = Guid.NewGuid();
      portalInvitation = new ecer_PortalInvitation
      {
        Id = guid,
        ecer_PortalInvitationId = guid,
        ecer_Name = name,
        ecer_FirstName = "Test firstname",
        ecer_LastName = "Test lastname",
        ecer_EmailAddress = "test@example.com",
        StatusCode = ecer_PortalInvitation_StatusCode.Sent,
      };

      context.AddObject(characterReference);
      context.AddObject(portalInvitation);
      context.AddLink(portalInvitation, ecer_PortalInvitation.Fields.ecer_portalinvitation_CharacterReferenceId, characterReference);
      context.AddLink(portalInvitation, ecer_PortalInvitation.Fields.ecer_portalinvitation_ApplicantId, registrant);
      context.AddLink(portalInvitation, ecer_PortalInvitation.Fields.ecer_portalinvitation_ApplicationId, inProgressTestApplication);
    }

    return portalInvitation;
  }

  private void CompletePortalInvitation_WorkExperienceReference(EcerContext context, string name)
  {
    var portalInvitations = context.ecer_PortalInvitationSet
      .Where(p => p.ecer_ApplicantId != null &&
                  p.ecer_ApplicationId != null &&
                  p.ecer_Name == name &&
                  p.ecer_WorkExperienceReferenceId != null)
      .ToList();

    foreach (var portalInvitation in portalInvitations)
    {
      portalInvitation.StateCode = ecer_portalinvitation_statecode.Inactive;
      portalInvitation.StatusCode = ecer_PortalInvitation_StatusCode.Completed;
      context.UpdateObject(portalInvitation);
    }

    context.SaveChanges();
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
        ecer_EmailAddress = "reference_test@example.com",
      };

      var guid = Guid.NewGuid();
      portalInvitation = new ecer_PortalInvitation
      {
        Id = guid,
        ecer_PortalInvitationId = guid,
        ecer_Name = name,
        ecer_FirstName = "Test firstname",
        ecer_LastName = "Test lastname",
        ecer_EmailAddress = "test@example.com",
        StatusCode = ecer_PortalInvitation_StatusCode.Sent,
      };

      context.AddObject(workexperienceReference);
      context.AddObject(portalInvitation);
      context.AddLink(portalInvitation, ecer_PortalInvitation.Fields.ecer_portalinvitation_ApplicantId, registrant);
      context.AddLink(portalInvitation, ecer_PortalInvitation.Fields.ecer_portalinvitation_ApplicationId, inProgressTestApplication);
      context.AddLink(portalInvitation, ecer_PortalInvitation.Fields.ecer_portalinvitation_WorkExperienceRefId, workexperienceReference);
    }

    return portalInvitation;
  }
}
