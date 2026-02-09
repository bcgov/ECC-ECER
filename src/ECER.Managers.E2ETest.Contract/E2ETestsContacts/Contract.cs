using MediatR;

namespace ECER.Managers.E2ETest.Contract.E2ETestsContacts;

/// <summary>
/// Invokes Delete Contact Applications via custom action use case
/// </summary>
public record E2ETestsDeleteContactApplicationsCommand(string contactId) : IRequest<string>;

public record E2ETestsGenerateCertificateCommand(string applicationId, bool CertIsActive, bool IsExpiredMoreThan5Years) : IRequest<string>;
