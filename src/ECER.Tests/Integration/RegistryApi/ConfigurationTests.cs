using Alba;
using ECER.Clients.RegistryPortal.Server;
using ECER.Clients.RegistryPortal.Server.Applications;
using ECER.Managers.Admin;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit.Abstractions;
using static Pipelines.Sockets.Unofficial.SocketConnection;

namespace ECER.Tests.Integration.RegistryApi;

public class ConfigurationTests : RegistryPortalWebAppScenarioBase
{
  public ConfigurationTests(ITestOutputHelper output, RegistryPortalWebAppFixture fixture) : base(output, fixture)
  {
  }

  [Fact]
  public async Task GetProvinces_ReturnsProvinces()
  {
    var provincesResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Get.Url("/api/provincelist");
      _.StatusCodeShouldBeOk();
    });

    var provinces = await provincesResponse.ReadAsJsonAsync<Province[]>();
    provinces.ShouldNotBeNull();
    provinces.Length.ShouldBeGreaterThan(0);
  }

  [Fact]
  public async Task GetCountries_ReturnsCountries()
  {
    var countriesResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Get.Url("/api/countrylist");
      _.StatusCodeShouldBeOk();
    });

    var countries = await countriesResponse.ReadAsJsonAsync<Country[]>();
    countries.ShouldNotBeNull();
    countries.Length.ShouldBeGreaterThan(0);
  }

  [Fact]
  public async Task GetPostSecondaryInstitutions_ReturnsPostSecondaryInstitutions()
  {
    var postSecondaryInstitutionsResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Get.Url("/api/postSecondaryInstitutionList");
      _.StatusCodeShouldBeOk();
    });

    var postSecondaryInstitutions = await postSecondaryInstitutionsResponse.ReadAsJsonAsync<PostSecondaryInstitution[]>();
    postSecondaryInstitutions.ShouldNotBeNull();
    postSecondaryInstitutions.Length.ShouldBeGreaterThan(0);
  }

  [Fact]
  public async Task GetSystemMessages_ReturnsSystemMessages()
  {
    var systemMessagesResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Get.Url("/api/systemMessages");
      _.StatusCodeShouldBeOk();
    });

    var systemMessages = await systemMessagesResponse.ReadAsJsonAsync<SystemMessage[]>();
    systemMessages.ShouldNotBeNull();
  }

  [Fact]
  public async Task GetDefaultContents_ReturnsDefaultContents()
  {
    var defaultConfigurationsResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Get.Url("/api/defaultContents");
      _.StatusCodeShouldBeOk();
    });

    var defaultContents = await defaultConfigurationsResponse.ReadAsJsonAsync<DefaultContent[]>();
    defaultContents.ShouldNotBeNull();
  }

  [Fact]
  public async Task GetIdentificationTypes_ReturnsIdentificationTypes()
  {
    var identificationTypesResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Get.Json(new IdentificationTypesQuery() { }).ToUrl("/api/identificationTypes");
      _.StatusCodeShouldBeOk();
    });

    var identificationTypes = await identificationTypesResponse.ReadAsJsonAsync<IdentificationType[]>();
    identificationTypes.ShouldNotBeNull();
  }

  [Fact]
  public async Task GetCaptchaSiteKey_ReturnsSiteKey()
  {
    var siteKeyResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Get.Url("/api/captchaSiteKey");
      _.StatusCodeShouldBeOk();
    });

    var siteKey = await siteKeyResponse.ReadAsJsonAsync<string>();
    siteKey.ShouldNotBeNull();
  }

  [Fact]
  public async Task GetProvinces_Returned()
  {
    var manager = Fixture.Services.GetRequiredService<MetadataHandlers>();
    var provinces = await manager.Handle(new ECER.Managers.Admin.Contract.Metadatas.ProvincesQuery(), default);
    provinces.Items.ShouldNotBeEmpty();
  }

  [Fact]
  public async Task GetCertificationComparison_NoParams_ReturnsAll()
  {
    var response = await Host.Scenario(_ =>
    {
      _.WithExistingUser(Fixture.AuthenticatedBcscUserIdentity, Fixture.AuthenticatedBcscUser);
      _.Get.Url("/api/certificationComparison");
      _.StatusCodeShouldBeOk();
    });

    var items = await response.ReadAsJsonAsync<CertificationComparison[]>();
    items.ShouldNotBeNull();
  }

  [Fact]
  public async Task GetCertificationComparison_ByProvinceOnly_FiltersByProvince()
  {
    var provincesResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Get.Url("/api/provincelist");
      _.StatusCodeShouldBeOk();
    });

    var provinces = await provincesResponse.ReadAsJsonAsync<Province[]>();
    var province = provinces.Select(x => x.ProvinceId).First();

    var response = await Host.Scenario(_ =>
    {
      _.WithExistingUser(Fixture.AuthenticatedBcscUserIdentity, Fixture.AuthenticatedBcscUser);
      _.Get.Url($"/api/certificationComparison?provinceId={province}");
      _.StatusCodeShouldBeOk();
    });

    var items = await response.ReadAsJsonAsync<CertificationComparison[]>();
    items.ShouldNotBeEmpty();
  }
}
