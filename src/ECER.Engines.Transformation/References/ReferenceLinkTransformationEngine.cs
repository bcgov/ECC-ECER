using ECER.Managers.Admin.Contract.References;
using Microsoft.AspNetCore.DataProtection;
using System.Net;

namespace ECER.Engines.Transformation.References;

internal sealed class ReferenceLinkTransformationEngine(IDataProtectionProvider dataProtectionProvider) : IReferenceLinkTransformationEngine
{
  public async Task<GenerateReferenceLinkResponse> Transform(GenerateReferenceLinkRequest request)
  {
    await Task.CompletedTask;

    var expiryDate = DateTime.UtcNow.AddDays(7); // Example expiry date

    var protector = dataProtectionProvider.CreateProtector(nameof(ReferenceLinkTransformationEngine)).ToTimeLimitedDataProtector();
    var encryptedInviteId = protector.Protect(request.portalInvitation.ToString(), expiryDate);

    // Concatenating ReferenceType with the encrypted invite ID
    var combinedData = $"{request.referenceType}:{encryptedInviteId}";
    var referenceLink = WebUtility.UrlEncode(combinedData);

    return new GenerateReferenceLinkResponse(request.portalInvitation, referenceLink);
  }

  public async Task<GenerateReferenceLinkRequest> UnTransform(GenerateReferenceLinkResponse response)
  {
    await Task.CompletedTask;

    var combinedData = WebUtility.UrlDecode(response.referenceLink);

    // Extracting ReferenceType and EncryptedInviteId
    var splitIndex = combinedData.IndexOf(':');
    if (splitIndex == -1)
    {
      throw new InvalidOperationException("Invalid reference link format.");
    }

    var referenceTypeString = combinedData.Substring(0, splitIndex);
    var encryptedInviteId = combinedData.Substring(splitIndex + 1);

    var protector = dataProtectionProvider.CreateProtector(nameof(ReferenceLinkTransformationEngine)).ToTimeLimitedDataProtector();
    var decryptedInviteId = Guid.Parse(protector.Unprotect(encryptedInviteId));

    if (!Enum.TryParse<ReferenceType>(referenceTypeString, out var referenceType))
    {
      throw new InvalidOperationException("Invalid reference type.");
    }

    return new GenerateReferenceLinkRequest(decryptedInviteId, referenceType);
  }
}
