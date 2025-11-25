using ECER.Clients.PSPPortal.Server.Shared;
using ECER.Utilities.Security;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using ECER.Managers.Registry.Contract.PspUsers;

namespace ECER.Clients.PSPPortal.Server;

public class AuthenticationService(IMediator messageBus, IDistributedCache cache, IOptions<ClaimCacheSettings> claimCacheSettings)
{
  public async Task<ClaimsPrincipal?> EnrichUserSecurityContext(ClaimsPrincipal principal, CancellationToken ct)
  {
    ArgumentNullException.ThrowIfNull(principal);

    var identityProvider = principal.FindFirst(PSPPortalClaims.IdentityProvider)?.Value;
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
    // try to find the psp user
    var pspRep = await cache.GetAsync($"userinfo:{userIdentity.UserId}@{userIdentity.IdentityProvider}",
      async ct => (await messageBus.Send(new SearchPspRepQuery { ByUserIdentity = userIdentity }, ct)).Items.SingleOrDefault(),
      new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(claimCacheSettings.Value.CacheTimeInSeconds) },
      ct);

    if (pspRep == null) return null;

    // add registrant claims
    var userId = new Claim("user_id", pspRep.Id);
    Claim hasTermsOfUse = new Claim("terms_of_use", pspRep.Profile.HasAcceptedTermsOfUse.HasValue && pspRep.Profile.HasAcceptedTermsOfUse.Value ? "true" : "false");

    return [userId, hasTermsOfUse];
  }
}
