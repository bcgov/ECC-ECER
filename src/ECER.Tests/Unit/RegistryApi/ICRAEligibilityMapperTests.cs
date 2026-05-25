using ECER.Managers.Registry;
using Shouldly;
using ContractApplications = ECER.Managers.Registry.Contract.Applications;
using ContractICRA = ECER.Managers.Registry.Contract.ICRA;
using ResourceApplications = ECER.Resources.Documents.Applications;
using ResourceICRA = ECER.Resources.Documents.ICRA;

namespace ECER.Tests.Unit.RegistryApi;

public class ICRAEligibilityMapperTests
{
  [Fact]
  public void MapEligibility_MapsInternationalCertificationStatusAndFiles()
  {
    var mapper = new ICRAEligibilityMapper();
    var source = new ResourceICRA.ICRAEligibility
    {
      Id = Guid.NewGuid().ToString(),
      ApplicantId = Guid.NewGuid().ToString(),
      Status = ResourceICRA.ICRAStatus.Eligible,
      Origin = ResourceICRA.IcraEligibilityOrigin.Portal,
      FullLegalName = "Full Name",
      UnderstandAgreesApplication = true,
      InternationalCertifications =
      [
        new ResourceICRA.InternationalCertification
        {
          Id = Guid.NewGuid().ToString(),
          CertificateStatus = ResourceICRA.CertificateStatus.Valid,
          Status = ResourceICRA.InternationalCertificationStatus.UnderReview,
          Files = [new ResourceApplications.FileInfo("file-1") { Name = "certificate.pdf", Url = "/files/1", Extention = ".pdf", Size = "100" }],
          DeletedFiles = ["old-file"],
          NewFiles = ["new-file"],
        }
      ],
      EmploymentReferences =
      [
        new ResourceICRA.EmploymentReference
        {
          Id = Guid.NewGuid().ToString(),
          FirstName = "Ref",
          LastName = "Person",
          EmailAddress = "ref@example.org",
          Status = ResourceApplications.WorkExperienceRefStage.Submitted,
          Type = ResourceICRA.WorkExperienceTypesIcra.ICRA,
        }
      ]
    };

    var result = mapper.MapEligibility(source)!;

    result.Status.ShouldBe(ContractICRA.ICRAStatus.Eligible);
    result.Origin.ShouldBe(ContractICRA.IcraEligibilityOrigin.Portal);
    result.InternationalCertifications.Single().status.ShouldBe(ContractICRA.InternationalCertificationStatus.UnderReview);
    result.InternationalCertifications.Single().Files.Single().Id.ShouldBe("file-1");
    result.InternationalCertifications.Single().DeletedFiles.ShouldBe(["old-file"]);
    result.InternationalCertifications.Single().NewFiles.ShouldBe(["new-file"]);
    result.EmploymentReferences.Single().Status.ShouldBe(ContractApplications.WorkExperienceRefStage.Submitted);
    result.EmploymentReferences.Single().Type.ShouldBe(ContractICRA.WorkExperienceTypesIcra.ICRA);
  }

  [Fact]
  public void MapIcraWorkExperienceReferenceSubmissionRequest_MapsOptionalFields()
  {
    var mapper = new ICRAEligibilityMapper();
    var source = new ContractICRA.ICRAWorkExperienceReferenceSubmissionRequest
    {
      FirstName = "Ref",
      LastName = "Person",
      EmailAddress = "ref@example.org",
      PhoneNumber = "250-555-0101",
      CountryId = Guid.NewGuid().ToString(),
      EmployerName = "Employer",
      PositionTitle = "Teacher",
      StartDate = new DateTime(2020, 1, 1),
      EndDate = new DateTime(2021, 1, 1),
      WorkedWithChildren = true,
      ChildcareAgeRanges = [ContractApplications.ChildcareAgeRanges._35years],
      ReferenceRelationship = ContractApplications.ReferenceRelationship.Supervisor,
      WillProvideReference = true,
      DateSigned = new DateTime(2025, 2, 2),
    };

    var result = mapper.MapIcraWorkExperienceReferenceSubmissionRequest(source);

    result.FirstName.ShouldBe("Ref");
    result.ChildcareAgeRanges.ShouldBe([ResourceApplications.ChildcareAgeRanges._35years]);
    result.ReferenceRelationship.ShouldBe(ResourceApplications.ReferenceRelationship.Supervisor);
    result.DateSigned.ShouldBe(new DateTime(2025, 2, 2));
  }
}
