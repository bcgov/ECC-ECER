

namespace ECER.Resources.E2ETests.Applications;

public interface IApplicationRepository
{
  Task<string> E2ETestsDeleteApplication(string applicationId, CancellationToken cancellationToken);

}
