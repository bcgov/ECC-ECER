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
}
