using ECER.Resources.Documents.Reconsiderations;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit.Abstractions;
using Xunit.Categories;

namespace ECER.Tests.Integration.Resources.Reconsiderations;

[IntegrationTest]
public class ReconsiderationRepositoryTests : RegistryPortalWebAppScenarioBase
{
  private readonly IReconsiderationRepository repository;

  public ReconsiderationRepositoryTests(ITestOutputHelper output, RegistryPortalWebAppFixture fixture) : base(output, fixture)
  {
    repository = Fixture.Services.GetRequiredService<IReconsiderationRepository>();
  }

  [Fact]
  public async Task QueryReconsideration_ReturnsResults()
  {
    var reconsiderationRequestId = this.Fixture.testReconsiderationRequest.Id.ToString();
    var reconsiderationRequestApplicantId = this.Fixture.testReconsiderationRequest.ecer_reconsiderationrequest_ApplicantId.Id.ToString();
    var reconsiderations = (await repository.Query(new ReconsiderationQuery { ById = reconsiderationRequestId, ByStatusCodes = [ReconsiderationStatusCode.New], ByApplicantId = reconsiderationRequestApplicantId }, CancellationToken.None));

    reconsiderations.Count().ShouldBe(1);
  }

  [Fact]
  public async Task SubmitReconsideration_ShouldUpdate()
  {
    var reconsiderationRequestId = this.Fixture.testReconsiderationRequestEdit.Id.ToString();
    var reconsiderationRequestApplicantId = this.Fixture.testReconsiderationRequestEdit.ecer_reconsiderationrequest_ApplicantId.Id.ToString();

    var updatedReconsiderationId = (await repository.Submit(new Reconsideration() { Id = reconsiderationRequestId, ExplanationAndEvidence = "updated evidence", Status = ReconsiderationStatusCode.InReview }, reconsiderationRequestApplicantId, CancellationToken.None));

    updatedReconsiderationId.ShouldNotBeNullOrEmpty();

    var reconsideration = (await repository.Query(
      new ReconsiderationQuery
      {
        ById = reconsiderationRequestId,
        ByStatusCodes = [ReconsiderationStatusCode.InReview],
        ByApplicantId = reconsiderationRequestApplicantId
      }, CancellationToken.None)).FirstOrDefault().ShouldNotBeNull();

    reconsideration.ExplanationAndEvidence.ShouldBe("updated evidence");
  }
}
