using PortalApplications = ECER.Clients.RegistryPortal.Server.Applications;
using ContractApplications = ECER.Managers.Registry.Contract.Applications;
using ContractMetadatas = ECER.Managers.Admin.Contract.Metadatas;
using Shouldly;

namespace ECER.Tests.Unit.RegistryPortal;

public class ApplicationMapperTests
{
  [Fact]
  public void MapApplication_PreservesTranscriptLocationMetadata()
  {
    var mapper = new PortalApplications.ApplicationMapper();
    var provinceId = Guid.NewGuid().ToString();

    var source = new ContractApplications.Application(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), ContractApplications.ApplicationStatus.Draft)
    {
      CreatedOn = new DateTime(2025, 4, 20),
      Transcripts =
      [
        new ContractApplications.Transcript(Guid.NewGuid().ToString(), "Northern College", "ECE", "12345", new DateTime(2024, 1, 1), new DateTime(2024, 12, 31), false, "Sam", "Student", false, ContractApplications.EducationRecognition.Recognized, ContractApplications.EducationOrigin.InsideBC)
        {
          Country = new ContractMetadatas.Country(Guid.NewGuid().ToString(), "Canada", "CA", true),
          Province = new ContractMetadatas.Province(provinceId, "British Columbia", "BC"),
          PostSecondaryInstitution = new ContractMetadatas.PostSecondaryInstitution(Guid.NewGuid().ToString(), "Northern College", provinceId),
        }
      ]
    };

    var result = mapper.MapApplication(source);
    var transcript = result.Transcripts.Single();

    transcript.Country.ShouldNotBeNull();
    transcript.Country.CountryName.ShouldBe("Canada");
    transcript.Country.CountryCode.ShouldBe("CA");
    transcript.Country.IsICRA.ShouldBeTrue();

    transcript.Province.ShouldNotBeNull();
    transcript.Province.ProvinceName.ShouldBe("British Columbia");
    transcript.Province.ProvinceCode.ShouldBe("BC");

    transcript.PostSecondaryInstitution.ShouldNotBeNull();
    transcript.PostSecondaryInstitution.Name.ShouldBe("Northern College");
    transcript.PostSecondaryInstitution.ProvinceId.ShouldBe(provinceId);
  }
}
