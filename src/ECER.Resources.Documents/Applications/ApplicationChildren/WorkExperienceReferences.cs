using AutoMapper;
using ECER.Utilities.DataverseSdk.Model;
using Microsoft.Xrm.Sdk.Client;

namespace ECER.Resources.Documents.Applications;

internal sealed partial class ApplicationRepository
{
  private async Task UpdateWorkExperienceReferences(ecer_Application application, List<ecer_WorkExperienceRef> updatedEntities)
  {
    await Task.CompletedTask;
    var existingWorkExperiences = context.ecer_WorkExperienceRefSet.Where(t => t.ecer_Applicationid.Id == application.Id).ToList();
    // Remove WorkExperienceReferences that they exist in the dataverse but not in the application
    foreach (var reference in existingWorkExperiences)
    {
      if (!updatedEntities.Any(t => t.ecer_WorkExperienceRefId == reference.ecer_WorkExperienceRefId))
      {
        context.DeleteObject(reference);
      }
    }
    // Update Existing WorkExperienceReferences
    foreach (var reference in updatedEntities.Where(d => d.ecer_WorkExperienceRefId != null))
    {
      var oldReference = existingWorkExperiences.SingleOrDefault(t => t.ecer_WorkExperienceRefId == reference.ecer_WorkExperienceRefId);
      if (oldReference != null)
      {
        context.Detach(oldReference);
        reference.StatusCode = oldReference.StatusCode;
      }
      context.Attach(reference);
      context.UpdateObject(reference);
    }
    // Add New WorkExperienceReferences that they exist in the application but not in the dataverse
    foreach (var reference in updatedEntities.Where(d => d.ecer_WorkExperienceRefId == null))
    {
      reference.ecer_WorkExperienceRefId = Guid.NewGuid();
      reference.StatusCode = ecer_WorkExperienceRef_StatusCode.Draft;
      context.AddObject(reference);
      context.AddLink(application, ecer_Application.Fields.ecer_workexperienceref_Applicationid_ecer, reference);
    }
  }

  private async Task<string> SubmitWorkexperienceReference(string referenceId, WorkExperienceReferenceSubmissionRequest request)
  {
    await Task.CompletedTask;
    var workExperienceReference = context.ecer_WorkExperienceRefSet.Single(c => c.ecer_WorkExperienceRefId == Guid.Parse(referenceId));

    mapper.Map(request, workExperienceReference);
    bool certificateProvinceIdIsGuid = Guid.TryParse(request.ReferenceContactInformation.CertificateProvinceId, out Guid certificateProvinceId);
    if (certificateProvinceIdIsGuid)
    {
      var province = context.ecer_ProvinceSet.SingleOrDefault(p => p.ecer_ProvinceId == certificateProvinceId);
      if (province != null)
      {
        context.AddLink(workExperienceReference, ecer_WorkExperienceRef.Fields.ecer_workexperienceref_RefCertifiedProvinceId, province);
      }
    }
    workExperienceReference.StatusCode = ecer_WorkExperienceRef_StatusCode.Submitted;
    context.UpdateObject(workExperienceReference);
    context.SaveChanges();
    return workExperienceReference.ecer_WorkExperienceRefId.ToString()!;
  }

  private async Task<string> OptOutWorkExperienceReference(OptOutReferenceRequest request)
  {
    await Task.CompletedTask;
    var workexperienceReference = context.ecer_WorkExperienceRefSet.Single(c => c.ecer_WorkExperienceRefId == Guid.Parse(request.PortalInvitation!.WorkexperienceReferenceId!));

    mapper.Map(request, workexperienceReference);
    workexperienceReference.ecer_WillProvideReference = ecer_YesNoNull.No;
    workexperienceReference.StatusCode = ecer_WorkExperienceRef_StatusCode.Rejected;
    workexperienceReference.StateCode = ecer_workexperienceref_statecode.Inactive;
    context.UpdateObject(workexperienceReference);
    context.SaveChanges();
    return workexperienceReference.ecer_WorkExperienceRefId.ToString()!;
  }

  public async Task<string> ResendWorkExperienceReferenceInvite(ResendReferenceInviteRequest request, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;
    var workexperienceReference = context.ecer_WorkExperienceRefSet.SingleOrDefault(c => c.ecer_WorkExperienceRefId == Guid.Parse(request.ReferenceId!));

    if (workexperienceReference == null)
    {
      throw new InvalidOperationException($"Work experience reference '{request.ReferenceId}' not found");
    }

    if (workexperienceReference.StatusCode == ecer_WorkExperienceRef_StatusCode.Rejected || workexperienceReference.StatusCode == ecer_WorkExperienceRef_StatusCode.Submitted)
    {
      throw new InvalidOperationException($"Work experience reference '{request.ReferenceId}' already responded");
    }

    workexperienceReference.ecer_InviteAgain = true;
    context.UpdateObject(workexperienceReference);
    context.SaveChanges();
    return workexperienceReference.ecer_WorkExperienceRefId.ToString()!;
  }

  public async Task<string> UpdateWorkExReferenceForSubmittedApplication(WorkExperienceReference updatedReference, string applicationId, string referenceId, string userId, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;
    var application = context.ecer_ApplicationSet.FirstOrDefault(
      d => d.ecer_ApplicationId == Guid.Parse(applicationId) && d.ecer_Applicantid.Id == Guid.Parse(userId)
      );
    if (application == null)
    {
      throw new InvalidOperationException($"Application '{applicationId}' not found");
    }

    var ecerWorkExperienceReference = mapper.Map<ecer_WorkExperienceRef>(updatedReference);

    var existingWorkExperiences = context.ecer_WorkExperienceRefSet.Where(t => t.ecer_Applicationid.Id == Guid.Parse(applicationId)).ToList();

    bool RefIdIsGuid = Guid.TryParse(referenceId, out Guid referenceIdGuid);
    if (RefIdIsGuid)
    {
      var oldReference = existingWorkExperiences.SingleOrDefault(t => t.Id == referenceIdGuid);
      // 1. Remove existing WorkExperienceReference

      if (oldReference != null)
      {
        context.DeleteObject(oldReference);
      }
      else
      {
        throw new InvalidOperationException($"Reference '{referenceIdGuid}' not found for application '{applicationId}'");
      }
    }

    // 2. Add New WorkExperienceReferences

    ecerWorkExperienceReference.ecer_WorkExperienceRefId = Guid.NewGuid();
    ecerWorkExperienceReference.StatusCode = ecer_WorkExperienceRef_StatusCode.ApplicationSubmitted;
    ecerWorkExperienceReference.ecer_IsAdditional = true;
    if (existingWorkExperiences.Count > 0)
    {
      var firstExistingWorkExp = existingWorkExperiences.FirstOrDefault();
      if (firstExistingWorkExp != null)
      {
        ecerWorkExperienceReference.ecer_Type = firstExistingWorkExp.ecer_Type; // setting ecer_type of new work experience reference to be same as existing ones
      }
    }
    context.AddObject(ecerWorkExperienceReference);
    context.AddLink(application, ecer_Application.Fields.ecer_workexperienceref_Applicationid_ecer, ecerWorkExperienceReference);
    context.SaveChanges();

    return ecerWorkExperienceReference.ecer_WorkExperienceRefId.ToString()!;
  }
}
