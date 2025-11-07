using System.Security.Claims;

namespace ECER.Utilities.Security;

public static class UserContextExtensions
{
  public static UserContext? GetUserContext(this ClaimsPrincipal principal)
  {
    if (principal == null) return null;

    var identityProvider = principal.FindFirst(RegistryPortalClaims.IdenityProvider)?.Value ?? string.Empty;
    var identityId = principal.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
    var userId = principal.FindFirst(RegistryPortalClaims.UserId)?.Value ?? string.Empty;
    var verifiedClaimValue = principal.FindFirst(RegistryPortalClaims.Verified)?.Value ?? string.Empty;
    bool isVerified = bool.TryParse(verifiedClaimValue, out var result) && result;

    return new UserContext(new UserIdentity(identityId, identityProvider), userId, isVerified);
  }
}

public record UserContext(UserIdentity Identity, string UserId, bool IsVerified);

public record UserIdentity(string UserId, string IdentityProvider);

public static class RegistryPortalClaims
{
  public const string UserId = "user_id";
  public const string Verified = "verified";
  public const string IdenityProvider = "identity_provider";
}
public static class PSPPortalClaims
{
  public const string UserId = "user_id";
  public const string Verified = "verified";
  public const string IdenityProvider = "identity_provider";
}
