using ECER.Utilities.DataverseSdk.Model;
using ECER.Utilities.DataverseSdk.Queries;
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
    var contactId = Guid.Parse("e761c9fc-0781-ef11-9039-00155d000103");
    var query = dataverseContext.ContactSet.Where(c => c.Id == contactId);

    var results = dataverseContext.From(query).Join().Include(c => c.ecer_application_Applicantid_contact).Execute();

    var contact = results.ShouldHaveSingleItem();
    contact.ecer_application_Applicantid_contact.ShouldNotBeNull().ShouldNotBeEmpty();
  }

  [Fact]
  public void Execute_WithIncludeOneToOne_CompleteObject()
  {
    var contactId = Guid.Parse("e761c9fc-0781-ef11-9039-00155d000103");
    var query = dataverseContext.ecer_ApplicationSet.Where(a => a.ecer_application_Applicantid_contact.Id == contactId);

    var results = dataverseContext.From(query).Join().Include(a => a.ecer_application_Applicantid_contact).Execute();

    results.Count().ShouldBeGreaterThan(0);
    results.ShouldAllBe(a => a.ecer_application_Applicantid_contact.Id == contactId);
  }

  [Fact]
  public void Execute_Count_Returned()
  {
    var contactId = Guid.Parse("e761c9fc-0781-ef11-9039-00155d000103");
    var query = dataverseContext.ecer_ApplicationSet.Where(c => c.ecer_application_Applicantid_contact.ContactId == contactId);

    var count = dataverseContext.From(query).Aggregate().Count();

    count.ShouldBeGreaterThan(0);
  }

  [Fact]
  public void Execute_SingleEntity_Returned()
  {
    var contactId = Guid.Parse("e761c9fc-0781-ef11-9039-00155d000103");
    var query = dataverseContext.ecer_ApplicationSet.Where(a => a.ecer_application_Applicantid_contact.Id == contactId);

    var results = dataverseContext.From(query).Execute();

    results.Count().ShouldBeGreaterThan(0);
    results.ShouldAllBe(r => r.Id != Guid.Empty);
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
