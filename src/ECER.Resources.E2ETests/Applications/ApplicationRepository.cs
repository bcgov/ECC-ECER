using AutoMapper;
using ECER.Utilities.DataverseSdk.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Xrm.Sdk;

namespace ECER.Resources.E2ETests.Applications;

internal sealed partial class ApplicationRepository : IApplicationRepository
{
    private readonly EcerContext context;
    private readonly IMapper mapper;
    private readonly IConfiguration configuration;

  public ApplicationRepository(
         EcerContext context,
         IMapper mapper,
         IConfiguration configuration)
  {
    this.context = context;
    this.mapper = mapper;
    this.configuration = configuration;
  }


  public async Task<string> E2ETestsDeleteApplication(string applicationId, CancellationToken cancellationToken)
  {

    if (!Guid.TryParse(applicationId, out Guid appGuid))
    {
      throw new ArgumentException("Invalid application ID", nameof(applicationId));
    }

    // Validate that the application exists.
    var application = context.ecer_ApplicationSet.SingleOrDefault(a => a.ecer_ApplicationId == appGuid);
    if (application == null)
    {
      throw new InvalidOperationException($"Application '{applicationId}' not found");
    }

    // Create the request for the custom action.
    var request = new ecer_CLEANUPDeleteApplicationActionRequest
    {
      ApplicationID = applicationId,
      Target = new EntityReference(application.LogicalName, application.Id)
    };   


    try
    {
      var response = (ecer_CLEANUPDeleteApplicationActionResponse) context.Execute(request);

      // Set the timeout duration and capture the start time.
      TimeSpan timeout = TimeSpan.FromSeconds(10);
      DateTime startTime = DateTime.UtcNow;

      while (DateTime.UtcNow - startTime < timeout)
      {
        application = context.ecer_ApplicationSet.SingleOrDefault(a => a.ecer_ApplicationId == appGuid);
        if (application != null)
        {
          context.Detach(application);
        }
        else
        {
          break;
        }
        
        await Task.Delay(2000, cancellationToken);
      }
      return applicationId;
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException("Failed to execute custom action", ex);
    }
  }


}
