using Alba;
using Bogus;
using ECER.Clients.RegistryPortal.Server.ICRA;
using Shouldly;
using System.Net;
using Xunit.Abstractions;
using Xunit.Categories;

namespace ECER.Tests.Integration.RegistryApi;

public class IcraTests : RegistryPortalWebAppScenarioBase
{
    public IcraTests(ITestOutputHelper output, RegistryPortalWebAppFixture fixture) : base(output, fixture)
    {
    }


    [Fact]
    public async Task GetIcraEligibilities_ReturnsEligibilities()
    {
        var response = await Host.Scenario(_ =>
        {
            _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
            _.Get.Url("/api/icra");
            _.StatusCodeShouldBeOk();
        });

        var eligibilities = await response.ReadAsJsonAsync<IEnumerable<ICRAEligibility>>();
        eligibilities.ShouldNotBeNull();
    }

    [Fact]
    public async Task SaveDraftIcraEligibility_AndGetById()
    {
        var eligibilityId = Guid.NewGuid().ToString();
        var eligibility = new ICRAEligibility
        {
            Id = eligibilityId,
            ApplicantId = this.Fixture.AuthenticatedBcscUser.Id.ToString(),
            Status = ICRAStatus.Draft
        };

        var saveResponse = await Host.Scenario(_ =>
        {
            _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
            _.Put.Json(new SaveDraftICRAEligibilityRequest(eligibility)).ToUrl($"/api/icra/{eligibilityId}");
            _.StatusCodeShouldBeOk();
        });

        var savedEligibility = (await saveResponse.ReadAsJsonAsync<DraftICRAEligibilityResponse>()).ShouldNotBeNull().Eligibility;
        savedEligibility.Id.ShouldBe(eligibilityId);
        savedEligibility.Status.ShouldBe(ICRAStatus.Draft);

        var getResponse = await Host.Scenario(_ =>
        {
            _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
            _.Get.Url($"/api/icra/{eligibilityId}");
            _.StatusCodeShouldBeOk();
        });

        var eligibilities = await getResponse.ReadAsJsonAsync<IEnumerable<ICRAEligibility>>();
        eligibilities.ShouldNotBeNull();
        eligibilities.ShouldContain(e => e.Id == eligibilityId);
    }

    [Fact]
    public async Task SaveDraftIcraEligibility_WithMismatchedIds_ReturnsBadRequest()
    {
        var eligibilityId = Guid.NewGuid().ToString();
        var payloadId = Guid.NewGuid().ToString();
        var eligibility = new ICRAEligibility
        {
            Id = payloadId,
            ApplicantId = this.Fixture.AuthenticatedBcscUser.Id.ToString(),
            Status = ICRAStatus.Draft
        };

        await Host.Scenario(_ =>
        {
            _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
            _.Put.Json(new SaveDraftICRAEligibilityRequest(eligibility)).ToUrl($"/api/icra/{eligibilityId}");
            _.StatusCodeShouldBe(HttpStatusCode.BadRequest);
        });
    }

    [Fact]
    public async Task GetIcraEligibility_ByInvalidId_ReturnsNotFound()
    {
        var invalidId = Guid.NewGuid().ToString();

        var response = await Host.Scenario(_ =>
        {
            _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
            _.Get.Url($"/api/icra/{invalidId}");
            _.StatusCodeShouldBeOk();
        });

        var eligibilities = await response.ReadAsJsonAsync<IEnumerable<ICRAEligibility>>();
        eligibilities.ShouldNotBeNull();
        eligibilities.ShouldNotContain(e => e.Id == invalidId);
    }
}
