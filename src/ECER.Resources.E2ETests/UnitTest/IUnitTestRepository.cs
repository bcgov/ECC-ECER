namespace ECER.Resources.E2ETests.UnitTest;

public interface IUnitTestRepository
{
  Task CancelApplication(string applicationId, CancellationToken cancellationToken);

  Task<string> SetIcraEligibility(string icraEligibilityId, bool eligible, CancellationToken cancellationToken);
  
  Task DeletePspRep(string pspRepId, CancellationToken cancellationToken);
}
