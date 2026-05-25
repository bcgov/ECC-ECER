using ECER.Utilities.DataverseSdk.Model;
using ECER.Utilities.DataverseSdk.Queries;
using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Shouldly;
using Xunit.Categories;

namespace ECER.Tests.Integration.Utilities.DataverseSdk;

[IntegrationTest]
public class QueryTests : IAsyncLifetime
{
  private readonly IConfigurationRoot configuration;
  private readonly string testRunKey = Guid.NewGuid().ToString("N")[..12];
  private ServiceClient serviceClient = null!;
  private EcerContext dataverseContext = null!;
  private Contact queryContact = null!;
  private Guid contactId;
  private Guid certificateId;
  private Guid professionalDevelopmentApplicationId;
  private Guid[] applicationIds = Array.Empty<Guid>();

  public QueryTests()
  {
    var configBuilder = new ConfigurationBuilder().AddUserSecrets(typeof(Clients.RegistryPortal.Server.Program).Assembly);
    var secretsFile = Environment.GetEnvironmentVariable("SECRETS_FILE_PATH");
    if (secretsFile != null && File.Exists(secretsFile)) configBuilder.AddJsonFile(secretsFile, true);
    configuration = configBuilder.Build();
  }

  [Fact]
  public void WhereIn_Filtered()
  {
    var statuses = new[] { ecer_Application_StatusCode.Draft, ecer_Application_StatusCode.InProgress };
    var query = dataverseContext.ecer_ApplicationSet
      .Where(a => a.ecer_application_Applicantid_contact.Id == contactId)
      .WhereIn(a => a.StatusCode!.Value, statuses)
      .ToList();

    query.Count.ShouldBe(2);
    query.ShouldAllBe(a => statuses.Contains(a.StatusCode!.Value));
  }

  [Fact]
  public void WhereNotIn_Filtered()
  {
    var statuses = new[] { ecer_Application_StatusCode.Draft, ecer_Application_StatusCode.InProgress };
    var query = dataverseContext.ecer_ApplicationSet
      .Where(a => a.ecer_application_Applicantid_contact.Id == contactId)
      .WhereNotIn(a => a.StatusCode!.Value, statuses)
      .ToList();

    query.Count.ShouldBe(2);
    query.ShouldAllBe(a => !statuses.Contains(a.StatusCode!.Value));
  }

  [Fact]
  public void Join_OneToMany_CompleteObject()
  {
    var query = dataverseContext.ecer_ApplicationSet.Where(a => a.ecer_application_Applicantid_contact.Id == contactId);

    var results = dataverseContext.From(query).Join().Include(a => a.ecer_application_Applicantid_contact).Execute();

    results.Count().ShouldBeGreaterThan(0);
    results.ShouldAllBe(a => a.ecer_application_Applicantid_contact.Id == contactId);
  }

  [Fact]
  public void Join_OneToMany_NestedObject()
  {
    var query = dataverseContext.ecer_CertificateSet.Where(a => a.ecer_CertificateId == certificateId);

    var results = dataverseContext.From(query).Join()
      .Include(a => a.ecer_certificate_Registrantid)
      .IncludeNested(a => a.ecer_certificateconditions_Registrantid)
      .Execute();

    var result = results.FirstOrDefault();
    result.ShouldNotBeNull();
    result!.ecer_certificate_Registrantid.ShouldNotBeNull();
    var output = result.ecer_certificate_Registrantid.ecer_certificateconditions_Registrantid;
    output.ShouldNotBeNull();
    output.ShouldNotBeEmpty();
  }

  [Theory]
  [InlineData(0, 2)]
  [InlineData(2, 2)]
  public void Join_OneToManyWithPaging_CorrectPageSize(int pageNumber, int pageSize)
  {
    var query = dataverseContext.ecer_ApplicationSet
      .WhereIn(c => c.Id, applicationIds)
      .OrderBy(a => a.Id)
      .Skip(pageNumber)
      .Take(pageSize);

    var results = dataverseContext
      .From(query)
      .Join()
      .Include(c => c.ecer_transcript_Applicationid)
      .Execute();

    results.Count().ShouldBe(pageSize);
    results.ShouldAllBe(r => applicationIds.Any(id => r.Id == id));
    results.ShouldAllBe(r => r.ecer_transcript_Applicationid.Any());
  }

  [Fact]
  public void Join_OneToMany_NestedObjectApplication()
  {
    var query = dataverseContext.ecer_ApplicationSet.Where(a => a.ecer_ApplicationId == professionalDevelopmentApplicationId);

    var results = dataverseContext.From(query).Join()
      .Include(a => a.ecer_ecer_professionaldevelopment_Applicationi)
      .IncludeNested(a => a.ecer_bcgov_documenturl_ProfessionalDevelopmentId)
      .Execute();

    var result = results.FirstOrDefault();
    result.ShouldNotBeNull();
    result!.ecer_ecer_professionaldevelopment_Applicationi.ShouldNotBeNull();

    foreach (var item in result.ecer_ecer_professionaldevelopment_Applicationi)
    {
      item.ecer_bcgov_documenturl_ProfessionalDevelopmentId.ShouldNotBeNull();
      item.ecer_bcgov_documenturl_ProfessionalDevelopmentId.ShouldNotBeEmpty();
    }
  }

  [Fact]
  public void Aggregate_Count_Returned()
  {
    var query = dataverseContext.ecer_ApplicationSet.Where(c => c.ecer_application_Applicantid_contact.ContactId == contactId);

    var count = dataverseContext.From(query).Aggregate().Count();

    count.ShouldBe(applicationIds.Length);
  }

  [Fact]
  public void Execute_SimpleQuery_Returned()
  {
    var query = dataverseContext.ecer_ApplicationSet.Where(a => a.ecer_application_Applicantid_contact.Id == contactId);

    var results = dataverseContext.From(query).Execute();

    results.Count().ShouldBe(applicationIds.Length);
    results.ShouldAllBe(r => r.Id != Guid.Empty);
  }

  public async Task InitializeAsync()
  {
    await Task.CompletedTask;
    serviceClient = new ServiceClient(configuration.GetValue<string>("Dataverse:ConnectionString"));
    dataverseContext = new EcerContext(serviceClient);
    SeedTestData();
  }

  public async Task DisposeAsync()
  {
    await Task.CompletedTask;

    CleanupTestData();
    dataverseContext.Dispose();
    serviceClient.Dispose();
  }

  private void SeedTestData()
  {
    queryContact = new Contact
    {
      Id = Guid.NewGuid(),
      FirstName = "Query",
      LastName = $"TEST_{testRunKey}",
      MiddleName = "Tests",
      Address1_Telephone1 = "1234567890",
      EMailAddress1 = $"{testRunKey}@example.invalid",
      BirthDate = new DateTime(2000, 3, 15),
      ecer_IsVerified = true,
      StatusCode = Contact_StatusCode.Verified,
    };
    queryContact.ContactId = queryContact.Id;
    contactId = queryContact.Id;
    dataverseContext.AddObject(queryContact);

    applicationIds =
    [
      CreateApplication(ecer_Application_StatusCode.Draft),
      CreateApplication(ecer_Application_StatusCode.InProgress),
      CreateApplication(ecer_Application_StatusCode.Submitted),
      CreateApplication(ecer_Application_StatusCode.Submitted)
    ];
    professionalDevelopmentApplicationId = applicationIds[0];

    CreateCertificateWithConditions();
    dataverseContext.SaveChanges();
  }

  private Guid CreateApplication(ecer_Application_StatusCode status)
  {
    var applicationId = Guid.NewGuid();
    var application = new ecer_Application
    {
      Id = applicationId,
      ecer_ApplicationId = applicationId,
      ecer_isECE5YR = true,
      ecer_CertificateType = "ECE 5 YR",
      StatusCode = status,
      StateCode = ecer_application_statecode.Active,
    };

    dataverseContext.AddObject(application);
    dataverseContext.AddLink(application, ecer_Application.Fields.ecer_application_Applicantid_contact, queryContact);

    var transcriptId = Guid.NewGuid();
    var transcript = new ecer_Transcript
    {
      Id = transcriptId,
      ecer_TranscriptId = transcriptId,
      ecer_StartDate = DateTime.UtcNow.AddYears(-1),
      ecer_EndDate = DateTime.UtcNow.AddMonths(-1),
      ecer_ProgramCourseName = "ECE Foundations",
      ecer_CampusLocation = "Victoria",
      ecer_StudentFirstName = "Query",
      ecer_StudentLastName = "Student",
      ecer_StudentNumber = transcriptId.ToString("N")[..8],
      ecer_EducationInstitutionFullName = "Query Test Institute",
      ecer_IsOfficialTranscriptRequested = true,
      ecer_IsNameUnverified = false,
      ecer_IsECEAssistant = false,
      ecer_EducationRecognition = ecer_EducationRecognition.Recognized,
      ecer_EducationOrigin = ecer_EducationOrigin.InsideBC,
      StatusCode = ecer_Transcript_StatusCode.Draft,
      StateCode = ecer_transcript_statecode.Active,
    };

    dataverseContext.AddObject(transcript);
    dataverseContext.AddLink(application, ecer_Transcript.Fields.ecer_transcript_Applicationid, transcript);
    dataverseContext.AddLink(queryContact, ecer_Transcript.Fields.ecer_transcript_Applicantid_Contact, transcript);

    if (status == ecer_Application_StatusCode.Draft)
    {
      var professionalDevelopmentId = Guid.NewGuid();
      var professionalDevelopment = new ecer_ProfessionalDevelopment
      {
        Id = professionalDevelopmentId,
        ecer_ProfessionalDevelopmentId = professionalDevelopmentId,
        ecer_CourseName = "Inclusive Practice",
        ecer_OrganizationName = "Query Test Org",
        ecer_StartDate = DateTime.UtcNow.AddMonths(-2),
        ecer_EndDate = DateTime.UtcNow.AddMonths(-1),
        StatusCode = ecer_ProfessionalDevelopment_StatusCode.Draft,
        StateCode = ecer_professionaldevelopment_statecode.Active,
      };

      dataverseContext.AddObject(professionalDevelopment);
      dataverseContext.AddLink(application, ecer_Application.Fields.ecer_ecer_professionaldevelopment_Applicationi, professionalDevelopment);
      dataverseContext.AddLink(queryContact, Contact.Fields.ecer_ecer_professionaldevelopment_Applicantid_, professionalDevelopment);

      var documentId = Guid.NewGuid();
      var document = new bcgov_DocumentUrl
      {
        Id = documentId,
        bcgov_DocumentUrlId = documentId,
        bcgov_FileName = "query-test.pdf",
        bcgov_FileExtension = ".pdf",
        bcgov_FileSize = "1234",
        bcgov_Url = $"https://example.invalid/{documentId:N}",
        ecer_Tag1 = "query-test",
        ecer_ApplicationName = "Registry",
        StateCode = bcgov_documenturl_statecode.Active,
        StatusCode = bcgov_DocumentUrl_StatusCode.Active,
      };

      dataverseContext.AddObject(document);
      dataverseContext.AddLink(queryContact, Contact.Fields.bcgov_contact_bcgov_documenturl, document);
      dataverseContext.AddLink(document, bcgov_DocumentUrl.Fields.ecer_bcgov_documenturl_ProfessionalDevelopmentId, professionalDevelopment);
    }

    return applicationId;
  }

  private void CreateCertificateWithConditions()
  {
    certificateId = Guid.NewGuid();
    var certificate = new ecer_Certificate
    {
      Id = certificateId,
      ecer_CertificateId = certificateId,
      ecer_CertificateNumber = $"cert_{testRunKey}",
      ecer_HasConditions = true,
      ecer_GenerateCertificate = true,
      ecer_ExpiryDate = DateTime.UtcNow.AddMonths(6),
      StatusCode = ecer_Certificate_StatusCode.Active,
      StateCode = ecer_certificate_statecode.Active,
    };

    dataverseContext.AddObject(certificate);
    dataverseContext.AddLink(certificate, ecer_Certificate.Fields.ecer_certificate_Registrantid, queryContact);

    var certificateType = dataverseContext.ecer_CertificateTypeSet.First();
    var certifiedLevelId = Guid.NewGuid();
    var certifiedLevel = new ecer_CertifiedLevel
    {
      Id = certifiedLevelId,
      ecer_CertifiedLevelId = certifiedLevelId,
    };

    dataverseContext.AddObject(certifiedLevel);
    dataverseContext.AddLink(certificate, ecer_Certificate.Fields.ecer_certifiedlevel_CertificateId, certifiedLevel);
    dataverseContext.AddLink(certifiedLevel, ecer_CertifiedLevel.Fields.ecer_certifiedlevel_CertificateTypeId, certificateType);

    var conditionId = Guid.NewGuid();
    var condition = new ecer_CertificateConditions
    {
      Id = conditionId,
      ecer_CertificateConditionsId = conditionId,
      ecer_Name = $"condition_{testRunKey}",
      ecer_Details = "Query test condition",
      ecer_StartDate = DateTime.UtcNow.AddDays(-7),
      ecer_EndDate = DateTime.UtcNow.AddDays(30),
      StatusCode = ecer_CertificateConditions_StatusCode.Active,
      StateCode = ecer_certificateconditions_statecode.Active,
    };

    dataverseContext.AddObject(condition);
    dataverseContext.AddLink(condition, ecer_CertificateConditions.Fields.ecer_certificateconditions_CertificateId, certificate);
    dataverseContext.AddLink(condition, ecer_CertificateConditions.Fields.ecer_certificateconditions_Registrantid, queryContact);
  }

  private void CleanupTestData()
  {
    if (queryContact == null)
    {
      return;
    }

    try
    {
      dataverseContext.Execute(new ecer_CLEANUPDeleteContactApplicationsActionRequest
      {
        ContactID = queryContact.Id.ToString(),
        Target = new EntityReference(queryContact.LogicalName, queryContact.Id)
      });

      dataverseContext.Execute(new ecer_CLEANUPDeleteContactActionRequest
      {
        ContactID = queryContact.Id.ToString(),
        Target = new EntityReference(queryContact.LogicalName, queryContact.Id)
      });
    }
    catch
    {
      // Best-effort cleanup so disposal does not mask test results.
    }
  }
}
