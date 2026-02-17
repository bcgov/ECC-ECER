using Alba;
using ECER.Clients.PSPPortal.Server.ProgramApplications;
using Shouldly;
using Xunit.Abstractions;

namespace ECER.Tests.Integration.PspApi;

public class ProgramApplicationTest : PspPortalWebAppScenarioBase
{
  public ProgramApplicationTest(ITestOutputHelper output, PspPortalWebAppFixture fixture) : base(output, fixture)
  {
  }
  
  [Fact]
  public async Task GetAllProgramApplications_ReturnsStatusOk()
  {
    var response = await Host.Scenario(_ =>
    {
      _.WithPspUser(this.Fixture.AuthenticatedPspUserIdentity, this.Fixture.AuthenticatedPspUserId);
      _.Get.Url($"/api/programApplications/null");
      _.StatusCodeShouldBeOk();
    });
  
    var status = await response.ReadAsJsonAsync<GetProgramApplicationResponse>();
    status.ShouldNotBeNull();
    
    var firstApplication = status.Applications!.Where(app => app.Id == Fixture.programApplicationId).ShouldNotBeNull();
    firstApplication.First().Status.ShouldBe(ApplicationStatus.RFAI);
  }
  
  [Fact]
  public async Task GetProgramApplication_ById_ReturnsStatusOk()
  {
    var response = await Host.Scenario(_ =>
    {
      _.WithPspUser(this.Fixture.AuthenticatedPspUserIdentity, this.Fixture.AuthenticatedPspUserId);
      _.Get.Url($"/api/programApplications/{this.Fixture.programApplicationId}");
      _.StatusCodeShouldBeOk();
    });

    var status = await response.ReadAsJsonAsync<GetProgramApplicationResponse>();
    status.ShouldNotBeNull();

    var firstApplication = status.Applications!.FirstOrDefault().ShouldNotBeNull();
    firstApplication.Status.ShouldBe(ApplicationStatus.RFAI);
    firstApplication.DeliveryType.ShouldBe(DeliveryType.Hybrid);
  }
  
  [Fact]
  public async Task UpdateProgramApplication_Type_Draft_ToWithdraw_ReturnsOk()
  {
    var program = await Host.Scenario(_ =>
    {
      _.WithPspUser(this.Fixture.AuthenticatedPspUserIdentity, this.Fixture.AuthenticatedPspUserId);
      _.Get.Url($"/api/programApplications/{this.Fixture.draftProgramApplicationId}");
      _.StatusCodeShouldBeOk();
    });
  
    var status = await program.ReadAsJsonAsync<GetProgramApplicationResponse>();
    status.ShouldNotBeNull();
    var application = status.Applications!.First();
    application.Status = ApplicationStatus.Withdrawn;
  
    var response = await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId);
      _.Put.Json(application).ToUrl($"/api/programApplications/{Fixture.draftProgramApplicationId}");
      _.StatusCodeShouldBeOk();
    });
  }
}
