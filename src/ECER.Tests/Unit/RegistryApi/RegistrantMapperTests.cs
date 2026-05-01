using ECER.Managers.Registry;
using ECER.Utilities.ObjectStorage.Providers;
using ECER.Utilities.Security;
using Shouldly;
using ContractRegistrants = ECER.Managers.Registry.Contract.Registrants;
using ResourceRegistrants = ECER.Resources.Accounts.Registrants;

namespace ECER.Tests.Unit.RegistryApi;

public class RegistrantMapperTests
{
  [Fact]
  public void MapRegisteredRegistrant_MapsIdentityAndEditableProfileFields()
  {
    var mapper = new RegistrantMapper();
    var identity = new UserIdentity(Guid.NewGuid().ToString("N"), "bcsc");
    var source = new ContractRegistrants.RegisterNewUserCommand(
      new ContractRegistrants.UserProfile
      {
        FirstName = "First",
        LastName = "Last",
        MiddleName = "Middle",
        PreferredName = "Preferred",
        AlternateContactPhone = "250-555-0102",
        DateOfBirth = new DateOnly(1990, 1, 2),
        RegistrationNumber = "12345",
        Email = "user@example.org",
        Phone = "250-555-0101",
        ResidentialAddress = new ContractRegistrants.Address("123 Main", "Unit 1", "Victoria", "V1V1V1", "BC", "Canada"),
        MailingAddress = new ContractRegistrants.Address("123 Main", "Unit 2", "Victoria", "V1V1V1", "BC", "Canada"),
        PreviousNames =
        [
          new ContractRegistrants.PreviousName("Prev", "Name")
          {
            Status = ContractRegistrants.PreviousNameStage.ReadyforVerification,
            Source = ContractRegistrants.PreviousNameSources.Profile,
            Documents =
            [
              new ContractRegistrants.IdentityDocument("doc-1")
              {
                Name = "identity.pdf",
                Url = "/files/1",
                Extention = ".pdf",
                Size = "100",
                EcerWebApplicationType = EcerWebApplicationType.Registry,
              }
            ]
          }
        ],
        IsRegistrant = true,
        Status = ContractRegistrants.StatusCode.Verified,
      },
      identity);

    var result = mapper.MapRegisteredRegistrant(source);

    result.Identities.Single().ShouldBe(identity);
    result.Profile.FirstName.ShouldBe("First");
    result.Profile.Status.ShouldBe(ResourceRegistrants.StatusCode.Verified);
    result.Profile.IDVerificationDecision.ShouldBeNull();
    result.Profile.PreviousNames.Single().Status.ShouldBe(ResourceRegistrants.PreviousNameStage.ReadyforVerification);
    result.Profile.PreviousNames.Single().Documents.Single().Id.ShouldBe("doc-1");
  }

  [Fact]
  public void MapRegistrants_MapsStatusByNameAndLeavesGivenNameUnset()
  {
    var mapper = new RegistrantMapper();
    var source = new ResourceRegistrants.Registrant
    {
      Id = Guid.NewGuid().ToString(),
      Profile = new ResourceRegistrants.UserProfile
      {
        FirstName = "First",
        LastName = "Last",
        Email = "user@example.org",
        Phone = "250-555-0101",
        Status = ResourceRegistrants.StatusCode.ReadyforRegistrantMatch,
        PreviousNames =
        [
          new ResourceRegistrants.PreviousName("Prev", "Name")
          {
            Source = ResourceRegistrants.PreviousNameSources.NameLog,
            Status = ResourceRegistrants.PreviousNameStage.Archived
          }
        ]
      }
    };

    var result = mapper.MapRegistrants([source]).Single();

    result.UserId.ShouldBe(source.Id);
    result.Profile.Status.ShouldBe(ContractRegistrants.StatusCode.ReadyforRegistrantMatch);
    result.Profile.GivenName.ShouldBeNull();
    result.Profile.PreviousNames.Single().Source.ShouldBe(ContractRegistrants.PreviousNameSources.NameLog);
    result.Profile.PreviousNames.Single().Status.ShouldBe(ContractRegistrants.PreviousNameStage.Archived);
  }
}
