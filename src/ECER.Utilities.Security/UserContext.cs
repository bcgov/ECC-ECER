using System.Security.Claims;

namespace ECER.Utilities.Security;

public static class UserContextExtensions
{
  public static UserContext? GetUserContext(this ClaimsPrincipal principal)
  {
    if (principal == null) return null;

    var identityProvider = principal.FindFirst("identity_provider")?.Value ?? string.Empty;
    var identityId = principal.FindFirst("identity_id")?.Value ?? string.Empty;
    var userId = principal.FindFirst("user_id")?.Value ?? string.Empty;

    return new UserContext(new UserIdentity(identityId, identityProvider), userId);
  }
}

public record UserContext(UserIdentity Identity, string UserId);

public record UserIdentity(string UserId, string IdentityProvider);
