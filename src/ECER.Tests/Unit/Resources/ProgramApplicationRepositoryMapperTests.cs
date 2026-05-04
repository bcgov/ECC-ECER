using ECER.Resources.Documents.ProgramApplications;
using ECER.Utilities.DataverseSdk.Model;
using Microsoft.Xrm.Sdk;
using Shouldly;

namespace ECER.Tests.Unit.DocumentsResources;

public class ProgramApplicationRepositoryMapperTests
{
  [Fact]
  public void MapProgramApplications_MapsDeclarantAndProgressFields()
  {
    var mapper = new ProgramApplicationRepositoryMapper();
    var applicationId = Guid.NewGuid();
    var instituteId = Guid.NewGuid();
    var representativeId = Guid.NewGuid();

    var source = new ecer_PostSecondaryInstituteProgramApplicaiton
    {
      ecer_PostSecondaryInstituteProgramApplicaitonId = applicationId,
      ecer_PostSecondaryInstitute = new EntityReference(ecer_PostSecondaryInstitute.EntityLogicalName, instituteId),
      ecer_SubmittedByProgramRepresentativeId = new EntityReference(ecer_ECEProgramRepresentative.EntityLogicalName, representativeId),
      ecer_BasicEntryProgress = ecer_PSPComponentProgress.Completed,
      ecer_ITEEntryProgress = ecer_PSPComponentProgress.InProgress,
      ecer_SNEEntryProgress = ecer_PSPComponentProgress.ToDo,
    };
    source.FormattedValues[ecer_PostSecondaryInstituteProgramApplicaiton.Fields.ecer_SubmittedByProgramRepresentativeId] = "Program Rep";

    var result = mapper.MapProgramApplications([source]).Single();

    result.Id.ShouldBe(applicationId.ToString());
    result.PostSecondaryInstituteId.ShouldBe(instituteId.ToString());
    result.DeclarantId.ShouldBe(representativeId.ToString());
    result.DeclarantName.ShouldBe("Program Rep");
    result.BasicProgress.ShouldBe(nameof(ecer_PSPComponentProgress.Completed));
    result.IteProgress.ShouldBe(nameof(ecer_PSPComponentProgress.InProgress));
    result.SneProgress.ShouldBe(nameof(ecer_PSPComponentProgress.ToDo));
  }
}
