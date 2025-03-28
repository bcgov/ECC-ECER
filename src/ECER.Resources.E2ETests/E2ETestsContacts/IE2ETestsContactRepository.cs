

namespace ECER.Resources.E2ETests.E2ETestsContacts;

public interface IE2ETestsContactRepository
{
  Task<string> E2ETestsDeleteContactApplications(string contactId, CancellationToken cancellationToken);

}
