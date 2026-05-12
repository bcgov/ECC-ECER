using ECER.Managers.Registry;
using ECER.Resources.Documents.PostSecondaryInstitutes;
using ECER.Utilities.DataverseSdk.Model;
using Shouldly;

namespace ECER.Tests.Unit.PspApi;

public class PostSecondaryInstituteMapperTests
{
  [Fact]
  public void MapPostSecondaryInstitute_MapsEditableInstituteFieldsForSave()
  {
    var mapper = new PostSecondaryInstituteRepositoryMapper();
    var instituteId = Guid.NewGuid().ToString();

    var source = new PostSecondaryInstitute
    {
      Id = instituteId,
      Name = "Test Institute",
      InstitutionType = PsiInstitutionType.Public,
      PrivateAuspiceType = PrivateAuspiceType.Other,
      PtiruInstitutionId = "PTIRU-1",
      WebsiteUrl = "https://example.org",
      Street1 = "123 Main St",
      Street2 = "Suite 200",
      Street3 = "Attn Testing",
      City = "Victoria",
      Country = "Canada",
      PostalCode = "V8V 8V8",
      BceidBusinessId = "BUS-123",
      BceidBusinessName = "Test Business",
    };

    var result = mapper.MapPostSecondaryInstitute(source);

    result.ecer_PostSecondaryInstituteId.ShouldBe(Guid.Parse(instituteId));
    result.ecer_Name.ShouldBe(source.Name);
    result.ecer_PSIInstitutionType.ShouldBe(ecer_psiinstitutiontype.Public);
    result.ecer_PrivateAuspiceType.ShouldBe(ecer_PrivateAuspiceType.Other);
    result.ecer_PTIRUInstitutionID.ShouldBe(source.PtiruInstitutionId);
    result.ecer_Website.ShouldBe(source.WebsiteUrl);
    result.ecer_Street1.ShouldBe(source.Street1);
    result.ecer_Street2.ShouldBe(source.Street2);
    result.ecer_Street3.ShouldBe(source.Street3);
    result.ecer_City.ShouldBe(source.City);
    result.ecer_Country.ShouldBe(source.Country);
    result.ecer_ZipPostalCode.ShouldBe(source.PostalCode);
    result.ecer_BusinessBCeID.ShouldBe(source.BceidBusinessId);
    result.ecer_BCeIDBusinessName.ShouldBe(source.BceidBusinessName);
  }
}
