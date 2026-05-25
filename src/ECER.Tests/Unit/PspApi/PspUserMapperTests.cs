using ECER.Managers.Registry;
using ECER.Utilities.Security;
using Shouldly;
using ContractPspUsers = ECER.Managers.Registry.Contract.PspUsers;
using ResourcePspReps = ECER.Resources.Accounts.PspReps;

namespace ECER.Tests.Unit.PspApi;

public class PspUserMapperTests
{
  [Fact]
  public void MapPspUsers_MapsQueryResults()
  {
    var mapper = new PspUserMapper();
    var source = new ResourcePspReps.PspUser
    {
      Id = Guid.NewGuid().ToString(),
      Profile = new ResourcePspReps.PspUserProfile
      {
        Id = Guid.NewGuid().ToString(),
        FirstName = "First",
        LastName = "Last",
        PreferredName = "Preferred",
        Phone = "250-555-0101",
        PhoneExtension = "123",
        JobTitle = "Coordinator",
        Role = ResourcePspReps.PspUserRole.Primary,
        Email = "psp@example.org",
        HasAcceptedTermsOfUse = true,
      },
      AccessToPortal = ResourcePspReps.PortalAccessStatus.Active,
      PostSecondaryInstituteId = Guid.NewGuid().ToString(),
    };

    var result = mapper.MapPspUsers([source]).Single();

    result.Id.ShouldBe(source.Id);
    result.Profile.Id.ShouldBe(source.Profile.Id);
    result.Profile.FirstName.ShouldBe(source.Profile.FirstName);
    result.Profile.LastName.ShouldBe(source.Profile.LastName);
    result.Profile.PreferredName.ShouldBe(source.Profile.PreferredName);
    result.Profile.Phone.ShouldBe(source.Profile.Phone);
    result.Profile.PhoneExtension.ShouldBe(source.Profile.PhoneExtension);
    result.Profile.JobTitle.ShouldBe(source.Profile.JobTitle);
    result.Profile.Role.ShouldBe(ContractPspUsers.PspUserRole.Primary);
    result.Profile.Email.ShouldBe(source.Profile.Email);
    result.Profile.HasAcceptedTermsOfUse.ShouldBe(true);
    result.AccessToPortal.ShouldBe(ContractPspUsers.PortalAccessStatus.Active);
    result.PostSecondaryInstituteId.ShouldBe(source.PostSecondaryInstituteId);
  }

  [Fact]
  public void MapPspUserProfile_MapsEditableProfileFields()
  {
    var mapper = new PspUserMapper();
    var source = new ContractPspUsers.PspUserProfile
    {
      Id = Guid.NewGuid().ToString(),
      FirstName = "First",
      LastName = "Last",
      PreferredName = "Preferred",
      Phone = "250-555-0101",
      PhoneExtension = "123",
      JobTitle = "Coordinator",
      Role = ContractPspUsers.PspUserRole.Secondary,
      Email = "psp@example.org",
      HasAcceptedTermsOfUse = false,
    };

    var result = mapper.MapPspUserProfile(source);

    result.Id.ShouldBe(source.Id);
    result.FirstName.ShouldBe(source.FirstName);
    result.LastName.ShouldBe(source.LastName);
    result.PreferredName.ShouldBe(source.PreferredName);
    result.Phone.ShouldBe(source.Phone);
    result.PhoneExtension.ShouldBe(source.PhoneExtension);
    result.JobTitle.ShouldBe(source.JobTitle);
    result.Role.ShouldBe(ResourcePspReps.PspUserRole.Secondary);
    result.Email.ShouldBe(source.Email);
    result.HasAcceptedTermsOfUse.ShouldBe(false);
  }

  [Fact]
  public void MapRegisteredPspUser_MapsIdentityAndProfile()
  {
    var mapper = new PspUserMapper();
    var identity = new UserIdentity(Guid.NewGuid().ToString("N"), "bceidbusiness");
    var source = new ContractPspUsers.RegisterNewPspUserCommand(
      Guid.NewGuid().ToString(),
      new ContractPspUsers.PspUserProfile
      {
        FirstName = "First",
        LastName = "Last",
        Email = "psp@example.org",
        Role = ContractPspUsers.PspUserRole.Secondary,
      },
      identity);

    var result = mapper.MapRegisteredPspUser(source);

    result.Id.ShouldBe(source.Id);
    result.Identities.Single().ShouldBe(identity);
    result.Profile.FirstName.ShouldBe(source.Profile.FirstName);
    result.Profile.LastName.ShouldBe(source.Profile.LastName);
    result.Profile.Email.ShouldBe(source.Profile.Email);
    result.Profile.Role.ShouldBe(ResourcePspReps.PspUserRole.Secondary);
    result.AccessToPortal.ShouldBeNull();
    result.PostSecondaryInstituteId.ShouldBeNull();
  }
}
