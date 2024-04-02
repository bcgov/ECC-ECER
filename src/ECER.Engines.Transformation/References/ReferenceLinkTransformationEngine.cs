﻿using ECER.Managers.Admin.Contract.References;
using Microsoft.AspNetCore.DataProtection;
using System.Net;

namespace ECER.Engines.Transformation.References;

internal sealed class ReferenceLinkTransformationEngine(IDataProtectionProvider dataProtectionProvider) : IReferenceLinkTransformationEngine
{
  public async Task<PortalInvitationToLinkResponse> Transform(PortalInvitationToLinkRequest request)
  {
    await Task.CompletedTask;

    var expiryDate = DateTime.UtcNow.AddDays(7); // Example expiry date

    var protector = dataProtectionProvider.CreateProtector(nameof(ReferenceLinkTransformationEngine)).ToTimeLimitedDataProtector();

    // Combine referenceType and portalInvitation into a single string
    var combinedData = $"{request.referenceType}:{request.portalInvitation}";
    var encryptedData = protector.Protect(combinedData, expiryDate);

    var referenceLink = WebUtility.UrlEncode(encryptedData);

    return new PortalInvitationToLinkResponse(request.portalInvitation, referenceLink);
  }

  public async Task<LinkToPortalInvitationResponse> Transform(LinkToPortalInvitationRequest request)
  {
    await Task.CompletedTask;

    var encryptedData = WebUtility.UrlDecode(request.referenceLink);

    var protector = dataProtectionProvider.CreateProtector(nameof(ReferenceLinkTransformationEngine)).ToTimeLimitedDataProtector();
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

    if (!Enum.TryParse<ReferenceType>(referenceTypeString, out var referenceType))
    {
      throw new InvalidOperationException("Invalid reference type.");
    }

    return new LinkToPortalInvitationResponse(portalInvitation, referenceType);
  }
}