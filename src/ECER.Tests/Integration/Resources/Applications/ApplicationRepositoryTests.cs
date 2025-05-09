﻿using Alba;
using Amazon;
using AutoMapper;
using Bogus;
using Bogus.DataSets;
using ECER.Resources.Documents.Applications;
using ECER.Resources.Documents.MetadataResources;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit.Abstractions;
using Xunit.Categories;
using static StackExchange.Redis.Role;
using Application = ECER.Resources.Documents.Applications.Application;
using ApplicationStatus = ECER.Resources.Documents.Applications.ApplicationStatus;
using CertificationType = ECER.Resources.Documents.Applications.CertificationType;
using CharacterReference = ECER.Resources.Documents.Applications.CharacterReference;
using Transcript = ECER.Resources.Documents.Applications.Transcript;
using WorkExperienceReference = ECER.Resources.Documents.Applications.WorkExperienceReference;
using System.Net.Http.Headers;

namespace ECER.Tests.Integration.Resources.Applications;

[IntegrationTest]
public class ApplicationRepositoryTests : RegistryPortalWebAppScenarioBase
{
  private readonly IApplicationRepository repository;
  private readonly IMapper mapper;

  public ApplicationRepositoryTests(ITestOutputHelper output, RegistryPortalWebAppFixture fixture) : base(output, fixture)
  {
    repository = Fixture.Services.GetRequiredService<IApplicationRepository>();
    mapper = Fixture.Services.GetRequiredService<IMapper>();
  }

  [Fact]
  public async Task SaveDraftApplication_New_Created()
  {
    var applicantId = Fixture.AuthenticatedBcscUserId;
    var applicationId = await repository.SaveApplication(new Application(null, applicantId, new[] { CertificationType.OneYear }), CancellationToken.None);

    applicationId.ShouldNotBeNull();

    var application = (await repository.Query(new ApplicationQuery { ById = applicationId }, default)).ShouldHaveSingleItem();
    application.Status.ShouldBe(ApplicationStatus.Draft);
    application.ApplicantId.ShouldBe(applicantId);
    application.CertificationTypes.ShouldBe(new[] { CertificationType.OneYear });
    application.SignedDate.ShouldBeNull();
  }

  [Fact]
  public async Task SaveDraftApplication_Existing_Updated()
  {
    var applicantId = Fixture.AuthenticatedBcscUserId;
    var newApplicationId = await repository.SaveApplication(new Application(null, applicantId, new[] { CertificationType.OneYear }), CancellationToken.None);
    var existingApplicationId = await repository.SaveApplication(new Application(newApplicationId, applicantId, new[] { CertificationType.OneYear, CertificationType.FiveYears }), CancellationToken.None);

    existingApplicationId.ShouldBe(newApplicationId);

    var application = (await repository.Query(new ApplicationQuery { ById = existingApplicationId }, default)).ShouldHaveSingleItem();
    application.Status.ShouldBe(ApplicationStatus.Draft);
    application.ApplicantId.ShouldBe(applicantId);
    application.CertificationTypes.ShouldBe(new[] { CertificationType.OneYear, CertificationType.FiveYears });
    application.SignedDate.ShouldBeNull();
  }

  [Fact]
  public async Task SaveDraftApplication_ExistingSigned_Updated()
  {
    var applicantId = Fixture.AuthenticatedBcscUserId;
    var newApplicationId = await repository.SaveApplication(new Application(null, applicantId, new[] { CertificationType.OneYear }), CancellationToken.None);
    var application = new Application(newApplicationId, applicantId, new[] { CertificationType.OneYear });
    application.SignedDate = DateTime.Now;
    var existingApplicationId = await repository.SaveApplication(application, CancellationToken.None);

    existingApplicationId.ShouldBe(newApplicationId);

    var existingApplication = (await repository.Query(new ApplicationQuery { ById = existingApplicationId }, default)).ShouldHaveSingleItem();
    existingApplication.Status.ShouldBe(ApplicationStatus.Draft);
    existingApplication.ApplicantId.ShouldBe(applicantId);
    existingApplication.SignedDate.ShouldNotBeNull();
  }

  [Fact]
  public async Task SaveDraftApplication_ExistingShouldNotUpdateSignedDate_Updated()
  {
    var today = DateTime.Today;
    var oneWeekAgo = today - TimeSpan.FromDays(7);

    var applicantId = Fixture.AuthenticatedBcscUserId;
    var newApplication = new Application(null, applicantId, new[] { CertificationType.OneYear });
    newApplication.SignedDate = oneWeekAgo;
    var newApplicationId = await repository.SaveApplication(newApplication, CancellationToken.None);
    var existingApplication = new Application(newApplicationId, applicantId, new[] { CertificationType.OneYear });
    existingApplication.SignedDate = today;
    var existingApplicationId = await repository.SaveApplication(existingApplication, CancellationToken.None);

    existingApplicationId.ShouldBe(newApplicationId);

    var application = (await repository.Query(new ApplicationQuery { ById = existingApplicationId }, default)).ShouldHaveSingleItem();
    application.SignedDate.ShouldBe(oneWeekAgo);
  }

  [Fact]
  public async Task QueryApplications_ByApplicantId_Found()
  {
    var applicantId = Fixture.AuthenticatedBcscUserId;
    await repository.SaveApplication(new Application(null, applicantId, new[] { CertificationType.OneYear }), CancellationToken.None);

    var applications = await repository.Query(new ApplicationQuery { ByApplicantId = applicantId }, default);
    applications.ShouldNotBeEmpty();
    applications.ShouldBeAssignableTo<IEnumerable<Application>>()!.ShouldAllBe(ca => ca.ApplicantId == applicantId);
  }

  [Fact]
  public async Task QueryApplications_ByApplictionId_Found()
  {
    var applicantId = Fixture.AuthenticatedBcscUserId;
    var applicationId = await repository.SaveApplication(new Application(null, applicantId, new[] { CertificationType.OneYear })
    {
      Transcripts = new List<Transcript> { CreateTranscript() }
    }, CancellationToken.None);

    var applications = await repository.Query(new ApplicationQuery { ById = applicationId }, default);
    var application = applications.ShouldHaveSingleItem();
    application.Transcripts.ShouldHaveSingleItem();
    application.ApplicantId.ShouldBe(applicantId);
  }

  [Fact]
  public async Task QueryApplications_ByApplicantIdAndStatus_Found()
  {
    var applicantId = Fixture.AuthenticatedBcscUserId;
    var statuses = new[] { ApplicationStatus.Draft, ApplicationStatus.Complete };
    await repository.SaveApplication(new Application(null, applicantId, new[] { CertificationType.OneYear }), CancellationToken.None);

    var applications = await repository.Query(new ApplicationQuery { ByApplicantId = applicantId, ByStatus = statuses }, default);
    applications.ShouldNotBeEmpty();
    applications.ShouldBeAssignableTo<IEnumerable<Application>>()!.ShouldAllBe(ca => ca.ApplicantId == applicantId && statuses.Contains(ca.Status));
  }

  [Fact]
  public async Task SaveDraftApplication_WithTranscripts_Created()
  {
    var applicantId = Fixture.AuthenticatedBcscUserId;
    var transcripts = new List<Transcript>
    {
        CreateTranscript()
    };
    var application = new Application(null, applicantId, new[] { CertificationType.OneYear })
    {
      Transcripts = transcripts
    };
    var applicationId = await repository.SaveApplication(application, CancellationToken.None);
    applicationId.ShouldNotBeNull();
    var query = await repository.Query(new ApplicationQuery { ById = applicationId }, default);
    var savedApplication = query.ShouldHaveSingleItem();
    savedApplication.Transcripts.Count().ShouldBe(transcripts.Count);
  }

  [Fact]
  public async Task SaveDraftApplication_WithProfessionalDevelopments_Created()
  {
    var applicantId = Fixture.AuthenticatedBcscUserId;
    var professionalDevelopments = new List<ProfessionalDevelopment>
    {
        CreateProfessionalDevelopment()
    };
    var application = new Application(null, applicantId, new[] { CertificationType.OneYear })
    {
      ProfessionalDevelopments = professionalDevelopments
    };
    var applicationId = await repository.SaveApplication(application, CancellationToken.None);
    applicationId.ShouldNotBeNull();
    var query = await repository.Query(new ApplicationQuery { ById = applicationId }, default);
    var savedApplication = query.ShouldHaveSingleItem();
    savedApplication.ProfessionalDevelopments.Count().ShouldBe(professionalDevelopments.Count);
  }

  [Fact]
  public async Task UpdateApplication_WithModifiedTranscripts_Updated()
  {
    var applicantId = Fixture.AuthenticatedBcscUserId;
    var originalTranscripts = new List<Transcript> {
        CreateTranscript()
    };
    var application = new Application(null, applicantId, new[] { CertificationType.OneYear });
    application.Transcripts = originalTranscripts;
    var applicationId = await repository.SaveApplication(application, CancellationToken.None);

    var query = await repository.Query(new ApplicationQuery { ById = applicationId }, default);
    var transcript = query.First().Transcripts.First();
    transcript.CampusLocation = "Updated Campus";

    var updatedTranscripts = new List<Transcript> { transcript };
    application = new Application(applicationId, applicantId, new[] { CertificationType.OneYear });
    application.Transcripts = updatedTranscripts;
    await repository.SaveApplication(application, CancellationToken.None);

    var updatedApplication = (await repository.Query(new ApplicationQuery { ById = applicationId }, default)).ShouldHaveSingleItem();
    updatedApplication.Transcripts.First().CampusLocation.ShouldBe("Updated Campus");
  }

  [Fact]
  public async Task UpdateApplication_RemoveTranscripts_Updated()
  {
    var applicantId = Fixture.AuthenticatedBcscUserId;
    var originalTranscripts = new List<Transcript> {
        CreateTranscript()
    };
    var application = new Application(null, applicantId, new[] { CertificationType.OneYear });
    application.Transcripts = originalTranscripts;
    var applicationId = await repository.SaveApplication(application, CancellationToken.None);

    // Update application with empty transcripts list
    application = new Application(applicationId, applicantId, new[] { CertificationType.OneYear });
    application.Transcripts = new List<Transcript>();
    await repository.SaveApplication(application, CancellationToken.None);

    var updatedApplication = (await repository.Query(new ApplicationQuery { ById = applicationId }, default)).ShouldHaveSingleItem();
    updatedApplication.Transcripts.ShouldBeEmpty();
  }

  [Fact]
  public async Task SaveDraftApplication_WithCharacterReferences_Created()
  {
    var applicantId = Fixture.AuthenticatedBcscUserId;
    var characterReferences = new List<CharacterReference>
    {
        CreateCharacterReference()
    };
    var application = new Application(null, applicantId, new[] { CertificationType.OneYear })
    {
      CharacterReferences = characterReferences
    };
    var applicationId = await repository.SaveApplication(application, CancellationToken.None);
    applicationId.ShouldNotBeNull();
    var query = await repository.Query(new ApplicationQuery { ById = applicationId }, default);
    var savedApplication = query.ShouldHaveSingleItem();
    savedApplication.CharacterReferences.Count().ShouldBe(characterReferences.Count);
  }

  [Fact]
  public async Task UpdateApplication_WithModifiedCharacterReferences_Updated()
  {
    var applicantId = Fixture.AuthenticatedBcscUserId;
    var originalCharacterReferences = new List<CharacterReference> {
      CreateCharacterReference()
    };
    var application = new Application(null, applicantId, new[] { CertificationType.OneYear });
    application.CharacterReferences = originalCharacterReferences;
    var applicationId = await repository.SaveApplication(application, CancellationToken.None);

    var query = await repository.Query(new ApplicationQuery { ById = applicationId }, default);
    var characterReference = query.First().CharacterReferences.First();

    var newCharacterReference = new CharacterReference("Roberto", "Firmino", characterReference.PhoneNumber, characterReference.EmailAddress) { Id = characterReference.Id };

    var updatedCharacterReferences = new List<CharacterReference> { newCharacterReference };
    application = new Application(applicationId, applicantId, new[] { CertificationType.OneYear });
    application.CharacterReferences = updatedCharacterReferences;
    await repository.SaveApplication(application, CancellationToken.None);

    var updatedApplication = (await repository.Query(new ApplicationQuery { ById = applicationId }, default)).ShouldHaveSingleItem();
    updatedApplication.CharacterReferences.First().FirstName.ShouldBe("Roberto");
    updatedApplication.CharacterReferences.First().LastName.ShouldBe("Firmino");
  }

  [Fact]
  public async Task UpdateApplication_WithModifiedProfessionalDevelopments_Updated()
  {
    var applicantId = Fixture.AuthenticatedBcscUserId;
    var originalProfessionalDevelopments = new List<ProfessionalDevelopment> {
        CreateProfessionalDevelopment()
    };
    var application = new Application(null, applicantId, new[] { CertificationType.OneYear });
    application.ProfessionalDevelopments = originalProfessionalDevelopments;
    var applicationId = await repository.SaveApplication(application, CancellationToken.None);

    var query = await repository.Query(new ApplicationQuery { ById = applicationId }, default);
    var professionalDevelopment = query.First().ProfessionalDevelopments.First();
    professionalDevelopment.OrganizationContactInformation = "Updated OrganizationContactInformation";

    var updatedProfessionalDevelopments = new List<ProfessionalDevelopment> { professionalDevelopment };
    application = new Application(applicationId, applicantId, new[] { CertificationType.OneYear });
    application.ProfessionalDevelopments = updatedProfessionalDevelopments;
    await repository.SaveApplication(application, CancellationToken.None);

    var updatedApplication = (await repository.Query(new ApplicationQuery { ById = applicationId }, default)).ShouldHaveSingleItem();
    updatedApplication.ProfessionalDevelopments.First().OrganizationContactInformation.ShouldBe("Updated OrganizationContactInformation");
  }

  [Fact]
  public async Task UpdateApplication_RemoveCharacterReferences_Updated()
  {
    var applicantId = Fixture.AuthenticatedBcscUserId;
    var originalCharacterReferences = new List<CharacterReference> {
      CreateCharacterReference()
    };
    var application = new Application(null, applicantId, new[] { CertificationType.OneYear });
    application.CharacterReferences = originalCharacterReferences;
    var applicationId = await repository.SaveApplication(application, CancellationToken.None);

    // Update application with empty character reference list
    application = new Application(applicationId, applicantId, new[] { CertificationType.OneYear });
    application.CharacterReferences = new List<CharacterReference>();
    await repository.SaveApplication(application, CancellationToken.None);

    var updatedApplication = (await repository.Query(new ApplicationQuery { ById = applicationId }, default)).ShouldHaveSingleItem();
    updatedApplication.CharacterReferences.ShouldBeEmpty();
  }

  [Fact]
  public async Task SaveDraftApplication_WithWorkExperienceReferences_Created()
  {
    var applicantId = Fixture.AuthenticatedBcscUserId;
    var workExperienceReferences = new List<WorkExperienceReference>
    {
        CreateWorkExperienceReference()
    };
    var application = new Application(null, applicantId, new[] { CertificationType.OneYear })
    {
      WorkExperienceReferences = workExperienceReferences
    };
    var applicationId = await repository.SaveApplication(application, CancellationToken.None);
    applicationId.ShouldNotBeNull();
    var query = await repository.Query(new ApplicationQuery { ById = applicationId }, default);
    var savedApplication = query.ShouldHaveSingleItem();
    savedApplication.WorkExperienceReferences.Count().ShouldBe(workExperienceReferences.Count);
  }

  [Fact]
  public async Task UpdateApplication_WithModifiedWorkExperienceReferences_Updated()
  {
    var applicantId = Fixture.AuthenticatedBcscUserId;
    var originalWorkExperienceReferences = new List<WorkExperienceReference> {
        CreateWorkExperienceReference()
  };
    var application = new Application(null, applicantId, new[] { CertificationType.OneYear });
    application.WorkExperienceReferences = originalWorkExperienceReferences;
    var applicationId = await repository.SaveApplication(application, CancellationToken.None);

    var query = await repository.Query(new ApplicationQuery { ById = applicationId }, default);
    var reference = query.First().WorkExperienceReferences.First();

    var newWorkExperienceReference = new WorkExperienceReference(reference.FirstName, reference.LastName, reference.EmailAddress, reference.Hours) { Id = reference.Id, PhoneNumber = "987-654-3210" };

    var updatedReferences = new List<WorkExperienceReference> { newWorkExperienceReference };
    application = new Application(applicationId, applicantId, new[] { CertificationType.OneYear });
    application.WorkExperienceReferences = updatedReferences;
    await repository.SaveApplication(application, CancellationToken.None);

    var updatedApplication = (await repository.Query(new ApplicationQuery { ById = applicationId }, default)).ShouldHaveSingleItem();
    updatedApplication.WorkExperienceReferences.First().PhoneNumber.ShouldBe("987-654-3210");
  }

  [Fact]
  public async Task UpdateApplication_RemoveProfessionalDevelopments_Updated()
  {
    var applicantId = Fixture.AuthenticatedBcscUserId;
    var originalProfessionalDevelopments = new List<ProfessionalDevelopment> {
        CreateProfessionalDevelopment()
    };
    var application = new Application(null, applicantId, new[] { CertificationType.OneYear });
    application.ProfessionalDevelopments = originalProfessionalDevelopments;
    var applicationId = await repository.SaveApplication(application, CancellationToken.None);

    // Update application with empty professional developments list
    application = new Application(applicationId, applicantId, new[] { CertificationType.OneYear });
    application.ProfessionalDevelopments = new List<ProfessionalDevelopment>();
    await repository.SaveApplication(application, CancellationToken.None);

    var updatedApplication = (await repository.Query(new ApplicationQuery { ById = applicationId }, default)).ShouldHaveSingleItem();
    updatedApplication.ProfessionalDevelopments.ShouldBeEmpty();
  }

  [Fact]
  public async Task UpdateApplication_RemoveWorkExperienceReferences_Updated()
  {
    var applicantId = Fixture.AuthenticatedBcscUserId;
    var originalWorkExperienceReferences = new List<WorkExperienceReference> {
        CreateWorkExperienceReference()
  };
    var application = new Application(null, applicantId, new[] { CertificationType.OneYear });
    application.WorkExperienceReferences = originalWorkExperienceReferences;
    var applicationId = await repository.SaveApplication(application, CancellationToken.None);

    // Update application with empty work experience references list
    application = new Application(applicationId, applicantId, new[] { CertificationType.OneYear });
    application.WorkExperienceReferences = new List<WorkExperienceReference>();
    await repository.SaveApplication(application, CancellationToken.None);

    var updatedApplication = (await repository.Query(new ApplicationQuery { ById = applicationId }, default)).ShouldHaveSingleItem();
    updatedApplication.WorkExperienceReferences.ShouldBeEmpty();
  }

  [Fact]
  public async Task SubmitApplication_QueryShouldReturnStatusSubmitted()
  {
    var applicantId = Fixture.AuthenticatedBcscUserId;
    var application = new Application(null, applicantId, new[] { CertificationType.OneYear })
    {
      WorkExperienceReferences = new List<WorkExperienceReference>
      {
        CreateWorkExperienceReference()
      },
      CharacterReferences = new List<CharacterReference>
      {
        CreateCharacterReference()
      },
      Transcripts = new List<Transcript>
      {
        CreateTranscript()
      }
    };
    var savedApplicationId = await repository.SaveApplication(application, CancellationToken.None);

    await repository.Submit(savedApplicationId, CancellationToken.None);

    var submittedApplication = (await repository.Query(new ApplicationQuery { ById = savedApplicationId }, default)).ShouldHaveSingleItem();
    submittedApplication.Status.ShouldBe(ApplicationStatus.Submitted);
  }

  [Fact]
  public async Task SubmitApplicationNotInDraft_ShouldThrowInvalidOperationException()
  {
    var applicantId = Fixture.AuthenticatedBcscUserId;
    var application = new Application(null, applicantId, new[] { CertificationType.OneYear }) { };
    application.Status = ApplicationStatus.Submitted;
    var savedApplicationId = await repository.SaveApplication(application, CancellationToken.None);

    await Assert.ThrowsAsync<InvalidOperationException>(async () => await repository.Submit(savedApplicationId, CancellationToken.None));
  }

  [Fact]
  public async Task CancelApplication_QueryShouldnotReturnResults_ThenTryToCancelAgain_ShouldThrowInvalidOperationException()
  {
    var applicantId = Fixture.AuthenticatedBcscUserId2;
    var application = new Application(null, applicantId, new[] { CertificationType.OneYear });
    var savedApplicationId = await repository.SaveApplication(application, CancellationToken.None);

    await repository.Cancel(savedApplicationId, CancellationToken.None);

    (await repository.Query(new ApplicationQuery { ById = savedApplicationId }, default)).ShouldBeEmpty();

    await Assert.ThrowsAsync<InvalidOperationException>(async () => await repository.Cancel(savedApplicationId, CancellationToken.None));
  }

  [Fact]
  public async Task SaveApplicationTranscripts_ShouldSaveSuccessfully()
  {
    var applicantId = Fixture.AuthenticatedBcscUserId;
    var transcript = CreateTranscript();
    var applicationId = await repository.SaveApplication(new Application(null, applicantId, new[] { CertificationType.OneYear })
    {
      Transcripts = new List<Transcript> { transcript }
    }, CancellationToken.None);

    var applications = await repository.Query(new ApplicationQuery { ById = applicationId }, default);
    var application = applications.ShouldHaveSingleItem();
    application.Transcripts.ShouldHaveSingleItem();
    transcript = application.Transcripts.FirstOrDefault();
    application.ApplicantId.ShouldBe(applicantId);
    var firstFileId = await UploadFile();
    var secondFileId = await UploadFile();
    var transcriptDocuments = new TranscriptDocuments(applicationId, transcript!.Id!)
    {
        NewCourseOutlineFiles = new List<string> { firstFileId },
        NewProgramConfirmationFiles = new List<string> { secondFileId }
    };

    var result = await repository.SaveApplicationTranscript(transcriptDocuments, CancellationToken.None);
    result.ShouldNotBeNull();
  }

  [Fact]
  public async Task SaveApplicationTranscripts_ShouldThrowException_WhenApplicationOrTranscriptNotFound()
  {
    var transcriptDocuments = new TranscriptDocuments(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

    await Should.ThrowAsync<InvalidOperationException>(async () =>
        await repository.SaveApplicationTranscript(transcriptDocuments, CancellationToken.None));
  }

  [Fact]
  public async Task SaveApplicationTranscripts_ShouldUpdateCourseOutlineOptions()
  {
    var applicantId = Fixture.AuthenticatedBcscUserId;
    var transcript = CreateTranscript();
    var applicationId = await repository.SaveApplication(new Application(null, applicantId, new[] { CertificationType.OneYear })
    {
      Transcripts = new List<Transcript> { transcript }
    }, CancellationToken.None);

    var applications = await repository.Query(new ApplicationQuery { ById = applicationId }, default);
    var application = applications.ShouldHaveSingleItem();
    application.Transcripts.ShouldHaveSingleItem();
    transcript = application.Transcripts.FirstOrDefault();
    application.ApplicantId.ShouldBe(applicantId);

    var transcriptDocuments = new TranscriptDocuments(applicationId, transcript!.Id!)
    {
        CourseOutlineOptions = CourseOutlineOptions.UploadNow
    };

    await repository.SaveApplicationTranscript(transcriptDocuments, CancellationToken.None);

    var freshApplication = await repository.Query(new ApplicationQuery { ById = transcriptDocuments.ApplicationId }, CancellationToken.None);
    var freshTranscript = freshApplication.FirstOrDefault()?.Transcripts.FirstOrDefault();
    freshTranscript.ShouldNotBeNull();
    freshTranscript.CourseOutlineOptions.ShouldBe(CourseOutlineOptions.UploadNow);
  }

  [Fact]
  public async Task SaveApplicationTranscripts_ShouldUpdateProgramConfirmationOptions()
  {
    var applicantId = Fixture.AuthenticatedBcscUserId;
    var transcript = CreateTranscript();
    var applicationId = await repository.SaveApplication(new Application(null, applicantId, new[] { CertificationType.OneYear })
    {
      Transcripts = new List<Transcript> { transcript }
    }, CancellationToken.None);

    var applications = await repository.Query(new ApplicationQuery { ById = applicationId }, default);
    var application = applications.ShouldHaveSingleItem();
    application.Transcripts.ShouldHaveSingleItem();
    transcript = application.Transcripts.FirstOrDefault();
    application.ApplicantId.ShouldBe(applicantId);
    var transcriptDocuments = new TranscriptDocuments(applicationId, transcript!.Id!)
    {
        ProgramConfirmationOptions = ProgramConfirmationOptions.UploadNow
    };

    await repository.SaveApplicationTranscript(transcriptDocuments, CancellationToken.None);

    var freshApplication = await repository.Query(new ApplicationQuery { ById = transcriptDocuments.ApplicationId }, CancellationToken.None);
    var freshTranscript = freshApplication.FirstOrDefault()?.Transcripts.FirstOrDefault();
    freshTranscript.ShouldNotBeNull();
    freshTranscript.ProgramConfirmationOptions.ShouldBe(ProgramConfirmationOptions.UploadNow);
  }

  private CharacterReference CreateCharacterReference()
  {
    var faker = new Faker("en_CA");

    return new CharacterReference(
      faker.Name.FirstName(), faker.Name.LastName(), faker.Phone.PhoneNumber(), "fake@test.com"
    )
    { Status = CharacterReferenceStage.Draft };
  }

  private WorkExperienceReference CreateWorkExperienceReference()
  {
    var faker = new Faker("en_CA");

    return new WorkExperienceReference(
      faker.Name.FirstName(), faker.Name.FirstName(), "fake@test.com", faker.Random.Number(10, 150)
    )
    {
      PhoneNumber = faker.Phone.PhoneNumber(),
      Status = WorkExperienceRefStage.Draft,
    };
  }

  private ProfessionalDevelopment CreateProfessionalDevelopment()
  {
    var faker = new Faker("en_CA");

    return new ProfessionalDevelopment(
        null,
        faker.Random.String2(20),
        faker.Company.CompanyName(),
        faker.Date.Past(),
        faker.Date.Recent()
    )
    {
      OrganizationContactInformation = faker.Phone.PhoneNumber(),
      InstructorName = faker.Name.FullName(),
      NumberOfHours = faker.Random.Int(1, 100),
      Status = ProfessionalDevelopmentStatusCode.Draft
    };
  }

  private Transcript CreateTranscript()
  {
    var faker = new Faker("en_CA");
    var transcript = new Transcript(null, faker.Company.CompanyName(), $"{faker.Hacker.Adjective()} Program", faker.Random.Number(10000000, 99999999).ToString(), faker.Date.Past(), faker.Date.Recent(), faker.Random.Bool(), faker.Name.FirstName(), faker.Name.LastName(), faker.Random.Bool(), EducationRecognition.Recognized, EducationOrigin.InsideBC)
    {
      CampusLocation = faker.Address.City(),
      Status = TranscriptStage.Draft,
      Country = mapper.Map<Country>(Fixture.Country),
      Province = mapper.Map<Province>(Fixture.Province),
      PostSecondaryInstitution = mapper.Map<PostSecondaryInstitution>(Fixture.PostSecondaryInstitution),
      TranscriptStatusOption = TranscriptStatusOptions.OfficialTranscriptRequested,
    };
    return transcript;
  }

  private readonly Faker faker = new Faker("en_CA");

  private async Task<string> UploadFile()
  {
    var fileLength = 1041;
    var testFile = await faker.GenerateTestFile(fileLength);
    var testFileId = Guid.NewGuid().ToString();
    var testFolder = "tempfolder";
    var testTags = "tag1=1,tag2=2";
    var testClassification = "test-classification";
    using var content = new StreamContent(testFile.Content);
    content.Headers.ContentType = new MediaTypeHeaderValue(testFile.ContentType);

    using var formData = new MultipartFormDataContent
        {
          { content, "file", testFile.FileName }
        };

    await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.WithRequestHeader("file-classification", testClassification);
      _.WithRequestHeader("file-tag", testTags);
      _.WithRequestHeader("file-folder", testFolder);
      _.Post.MultipartFormData(formData).ToUrl($"/api/files/{testFileId}");
      _.StatusCodeShouldBeOk();
    });
    return testFileId;
  }

}
