using ECER.Managers.Admin.Contract.PortalInvitations;
using Microsoft.AspNetCore.DataProtection;
using System.Net;

namespace ECER.Engines.Transformation.PortalInvitations;

internal sealed class PortalInvitationTransformationEngine(IDataProtectionProvider dataProtectionProvider) : IPortalInvitationTransformationEngine
{
  public async Task<TransformResponse> Transform(TransformRequest request)
  {
    return request switch
    {
      EncryptInviteTokenRequest r => await EncryptInviteToken(r),
      DecryptInviteTokenRequest r => await DecryptInviteToken(r),
      _ => throw new NotSupportedException($"{request.GetType().Name} is not supported")
    };
  }

  private async Task<EncryptInviteTokenResponse> EncryptInviteToken(EncryptInviteTokenRequest request)
  {
    await Task.CompletedTask;

    var expiryDate = DateTime.UtcNow.AddDays(request.ValidDays);
    var protector = dataProtectionProvider.CreateProtector(nameof(PortalInvitationTransformationEngine)).ToTimeLimitedDataProtector();

    var encryptedData = protector.Protect(request.PortalInvitation.ToString(), expiryDate);

    var referenceLink = WebUtility.UrlEncode(encryptedData);
    return new EncryptInviteTokenResponse(request.PortalInvitation, referenceLink);
  }

  private async Task<DecryptInviteTokenResponse> DecryptInviteToken(DecryptInviteTokenRequest request)
  {
    await Task.CompletedTask;

    var encryptedData = WebUtility.UrlDecode(request.VerificationToken);

    var protector = dataProtectionProvider.CreateProtector(nameof(PortalInvitationTransformationEngine)).ToTimeLimitedDataProtector();
    var decryptedData = protector.Unprotect(encryptedData);

    var portalInvitation = Guid.Parse(decryptedData);

    return new DecryptInviteTokenResponse(portalInvitation);
  }
}
