using Alba;
using Bogus;
using ECER.Clients.RegistryPortal.Server.Applications;
using ECER.Clients.RegistryPortal.Server.Certifications;
using Newtonsoft.Json.Linq;
using Shouldly;
using System;
using Xunit.Abstractions;

namespace ECER.Tests.Integration.RegistryApi;

public class CertificationsTests : RegistryPortalWebAppScenarioBase
{
  public CertificationsTests(ITestOutputHelper output, RegistryPortalWebAppFixture fixture) : base(output, fixture)
  {
  }

  [Fact]
  public async Task GetCertifications_ReturnsCertifications()
  {
    var CertificationsResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Get.Url("/api/certifications");
      _.StatusCodeShouldBeOk();
    });

    var Certifications = await CertificationsResponse.ReadAsJsonAsync<Clients.RegistryPortal.Server.Certifications.Certification[]>();
    Certifications.ShouldNotBeNull();
  }

  [Fact]
  public async Task CertificationsLookup_ByName_ReturnsCertifications()
  {
    var faker = new Faker("en_CA");

    var CertificationId = this.Fixture.certificationOneId;

    var CertificationsIdResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Get.Url($"/api/certifications/{CertificationId}");
      _.StatusCodeShouldBeOk();
    });

    var CertificationsId = await CertificationsIdResponse.ReadAsJsonAsync<Certification[]>().ShouldNotBeNull();

    CertificationsId.ShouldNotBeEmpty();
    CertificationsId.ShouldHaveSingleItem();
    // Find the last space and split
    int lastSpaceIndex = CertificationsId[0].Name!.LastIndexOf(' ');

    var lastName = CertificationsId[0].Name!.Substring(lastSpaceIndex + 1);

    var certificationLookupRequest = new CertificationLookupRequest(faker.Random.Word()) { LastName = lastName };
    var CertificationsResponse = await Host.Scenario(_ =>
    {
      _.Post.Json(certificationLookupRequest).ToUrl($"/api/certifications/lookup");
      _.StatusCodeShouldBeOk();
    });

    var Certifications = await CertificationsResponse.ReadAsJsonAsync<Clients.RegistryPortal.Server.Certifications.Certification[]>().ShouldNotBeNull();
    Certifications.ShouldNotBeEmpty();
    Certifications.ShouldAllBe(c => c.Name != null && c.Name.EndsWith(lastName));
  }

  [Fact]
  public async Task CertificationsLookup_ByRegistrationNumber_ReturnsCertifications()
  {
    var faker = new Faker("en_CA");
    var certificationLookupRequest = new CertificationLookupRequest(faker.Random.Word()) { RegistrationNumber = "016444" };
    var CertificationsResponse = await Host.Scenario(_ =>
    {
      _.Post.Json(certificationLookupRequest).ToUrl($"/api/certifications/lookup");
      _.StatusCodeShouldBeOk();
    });

    var Certifications = await CertificationsResponse.ReadAsJsonAsync<Clients.RegistryPortal.Server.Certifications.Certification[]>();
    Certifications.ShouldNotBeNull();
  }
}
