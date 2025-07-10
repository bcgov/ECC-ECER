using Alba;
using Bogus;
using ECER.Clients.RegistryPortal.Server.Certifications;
using Shouldly;
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
  public async Task CertificationsLookup_ByLastName_ReturnsCertifications_ByPage()
  {
    var faker = new Faker("en_CA");
    int lookUpPageSize = 20;

    var CertificationId = this.Fixture.activeCertificationOneId;

    var CertificationsIdResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Get.Url($"/api/certifications/{CertificationId}");
      _.StatusCodeShouldBeOk();
    });

    var CertificationsId = await CertificationsIdResponse.ReadAsJsonAsync<Certification[]>().ShouldNotBeNull();

    CertificationsId.ShouldNotBeEmpty();
    CertificationsId.ShouldHaveSingleItem();

    // Check if the Name is not null or empty and contains at least one space
    var fullName = CertificationsId[0].Name;
    var lastName = string.Empty;

    if (!string.IsNullOrWhiteSpace(fullName) && fullName.Contains(' '))
    {
      int lastSpaceIndex = fullName.LastIndexOf(' ');
      lastName = fullName.Substring(lastSpaceIndex + 1);
    }
    else
    {
      // Handle the case where there is no space in the name
      lastName = fullName;
    }

    var certificationLookupRequest = new CertificationLookupRequest(faker.Random.Word()) { LastName = lastName, PageSize = lookUpPageSize };
    var CertificationsResponse = await Host.Scenario(_ =>
    {
      _.Post.Json(certificationLookupRequest).ToUrl($"/api/certifications/lookup");
      _.StatusCodeShouldBeOk();
    });

    var Certifications = await CertificationsResponse.ReadAsJsonAsync<IEnumerable<CertificationLookupResponse>>().ShouldNotBeNull();
    Certifications.ShouldNotBeEmpty();
    Certifications.ShouldAllBe(c => c.Name != null && c.Name.EndsWith(lastName!));
    Certifications.Count().ShouldBeLessThanOrEqualTo(lookUpPageSize);
    var CertificateWithCondition = Certifications.FirstOrDefault(c => c.HasConditions == true);
    if (CertificateWithCondition != null)
    {
      CertificateWithCondition.CertificateConditions.Count().ShouldBeGreaterThan(0);
    }
  }

  [Fact]
  public async Task RequestCertificationPdf_ReturnsCertificationId()
  {
    var validCertificationId = this.Fixture.activeCertificationOneId;
    var expectedCertificationId = validCertificationId;

    var validResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Put.Json(new { }).ToUrl($"/api/certifications/RequestPdf/{validCertificationId}");
      _.StatusCodeShouldBeOk();
    });

    var returnedCertificationId = await validResponse.ReadAsJsonAsync<string>();
    returnedCertificationId.ShouldBe(expectedCertificationId);
  }

  [Fact]
  public async Task CertificationsLookup_ByRegistrationNumber_ReturnsCertifications()
  {
    var faker = new Faker("en_CA");

    var CertificationId = this.Fixture.activeCertificationTwoId;

    var CertificationsIdResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Get.Url($"/api/certifications/{CertificationId}");
      _.StatusCodeShouldBeOk();
    });

    var CertificationsId = await CertificationsIdResponse.ReadAsJsonAsync<Certification[]>().ShouldNotBeNull();

    CertificationsId.ShouldNotBeEmpty();
    CertificationsId.ShouldHaveSingleItem();

    var certificateNumber = CertificationsId[0].Number;

    var certificationLookupRequest = new CertificationLookupRequest(faker.Random.Word()) { RegistrationNumber = certificateNumber };
    var CertificationsResponse = await Host.Scenario(_ =>
    {
      _.Post.Json(certificationLookupRequest).ToUrl($"/api/certifications/lookup");
      _.StatusCodeShouldBeOk();
    });

    var Certifications = await CertificationsResponse.ReadAsJsonAsync<Clients.RegistryPortal.Server.Certifications.CertificationLookupResponse[]>().ShouldNotBeNull();
    Certifications.ShouldNotBeEmpty();
    Certifications[0].RegistrationNumber.ShouldBe(certificateNumber);
  }
  [Fact]
  public async Task CertificationsLookup_ByRegistrant_ReturnsCertifications()
  {
    var faker = new Faker("en_CA");

    var user = this.Fixture.AuthenticatedBcscUser;

    var certificationLookupRequest = new CertificationLookupRequest(faker.Random.Word()) { FirstName=user.FirstName,LastName=user.LastName };
    var CertificationsResponse = await Host.Scenario(_ =>
    {
      _.Post.Json(certificationLookupRequest).ToUrl($"/api/certifications/lookup");
      _.StatusCodeShouldBeOk();
    });

    var Certifications = await CertificationsResponse.ReadAsJsonAsync<Clients.RegistryPortal.Server.Certifications.CertificationLookupResponse[]>().ShouldNotBeNull();
    Certifications.ShouldNotBeEmpty();
    Certifications.Length.ShouldBeGreaterThan(1);

  }
  
}
