using AutoMapper;
using ECER.Utilities.DataverseSdk.Model;
using ECER.Utilities.DataverseSdk.Queries;
using Microsoft.Extensions.Configuration;
using Microsoft.Xrm.Sdk;

namespace ECER.Resources.E2ETests.E2ETestsContacts;

internal sealed partial class E2ETestsContactRepository : IE2ETestsContactRepository
{
  private readonly EcerContext context;

  public E2ETestsContactRepository(EcerContext context)
  {
    this.context = context;
  }

  public async Task<string> E2ETestsDeleteContactApplications(string contactId, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;

    if (!Guid.TryParse(contactId, out Guid contactGuid))
    {
      throw new ArgumentException("Invalid contact ID", nameof(contactId));
    }

    // Validate that the contact exists.
    var contact = context.ContactSet.SingleOrDefault(a => a.ContactId == contactGuid);
    if (contact == null)
    {
      throw new InvalidOperationException($"Contact '{contactId}' not found");
    }

    // Create the request for the custom action.
    var request = new ecer_CLEANUPDeleteContactApplicationsActionRequest
    {
      ContactID = contactId,
      Target = new EntityReference(contact.LogicalName, contact.Id)
    };

    try
    {
      var response = (ecer_CLEANUPDeleteContactApplicationsActionResponse)context.Execute(request);
      if (response.Completed)
      {
        return contactId;
      }
      else
      {
        throw new InvalidOperationException(" Custom Action - 'ecer_CLEANUPDeleteContactApplicationsAction' failed to Complete");
      }
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException("Failed to execute custom action - 'ecer_CLEANUPDeleteContactApplicationsAction'", ex);
    }
  }

  public async Task<string> E2ETestsGenerateCertificate(string applicationId, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;

    if (!Guid.TryParse(applicationId, out Guid applicationGuid))
    {
      throw new ArgumentException("Invalid application ID", nameof(applicationId));
    }

    // Validate that the application is submitted.
    var application = context.ecer_ApplicationSet.SingleOrDefault(a => a.Id == applicationGuid && a.StatusCode == ecer_Application_StatusCode.Submitted);
    if (application == null)
    {
      throw new InvalidOperationException($"Application '{applicationId}' not found/submitted");
    }

    application.StatusCode = ecer_Application_StatusCode.Decision;
    application.ecer_StatusReasonDetail = ecer_ApplicationStatusReasonDetail.Certified;
    application.ecer_GenerateCertificateRecord = ecer_YesNoNull.Yes;

    context.UpdateObject(application);
    context.SaveChanges();

    // Wait for 10 seconds with cancellation support
    await Task.Delay(10000, cancellationToken);

    // Retrieve the updated application
    var updatedApplication = context.ecer_ApplicationSet.SingleOrDefault(a => a.Id == applicationGuid);
    if (updatedApplication == null)
    {
      throw new InvalidOperationException($"Application not found");
    }

    var certificate = context.ecer_CertificateSet.SingleOrDefault(c => c.ecer_CertificateId == updatedApplication.ecer_Certificateid.Id);
    if (certificate == null)
    {
      throw new InvalidOperationException($"certificate '{application.ecer_Certificateid.Id}' not found");
    }
    certificate.ecer_EffectiveDate = DateTime.UtcNow.AddMonths(-62);
    certificate.ecer_ExpiryDate = DateTime.UtcNow.AddMonths(-2);
    context.UpdateObject(certificate);
    context.SaveChanges();

    return applicationId;
  }
}
