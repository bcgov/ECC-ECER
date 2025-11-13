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

    scenario.WithClaim(RegistryPortalClaims.IdentityProvider, identity.IdentityProvider);
    scenario.WithClaim(ClaimTypes.Name, identity.UserId);
    scenario.WithClaim(ClaimTypes.NameIdentifier, identity.UserId);
    if (user != null)
    {
      scenario.WithClaim(RegistryPortalClaims.UserId, user.Id.ToString());
      Claim verificationStatus = new Claim(RegistryPortalClaims.Verified, "");
      if (user.StatusCode == Contact_StatusCode.Verified)
      {
        verificationStatus = new Claim(RegistryPortalClaims.Verified, "true");
      }
      else if (user.StatusCode is Contact_StatusCode.Unverified or Contact_StatusCode.PendingforDocuments)
      {
        verificationStatus = new Claim(RegistryPortalClaims.Verified, "false");
      }
      scenario.WithClaim(verificationStatus);
    }
    return scenario;
  }

  public static Scenario WithClaim(this Scenario scenario, string type, string value)
  {
    scenario.WithClaim(new Claim(type, value));

    return scenario;
  }
}
