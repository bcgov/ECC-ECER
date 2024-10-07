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
  public Contact AuthenticatedBcscUser { get; set; } = null!;

  public Contact AuthenticatedBcscUser2 { get; set; } = null!;

  private IServiceScope serviceScope = null!;
  private ecer_Application inProgressTestApplication = null!;
  private ecer_Application draftTestApplication = null!;
  private bcgov_DocumentUrl testDocument1 = null!;

  private ecer_Communication testCommunication1 = null!;
  private ecer_Communication testCommunication2 = null!;
  private ecer_Communication testCommunication3 = null!;
  private ecer_Communication testCommunication4 = null!;

  private ecer_Certificate testCertification = null!;

  private ecer_PortalInvitation testPortalInvitationOne = null!;
  private ecer_PortalInvitation testPortalInvitationCharacterReferenceSubmit = null!;
  private ecer_PortalInvitation testPortalInvitationWorkExperienceReferenceSubmit = null!;
  private ecer_PortalInvitation testPortalInvitationCharacterReferenceOptout = null!;
  private ecer_PortalInvitation testPortalInvitationWorkExperienceReferenceOptout = null!;
  private ecer_PortalInvitation testPortalInvitationWorkExperienceReferenceCompleted = null!;
  private ecer_PortalInvitation testPortalInvitation400HoursTypeWorkExperienceReferenceSubmit = null!;

  private ecer_PreviousName previousName = null!;

  public IServiceProvider Services => serviceScope.ServiceProvider;
  public UserIdentity AuthenticatedBcscUserIdentity => AuthenticatedBcscUser.ecer_contact_ecer_authentication_455.Select(a => new UserIdentity(a.ecer_ExternalID, a.ecer_IdentityProvider)).First();
  public string AuthenticatedBcscUserId => AuthenticatedBcscUser.Id.ToString();
  public string documentOneId => testDocument1.Id.ToString();
  public string communicationOneId => testCommunication1.Id.ToString();
  public string communicationTwoId => testCommunication2.Id.ToString();
  public string communicationThreeId => testCommunication3.Id.ToString();
  public string communicationFourId => testCommunication4.Id.ToString();
  public string inProgressApplicationId => inProgressTestApplication.Id.ToString();
  public string draftTestApplicationId => draftTestApplication.Id.ToString();

  public string certificationOneId => testCertification.Id.ToString();

  public Guid portalInvitationOneId => testPortalInvitationOne.ecer_PortalInvitationId ?? Guid.Empty;
  public Guid portalInvitationCharacterReferenceIdSubmit => testPortalInvitationCharacterReferenceSubmit.ecer_PortalInvitationId ?? Guid.Empty;
  public Guid portalInvitationWorkExperienceReferenceIdSubmit => testPortalInvitationWorkExperienceReferenceSubmit.ecer_PortalInvitationId ?? Guid.Empty;
  public Guid portalInvitationCharacterReferenceIdOptout => testPortalInvitationCharacterReferenceOptout.ecer_PortalInvitationId ?? Guid.Empty;
  public Guid portalInvitationWorkExperienceReferenceIdOptout => testPortalInvitationWorkExperienceReferenceOptout.ecer_PortalInvitationId ?? Guid.Empty;
  public Guid portalInvitationWorkExperienceReferenceIdCompleted => testPortalInvitationWorkExperienceReferenceCompleted.ecer_PortalInvitationId ?? Guid.Empty;

  public Guid portalInvitation400HoursTypeWorkExperienceReferenceIdSubmit => testPortalInvitation400HoursTypeWorkExperienceReferenceSubmit.ecer_PortalInvitationId ?? Guid.Empty;
  public UserIdentity AuthenticatedBcscUserIdentity2 => AuthenticatedBcscUser2.ecer_contact_ecer_authentication_455.Select(a => new UserIdentity(a.ecer_ExternalID, a.ecer_IdentityProvider)).First();
  public string AuthenticatedBcscUserId2 => AuthenticatedBcscUser2.Id.ToString();
  private ecer_Application inProgressTestApplication2 = null!;
  private ecer_Application draftTestApplication2 = null!;
  private ecer_Application draftTestApplication3 = null!;
  private ecer_Application draftTestApplication4 = null!;
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

  public string draftTestApplicationId4 => draftTestApplication4.Id.ToString();
  public string submittedTestApplicationId => submittedTestApplication.Id.ToString();
  public string submittedTestApplicationId2 => submittedTestApplication2.Id.ToString();
  public string submittedTestApplicationId3 => submittedTestApplication3.Id.ToString();
  public string submittedTestApplicationId4 => submittedTestApplication4.Id.ToString();
  public string submittedTestApplicationWorkExperienceRefId => submittedTestApplicationWorkExperienceRef.Id.ToString();
  public string submittedTestApplicationWorkExperienceRefId2 => submittedTestApplicationWorkExperienceRef2.Id.ToString();

  public string submittedTestApplicationCharacterRefId => submittedTestApplicationCharacterRef.Id.ToString();
  public string PreviousNameId => previousName.ecer_PreviousNameId!.Value.ToString();

  protected override void AddAuthorizationOptions(AuthorizationOptions opts)
  {
    ArgumentNullException.ThrowIfNull(opts);
    opts.AddPolicy("registry_user", new AuthorizationPolicyBuilder(opts.GetPolicy("registry_user")!).AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme).Build());
    opts.AddPolicy("registry_new_user", new AuthorizationPolicyBuilder(opts.GetPolicy("registry_new_user")!).AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme).Build());
    opts.AddPolicy("registry_unverified_user", new AuthorizationPolicyBuilder(opts.GetPolicy("registry_unverified_user")!).AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme).Build());
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
    serviceScope?.Dispose();
  }

  private async Task InitializeDataverseTestData(EcerContext context)
  {
    await Task.CompletedTask;

    AuthenticatedBcscUser = GetOrAddApplicant(context, "bcsc", $"{TestRunId}_user1");

    inProgressTestApplication = GetOrAddApplication(context, AuthenticatedBcscUser, ecer_Application_StatusCode.InProgress);
    inProgressTestApplication2 = GetOrAddApplication(context, AuthenticatedBcscUser, ecer_Application_StatusCode.InProgress);
    draftTestApplication = GetOrAddApplication(context, AuthenticatedBcscUser, ecer_Application_StatusCode.Draft);
    draftTestApplication2 = GetOrAddApplication(context, AuthenticatedBcscUser, ecer_Application_StatusCode.Draft);
    draftTestApplication3 = GetOrAddApplication(context, AuthenticatedBcscUser, ecer_Application_StatusCode.Draft);
    submittedTestApplication = GetOrAddApplication(context, AuthenticatedBcscUser, ecer_Application_StatusCode.Submitted);
    submittedTestApplication2 = GetOrAddApplication(context, AuthenticatedBcscUser, ecer_Application_StatusCode.Submitted);
    submittedTestApplication3 = GetOrAddApplication(context, AuthenticatedBcscUser, ecer_Application_StatusCode.Submitted);
    submittedTestApplication4 = GetOrAddApplication(context, AuthenticatedBcscUser, ecer_Application_StatusCode.Submitted);
    submittedTestApplicationWorkExperienceRef = AddWorkExperienceReferenceToApplication(context, submittedTestApplication);
    submittedTestApplicationWorkExperienceRef2 = AddWorkExperienceReferenceToApplication(context, submittedTestApplication2);
    submittedTestApplicationCharacterRef = AddCharacterReferenceToApplication(context, submittedTestApplication3);

    testDocument1 = GetOrAddDocument(context, AuthenticatedBcscUser, "https://example.com/document1.pdf");
    testCommunication1 = GetOrAddCommunication(context, inProgressTestApplication, "comm1", null);
    testCommunication2 = GetOrAddCommunication(context, inProgressTestApplication, "comm2", null);
    testCommunication3 = GetOrAddCommunication(context, inProgressTestApplication, "comm3", null);
    testCommunication4 = GetOrAddCommunication(context, inProgressTestApplication, "comm4", null);
    testCertification = GetOrAddCertification(context);

    previousName = GetOrAddPreviousName(context, AuthenticatedBcscUser);
    testPortalInvitationOne = GetOrAddPortalInvitation_CharacterReference(context, AuthenticatedBcscUser, "name1");
    testPortalInvitationCharacterReferenceSubmit = GetOrAddPortalInvitation_CharacterReference(context, AuthenticatedBcscUser, "name2");
    testPortalInvitationWorkExperienceReferenceSubmit = GetOrAddPortalInvitation_WorkExperienceReference(context, AuthenticatedBcscUser, "name3");
    testPortalInvitationCharacterReferenceOptout = GetOrAddPortalInvitation_CharacterReference(context, AuthenticatedBcscUser, "name4");
    testPortalInvitationWorkExperienceReferenceOptout = GetOrAddPortalInvitation_WorkExperienceReference(context, AuthenticatedBcscUser, "name5");
    testPortalInvitationWorkExperienceReferenceCompleted = GetOrAddPortalInvitation_WorkExperienceReference(context, AuthenticatedBcscUser, "name6");
    testPortalInvitation400HoursTypeWorkExperienceReferenceSubmit = GetOrAddPortalInvitation_400HoursTypeWorkExperienceReference(context, AuthenticatedBcscUser, "name7");

    context.SaveChanges();

    CompletePortalInvitation_WorkExperienceReference(context, "name6");

    //load dependent properties
    context.Attach(AuthenticatedBcscUser);
    context.LoadProperty(AuthenticatedBcscUser, Contact.Fields.ecer_contact_ecer_authentication_455);

    //load user 2
    AuthenticatedBcscUser2 = GetOrAddApplicant(context, "bcsc", $"{TestRunId}_user2");
    inProgressTestApplication2 = GetOrAddApplication(context, AuthenticatedBcscUser2, ecer_Application_StatusCode.InProgress);
    draftTestApplication2 = GetOrAddApplication(context, AuthenticatedBcscUser2, ecer_Application_StatusCode.Draft);

    context.SaveChanges();

    //load dependent properties
    context.Attach(AuthenticatedBcscUser2);
    context.LoadProperty(AuthenticatedBcscUser2, Contact.Fields.ecer_contact_ecer_authentication_455);
  }

  private bcgov_DocumentUrl GetOrAddDocument(EcerContext context, Contact registrant, string url)
  {
    var document = new bcgov_DocumentUrl
    {
      Id = Guid.NewGuid(),
      bcgov_FileName = "Test Document",
      bcgov_Url = url,
      StateCode = bcgov_documenturl_statecode.Active,
      StatusCode = bcgov_DocumentUrl_StatusCode.Active,
    };
    context.AddObject(document);
    context.AddLink(registrant, Contact.Fields.bcgov_contact_bcgov_documenturl, document);

    return document;
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
        FirstName = "test1",
        LastName = "test1",
        MiddleName = "test1",
        Address1_Telephone1 = "1234567890",
        EMailAddress1 = "test@test.com",
        ecer_IsVerified = true,
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

  private ecer_PreviousName GetOrAddPreviousName(EcerContext context, Contact applicant)
  {
    var existing = (from p in context.ecer_PreviousNameSet
                    where p.ecer_Contactid.Id == applicant.Id
                    select p).FirstOrDefault();

    if (existing == null)
    {
      existing = new ecer_PreviousName
      {
        Id = Guid.NewGuid(),
        ecer_Source = ecer_PreviousNameSources.Transcript,
        ecer_FirstName = "Previously",
        ecer_MiddleName = "I",
        ecer_LastName = "Was",
        ecer_PreferredName = "Longtimeago",
      };
      context.AddObject(existing);
      context.AddLink(existing, ecer_PreviousName.Fields.ecer_previousname_Contactid, applicant);
    }

    return existing;
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
      //ecer_Name = "Test name",
      ecer_FirstName = "autotest_firstname",
      ecer_LastName = "autotest_lastname",
      ecer_EmailAddress = "Work_Experience_Reference@test.gov.bc.ca",
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
      //ecer_Name = "Test name",
      ecer_FirstName = "autotest_firstname",
      ecer_LastName = "autotest_lastname",
      ecer_EmailAddress = "Character_Reference@test.gov.bc.ca",
      ecer_PhoneNumber = "9999999999"
    };

    context.AddObject(characterReference);
    context.AddLink(characterReference, ecer_CharacterReference.Fields.ecer_characterreference_Applicationid, application);

    return characterReference;
  }

  private ecer_Communication GetOrAddCommunication(EcerContext context, ecer_Application application, string message, Guid? parentCommunicationId)
  {
    var communication = context.ecer_CommunicationSet.FirstOrDefault(c => c.ecer_Applicationid.Id == application.Id && c.ecer_Registrantid.Id == AuthenticatedBcscUser.Id && c.ecer_Message == message && c.StatusCode == ecer_Communication_StatusCode.NotifiedRecipient);

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

  private ecer_Certificate GetOrAddCertification(EcerContext context)
  {
    var certification = context.ecer_CertificateSet.FirstOrDefault(c => c.ecer_CertificateNumber == "autotest_1234");

    if (certification == null)
    {
      certification = new ecer_Certificate
      {
        Id = Guid.NewGuid(),
        ecer_CertificateNumber = "autotest_1234",
        StatusCode = ecer_Certificate_StatusCode.Active,
        ecer_GenerateCertificate = true
      };
      context.AddObject(certification);

      var level = new ecer_CertifiedLevel
      {
        Id = Guid.NewGuid(),
      };
      context.AddObject(level);

      var type = context.ecer_CertificateTypeSet.First();

      context.AddLink(certification, ecer_Certificate.Fields.ecer_certifiedlevel_CertificateId, level);
      context.AddLink(level, ecer_CertifiedLevel.Fields.ecer_certifiedlevel_CertificateTypeId, type);
    }

    return certification;
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
        ecer_Name = name,
        ecer_FirstName = "autotest_charref_first",
        ecer_LastName = "autotest_charref_last",
        ecer_EmailAddress = "reference_test@test.gov.bc.ca"
      };

      var guid = Guid.NewGuid();
      portalInvitation = new ecer_PortalInvitation
      {
        Id = guid,
        ecer_PortalInvitationId = guid,
        ecer_Name = name,
        ecer_FirstName = "autotest_charref_first",
        ecer_LastName = "autotest_charref_last",
        ecer_EmailAddress = "reference_test@test.gov.bc.ca",
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

  private static void CompletePortalInvitation_WorkExperienceReference(EcerContext context, string name)
  {
    var portalInvitations = context.ecer_PortalInvitationSet
      .Where(p => p.ecer_ApplicantId != null &&
                  p.ecer_ApplicationId != null &&
                  p.ecer_Name == name &&
                  p.ecer_WorkExperienceReferenceId != null &&
                  p.StateCode == ecer_portalinvitation_statecode.Active)
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
        ecer_Name = name,
        ecer_FirstName = "autotest_workref_first",
        ecer_LastName = "autotest_workref_last",
        ecer_EmailAddress = "reference_test@test.gov.bc.ca",
      };

      var guid = Guid.NewGuid();
      portalInvitation = new ecer_PortalInvitation
      {
        Id = guid,
        ecer_PortalInvitationId = guid,
        ecer_Name = name,
        ecer_FirstName = "autotest_workref_first",
        ecer_LastName = "autotest_workref_last",
        ecer_EmailAddress = "reference_test@test.gov.bc.ca",
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

  private ecer_PortalInvitation GetOrAddPortalInvitation_400HoursTypeWorkExperienceReference(EcerContext context, Contact registrant, string name)
  {
    var portalInvitation = (from p in context.ecer_PortalInvitationSet
                            join w in context.ecer_WorkExperienceRefSet
                            on p.ecer_WorkExperienceReferenceId.Id equals w.ecer_WorkExperienceRefId
                            where p.ecer_ApplicantId != null &&
                                  p.ecer_ApplicationId != null &&
                                  p.ecer_Name == name &&
                                  p.ecer_WorkExperienceReferenceId != null &&
                                  p.StatusCode == ecer_PortalInvitation_StatusCode.Sent &&
                                  w.ecer_Type == ecer_WorkExperienceTypes._400Hours
                            select p).FirstOrDefault();

    if (portalInvitation == null)
    {
      var wpGuid = Guid.NewGuid();

      var workexperienceReference = new ecer_WorkExperienceRef
      {
        Id = wpGuid,
        ecer_WorkExperienceRefId = wpGuid,
        //ecer_Name = "autotest_Reference Test name",
        ecer_FirstName = "autotest_workref_first",
        ecer_LastName = "autotest_workref_last",
        ecer_EmailAddress = "reference_test@test.gov.bc.ca",
        ecer_Type = ecer_WorkExperienceTypes._400Hours, // 400 Hours Type work experience reference
      };

      var guid = Guid.NewGuid();
      portalInvitation = new ecer_PortalInvitation
      {
        Id = guid,
        ecer_PortalInvitationId = guid,
        //ecer_Name = name,
        ecer_FirstName = "autotest_workref_first",
        ecer_LastName = "autotest_workref_last",
        ecer_EmailAddress = "reference_test@test.gov.bc.ca",
        StatusCode = ecer_PortalInvitation_StatusCode.Sent,
      };

      context.AddObject(workexperienceReference);
      context.AddObject(portalInvitation);
      context.AddLink(portalInvitation, ecer_PortalInvitation.Fields.ecer_portalinvitation_ApplicantId, registrant);
      context.AddLink(portalInvitation, ecer_PortalInvitation.Fields.ecer_portalinvitation_ApplicationId, inProgressTestApplication2);
      context.AddLink(portalInvitation, ecer_PortalInvitation.Fields.ecer_portalinvitation_WorkExperienceRefId, workexperienceReference);
    }
    return portalInvitation;
  }
}