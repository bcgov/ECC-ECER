using System.Net;
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
  public async Task CreateProgramApplication_ReturnsOkAndCreatedApplication()
  {
    var request = new CreateProgramApplicationRequest
    {
      ProgramApplicationName = $"Test program application {Guid.NewGuid():N}",
      ProgramTypes = new[] { ProgramCertificationType.Basic, ProgramCertificationType.ITE },
      DeliveryType = DeliveryType.Hybrid
    };

    var response = await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId);
      _.Post.Json(request).ToUrl("/api/programApplications");
      _.StatusCodeShouldBeOk();
    });

    var createResult = await response.ReadAsJsonAsync<CreateProgramApplicationResponse>();
    createResult.ShouldNotBeNull();
    createResult.ProgramApplication.ShouldNotBeNull();
    createResult.ProgramApplication.Id.ShouldNotBeNull();
    createResult.ProgramApplication.Status.ShouldBe(ApplicationStatus.Draft);
    createResult.ProgramApplication.ProgramApplicationName.ShouldBe(request.ProgramApplicationName);
    createResult.ProgramApplication.DeliveryType.ShouldBe(request.DeliveryType);
    createResult.ProgramApplication.PostSecondaryInstituteId.ShouldBe(Fixture.PostSecondaryInstituteId);

    var getResponse = await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId);
      _.Get.Url($"/api/programApplications/{createResult.ProgramApplication.Id}");
      _.StatusCodeShouldBeOk();
    });

    var getResult = await getResponse.ReadAsJsonAsync<GetProgramApplicationResponse>();
    getResult.ShouldNotBeNull();
    var fetched = getResult.Applications!.FirstOrDefault(a => a.Id == createResult.ProgramApplication.Id).ShouldNotBeNull();
    fetched.Status.ShouldBe(ApplicationStatus.Draft);
    fetched.ProgramApplicationName.ShouldBe(request.ProgramApplicationName);
    fetched.DeliveryType.ShouldBe(DeliveryType.Hybrid);
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
    firstApplication.First().Status.ShouldBe(ApplicationStatus.ReviewAnalysis);
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
    firstApplication.Status.ShouldBe(ApplicationStatus.ReviewAnalysis);
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
