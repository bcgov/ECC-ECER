using ECER.Utilities.DataverseSdk.Model;
using Microsoft.Xrm.Sdk.Client;

namespace ECER.Resources.Documents.ICRA;

internal sealed partial class ICRARepository
{
  private async Task UpdateEmploymentReferences(ecer_ICRAEligibilityAssessment icraEligibility, Contact contact, List<ecer_WorkExperienceRef> updatedEntities, CancellationToken ct)
  {
    await Task.CompletedTask;

    var existing = context.ecer_WorkExperienceRefSet.Where(t => t.ecer_ICRAEligibilityAssessment.Id == icraEligibility.Id).ToList();

    // Remove those not present anymore
    foreach (var reference in existing)
    {
      if (!updatedEntities.Any(t => t.ecer_WorkExperienceRefId == reference.ecer_WorkExperienceRefId))
      {
        context.DeleteObject(reference);
      }
    }

    // Update existing
    foreach (var reference in updatedEntities.Where(d => d.ecer_WorkExperienceRefId != null))
    {
      var oldRef = existing.SingleOrDefault(t => t.ecer_WorkExperienceRefId == reference.ecer_WorkExperienceRefId);
      if (oldRef != null)
      {
        context.Detach(oldRef);
        reference.StatusCode = oldRef.StatusCode;
      }
      reference.ecer_Origin = ecer_Origin.Portal;
      context.Attach(reference);
      context.UpdateObject(reference);
    }

    // Add new
    foreach (var reference in updatedEntities.Where(d => d.ecer_WorkExperienceRefId == null))
    {
      reference.ecer_WorkExperienceRefId = Guid.NewGuid();
      reference.StatusCode = ecer_WorkExperienceRef_StatusCode.Draft;
      reference.ecer_Origin = ecer_Origin.Portal;
      context.AddObject(reference);
      context.AddLink(icraEligibility, ecer_ICRAEligibilityAssessment.Fields.ecer_WorkExperienceRef_ecer_ICRAEligibilityAssessment_ecer_ICRAEligibilityAssessment, reference);
      context.AddLink(contact, ecer_WorkExperienceRef.Fields.ecer_workexperienceref_Applicantid, reference);
    }
  }

  public async Task<string> ResendIcraWorkExperienceReferenceInvite(ResendIcraReferenceInviteRequest request, CancellationToken cancellationToken)
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

  private async Task<EmploymentReference> AddIcraWorkExperienceReferenceWithoutSave(AddIcraWorkExperienceReferenceRequest request)
  {
    await Task.CompletedTask;
    var icraEligibilityApplication = context.ecer_ICRAEligibilityAssessmentSet.FirstOrDefault(
      d => d.ecer_ICRAEligibilityAssessmentId == Guid.Parse(request.icraEligibilityId) && d.ecer_icraeligibilityassessment_ApplicantId.Id == Guid.Parse(request.userId));

    if (icraEligibilityApplication == null)
    {
      throw new InvalidOperationException($"Icra eligibility application '{request.icraEligibilityId}' not found");
    }

    var ecerIcraWorkExperienceReference = mapper.Map<ecer_WorkExperienceRef>(request.employmentReference);
    ecerIcraWorkExperienceReference.ecer_WorkExperienceRefId = Guid.NewGuid();
    ecerIcraWorkExperienceReference.StatusCode = ecer_WorkExperienceRef_StatusCode.ICRAEligibilitySubmitted;
    ecerIcraWorkExperienceReference.ecer_IsAdditional = true;
    ecerIcraWorkExperienceReference.ecer_Type = ecer_WorkExperienceTypes.ICRA;
    ecerIcraWorkExperienceReference.ecer_Origin = ecer_Origin.Portal;

    context.AddObject(ecerIcraWorkExperienceReference);
    context.AddLink(icraEligibilityApplication, ecer_ICRAEligibilityAssessment.Fields.ecer_WorkExperienceRef_ecer_ICRAEligibilityAssessment_ecer_ICRAEligibilityAssessment, ecerIcraWorkExperienceReference);

    return mapper.Map<EmploymentReference>(ecerIcraWorkExperienceReference);
  }

  private async Task DeleteIcraWorkExperienceReferenceWithoutSave(DeleteIcraWorkExperienceReferenceRequest request)
  {
    await Task.CompletedTask;
    var icraEligibilityApplication = context.ecer_ICRAEligibilityAssessmentSet.FirstOrDefault(
      d => d.ecer_ICRAEligibilityAssessmentId == Guid.Parse(request.icraEligibilityId) && d.ecer_icraeligibilityassessment_ApplicantId.Id == Guid.Parse(request.userId));

    if (icraEligibilityApplication == null)
    {
      throw new InvalidOperationException($"Icra eligibility application '{request.icraEligibilityId}' not found");
    }
    var existingWorkExperiences = context.ecer_WorkExperienceRefSet.Where(t => t.ecer_WorkExperienceRef_ecer_ICRAEligibilityAssessment_ecer_ICRAEligibilityAssessment.Id == icraEligibilityApplication.Id).ToList();

    bool RefIdIsGuid = Guid.TryParse(request.referenceId, out Guid referenceIdGuid);
    if (RefIdIsGuid)
    {
      var oldReference = existingWorkExperiences.SingleOrDefault(t => t.Id == referenceIdGuid);
      if (oldReference != null)
      {
        context.DeleteObject(oldReference);
      }
      else
      {
        throw new InvalidOperationException($"Reference '{referenceIdGuid}' not found for icra eligibility application '{icraEligibilityApplication.Id}'");
      }
    }
  }

  public async Task<EmploymentReference> AddIcraWorkExperienceReference(AddIcraWorkExperienceReferenceRequest request, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;
    var employmentReference = await AddIcraWorkExperienceReferenceWithoutSave(request);
    context.SaveChanges();

    return employmentReference;
  }

  public async Task<EmploymentReference> ReplaceIcraWorkExperienceReference(ReplaceIcraWorkExperienceReferenceRequest request, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;

    var employmentReference = await AddIcraWorkExperienceReferenceWithoutSave(new AddIcraWorkExperienceReferenceRequest(request.employmentReference, request.icraEligibilityId, request.userId));
    await DeleteIcraWorkExperienceReferenceWithoutSave(new DeleteIcraWorkExperienceReferenceRequest(request.icraEligibilityId, request.referenceId, request.userId));
    context.SaveChanges();

    return employmentReference;
  }

  public async Task<EmploymentReference> GetIcraWorkExperienceReferenceById(string referenceId, string applicantId, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;

    var reference = context.ecer_WorkExperienceRefSet.Where(
      t => t.ecer_Applicantid.Id == Guid.Parse(applicantId) &&
      t.ecer_WorkExperienceRefId == Guid.Parse(referenceId))
    .FirstOrDefault();

    return mapper.Map<EmploymentReference>(reference);
  }
}
