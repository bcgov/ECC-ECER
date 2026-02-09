using System.Security.Claims;

namespace ECER.Utilities.Security;

public static class UserContextExtensions
{
  public static UserContext? GetUserContext(this ClaimsPrincipal principal)
  {
    if (principal == null) return null;

    var identityProvider = principal.FindFirst(RegistryPortalClaims.IdentityProvider)?.Value ?? string.Empty;
    var identityId = principal.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
    var userId = principal.FindFirst(RegistryPortalClaims.UserId)?.Value ?? string.Empty;
    var verifiedClaimValue = principal.FindFirst(RegistryPortalClaims.Verified)?.Value ?? string.Empty;
    var termsOfUseClaimValue = principal.FindFirst(PSPPortalClaims.TermsOfUse)?.Value ?? string.Empty;
    bool isVerified = bool.TryParse(verifiedClaimValue, out var result) && result;
    bool isTermsOfUseAccepted = bool.TryParse(termsOfUseClaimValue, out var termsResult) && termsResult;
    return new UserContext(new UserIdentity(identityId, identityProvider), userId, isVerified, isTermsOfUseAccepted);
  }
}

public record UserContext(UserIdentity Identity, string UserId, bool IsVerified, bool IsTermsOfUseAccepted);

public record UserIdentity(string UserId, string IdentityProvider);

public static class RegistryPortalClaims
{
  public const string UserId = "user_id";
  public const string Verified = "verified";
  public const string IdentityProvider = "identity_provider";
}
public static class PSPPortalClaims
{
  public const string UserId = "user_id";
  public const string TermsOfUse = "terms_of_use";
  public const string IdentityProvider = "identity_provider";
}
