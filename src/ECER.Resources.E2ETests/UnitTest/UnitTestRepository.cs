using ECER.Utilities.DataverseSdk.Model;

namespace ECER.Resources.E2ETests.UnitTest;

internal sealed partial class UnitTestRepository : IUnitTestRepository
{
  private readonly EcerContext context;

  public UnitTestRepository(EcerContext context)
  {
    this.context = context;
  }

  public async Task<string> SetIcraEligibility(string icraEligibilityId, bool eligible, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;
    var icra = context.ecer_ICRAEligibilityAssessmentSet.FirstOrDefault(d => d.ecer_ICRAEligibilityAssessmentId == Guid.Parse(icraEligibilityId) && d.ecer_ApplicantIdName.Contains("TEST"));
    if (icra == null) throw new InvalidOperationException($"ICRA Eligibility '{icraEligibilityId}' not found or this application does not belong to a test account");

    icra.StatusCode = eligible ? ecer_ICRAEligibilityAssessment_StatusCode.Eligible : ecer_ICRAEligibilityAssessment_StatusCode.Ineligible;
    context.UpdateObject(icra);
    context.SaveChanges();
    return icraEligibilityId;
  }

  public async Task CancelApplication(string applicationId, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;
    var application = context.ecer_ApplicationSet.FirstOrDefault(
      d => d.ecer_ApplicationId == Guid.Parse(applicationId) && d.ecer_ApplicantidName.Contains("TEST")
      );
    if (application == null) throw new InvalidOperationException($"Application '{applicationId}' not found or user is not a test account");
    application.StatusCode = ecer_Application_StatusCode.Withdrawn;
    application.StateCode = ecer_application_statecode.Inactive;
    context.UpdateObject(application);
    context.SaveChanges();
  }

  public async Task DeletePspRep(string pspRepId, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;
    var pspRep = ((IEnumerable<ecer_ECEProgramRepresentative>)context.ecer_ECEProgramRepresentativeSet)
      .FirstOrDefault(d => d.ecer_ECEProgramRepresentativeId == Guid.Parse(pspRepId) && d.ecer_EmailAddress.Contains("TEST", StringComparison.OrdinalIgnoreCase));
    if (pspRep == null) throw new InvalidOperationException($"PSP Program Rep '{pspRepId}' not found or user is not a test account");

    try
    {
      context.DeleteObject(pspRep);
      context.SaveChanges();
    }
    catch (Microsoft.Xrm.Sdk.SaveChangesException)
    {
      // Some Dataverse plugins block representative deletion. Fall back to disabling the test rep so reruns stay isolated.
      context.Detach(pspRep);
      var existing = context.ecer_ECEProgramRepresentativeSet.FirstOrDefault(d => d.ecer_ECEProgramRepresentativeId == Guid.Parse(pspRepId));
      if (existing == null)
      {
        return;
      }

      existing.ecer_AccessToPortal = ecer_AccessToPortal.Disabled;
      existing.ecer_HasAcceptedTermsofUse = false;
      context.UpdateObject(existing);
      context.SaveChanges();
    }
  }
}
