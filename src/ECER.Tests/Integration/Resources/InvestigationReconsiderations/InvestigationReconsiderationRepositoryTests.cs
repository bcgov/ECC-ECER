using ECER.Resources.Documents.InvestigationReconsiderations;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit.Abstractions;
using Xunit.Categories;

namespace ECER.Tests.Integration.Resources.InvestigationReconsiderations;

[IntegrationTest]
public class InvestigationReconsiderationRepositoryTests : RegistryPortalWebAppScenarioBase
{
  private readonly IInvestigationReconsiderationRepository repository;

  public InvestigationReconsiderationRepositoryTests(ITestOutputHelper output, RegistryPortalWebAppFixture fixture) : base(output, fixture)
  {
    repository = Fixture.Services.GetRequiredService<IInvestigationReconsiderationRepository>();
  }

  [Fact]
  public async Task QueryReconsideration_ReturnsResults()
  {
    var investigationReconsiderationRequestId = this.Fixture.testInvestigationReconsiderationRequest.Id.ToString();
    var investigationReconsiderationRequestApplicantId = this.Fixture.testInvestigationReconsiderationRequest.ecer_reconsiderationinvestigationoutcome_RegistrantId.Id.ToString();
    var investigationReconsiderations = (await repository.Query(new InvestigationReconsiderationQuery { ById = investigationReconsiderationRequestId, ByStatusCodes = [InvestigationReconsiderationStatusCode.New], ByApplicantId = investigationReconsiderationRequestApplicantId }, CancellationToken.None));

    investigationReconsiderations.Count().ShouldBe(1);
  }

  [Fact]
  public async Task SubmitInvestigationReconsideration_ShouldUpdateAndChangeStatus()
  {
    var investigationReconsiderationRequestId = this.Fixture.testInvestigationReconsiderationSubmit.Id.ToString();
    var investigationReconsiderationRequestApplicantId = this.Fixture.testInvestigationReconsiderationSubmit.ecer_reconsiderationinvestigationoutcome_RegistrantId.Id.ToString();

    var updatedReconsiderationId = (await repository.Submit(new InvestigationReconsideration() { Id = investigationReconsiderationRequestId, ExplanationAndEvidence = "updated evidence" }, investigationReconsiderationRequestApplicantId, CancellationToken.None));

    updatedReconsiderationId.ShouldNotBeNullOrEmpty();

    var investigationReconsideration = (await repository.Query(
      new InvestigationReconsiderationQuery
      {
        ById = investigationReconsiderationRequestId,
        ByStatusCodes = [InvestigationReconsiderationStatusCode.InReview],
        ByApplicantId = investigationReconsiderationRequestApplicantId
      }, CancellationToken.None)).FirstOrDefault().ShouldNotBeNull();

    investigationReconsideration.ExplanationAndEvidence.ShouldBe("updated evidence");
  }
}
