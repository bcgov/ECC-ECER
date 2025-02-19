using ECER.Clients.RegistryPortal.Server.Shared;
using ECER.Managers.Registry.Contract.Registrants;
using ECER.Utilities.Security;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace ECER.Clients.RegistryPortal.Server;

public class AuthenticationService(IMediator messageBus, IDistributedCache cache, IOptions<ClaimCacheSettings> claimCacheSettings)
{
  public async Task<ClaimsPrincipal?> EnrichUserSecurityContext(ClaimsPrincipal principal, CancellationToken ct)
  {
    ArgumentNullException.ThrowIfNull(principal);

    var identityProvider = principal.FindFirst(RegistryPortalClaims.IdenityProvider)?.Value;
    var identityId = principal.FindFirst(ClaimTypes.Name)?.Value;

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
      async ct => (await messageBus.Send(new SearchRegistrantQuery { ByUserIdentity = userIdentity }, ct)).Items.SingleOrDefault(),
      new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(claimCacheSettings.Value.CacheTimeInSeconds) },
      ct);

    if (registrant == null) return null;

    // add registrant claims
    var userId = new Claim("user_id", registrant.UserId);
    Claim verificationStatus = new Claim("verified", "");
    if (registrant.Profile.Status == StatusCode.Verified)
    {
      verificationStatus = new Claim("verified", "true");
    }
    else if (registrant.Profile.Status == StatusCode.Unverified)
    {
      verificationStatus = new Claim("verified", "false");
    }

    return [userId, verificationStatus];
  }
}
