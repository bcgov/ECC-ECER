using ECER.Managers.Registry;
using Shouldly;
using ContractApplications = ECER.Managers.Registry.Contract.Applications;
using ContractMetadatas = ECER.Managers.Admin.Contract.Metadatas;
using ResourceApplications = ECER.Resources.Documents.Applications;
using ResourceMetadata = ECER.Resources.Documents.MetadataResources;

namespace ECER.Tests.Unit.RegistryApi;

public class ApplicationMapperTests
{
  [Fact]
  public void MapApplication_ContractToResource_PreservesFieldsAndLeavesStatusDefault()
  {
    var mapper = new ApplicationMapper();
    var source = new ContractApplications.Application(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), ContractApplications.ApplicationStatus.Submitted)
    {
      CertificationTypes = [ContractApplications.CertificationType.OneYear, ContractApplications.CertificationType.Sne],
      SignedDate = new DateTime(2025, 1, 2),
      Stage = "Documents",
      FromCertificate = Guid.NewGuid().ToString(),
      ApplicationType = ContractApplications.ApplicationTypes.Renewal,
      EducationOrigin = ContractApplications.EducationOrigin.InsideBC,
      EducationRecognition = ContractApplications.EducationRecognition.Recognized,
      OneYearRenewalExplanationChoice = ContractApplications.OneYearRenewalexplanations.Other,
      FiveYearRenewalExplanationChoice = ContractApplications.FiveYearRenewalExplanations.Other,
      RenewalExplanationOther = "Explanation",
      Origin = ContractApplications.ApplicationOrigin.Portal,
      LabourMobilityCertificateInformation = new ContractApplications.CertificateInformation
      {
        CertificateComparisonId = Guid.NewGuid().ToString(),
        LabourMobilityProvince = new ContractMetadatas.Province(Guid.NewGuid().ToString(), "British Columbia", "BC"),
        CurrentCertificationNumber = "12345",
        ExistingCertificationType = "ECE",
        LegalFirstName = "Legal",
        LegalMiddleName = "Middle",
        LegalLastName = "Last",
        HasOtherName = true,
      },
      Transcripts =
      [
        new ContractApplications.Transcript(Guid.NewGuid().ToString(), "Institute", "Program", "123456", new DateTime(2020, 1, 1), new DateTime(2021, 1, 1), true, "Student", "Last", false, ContractApplications.EducationRecognition.Recognized, ContractApplications.EducationOrigin.InsideBC)
        {
          Country = new ContractMetadatas.Country(Guid.NewGuid().ToString(), "Canada", "CA", false),
          Province = new ContractMetadatas.Province(Guid.NewGuid().ToString(), "British Columbia", "BC"),
          PostSecondaryInstitution = new ContractMetadatas.PostSecondaryInstitution(Guid.NewGuid().ToString(), "Institute", Guid.NewGuid().ToString()),
          TranscriptStatusOption = ContractApplications.TranscriptStatusOptions.OfficialTranscriptRequested,
          CourseOutlineOptions = ContractApplications.CourseOutlineOptions.UploadNow,
          ProgramConfirmationOptions = ContractApplications.ProgramConfirmationOptions.RegistryAlreadyHas,
          ComprehensiveReportOptions = ContractApplications.ComprehensiveReportOptions.FeeWaiver,
          CourseOutlineFiles = [new ContractApplications.FileInfo("file-1") { Name = "outline.pdf", Url = "/files/1", Extention = ".pdf", Size = "100" }],
        }
      ],
      ProfessionalDevelopments =
      [
        new ContractApplications.ProfessionalDevelopment(Guid.NewGuid().ToString(), "Course", "Org", new DateTime(2024, 1, 1), new DateTime(2024, 1, 2))
        {
          Status = ContractApplications.ProfessionalDevelopmentStatusCode.Submitted,
        }
      ],
      WorkExperienceReferences =
      [
        new ContractApplications.WorkExperienceReference("Jane", "Doe", "jane@example.org", 500)
        {
          Status = ContractApplications.WorkExperienceRefStage.Submitted,
          Type = ContractApplications.WorkExperienceTypes.Is500Hours,
        }
      ],
      CharacterReferences =
      [
        new ContractApplications.CharacterReference("Ref", "Person", "250-555-0101", "ref@example.org")
        {
          Status = ContractApplications.CharacterReferenceStage.Submitted,
        }
      ]
    };

    var result = mapper.MapApplication(source);

    result.Id.ShouldBe(source.Id);
    result.ApplicantId.ShouldBe(source.RegistrantId);
    result.Status.ShouldBe(ResourceApplications.ApplicationStatus.Draft);
    result.CertificationTypes.ShouldBe([ResourceApplications.CertificationType.OneYear, ResourceApplications.CertificationType.Sne]);
    result.ApplicationType.ShouldBe(ResourceApplications.ApplicationTypes.Renewal);
    result.Origin.ShouldBe(ResourceApplications.ApplicationOrigin.Portal);
    result.Stage.ShouldBe("Documents");
    result.Transcripts.Single().Country!.CountryName.ShouldBe("Canada");
    result.Transcripts.Single().CourseOutlineFiles.Single().Id.ShouldBe("file-1");
    result.ProfessionalDevelopments.Single().Status.ShouldBe(ResourceApplications.ProfessionalDevelopmentStatusCode.Submitted);
    result.WorkExperienceReferences.Single().Type.ShouldBe(ResourceApplications.WorkExperienceTypes.Is500Hours);
    result.CharacterReferences.Single().Status.ShouldBe(ResourceApplications.CharacterReferenceStage.Submitted);
    result.LabourMobilityCertificateInformation!.LabourMobilityProvince!.ProvinceCode.ShouldBe("BC");
  }

  [Fact]
  public void MapApplication_ResourceToContract_MapsApplicationStatusByName()
  {
    var mapper = new ApplicationMapper();

    mapper.MapApplication(new ResourceApplications.Application("1", "user", [])
    {
      Status = ResourceApplications.ApplicationStatus.Closed
    })!.Status.ShouldBe(ContractApplications.ApplicationStatus.Closed);

    mapper.MapApplication(new ResourceApplications.Application("2", "user", [])
    {
      Status = ResourceApplications.ApplicationStatus.Reconsideration
    })!.Status.ShouldBe(ContractApplications.ApplicationStatus.Reconsideration);

    mapper.MapApplication(new ResourceApplications.Application("3", "user", [])
    {
      Status = ResourceApplications.ApplicationStatus.Cancelled
    })!.Status.ShouldBe(ContractApplications.ApplicationStatus.Cancelled);
  }

  [Fact]
  public void MapApplication_ResourceToContract_PreservesTranscriptLocationMetadata()
  {
    var mapper = new ApplicationMapper();
    var provinceId = Guid.NewGuid().ToString();

    var source = new ResourceApplications.Application(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), [])
    {
      Transcripts =
      [
        new ResourceApplications.Transcript(Guid.NewGuid().ToString(), "Northern College", "ECE", "12345", new DateTime(2024, 1, 1), new DateTime(2024, 12, 31), false, "Sam", "Student", false, ResourceApplications.EducationRecognition.Recognized, ResourceApplications.EducationOrigin.InsideBC)
        {
          Country = new ResourceMetadata.Country(Guid.NewGuid().ToString(), "Canada", "CA", true),
          Province = new ResourceMetadata.Province(provinceId, "British Columbia", "BC"),
          PostSecondaryInstitution = new ResourceMetadata.PostSecondaryInstitution(Guid.NewGuid().ToString(), "Northern College", provinceId),
        }
      ]
    };

    var result = mapper.MapApplication(source)!;
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
