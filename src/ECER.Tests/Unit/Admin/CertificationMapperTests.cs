using ECER.Managers.Admin;
using Shouldly;
using ResourceCertifications = ECER.Resources.Documents.Certifications;

namespace ECER.Tests.Unit.Admin;

public class CertificationMapperTests
{
  [Fact]
  public void MapCertificationSummaries_MapsAllFields()
  {
    var mapper = new CertificationMapper();
    var source = new ResourceCertifications.CertificationSummary(Guid.NewGuid().ToString())
    {
      FileName = "certificate.pdf",
      FilePath = "folder/path",
      FileExtention = ".pdf",
      FileId = Guid.NewGuid().ToString(),
      CreatedOn = new DateTime(2025, 4, 3),
    };

    var result = mapper.MapCertificationSummaries([source]).Single();

    result.Id.ShouldBe(source.Id);
    result.FileName.ShouldBe(source.FileName);
    result.FilePath.ShouldBe(source.FilePath);
    result.FileExtention.ShouldBe(source.FileExtention);
    result.FileId.ShouldBe(source.FileId);
    result.CreatedOn.ShouldBe(source.CreatedOn);
  }
}
