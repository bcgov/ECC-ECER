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

    institute.InstitutionType = Clients.PSPPortal.Server.EducationInstitutions.PsiInstitutionType.Public;
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

    institute.InstitutionType.ShouldBe(Clients.PSPPortal.Server.EducationInstitutions.PsiInstitutionType.Public);
    institute.City.ShouldBe("Victoria");
    institute.Name.ShouldBe("Test Name");
    institute.Street1.ShouldBe("123 Test Rd");
    institute.Street2.ShouldBe("Unit 5");
    institute.Street3.ShouldBe("co/ Steve");
    institute.PostalCode.ShouldBe("V8V 8V8");
    institute.Province.ShouldBe("British Columbia");
    institute.WebsiteUrl.ShouldBe("www.test.com");

  }

  [Fact]
  public async Task CanCreateCampus()
  {
    var request = new CreateCampusRequest
    {
      Name = "Test Campus",
      IsSatelliteOrTemporaryLocation = false,
      Street1 = "456 Campus Ave",
      City = "Victoria",
      Province = "British Columbia",
      PostalCode = "V8W 1W1",
    };

    var response = await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId, true);
      _.Post.Json(request).ToUrl("/api/education-institution/campus");
      _.StatusCodeShouldBeOk();
    });

    var newCampusId = await response.ReadAsJsonAsync<string>();
    newCampusId.ShouldNotBeNullOrEmpty();
  }

  [Fact]
  public async Task CanUpdateCampus()
  {
    var createRequest = new CreateCampusRequest
    {
      Name = "Update Test Campus",
      IsSatelliteOrTemporaryLocation = true,
      Street1 = "789 Campus Blvd",
      City = "Vancouver",
      Province = "British Columbia",
      PostalCode = "V6B 1A1",
    };

    var createResponse = await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId, true);
      _.Post.Json(createRequest).ToUrl("/api/education-institution/campus");
      _.StatusCodeShouldBeOk();
    });

    var campusId = await createResponse.ReadAsJsonAsync<string>();
    campusId.ShouldNotBeNullOrEmpty();

    var updateRequest = new UpdateCampusRequest
    {
      Name = "Updated Campus Name",
      Street1 = "999 New St",
      City = "Kelowna",
      Province = "British Columbia",
      PostalCode = "V1Y 1A1",
    };

    await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId, true);
      _.Put.Json(updateRequest).ToUrl($"/api/education-institution/campus/{campusId}");
      _.StatusCodeShouldBeOk();
    });

    // Verify via GET that campus was updated and IsSatelliteOrTemporaryLocation was not changed
    var institutionResponse = await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId, true);
      _.Get.Url("/api/education-institution");
      _.StatusCodeShouldBeOk();
    });

    var institution = await institutionResponse.ReadAsJsonAsync<EducationInstitution>();
    institution.ShouldNotBeNull();

    var updatedCampus = institution.Campuses?.SingleOrDefault(c => c.Id == campusId);
    updatedCampus.ShouldNotBeNull();
    updatedCampus.Name.ShouldBe("Updated Campus Name");
    updatedCampus.City.ShouldBe("Kelowna");
    updatedCampus.IsSatelliteOrTemporaryLocation.ShouldBe(true); // should not have changed
  }

}
