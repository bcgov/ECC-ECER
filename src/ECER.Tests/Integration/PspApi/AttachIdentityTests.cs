using System.Linq;
using ECER.Resources.Accounts.PspReps;
using ECER.Utilities.DataverseSdk.Model;
using ECER.Utilities.Security;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xrm.Sdk;
using Shouldly;
using Xunit.Abstractions;

namespace ECER.Tests.Integration.PspApi;

public class AttachIdentityTests : PspPortalWebAppScenarioBase
{
  private readonly IPspRepRepository repository;
  private readonly EcerContext context;

  public AttachIdentityTests(ITestOutputHelper output, PspPortalWebAppFixture fixture) : base(output, fixture)
  {
    repository = Fixture.Services.GetRequiredService<IPspRepRepository>();
    context = Fixture.Services.GetRequiredService<EcerContext>();
  }

  [Fact]
  public async Task AttachIdentity_SetsPortalAccessToActive()
  {
    var institute = context.ecer_PostSecondaryInstituteSet.Single(i => i.Id == Guid.Parse(Fixture.PostSecondaryInstituteId));

    var representativeId = Guid.NewGuid();
    var representative = new ecer_ECEProgramRepresentative
    {
      Id = representativeId,
      ecer_ECEProgramRepresentativeId = representativeId,
      ecer_Name = $"{Fixture.TestRunId}psp_attach_identity",
      ecer_FirstName = $"{Fixture.TestRunId}psp_attach_identity_first",
      ecer_LastName = "autotest",
      ecer_PreferredFirstName = $"{Fixture.TestRunId}psp_attach_identity_first",
      ecer_EmailAddress = "attach_identity@test.gov.bc.ca",
      ecer_PhoneNumber = "5555555555",
      ecer_Role = "Program Representative",
      ecer_RepresentativeRole = ecer_RepresentativeRole.Secondary,
      ecer_AccessToPortal = ecer_AccessToPortal.Invited,
      ecer_HasAcceptedTermsofUse = true,
      StateCode = ecer_eceprogramrepresentative_statecode.Active,
      StatusCode = ecer_ECEProgramRepresentative_StatusCode.Active,
      ecer_PostSecondaryInstitute = new EntityReference(ecer_PostSecondaryInstitute.EntityLogicalName, institute.Id)
    };

    context.AddObject(representative);
    context.AddLink(representative, new Relationship(ecer_ECEProgramRepresentative.Fields.ecer_eceprogramrepresentative_PostSecondaryIns), institute);
    context.SaveChanges();

    var identity = new UserIdentity($"{Fixture.TestRunId}attach_identity_user", "bceidbusiness");
    var user = new PspUser
    {
      Id = representativeId.ToString(),
      Identities = new[] { identity }
    };

    await repository.AttachIdentity(user, CancellationToken.None);

    var updated = (await repository.Query(new PspRepQuery { ById = representativeId.ToString() }, CancellationToken.None)).Single();

    updated.AccessToPortal.ShouldBe(PortalAccessStatus.Active);
    var authentications = context.ecer_AuthenticationSet.Where(a =>
      a.ecer_eceprogramrepresentative != null &&
      a.ecer_eceprogramrepresentative.Id == representativeId &&
      a.ecer_ExternalID == identity.UserId &&
      a.ecer_IdentityProvider == identity.IdentityProvider).ToList();

    authentications.ShouldNotBeEmpty();
  }
}
