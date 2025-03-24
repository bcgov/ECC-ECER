using MediatR;

namespace ECER.Managers.E2ETest.Contract.Applications;



/// <summary>
/// Invokes Delete Application custom action use case
/// </summary>
public record E2ETestsDeleteApplicationCommand(string applicationId) : IRequest<string>;

