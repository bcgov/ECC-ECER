using System.Security.Claims;
using Alba;
using ECER.Utilities.Security;

namespace ECER.Tests.Integration;

public static class AuthenticationHelper
{
    public static Scenario WithNewUser(this Scenario scenario, UserIdentity identity)
    {
        ArgumentNullException.ThrowIfNull(identity);

        scenario.WithClaim("identity_provider", identity.IdentityProvider);
        if (identity.IdentityProvider == "bcsc")
        {
            scenario.WithClaim(ClaimTypes.NameIdentifier, identity.UserId);
        }
        else
        {
            scenario.WithClaim("bceid_user_guid", identity.UserId);
        }

        return scenario;
    }

    public static Scenario WithExistingUser(this Scenario scenario, UserIdentity identity, string userId)
    {
        ArgumentNullException.ThrowIfNull(identity);

        scenario.WithClaim("identity_provider", identity.IdentityProvider);
        if (identity.IdentityProvider == "bcsc")
        {
            scenario.WithClaim(ClaimTypes.NameIdentifier, identity.UserId);
        }
        else
        {
            scenario.WithClaim("bceid_user_guid", identity.UserId);
        }
        scenario.WithClaim("userId", userId);

        return scenario;
    }

    public static Scenario WithClaim(this Scenario scenario, string type, string value)
    {
        scenario.WithClaim(new Claim(type, value));

        return scenario;
    }
}