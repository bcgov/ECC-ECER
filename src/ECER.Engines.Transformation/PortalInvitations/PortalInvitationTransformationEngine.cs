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

    var expiryDate = DateTime.UtcNow.AddDays(request.validDays); // Example expiry date
    var protector = dataProtectionProvider.CreateProtector(nameof(PortalInvitationTransformationEngine)).ToTimeLimitedDataProtector();

    // Combine referenceType and portalInvitation into a single string
    var combinedData = $"{request.inviteType}:{request.portalInvitation}";
    var encryptedData = protector.Protect(combinedData, expiryDate);

    var referenceLink = WebUtility.UrlEncode(encryptedData);
    return new EncryptInviteTokenResponse(request.portalInvitation, referenceLink);
  }

  private async Task<DecryptInviteTokenResponse> DecryptInviteToken(DecryptInviteTokenRequest request)
  {
    await Task.CompletedTask;

    var encryptedData = WebUtility.UrlDecode(request.verificationToken);

    var protector = dataProtectionProvider.CreateProtector(nameof(PortalInvitationTransformationEngine)).ToTimeLimitedDataProtector();
    var decryptedData = protector.Unprotect(encryptedData);

    // Split the decrypted data back into ReferenceType and PortalInvitation
    var splitIndex = decryptedData.IndexOf(':');
    if (splitIndex == -1)
    {
      throw new InvalidOperationException("Invalid reference link format.");
    }

    var referenceTypeString = decryptedData.Substring(0, splitIndex);
    var inviteIdString = decryptedData.Substring(splitIndex + 1);
    var portalInvitation = Guid.Parse(inviteIdString);

    if (!Enum.TryParse<InviteType>(referenceTypeString, out var referenceType))
    {
      throw new InvalidOperationException("Invalid reference type.");
    }

    return new DecryptInviteTokenResponse(portalInvitation, referenceType);
  }
}
