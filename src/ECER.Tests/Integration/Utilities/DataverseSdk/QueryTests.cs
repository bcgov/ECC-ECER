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
  private static readonly Guid contactId = Guid.Parse("73127545-c481-ef11-899c-00090faa0001");

  private static readonly Guid[] applicationIds =
  [
    Guid.Parse( "bd1d0027-8193-49e8-bfe8-03ce36f4d00b"),
    Guid.Parse("8d15dd02-21f9-48a3-92f0-3496b092632f"),
    Guid.Parse("ff8ade9b-446b-4b6f-9613-e91cbf75ebdf"),
    Guid.Parse("40188b8a-9fa7-4a2f-b065-c4f9dd3b0fdf")
  ];

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
  public void Join_OneToMany_CompleteObject()
  {
    var query = dataverseContext.ContactSet.Where(c => c.Id == contactId);

    var results = dataverseContext.From(query).Join().Include(c => c.ecer_application_Applicantid_contact).Execute();

    var contact = results.ShouldHaveSingleItem();
    contact.ecer_application_Applicantid_contact.ShouldNotBeNull().ShouldNotBeEmpty();
  }

  [Fact]
  public void Join_ManyToOne_CompleteObject()
  {
    var query = dataverseContext.ecer_ApplicationSet.Where(a => a.ecer_application_Applicantid_contact.Id == contactId);

    var results = dataverseContext.From(query).Join().Include(a => a.ecer_application_Applicantid_contact).Execute();

    results.Count().ShouldBeGreaterThan(0);
    results.ShouldAllBe(a => a.ecer_application_Applicantid_contact.Id == contactId);
  }

  [Fact]
  public void Join_OneToManyWithPaging_CorrectPageSize()
  {
    var query = dataverseContext.ecer_ApplicationSet.WhereIn(c => c.Id, applicationIds).OrderBy(a => a.Id).Skip(2).Take(2);
    var results = dataverseContext.From(query).Join().Include(c => c.ecer_transcript_Applicationid).Execute();

    results.Count().ShouldBe(2);
    results.ShouldAllBe(r => applicationIds.Any(id => r.Id == id));
    results.ShouldAllBe(r => r.ecer_transcript_Applicationid.Any());
  }

  [Fact]
  public void Aggregate_Count_Returned()
  {
    var query = dataverseContext.ecer_ApplicationSet.Where(c => c.ecer_application_Applicantid_contact.ContactId == contactId);

    var count = dataverseContext.From(query).Aggregate().Count();

    count.ShouldBeGreaterThan(0);
  }

  [Fact]
  public void Execute_SimpleQuery_Returned()
  {
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
