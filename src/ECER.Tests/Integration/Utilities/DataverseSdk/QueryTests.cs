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
  private static readonly Guid contactId = Guid.Parse("34852992-be02-4815-a4ef-9cd1f934109d");
  private static string applicationId = "0e40773d-39a7-47cb-93a7-47fe46ec7e74";

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
    var secretsFile = Environment.GetEnvironmentVariable("SECRETS_FILE_PATH");
    if (secretsFile != null && File.Exists(secretsFile)) configBuilder.AddJsonFile(secretsFile, true);
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
    var query = dataverseContext.ecer_ApplicationSet.Where(a => a.ecer_application_Applicantid_contact.Id == contactId);

    var results = dataverseContext.From(query).Join().Include(a => a.ecer_application_Applicantid_contact).Execute();

    results.Count().ShouldBeGreaterThan(0);
    results.ShouldAllBe(a => a.ecer_application_Applicantid_contact.Id == contactId);
  }

  [Fact]
  public void Join_OneToMany_NestedObject()
  {
    var query = dataverseContext.ecer_CertificateSet.Where(a => a.ecer_HasConditions == true).Take(1);

    var results = dataverseContext.From(query).Join()
      .Include(a => a.ecer_certificate_Registrantid)
      .IncludeNested(a => a.ecer_certificateconditions_Registrantid)
      .Execute();

    var result = results.FirstOrDefault();
    result.ShouldNotBeNull();
    result!.ecer_certificate_Registrantid.ShouldNotBeNull();
    var output = result.ecer_certificate_Registrantid.ecer_certificateconditions_Registrantid;
    output.ShouldNotBeNull();
  }

  [Theory]
  [InlineData(0, 2)] // Page number 0, page size 2
  [InlineData(2, 2)] // Page number 2, page size 2
  public void Join_OneToManyWithPaging_CorrectPageSize(int pageNumber, int pageSize)
  {
    var query = dataverseContext.ecer_ApplicationSet
                .WhereIn(c => c.Id, applicationIds)
                .OrderBy(a => a.Id)
                .Skip(pageNumber)
                .Take(pageSize);

    var results = dataverseContext
                  .From(query)
                  .Join()
                  .Include(c => c.ecer_transcript_Applicationid)
                  .Execute();

    results.Count().ShouldBe(pageSize);
    results.ShouldAllBe(r => applicationIds.Any(id => r.Id == id));
    results.ShouldAllBe(r => r.ecer_transcript_Applicationid.Any());
  }

  [Fact]
  public void Join_OneToMany_NestedObjectApplication()
  {
    var query = dataverseContext.ecer_ApplicationSet.Where(a => a.ecer_ApplicationId == Guid.Parse(applicationId));

    var results = dataverseContext.From(query).Join()
      .Include(a => a.ecer_ecer_professionaldevelopment_Applicationi)
      .IncludeNested(a => a.ecer_bcgov_documenturl_ProfessionalDevelopmentId)
      .Execute();

    var result = results.FirstOrDefault();
    result.ShouldNotBeNull();
    result!.ecer_ecer_professionaldevelopment_Applicationi.ShouldNotBeNull();

    foreach (var item in result!.ecer_ecer_professionaldevelopment_Applicationi)
    {
      item.ecer_bcgov_documenturl_ProfessionalDevelopmentId.ShouldNotBeNull();
    }
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
