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

    return new UserContext(new UserIdentity(identityId, identityProvider), userId);
  }
}

public record UserContext(UserIdentity Identity, string UserId);

public record UserIdentity(string UserId, string IdentityProvider);

public static class RegistryPortalClaims
{
  public const string UserId = "user_id";
  public const string IdenityProvider = "identity_provider";
}
