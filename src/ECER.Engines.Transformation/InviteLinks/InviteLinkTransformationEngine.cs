using ECER.Managers.Admin.Contract.InviteLinks;
using Microsoft.AspNetCore.DataProtection;
using System.Net;

namespace ECER.Engines.Transformation.InviteLinks;

internal sealed class InviteLinkTransformationEngine(IDataProtectionProvider dataProtectionProvider) : IInviteLinkTransformationEngine
{
  public async Task<PortalInvitationToLinkResponse> Transform(PortalInvitationToLinkRequest request)
  {
    await Task.CompletedTask;

    var expiryDate = DateTime.UtcNow.AddDays(7); // Example expiry date

    var protector = dataProtectionProvider.CreateProtector(nameof(InviteLinkTransformationEngine)).ToTimeLimitedDataProtector();

    // Combine referenceType and portalInvitation into a single string
    var combinedData = $"{request.inviteType}:{request.portalInvitation}";
    var encryptedData = protector.Protect(combinedData, expiryDate);

    var referenceLink = WebUtility.UrlEncode(encryptedData);

    return new PortalInvitationToLinkResponse(request.portalInvitation, referenceLink);
  }

  public async Task<LinkToPortalInvitationResponse> Transform(LinkToPortalInvitationRequest request)
  {
    await Task.CompletedTask;

    var encryptedData = WebUtility.UrlDecode(request.encryptedVerificationToken);

    var protector = dataProtectionProvider.CreateProtector(nameof(InviteLinkTransformationEngine)).ToTimeLimitedDataProtector();
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

    return new LinkToPortalInvitationResponse(portalInvitation, referenceType);
  }
}
