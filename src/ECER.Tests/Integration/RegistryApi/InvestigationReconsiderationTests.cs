using Alba;
using Bogus;
using ECER.Clients.RegistryPortal.Server.Files;
using ECER.Clients.RegistryPortal.Server.InvestigationReconsiderations;
using Microsoft.AspNetCore.Mvc;
using Shouldly;
using System.Net;
using System.Net.Http.Headers;
using Xunit.Abstractions;

namespace ECER.Tests.Integration.RegistryApi;

public class InvestigationReconsiderationTests : RegistryPortalWebAppScenarioBase
{
  private readonly Faker faker = new Faker("en_CA");

  public InvestigationReconsiderationTests(ITestOutputHelper output, RegistryPortalWebAppFixture fixture) : base(output, fixture)
  {
  }

  [Fact]
  public async Task QueryExistingThenSubmitInvestigationReconsiderationWithFile_ReturnsStatusOk_CannotSubmitAgain()
  {
    //we need to load test file into temp folder first
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

    var uploadedFileResponse = (await fileResponse.ReadAsJsonAsync<FileResponse>()).ShouldNotBeNull();

    var queryResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Get.Url($"/api/investigation/reconsiderations/?byId={this.Fixture.testInvestigationReconsiderationE2ESubmit.Id}");
      _.StatusCodeShouldBeOk();
    });

    var existingReconsideration = (await queryResponse.ReadAsJsonAsync<IEnumerable<InvestigationReconsideration>>()).FirstOrDefault().ShouldNotBeNull();

    existingReconsideration.ExplanationAndEvidence = "updated explanation and evidence for investigation reconsideration submission";
    existingReconsideration.Files = [new Clients.RegistryPortal.Server.Applications.FileInfo(uploadedFileResponse.fileId) { }];

    await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Put.Json(existingReconsideration).ToUrl($"/api/investigation/reconsiderations/submit/{this.Fixture.testInvestigationReconsiderationE2ESubmit.Id}");
      _.StatusCodeShouldBeOk();
    });

    var submittedReconsiderationResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Get.Url($"/api/investigation/reconsiderations/?byId={this.Fixture.testInvestigationReconsiderationE2ESubmit.Id}");
      _.StatusCodeShouldBeOk();
    });

    var submittedReconsideration = (await submittedReconsiderationResponse.ReadAsJsonAsync<IEnumerable<InvestigationReconsideration>>()).FirstOrDefault().ShouldNotBeNull();
    submittedReconsideration.Status.ShouldBe(InvestigationReconsiderationStatusCode.InReview);
    submittedReconsideration.ExplanationAndEvidence.ShouldBe("updated explanation and evidence for investigation reconsideration submission");
    submittedReconsideration.Files.Select(f => f.Id).ShouldContain(uploadedFileResponse.fileId);

    var errorResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Put.Json(existingReconsideration).ToUrl($"/api/investigation/reconsiderations/submit/{this.Fixture.testInvestigationReconsiderationE2ESubmit.Id}");
      _.StatusCodeShouldBe(HttpStatusCode.BadRequest);
    });

    var problemDetails = await errorResponse.ReadAsJsonAsync<ProblemDetails>();
    problemDetails.Status.ShouldBe((int)HttpStatusCode.BadRequest);
    problemDetails.Detail.ShouldBe("investigation reconsideration is in the wrong status");
  }

  [Fact]
  public async Task SubmitInvestigationReconsiderationRandomGuid_AndInvalidGuid_ReturnsError()
  {
    //id does not match payload

    var mismatchResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Put.Json(new InvestigationReconsideration() { Id = Guid.NewGuid().ToString() }).ToUrl($"/api/investigation/reconsiderations/submit/{this.Fixture.testInvestigationReconsiderationE2ESubmit.Id}");
      _.StatusCodeShouldBe(HttpStatusCode.BadRequest);
    });

    var mismatchProblemDetails = await mismatchResponse.ReadAsJsonAsync<ProblemDetails>();
    mismatchProblemDetails.Detail.ShouldBe("resource id and payload id do not match");

    var randomGuid = Guid.NewGuid().ToString();

    var notFoundResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Put.Json(new InvestigationReconsideration() { Id = randomGuid }).ToUrl($"/api/investigation/reconsiderations/submit/{randomGuid}");
      _.StatusCodeShouldBe(HttpStatusCode.BadRequest);
    });

    var notFoundProblemDetails = await notFoundResponse.ReadAsJsonAsync<ProblemDetails>();
    notFoundProblemDetails.Detail.ShouldBe("investigation reconsideration not found");

    await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Put.Json(new InvestigationReconsideration() { Id = "invalid-guid" }).ToUrl($"/api/investigation/reconsiderations/submit/{randomGuid}");
      _.StatusCodeShouldBe(HttpStatusCode.BadRequest);
    });

    await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Put.Json(new InvestigationReconsideration() { }).ToUrl($"/api/investigation/reconsiderations/submit/not-a-guid");
      _.StatusCodeShouldBe(HttpStatusCode.BadRequest);
    });
  }
}
