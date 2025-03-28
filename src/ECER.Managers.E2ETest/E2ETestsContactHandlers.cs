﻿
using ECER.Resources.E2ETests.E2ETestsContacts;
using MediatR;
using ECER.Managers.E2ETest.Contract.E2ETestsContacts;

namespace ECER.Managers.E2ETest;

/// <summary>
/// Message handlers
/// </summary>
public class E2ETestsContactHandlers(IE2ETestsContactRepository E2ETestsContactRepository) : IRequestHandler<E2ETestsDeleteContactApplicationsCommand, string>
{
  public async Task<string> Handle(E2ETestsDeleteContactApplicationsCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var contactId = await E2ETestsContactRepository.E2ETestsDeleteContactApplications(request.contactId, cancellationToken);

    return contactId;
  }
}
