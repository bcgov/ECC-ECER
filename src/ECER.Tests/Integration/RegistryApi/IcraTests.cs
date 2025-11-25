using Alba;
using Bogus;
using ECER.Clients.RegistryPortal.Server.ICRA;
using ECER.Resources.E2ETests.UnitTest;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Net;
using System.Net.Http.Headers;
using Xunit.Abstractions;

namespace ECER.Tests.Integration.RegistryApi;

public class IcraTests : RegistryPortalWebAppScenarioBase
{
  private readonly IUnitTestRepository repository;

  public IcraTests(ITestOutputHelper output, RegistryPortalWebAppFixture fixture) : base(output, fixture)
  {
    repository = Fixture.Services.GetRequiredService<IUnitTestRepository>();
  }

  private readonly Faker faker = new Faker("en_CA");

  [Fact]
  public async Task GetIcraEligibilities_ReturnsEligibilities()
  {
    var response = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Get.Url("/api/icra");
      _.StatusCodeShouldBeOk();
    });

    var eligibilities = await response.ReadAsJsonAsync<IEnumerable<Clients.RegistryPortal.Server.ICRA.ICRAEligibility>>();
    eligibilities.ShouldNotBeNull();
  }

  [Fact]
  public async Task SaveDraftIcraEligibility_AndGetById()
  {
    var eligibility = new Clients.RegistryPortal.Server.ICRA.ICRAEligibility
    {
      ApplicantId = this.Fixture.AuthenticatedBcscUser.Id.ToString(),
      Status = Clients.RegistryPortal.Server.ICRA.ICRAStatus.Draft
    };

    var saveResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Put.Json(new SaveDraftICRAEligibilityRequest(eligibility)).ToUrl($"/api/icra/");
      _.StatusCodeShouldBeOk();
    });

    var savedEligibility = (await saveResponse.ReadAsJsonAsync<DraftICRAEligibilityResponse>()).ShouldNotBeNull().Eligibility;
    savedEligibility.Id.ShouldNotBeNull();
    savedEligibility.Status.ShouldBe(Clients.RegistryPortal.Server.ICRA.ICRAStatus.Draft);

    savedEligibility.EmploymentReferences = new[]
    {
      new Clients.RegistryPortal.Server.ICRA.EmploymentReference { Id = null, FirstName = "John", LastName = "Doe", EmailAddress = "john.doe@example.com", PhoneNumber = "" },
      new Clients.RegistryPortal.Server.ICRA.EmploymentReference { Id = null, FirstName = "Jane", LastName = "Smith", EmailAddress = "jane.smith@example.com", PhoneNumber = "1234567890" }
    };

    var saveWithRefsResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Put.Json(new SaveDraftICRAEligibilityRequest(savedEligibility)).ToUrl($"/api/icra/{savedEligibility.Id}");
      _.StatusCodeShouldBeOk();
    });
    var savedWithRefs = (await saveWithRefsResponse.ReadAsJsonAsync<DraftICRAEligibilityResponse>()).ShouldNotBeNull().Eligibility;
    savedWithRefs.EmploymentReferences.ShouldNotBeNull();
    savedWithRefs.EmploymentReferences.Count().ShouldBe(2);

    var getResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Get.Url($"/api/icra/{savedEligibility.Id}");
      _.StatusCodeShouldBeOk();
    });

    var eligibilities = await getResponse.ReadAsJsonAsync<IEnumerable<Clients.RegistryPortal.Server.ICRA.ICRAEligibility>>();
    eligibilities.ShouldNotBeNull();
    eligibilities.ShouldContain(e => e.Id == savedEligibility.Id);
    eligibilities.First(e => e.Id == savedEligibility.Id).EmploymentReferences.Count().ShouldBe(2);

    await SetEligibilityToIneligible(eligibilities.First().Id!);
  }

  [Fact]
  public async Task SubmitIcraEligibility_Succeeds()
  {
    var eligibility = new Clients.RegistryPortal.Server.ICRA.ICRAEligibility
    {
      ApplicantId = this.Fixture.AuthenticatedBcscUser.Id.ToString(),
      Status = Clients.RegistryPortal.Server.ICRA.ICRAStatus.Draft,
      EmploymentReferences = new[]
        {
          new Clients.RegistryPortal.Server.ICRA.EmploymentReference { FirstName = "John", LastName = "Doe", EmailAddress = "john.doe@example.com" }
        },
      InternationalCertifications = new List<Clients.RegistryPortal.Server.ICRA.InternationalCertification>
        {
          new Clients.RegistryPortal.Server.ICRA.InternationalCertification
          {
              CertificateStatus = Clients.RegistryPortal.Server.ICRA.CertificateStatus.Valid,
              CertificateTitle = faker.Company.CatchPhrase(),
              IssueDate = faker.Date.Past(),
              ExpiryDate = faker.Date.Soon(),
              CountryId = this.Fixture.Country.ecer_CountryId!.Value.ToString(),
          }
        }
    };

    var saveResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Put.Json(new SaveDraftICRAEligibilityRequest(eligibility)).ToUrl($"/api/icra/");
      _.StatusCodeShouldBeOk();
    });

    var saved = (await saveResponse.ReadAsJsonAsync<DraftICRAEligibilityResponse>()).ShouldNotBeNull().Eligibility;

    var submitResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Post.Json(new ICRAEligibilitySubmissionRequest(saved.Id!)).ToUrl($"/api/icra");
      _.StatusCodeShouldBeOk();
    });

    var submitted = (await submitResponse.ReadAsJsonAsync<SubmitICRAEligibilityResponse>()).ShouldNotBeNull().Eligibility;
    submitted.ShouldNotBeNull();
    submitted.Id.ShouldBe(saved.Id);
    submitted.Status.ShouldBe(Clients.RegistryPortal.Server.ICRA.ICRAStatus.Submitted);

    await SetEligibilityToIneligible(submitted.Id!);
  }

  [Fact]
  public async Task SubmitIcraEligibility_BadId_ReturnsBadRequest()
  {
    await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Post.Json(new ICRAEligibilitySubmissionRequest("not-a-guid")).ToUrl($"/api/icra");
      _.StatusCodeShouldBe(HttpStatusCode.BadRequest);
    });
  }

  [Fact]
  public async Task GetIcraEligibilityStatus_BadId_ReturnsBadRequest()
  {
    await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Get.Url($"/api/icra/not-a-guid/status");
      _.StatusCodeShouldBe(HttpStatusCode.BadRequest);
    });
  }

  [Fact]
  public async Task GetIcraEligibilityStatus_NotFound_ReturnsNotFound()
  {
    var invalidId = Guid.NewGuid().ToString();
    await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Get.Url($"/api/icra/{invalidId}/status");
      _.StatusCodeShouldBe(HttpStatusCode.NotFound);
    });
  }

  [Fact]
  public async Task GetIcraEligibilityStatus_ReturnsExpectedFields()
  {
    var eligibility = new Clients.RegistryPortal.Server.ICRA.ICRAEligibility
    {
      ApplicantId = this.Fixture.AuthenticatedBcscUser.Id.ToString(),
      Status = Clients.RegistryPortal.Server.ICRA.ICRAStatus.Draft,
      InternationalCertifications = new List<Clients.RegistryPortal.Server.ICRA.InternationalCertification>
      {
        new Clients.RegistryPortal.Server.ICRA.InternationalCertification
        {
          CertificateStatus = Clients.RegistryPortal.Server.ICRA.CertificateStatus.Valid,
          CertificateTitle = faker.Company.CatchPhrase(),
          IssueDate = faker.Date.Past(),
          ExpiryDate = faker.Date.Soon(),
          CountryId = this.Fixture.Country.ecer_CountryId!.Value.ToString(),
        }
      }
    };

    var saveResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Put.Json(new SaveDraftICRAEligibilityRequest(eligibility)).ToUrl($"/api/icra/");
      _.StatusCodeShouldBeOk();
    });

    var saved = (await saveResponse.ReadAsJsonAsync<DraftICRAEligibilityResponse>()).ShouldNotBeNull().Eligibility;

    var statusResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Get.Url($"/api/icra/{saved.Id}/status");
      _.StatusCodeShouldBeOk();
    });

    var status = await statusResponse.ReadAsJsonAsync<Clients.RegistryPortal.Server.ICRA.ICRAEligibilityStatus>();
    status.ShouldNotBeNull();
    status.Id.ShouldBe(saved.Id);
    status.Status.ShouldBe(Clients.RegistryPortal.Server.ICRA.ICRAStatus.Draft);
    status.InternationalCertifications.ShouldNotBeNull();
    status.InternationalCertifications.Count().ShouldBe(1);

    // employment references status should be present and empty (none added yet)
    status.EmploymentReferencesStatus.ShouldNotBeNull();
    status.EmploymentReferencesStatus.Count().ShouldBe(0);

    await SetEligibilityToIneligible(saved.Id!);
  }

  [Fact]
  public async Task GetIcraEligibilityStatus_IncludesEmploymentReferencesStatuses()
  {
    var eligibility = new Clients.RegistryPortal.Server.ICRA.ICRAEligibility
    {
      ApplicantId = this.Fixture.AuthenticatedBcscUser.Id.ToString(),
      Status = Clients.RegistryPortal.Server.ICRA.ICRAStatus.Draft,
      EmploymentReferences = new[]
      {
        new Clients.RegistryPortal.Server.ICRA.EmploymentReference { FirstName = "John", LastName = "Doe", EmailAddress = "john.doe@example.com" },
        new Clients.RegistryPortal.Server.ICRA.EmploymentReference { FirstName = "Jane", LastName = "Smith", EmailAddress = "jane.smith@example.com" }
      },
      InternationalCertifications = new List<Clients.RegistryPortal.Server.ICRA.InternationalCertification>
      {
        new Clients.RegistryPortal.Server.ICRA.InternationalCertification
        {
          CertificateStatus = Clients.RegistryPortal.Server.ICRA.CertificateStatus.Valid,
          CertificateTitle = faker.Company.CatchPhrase(),
          IssueDate = faker.Date.Past(),
          ExpiryDate = faker.Date.Soon(),
          CountryId = this.Fixture.Country.ecer_CountryId!.Value.ToString(),
        }
      }
    };

    var saveResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Put.Json(new SaveDraftICRAEligibilityRequest(eligibility)).ToUrl($"/api/icra/");
      _.StatusCodeShouldBeOk();
    });

    var saved = (await saveResponse.ReadAsJsonAsync<DraftICRAEligibilityResponse>()).ShouldNotBeNull().Eligibility;

    var statusResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Get.Url($"/api/icra/{saved.Id}/status");
      _.StatusCodeShouldBeOk();
    });

    var status = await statusResponse.ReadAsJsonAsync<Clients.RegistryPortal.Server.ICRA.ICRAEligibilityStatus>();
    status.ShouldNotBeNull();
    status.EmploymentReferencesStatus.ShouldNotBeNull();
    status.EmploymentReferencesStatus.Count().ShouldBe(2);
    status.EmploymentReferencesStatus.All(r => !string.IsNullOrWhiteSpace(r.FirstName)).ShouldBeTrue();
    status.EmploymentReferencesStatus.All(r => r.Status.HasValue).ShouldBeTrue();

    await SetEligibilityToIneligible(saved.Id!);
  }

  [Fact]
  public async Task SaveDraftIcraEligibility_WithMismatchedIds_ReturnsBadRequest()
  {
    var eligibilityId = Guid.NewGuid().ToString();
    var payloadId = Guid.NewGuid().ToString();
    var eligibility = new Clients.RegistryPortal.Server.ICRA.ICRAEligibility
    {
      Id = payloadId,
      ApplicantId = this.Fixture.AuthenticatedBcscUser.Id.ToString(),
      Status = Clients.RegistryPortal.Server.ICRA.ICRAStatus.Draft,
      SignedDate = DateTime.UtcNow
    };

    await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Put.Json(new SaveDraftICRAEligibilityRequest(eligibility)).ToUrl($"/api/icra/{eligibilityId}");
      _.StatusCodeShouldBe(HttpStatusCode.BadRequest);
    });
  }

  [Fact]
  public async Task GetIcraEligibility_ByInvalidId_ReturnsNotFound()
  {
    var invalidId = Guid.NewGuid().ToString();

    var response = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Get.Url($"/api/icra/{invalidId}");
      _.StatusCodeShouldBeOk();
    });

    var eligibilities = await response.ReadAsJsonAsync<IEnumerable<Clients.RegistryPortal.Server.ICRA.ICRAEligibility>>();
    eligibilities.ShouldNotBeNull();
    eligibilities.ShouldNotContain(e => e.Id == invalidId);
  }

  [Fact]
  public async Task SaveDraftIcraEligibility_WithInternationalCertificationAndFiles_Saved()
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

    var fileResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.WithRequestHeader("file-classification", testClassification);
      _.WithRequestHeader("file-tag", testTags);
      _.WithRequestHeader("file-folder", testFolder);
      _.Post.MultipartFormData(formData).ToUrl($"/api/files/{testFileId}");
      _.StatusCodeShouldBeOk();
    });

    var uploadedFileResponse = (await fileResponse.ReadAsJsonAsync<ECER.Clients.RegistryPortal.Server.Files.FileResponse>()).ShouldNotBeNull();

    var countryId = this.Fixture.Country.ecer_CountryId!.Value.ToString();

    var eligibility = new Clients.RegistryPortal.Server.ICRA.ICRAEligibility
    {
      Status = Clients.RegistryPortal.Server.ICRA.ICRAStatus.Draft,
      InternationalCertifications = new List<Clients.RegistryPortal.Server.ICRA.InternationalCertification>
            {
                new Clients.RegistryPortal.Server.ICRA.InternationalCertification
                {
                    CertificateStatus = Clients.RegistryPortal.Server.ICRA.CertificateStatus.Valid,
                    CertificateTitle = faker.Company.CatchPhrase(),
                    IssueDate = faker.Date.Past(),
                    ExpiryDate = faker.Date.Soon(),
                    CountryId = countryId,
                    NewFiles = new [] { uploadedFileResponse.fileId }
                }
            }
    };

    var saveResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Put.Json(new SaveDraftICRAEligibilityRequest(eligibility)).ToUrl($"/api/icra/");
      _.StatusCodeShouldBeOk();
    });

    var savedEligibility = (await saveResponse.ReadAsJsonAsync<DraftICRAEligibilityResponse>()).ShouldNotBeNull().Eligibility;
    savedEligibility.Id.ShouldNotBeNull();

    var getResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Get.Url($"/api/icra/{savedEligibility.Id}");
      _.StatusCodeShouldBeOk();
    });

    var eligibilities = await getResponse.ReadAsJsonAsync<IEnumerable<Clients.RegistryPortal.Server.ICRA.ICRAEligibility>>();
    var fetched = eligibilities.ShouldHaveSingleItem();
    fetched.InternationalCertifications.ShouldHaveSingleItem();
    fetched.InternationalCertifications.First().Files.ShouldHaveSingleItem();
    fetched.InternationalCertifications.First().Files.First().Id!.ShouldContain(uploadedFileResponse.fileId);
    fetched.InternationalCertifications.First().CountryId.ShouldBe(countryId);

    await SetEligibilityToIneligible(fetched.Id!);
  }

  [Fact]
  public async Task SaveDraft_WithSubmittedApplicationAndExistingDraft_ReturnsBadRequest()
  {
    var eligibility = new Clients.RegistryPortal.Server.ICRA.ICRAEligibility
    {
      ApplicantId = this.Fixture.AuthenticatedBcscUser.Id.ToString(),
      Status = Clients.RegistryPortal.Server.ICRA.ICRAStatus.Draft,
      EmploymentReferences = new[]
        {
            new Clients.RegistryPortal.Server.ICRA.EmploymentReference { FirstName = "John", LastName = "Doe", EmailAddress = "john.doe@example.com" }
        },
      InternationalCertifications = new List<Clients.RegistryPortal.Server.ICRA.InternationalCertification>
      {
          new Clients.RegistryPortal.Server.ICRA.InternationalCertification
          {
              CertificateStatus = Clients.RegistryPortal.Server.ICRA.CertificateStatus.Valid,
              CertificateTitle = faker.Company.CatchPhrase(),
              IssueDate = faker.Date.Past(),
              ExpiryDate = faker.Date.Soon(),
              CountryId = this.Fixture.Country.ecer_CountryId!.Value.ToString(),
          }
      }
    };

    var saveResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Put.Json(new SaveDraftICRAEligibilityRequest(eligibility)).ToUrl($"/api/icra/");
      _.StatusCodeShouldBeOk();
    });

    var saved = (await saveResponse.ReadAsJsonAsync<DraftICRAEligibilityResponse>()).ShouldNotBeNull().Eligibility;

    //try to save another draft application should fail
    await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Put.Json(new SaveDraftICRAEligibilityRequest(eligibility)).ToUrl($"/api/icra/");
      _.StatusCodeShouldBe(HttpStatusCode.InternalServerError);
    });

    //submit application
    var submittedResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Post.Json(new ICRAEligibilitySubmissionRequest(saved.Id!)).ToUrl($"/api/icra");
      _.StatusCodeShouldBeOk();
    });

    //try to save another draft application with a submitted one should fail
    await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Put.Json(new SaveDraftICRAEligibilityRequest(eligibility)).ToUrl($"/api/icra/");
      _.StatusCodeShouldBe(HttpStatusCode.InternalServerError);
    });

    var submittedEligibility = await submittedResponse.ReadAsJsonAsync<DraftICRAEligibilityResponse>();
    await SetEligibilityToIneligible(submittedEligibility.Eligibility.Id!);
  }

  //private method to set eligibilty application to ineligible so multiple tests do not conflict for one another with the same user
  private async Task SetEligibilityToIneligible(string eligibilityId)
  {
    await repository.SetIcraEligibility(eligibilityId, false, CancellationToken.None);
  }
}
