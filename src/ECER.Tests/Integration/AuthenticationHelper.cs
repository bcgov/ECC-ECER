using System.Security.Claims;
using Alba;

namespace ECER.Tests.Integration;

public static class AuthenticationHelper
{
    public static Scenario WithBceidUser(this Scenario scenario, string userGuid)
    {
        scenario.WithClaim("identity_provider", "bceidbasic");
        scenario.WithClaim("bceid_user_guid", userGuid);

        return scenario;
    }

    public static Scenario WithBcscUser(this Scenario scenario, string userGuid)
    {
        scenario.WithClaim("identity_provider", "bcsc");
        scenario.WithClaim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString("N").ToUpperInvariant());

        return scenario;
    }

    public static Scenario WithClaim(this Scenario scenario, string type, string value)
    {
        scenario.WithClaim(new Claim(type, value));

        return scenario;
    }
}