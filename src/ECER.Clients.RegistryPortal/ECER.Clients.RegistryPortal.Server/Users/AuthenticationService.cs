using System.Security.Claims;

namespace ECER.Clients.RegistryPortal.Server.Users;

public static class AuthenticationService
{
    public static (string id, string identityProvider)? GetUserLogin(ClaimsPrincipal principal)
    {
        if (principal == null) return null;
        var identityProvider = principal.FindFirstValue("identity_provider");

        return identityProvider?.ToLowerInvariant() switch
        {
            "bceidbasic" => (principal.FindFirstValue("bceid_user_guid") ?? throw new InvalidOperationException("bceid_user_guid claim doesn't exist"), identityProvider),
            "bcsc" => (principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException($"{ClaimTypes.NameIdentifier} doesn't exist"), identityProvider),
            _ => null
        };
    }
}