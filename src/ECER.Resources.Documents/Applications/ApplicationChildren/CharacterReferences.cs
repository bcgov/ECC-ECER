using AutoMapper;
using ECER.Utilities.DataverseSdk.Model;
using Microsoft.Xrm.Sdk.Client;

namespace ECER.Resources.Documents.Applications;

internal partial class ApplicationRepository
{
  private async Task UpdateCharacterReferences(ecer_Application application, List<ecer_CharacterReference> updatedEntities)
  {
    await Task.CompletedTask;
    var existingCharacterReferences = context.ecer_CharacterReferenceSet.Where(t => t.ecer_Applicationid.Id == application.Id).ToList();

    foreach (var reference in existingCharacterReferences)
    {
      if (!updatedEntities.Any(t => t.Id == reference.Id))
      {
        context.DeleteObject(reference);
      }
    }

    foreach (var reference in updatedEntities.Where(d => d.ecer_CharacterReferenceId != null))
    {
      var oldReference = existingCharacterReferences.SingleOrDefault(t => t.Id == reference.Id);
      if (oldReference != null)
      {
        context.Detach(oldReference);
      }
      context.Attach(reference);
      context.UpdateObject(reference);
    }

    foreach (var reference in updatedEntities.Where(d => d.ecer_CharacterReferenceId == null))
    {
      reference.ecer_CharacterReferenceId = Guid.NewGuid();
      context.AddObject(reference);
      context.AddLink(application, ecer_Application.Fields.ecer_characterreference_Applicationid, reference);
    }
  }

  private async Task<string> SubmitCharacterReference(string referenceId, CharacterReferenceSubmissionRequest request)
  {
    await Task.CompletedTask;
    var characterReference = context.ecer_CharacterReferenceSet.Single(c => c.ecer_CharacterReferenceId == Guid.Parse(referenceId));

    mapper.Map(request, characterReference);
    bool certificateProvinceIdIsGuid = Guid.TryParse(request.ReferenceContactInformation.CertificateProvinceId, out Guid certificateProvinceId);

    if (certificateProvinceIdIsGuid)
    {
      var province = context.ecer_ProvinceSet.SingleOrDefault(p => p.ecer_ProvinceId == certificateProvinceId);
      if (province != null)
      {
        context.AddLink(characterReference, ecer_CharacterReference.Fields.ecer_characterreference_RefCertifiedProvinceId, province);
      }
    }
    characterReference.StatusCode = ecer_CharacterReference_StatusCode.Submitted;
    context.UpdateObject(characterReference);
    context.SaveChanges();
    return characterReference.ecer_CharacterReferenceId.ToString()!;
  }

  private async Task<string> OptOutCharacterReference(OptOutReferenceRequest request)
  {
    await Task.CompletedTask;
    var characterReference = context.ecer_CharacterReferenceSet.Single(c => c.ecer_CharacterReferenceId == Guid.Parse(request.PortalInvitation!.CharacterReferenceId!));

    mapper.Map(request, characterReference);
    characterReference.ecer_WillProvideReference = ecer_YesNoNull.No;
    characterReference.StatusCode = ecer_CharacterReference_StatusCode.Rejected;
    characterReference.StateCode = ecer_characterreference_statecode.Inactive;
    context.UpdateObject(characterReference);
    context.SaveChanges();
    return characterReference.ecer_CharacterReferenceId.ToString()!;
  }

  public async Task<string> UpdateCharacterReferenceForSubmittedApplication(CharacterReference updatedReference, string applicationId, string referenceId, string userId, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;
    var application = context.ecer_ApplicationSet.FirstOrDefault(
      d => d.ecer_ApplicationId == Guid.Parse(applicationId) && d.ecer_Applicantid.Id == Guid.Parse(userId)
      );
    if (application == null)
    {
      throw new InvalidOperationException($"Application '{applicationId}' not found");
    }

    var ecerCharacterReference = mapper.Map<ecer_CharacterReference>(updatedReference);

    var existingCharacterReferences = context.ecer_CharacterReferenceSet.Where(t => t.ecer_Applicationid.Id == Guid.Parse(applicationId)).ToList();

    bool RefIdIsGuid = Guid.TryParse(referenceId, out Guid referenceIdGuid);
    if (RefIdIsGuid)
    {
      var oldReference = existingCharacterReferences.SingleOrDefault(t => t.Id == referenceIdGuid);

      // 1. Remove existing Character Reference
      if (oldReference != null)
      {
        context.DeleteObject(oldReference);
      }
      else
      {
        throw new InvalidOperationException($"Reference '{referenceIdGuid}' not found for application '{applicationId}'");
      }
    }

    // 2. Add New Character Reference

    ecerCharacterReference.ecer_CharacterReferenceId = Guid.NewGuid();
    ecerCharacterReference.StatusCode = ecer_CharacterReference_StatusCode.ApplicationSubmitted;
    ecerCharacterReference.ecer_IsAdditional = true;
    context.AddObject(ecerCharacterReference);
    context.AddLink(application, ecer_Application.Fields.ecer_characterreference_Applicationid, ecerCharacterReference);
    context.SaveChanges();

    return ecerCharacterReference.ecer_CharacterReferenceId.ToString()!;
  }

  public async Task<string> ResendCharacterReferenceInvite(ResendReferenceInviteRequest request, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;
    var characterReference = context.ecer_CharacterReferenceSet.SingleOrDefault(c => c.ecer_CharacterReferenceId == Guid.Parse(request.ReferenceId!));

    if (characterReference == null)
    {
      throw new InvalidOperationException($"Character reference '{request.ReferenceId}' not found");
    }

    if (characterReference.StatusCode == ecer_CharacterReference_StatusCode.Rejected || characterReference.StatusCode == ecer_CharacterReference_StatusCode.Submitted)
    {
      throw new InvalidOperationException($"Character reference '{request.ReferenceId}' already responded");
    }

    mapper.Map(request, characterReference);
    characterReference.ecer_InviteAgain = true;
    context.UpdateObject(characterReference);
    context.SaveChanges();

    return characterReference.ecer_CharacterReferenceId.ToString()!;
  }
}
