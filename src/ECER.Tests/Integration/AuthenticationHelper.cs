using Alba;
using ECER.Utilities.Security;
using System.Security.Claims;

namespace ECER.Tests.Integration;

public static class AuthenticationHelper
{
  public static Scenario WithNewUser(this Scenario scenario, UserIdentity identity) => scenario.WithUser(identity);

  public static Scenario WithExistingUser(this Scenario scenario, UserIdentity identity, string userId) => scenario.WithUser(identity, userId);

  private static Scenario WithUser(this Scenario scenario, UserIdentity identity, string? userId = null)
  {
    ArgumentNullException.ThrowIfNull(identity);

    scenario.WithClaim(RegistryPortalClaims.IdenityProvider, identity.IdentityProvider);
    scenario.WithClaim(ClaimTypes.Name, identity.UserId);
    scenario.WithClaim(ClaimTypes.NameIdentifier, identity.UserId);
    if (!string.IsNullOrEmpty(userId)) scenario.WithClaim(RegistryPortalClaims.UserId, userId);

    return scenario;
  }

  public static Scenario WithClaim(this Scenario scenario, string type, string value)
  {
    scenario.WithClaim(new Claim(type, value));

    return scenario;
  }
}
