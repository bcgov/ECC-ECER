using ECER.Utilities.DataverseSdk.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk.Client;
using Shouldly;
using Xunit.Categories;

namespace ECER.Tests.Integration.Utilities.DataverseSdk;

[IntegrationTest]
public class QueryTests : IAsyncLifetime
{
  private readonly IConfigurationRoot configuration;
  private ServiceClient serviceClient = null!;
  private EcerContext dataverseContext = null!;

  public QueryTests()
  {
    var configBuilder = new ConfigurationBuilder().AddUserSecrets(typeof(Clients.RegistryPortal.Server.Program).Assembly);
    configuration = configBuilder.Build();
  }

  [Fact]
  public void WhereIn_Filtered()
  {
    var statuses = new[] { ecer_Application_StatusCode.Draft, ecer_Application_StatusCode.Complete };
    var query = dataverseContext.ecer_ApplicationSet.WhereIn(a => a.StatusCode!.Value, statuses).ToList();
    query.ShouldNotBeEmpty();
    query.ShouldAllBe(a => statuses.Contains(a.StatusCode!.Value));
  }

  [Fact]
  public void WhereNotIn_Filtered()
  {
    var statuses = new[] { ecer_Application_StatusCode.Draft, ecer_Application_StatusCode.Complete };
    var query = dataverseContext.ecer_ApplicationSet.WhereNotIn(a => a.StatusCode!.Value, statuses).ToList();
    query.ShouldNotBeEmpty();
    query.ShouldAllBe(a => !statuses.Contains(a.StatusCode!.Value));
  }

  [Fact]
  public void Execute_WithIncludeOneToMany_CompleteObject()
  {
    var contactId = Guid.Parse("457ba5d2-0b1b-4c5c-af8b-4ae5ee6ba3ac");
    var query = dataverseContext.ContactSet.Where(c => c.Id == contactId);

    var results = dataverseContext.From(query).Include(c => c.ecer_application_Applicantid_contact).Execute();

    var contact = results.ShouldHaveSingleItem();
    contact.ecer_application_Applicantid_contact.ShouldNotBeNull().ShouldNotBeEmpty();
  }

  [Fact]
  public void Execute_WithIncludeOneToOne_CompleteObject()
  {
    var contactId = Guid.Parse("457ba5d2-0b1b-4c5c-af8b-4ae5ee6ba3ac");
    var query = dataverseContext.ecer_ApplicationSet.Where(a => a.ecer_application_Applicantid_contact.Id == contactId);

    var results = dataverseContext.From(query).Include(a => a.ecer_application_Applicantid_contact).Execute();

    results.Count().ShouldBe(2);
    results.ShouldAllBe(a => a.ecer_application_Applicantid_contact.Id == contactId);
  }

  public async Task InitializeAsync()
  {
    await Task.CompletedTask;
    serviceClient = new ServiceClient(configuration.GetValue<string>("Dataverse:ConnectionString"));
    dataverseContext = new EcerContext(serviceClient);
  }

  public async Task DisposeAsync()
  {
    await Task.CompletedTask;
    dataverseContext.Dispose();
    serviceClient.Dispose();
  }
}
