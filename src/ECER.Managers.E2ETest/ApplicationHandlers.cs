
using ECER.Resources.E2ETests.Applications;
using MediatR;
using ECER.Managers.E2ETest.Contract.Applications;

namespace ECER.Managers.E2ETest;

/// <summary>
/// Message handlers
/// </summary>
public class ApplicationHandlers(IApplicationRepository applicationRepository) : IRequestHandler<E2ETestsDeleteApplicationCommand, string>
{
  public async Task<string> Handle(E2ETestsDeleteApplicationCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var applicationId = await applicationRepository.E2ETestsDeleteApplication(request.applicationId, cancellationToken);

    return applicationId;
  }
}
