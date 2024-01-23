using System.Security.Claims;

namespace ECER.Utilities.Security;

public static class UserContextExtensions
{
    public static UserContext? GetUserContext(this ClaimsPrincipal principal)
    {
        if (principal == null) return null;
        var identityProvider = principal.FindFirst("identity_provider")?.Value;

        if (identityProvider == null) return null;

        var identityId = identityProvider.ToLowerInvariant() switch
        {
            "bceidbasic" => principal.FindFirst("bceid_user_guid")?.Value ?? throw new InvalidOperationException("bceid_user_guid claim doesn't exist"),
            "bcsc" => principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException($"{ClaimTypes.NameIdentifier} doesn't exist"),
            _ => throw new NotImplementedException()
        };

        var userId = principal.FindFirst("userId")?.Value ?? string.Empty;

        return new UserContext(new UserIdentity(identityId, identityProvider))
        {
            UserId = userId
        };
    }
}

public record UserContext(UserIdentity Identity)
{
    public string? UserId { get; set; }
}

public record UserIdentity(string UserId, string IdentityProvider);