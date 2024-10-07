using Alba;
using ECER.Utilities.DataverseSdk.Model;
using ECER.Utilities.Security;
using System.Security.Claims;

namespace ECER.Tests.Integration;

public static class AuthenticationHelper
{
  public static Scenario WithNewUser(this Scenario scenario, UserIdentity identity) => scenario.WithUser(identity, null);

  public static Scenario WithExistingUser(this Scenario scenario, UserIdentity identity, Contact user) => scenario.WithUser(identity, user);

  private static Scenario WithUser(this Scenario scenario, UserIdentity identity, Contact? user)
  {
    ArgumentNullException.ThrowIfNull(identity);

    scenario.WithClaim(RegistryPortalClaims.IdenityProvider, identity.IdentityProvider);
    scenario.WithClaim(ClaimTypes.Name, identity.UserId);
    scenario.WithClaim(ClaimTypes.NameIdentifier, identity.UserId);
    if (user != null)
    {
      scenario.WithClaim(RegistryPortalClaims.UserId, user.Id.ToString());
      bool verified = user.ecer_IsVerified ?? false;
      scenario.WithClaim(RegistryPortalClaims.Verified, verified.ToString());
    }
    return scenario;
  }

  public static Scenario WithClaim(this Scenario scenario, string type, string value)
  {
    scenario.WithClaim(new Claim(type, value));

    return scenario;
  }
}
