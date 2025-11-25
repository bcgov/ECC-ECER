namespace ECER.Resources.E2ETests.E2ETestsContacts;

public interface IUnitTestRepository
{
  Task CancelApplication(string applicationId, CancellationToken cancellationToken);

  Task<string> SetIcraEligibility(string icraEligibilityId, bool eligible, CancellationToken cancellationToken);
}
