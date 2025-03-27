using AutoMapper;
using ECER.Utilities.DataverseSdk.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Xrm.Sdk;

namespace ECER.Resources.E2ETests.E2ETestsContacts;

internal sealed partial class E2ETestsContactRepository : IE2ETestsContactRepository
{
    private readonly EcerContext context;
    private readonly IMapper mapper;
    private readonly IConfiguration configuration;

  public E2ETestsContactRepository(
         EcerContext context,
         IMapper mapper,
         IConfiguration configuration)
  {
    this.context = context;
    this.mapper = mapper;
    this.configuration = configuration;
  }


  public async Task<string> E2ETestsDeleteContactApplications(string contactId, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;

    if (!Guid.TryParse(contactId, out Guid contactGuid))
    {
      throw new ArgumentException("Invalid contact ID", nameof(contactId));
    }

    // Validate that the application exists.
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
      var response = (ecer_CLEANUPDeleteContactApplicationsActionResponse) context.Execute(request);
      if(response.Completed)
      {
        return contactId;
      }

      else
      {
        throw new InvalidOperationException("Custom Action failed to Complete");
      }
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException("Failed to execute custom action", ex);
    }
  }


}
