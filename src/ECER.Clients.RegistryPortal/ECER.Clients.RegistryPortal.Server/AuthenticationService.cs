using System.Security.Claims;
using ECER.Managers.Registry.Contract.Registrants;
using ECER.Utilities.Security;
using Microsoft.Extensions.Caching.Distributed;
using Wolverine;

namespace ECER.Clients.RegistryPortal.Server;

public class AuthenticationService(IMessageBus messageBus, IDistributedCache cache)
{
  public async Task<ClaimsPrincipal?> EnrichUserSecurityContext(ClaimsPrincipal principal, CancellationToken ct)
  {
    ArgumentNullException.ThrowIfNull(principal);

    var identityProvider = principal.FindFirst("identity_provider")?.Value;
    var identityId = principal.FindFirst("identity_id")?.Value;

    if (string.IsNullOrEmpty(identityProvider) || string.IsNullOrEmpty(identityId)) return principal;

    // try and get the current user information
    var userClaims = await GetUserClaims(new UserIdentity(identityId, identityProvider), ct);

    if (userClaims == null) return principal;

    principal.AddIdentity(new ClaimsIdentity(userClaims));

    return principal;
  }

  private async Task<Claim[]?> GetUserClaims(UserIdentity userIdentity, CancellationToken ct)
  {
    // try to find the registrant
    var registrant = await cache.GetAsync($"userinfo:{userIdentity.UserId}@{userIdentity.IdentityProvider}",
      async ct => (await messageBus.InvokeAsync<RegistrantQueryResults>(new SearchRegistrantQuery { ByUserIdentity = userIdentity }, ct)).Items.SingleOrDefault(),
      new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) },
      ct);

    if (registrant == null) return null;

    // add registrant claims
    return [new Claim("user_id", registrant.UserId)];
  }
}
