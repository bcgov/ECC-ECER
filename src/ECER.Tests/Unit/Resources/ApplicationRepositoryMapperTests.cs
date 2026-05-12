using ECER.Resources.Documents.Applications;
using ECER.Utilities.DataverseSdk.Model;
using Microsoft.Xrm.Sdk;
using Shouldly;

namespace ECER.Tests.Unit.DocumentsResources;

public class ApplicationRepositoryMapperTests
{
  [Fact]
  public void MapApplications_UsesLoadedTranscriptRelationshipsForLocationMetadata()
  {
    var mapper = new ApplicationRepositoryMapper();
    var countryId = Guid.NewGuid();
    var provinceId = Guid.NewGuid();
    var institutionId = Guid.NewGuid();

    var application = new ecer_Application
    {
      ecer_ApplicationId = Guid.NewGuid(),
      ecer_Applicantid = new EntityReference("contact", Guid.NewGuid()),
      StatusCode = ecer_Application_StatusCode.Draft,
      ecer_transcript_Applicationid =
      [
        new ecer_Transcript
        {
          ecer_TranscriptId = Guid.NewGuid(),
          ecer_StartDate = new DateTime(2024, 1, 1),
          ecer_EndDate = new DateTime(2024, 12, 31),
          ecer_StudentFirstName = "Sam",
          ecer_StudentLastName = "Student",
          ecer_transcript_InstituteCountryId = new ecer_Country
          {
            ecer_CountryId = countryId,
            ecer_Name = "Canada",
            ecer_ShortName = "CA",
            ecer_EligibleforICRA = true,
          },
          ecer_transcript_ProvinceId = new ecer_Province
          {
            ecer_ProvinceId = provinceId,
            ecer_Name = "British Columbia",
            ecer_Abbreviation = "BC",
          },
          ecer_transcript_postsecondaryinstitutionid = new ecer_PostSecondaryInstitute
          {
            ecer_PostSecondaryInstituteId = institutionId,
            ecer_Name = "Northern College",
            ecer_ProvinceId = new EntityReference("ecer_province", provinceId),
          }
        }
      ]
    };

    var result = mapper.MapApplications([application]).Single();
    var transcript = result.Transcripts.Single();

    transcript.Country.ShouldNotBeNull();
    transcript.Country.CountryId.ShouldBe(countryId.ToString());
    transcript.Country.CountryName.ShouldBe("Canada");
    transcript.Country.CountryCode.ShouldBe("CA");
    transcript.Country.IsICRA.ShouldBeTrue();

    transcript.Province.ShouldNotBeNull();
    transcript.Province.ProvinceId.ShouldBe(provinceId.ToString());
    transcript.Province.ProvinceName.ShouldBe("British Columbia");
    transcript.Province.ProvinceCode.ShouldBe("BC");

    transcript.PostSecondaryInstitution.ShouldNotBeNull();
    transcript.PostSecondaryInstitution.Id.ShouldBe(institutionId.ToString());
    transcript.PostSecondaryInstitution.Name.ShouldBe("Northern College");
    transcript.PostSecondaryInstitution.ProvinceId.ShouldBe(provinceId.ToString());
  }
}
