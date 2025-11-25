using ECER.Utilities.DataverseSdk.Model;

namespace ECER.Resources.E2ETests.E2ETestsContacts;

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
    application.StatusCode = ecer_Application_StatusCode.Cancelled;
    application.StateCode = ecer_application_statecode.Inactive;
    context.UpdateObject(application);
    context.SaveChanges();
  }
}
