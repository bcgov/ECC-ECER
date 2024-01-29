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
    scenario.WithClaim("identity_id", identity.UserId);

    if (identity.IdentityProvider == "bcsc")
    {
      scenario.WithClaim(ClaimTypes.NameIdentifier, identity.UserId);
    }
    else if (identity.IdentityProvider == "bceidbasic")
    {
      scenario.WithClaim("bceid_user_guid", identity.UserId);
    }
    else
    {
      throw new NotImplementedException();
    }

    return scenario;
  }

  public static Scenario WithExistingUser(this Scenario scenario, UserIdentity identity, string userId)
  {
    ArgumentNullException.ThrowIfNull(identity);

    scenario.WithClaim("identity_provider", identity.IdentityProvider);
    scenario.WithClaim("identity_id", identity.UserId);

    if (identity.IdentityProvider == "bcsc")
    {
      scenario.WithClaim(ClaimTypes.NameIdentifier, identity.UserId);
    }
    else if (identity.IdentityProvider == "bceidbasic")
    {
      scenario.WithClaim("bceid_user_guid", identity.UserId);
    }
    else
    {
      throw new NotImplementedException();
    }

    scenario.WithClaim("user_id", userId);

    return scenario;
  }

  public static Scenario WithClaim(this Scenario scenario, string type, string value)
  {
    scenario.WithClaim(new Claim(type, value));

    return scenario;
  }
}
