using Alba;
using ECER.Clients.PSPPortal.Server.EducationInstitutions;
using ECER.Clients.PSPPortal.Server.Users;
using ECER.Clients.RegistryPortal.Server;
using ECER.Clients.RegistryPortal.Server.Applications;
using ECER.Managers.Admin.Contract.PortalInvitations;
using ECER.Managers.Registry.Contract.PortalInvitations;
using ECER.Managers.Registry.Contract.PostSecondaryInstitutes;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace ECER.Tests.Integration.PspApi;

public class EducationInstituteTests : PspPortalWebAppScenarioBase
{
  public EducationInstituteTests(ITestOutputHelper output, PspPortalWebAppFixture fixture) : base(output, fixture)
  {
  }
  [Fact]
  public async Task CanGetEducationInstituteData()
  {
    var response = await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId, true);
      _.Get.Url("/api/education-institution");
      _.StatusCodeShouldBeOk();
    });

    EducationInstitution? institute = await response.ReadAsJsonAsync<EducationInstitution>();
    institute.ShouldNotBeNull();

    institute.Auspice = Clients.PSPPortal.Server.EducationInstitutions.Auspice.Public;
    institute.City = "Victoria";
    institute.Name = "Test Name";
    institute.Street1 = "123 Test Rd";
    institute.Street2 = "Unit 5";
    institute.Street3 = "co/ Steve";
    institute.PostalCode = "V8V 8V8";
    institute.Province = "British Columbia";
    institute.WebsiteUrl = "www.test.com";

    await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId, true);
      _.Put.Json(institute).ToUrl("/api/education-institution");
      _.StatusCodeShouldBeOk();
    });

    response = await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId, true);
      _.Get.Url("/api/education-institution");
      _.StatusCodeShouldBeOk();
    });
    institute = await response.ReadAsJsonAsync<EducationInstitution>();
    institute.ShouldNotBeNull();

    institute.Auspice.ShouldBe(Clients.PSPPortal.Server.EducationInstitutions.Auspice.Public);
    institute.City.ShouldBe("Victoria");
    institute.Name.ShouldBe("Test Name");
    institute.Street1.ShouldBe("123 Test Rd");
    institute.Street2.ShouldBe("Unit 5");
    institute.Street3.ShouldBe("co/ Steve");
    institute.PostalCode.ShouldBe("V8V 8V8");
    institute.Province.ShouldBe("British Columbia");
    institute.WebsiteUrl.ShouldBe("www.test.com");

  }

}
