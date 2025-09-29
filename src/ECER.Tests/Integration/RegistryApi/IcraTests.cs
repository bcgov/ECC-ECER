using Alba;
using Bogus;
using ECER.Clients.RegistryPortal.Server.ICRA;
using Shouldly;
using System.Net;
using System.Net.Http.Headers;
using Xunit.Abstractions;
using Xunit.Categories;

namespace ECER.Tests.Integration.RegistryApi;

public class IcraTests : RegistryPortalWebAppScenarioBase
{
    public IcraTests(ITestOutputHelper output, RegistryPortalWebAppFixture fixture) : base(output, fixture)
    {
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

        var eligibilities = await response.ReadAsJsonAsync<IEnumerable<ICRAEligibility>>();
        eligibilities.ShouldNotBeNull();
    }

    [Fact]
    public async Task SaveDraftIcraEligibility_AndGetById()
    {
        var eligibility = new ICRAEligibility
        {
            ApplicantId = this.Fixture.AuthenticatedBcscUser.Id.ToString(),
            Status = ICRAStatus.Draft
        };

        var saveResponse = await Host.Scenario(_ =>
        {
            _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
            _.Put.Json(new SaveDraftICRAEligibilityRequest(eligibility)).ToUrl($"/api/icra/");
            _.StatusCodeShouldBeOk();
        });

        var savedEligibility = (await saveResponse.ReadAsJsonAsync<DraftICRAEligibilityResponse>()).ShouldNotBeNull().Eligibility;
        savedEligibility.Id.ShouldNotBeNull();
        savedEligibility.Status.ShouldBe(ICRAStatus.Draft);

    savedEligibility.EmploymentReferences = new []
    {
      new EmploymentReference { Id = null, FirstName = "John", LastName = "Doe", EmailAddress = "john.doe@example.com", PhoneNumber = "" },
      new EmploymentReference { Id = null, FirstName = "Jane", LastName = "Smith", EmailAddress = "jane.smith@example.com", PhoneNumber = "1234567890" }
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

        var eligibilities = await getResponse.ReadAsJsonAsync<IEnumerable<ICRAEligibility>>();
        eligibilities.ShouldNotBeNull();
        eligibilities.ShouldContain(e => e.Id == savedEligibility.Id);
        eligibilities.First(e => e.Id == savedEligibility.Id).EmploymentReferences.Count().ShouldBe(2);
    }

    [Fact]
    public async Task SubmitIcraEligibility_Succeeds()
    {
        var eligibility = new ICRAEligibility
        {
            ApplicantId = this.Fixture.AuthenticatedBcscUser.Id.ToString(),
            Status = ICRAStatus.Draft,
            EmploymentReferences = new []
            {
                new EmploymentReference { FirstName = "John", LastName = "Doe", EmailAddress = "john.doe@example.com" }
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
        submitted.Status.ShouldBe(ICRAStatus.Submitted);
    }

    [Fact]
    public async Task SubmitIcraEligibility_BadId_ReturnsBadRequest()
    {
        var response = await Host.Scenario(_ =>
        {
            _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
            _.Post.Json(new ICRAEligibilitySubmissionRequest("not-a-guid")).ToUrl($"/api/icra");
            _.StatusCodeShouldBe(HttpStatusCode.BadRequest);
        });
    }

    [Fact]
    public async Task SaveDraftIcraEligibility_WithMismatchedIds_ReturnsBadRequest()
    {
        var eligibilityId = Guid.NewGuid().ToString();
        var payloadId = Guid.NewGuid().ToString();
        var eligibility = new ICRAEligibility
        {
            Id = payloadId,
            ApplicantId = this.Fixture.AuthenticatedBcscUser.Id.ToString(),
            Status = ICRAStatus.Draft,
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

        var eligibilities = await response.ReadAsJsonAsync<IEnumerable<ICRAEligibility>>();
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

        var eligibility = new ICRAEligibility
        {
            Status = ICRAStatus.Draft,
            InternationalCertifications = new List<InternationalCertification>
            {
                new InternationalCertification
                {
                    CertificateStatus = CertificateStatus.Valid,
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

        var eligibilities = await getResponse.ReadAsJsonAsync<IEnumerable<ICRAEligibility>>();
        var fetched = eligibilities.ShouldHaveSingleItem();
        fetched.InternationalCertifications.ShouldHaveSingleItem();
        fetched.InternationalCertifications.First().Files.ShouldHaveSingleItem();
        fetched.InternationalCertifications.First().Files.First().Id!.ShouldContain(uploadedFileResponse.fileId);
        fetched.InternationalCertifications.First().CountryId.ShouldBe(countryId);
    }
}
