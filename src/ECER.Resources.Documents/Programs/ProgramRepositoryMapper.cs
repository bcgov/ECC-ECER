using ECER.Resources.Documents.Shared;
using ECER.Utilities.DataverseSdk.Model;
using Riok.Mapperly.Abstractions;

namespace ECER.Resources.Documents.Programs;

internal interface IProgramRepositoryMapper
{
  List<Program> MapPrograms(IEnumerable<ecer_Program> source);
  List<ecer_Program_StatusCode> MapProgramStatuses(IEnumerable<ProgramStatus> source);
  ecer_Program MapProgram(Program source);
}

[Mapper]
internal partial class ProgramRepositoryMapper : IProgramRepositoryMapper
{
  public List<Program> MapPrograms(IEnumerable<ecer_Program> source) => source.Select(MapProgram).ToList();

  public List<ecer_Program_StatusCode> MapProgramStatuses(IEnumerable<ProgramStatus> source) => source.Select(MapProgramStatus).ToList();

  public ecer_Program MapProgram(Program source) => new()
  {
    ecer_ProgramId = string.IsNullOrWhiteSpace(source.Id) ? null : Guid.Parse(source.Id),
    StatusCode = MapProgramStatus(source.Status),
    ecer_Name = source.Name,
    ecer_NewDescriptiveProgramName = source.ProgramName,
    ecer_PortalStage = source.PortalStage,
    ecer_StartDate = source.StartDate,
    ecer_EndDate = source.EndDate,
    ecer_NewOfferingType = source.OfferedProgramTypes?.Select(MapOfferedProgramType).ToList(),
  };

  private Program MapProgram(ecer_Program source) => new(
    source.ecer_ProgramId?.ToString(),
    source.ecer_PostSecondaryInstitution?.Id.ToString() ?? string.Empty)
  {
    Status = MapProgramStatus(source.StatusCode),
    PortalStage = source.ecer_PortalStage,
    CreatedOn = source.CreatedOn,
    Name = source.ecer_Name,
    ProgramName = source.ecer_NewDescriptiveProgramName ?? source.ecer_DescriptiveProgramName,
    PostSecondaryInstituteName = source.ecer_PostSecondaryInstitutionName,
    StartDate = source.ecer_StartDate,
    EndDate = source.ecer_EndDate,
    Courses = (source.ecer_course_Programid ?? Array.Empty<ecer_Course>()).Select(MapCourse).ToList(),
    ProgramTypes = source.ecer_ProgramTypes?.Select(static type => type.ToString()).ToList(),
    OfferedProgramTypes = GetOfferedProgramTypes(source)?.ToList(),
    NewBasicTotalHours = MapHours(source.ecer_NewBasicTotalHours),
    NewSneTotalHours = MapHours(source.ecer_NewSNETotalHours),
    NewIteTotalHours = MapHours(source.ecer_NewITETotalHours),
    ProgramProfileType = MapProgramProfileType(source.ecer_Type),
    DeclarationDate = source.ecer_DeclarationDate?.ToString(),
    DeclarationUserName = source.ecer_UserName,
    ChangesMade = source.ecer_ChangesMade == ecer_YesNoNull.Yes,
    FromProgramProfileId = source.ecer_FromProgramProfileId?.Id.ToString(),
    ReadyForReview = source.ecer_ReadyforReview.HasValue ? source.ecer_ReadyforReview == ecer_YesNoNull.Yes : null,
  };

  private static IEnumerable<string>? GetOfferedProgramTypes(ecer_Program source)
  {
    if (source.ecer_NewOfferingType != null && source.ecer_NewOfferingType.Any())
    {
      return source.ecer_NewOfferingType.Select(static type => type.ToString());
    }

    if (source.ecer_OfferingType != null && source.ecer_OfferingType.Any())
    {
      return source.ecer_OfferingType.Select(static type => type.ToString());
    }

    return source.ecer_ProgramTypes?.Select(static type => type.ToString());
  }

  private static float? MapHours(decimal? source) => source.HasValue ? Convert.ToSingle(source.Value) : null;

  private static Course MapCourse(ecer_Course source) => new()
  {
    CourseId = source.ecer_CourseId?.ToString() ?? string.Empty,
    CourseNumber = source.ecer_Code ?? string.Empty,
    CourseTitle = source.ecer_CourseName ?? string.Empty,
    NewCourseNumber = source.ecer_NewCode,
    NewCourseTitle = source.ecer_NewCourseName,
    CourseAreaOfInstruction = (source.ecer_courseprovincialrequirement_CourseId ?? Array.Empty<ecer_CourseProvincialRequirement>())
      .Select(MapCourseAreaOfInstruction)
      .ToList(),
    ProgramType = source.ecer_ProgramType?.ToString() ?? string.Empty,
  };

  private static CourseAreaOfInstruction MapCourseAreaOfInstruction(ecer_CourseProvincialRequirement source) => new()
  {
    NewHours = source.ecer_NewHours.HasValue ? Convert.ToSingle(source.ecer_NewHours.Value) : 0,
    CourseAreaOfInstructionId = source.Id.ToString(),
    AreaOfInstructionId = source.ecer_ProgramAreaId?.Id.ToString() ?? string.Empty,
  };

  [MapEnumValue(ProgramStatus.Draft, ecer_Program_StatusCode.RequiresReview)]
  [MapEnumValue(ProgramStatus.UnderReview, ecer_Program_StatusCode.UnderRegistryReview)]
  [MapEnumValue(ProgramStatus.Approved, ecer_Program_StatusCode.RegistryReviewComplete)]
  [MapEnumValue(ProgramStatus.Denied, ecer_Program_StatusCode.Denied)]
  [MapEnumValue(ProgramStatus.Inactive, ecer_Program_StatusCode.Archived)]
  [MapEnumValue(ProgramStatus.ChangeRequestInProgress, ecer_Program_StatusCode.ChangeRequestInProgress)]
  [MapEnumValue(ProgramStatus.Withdrawn, ecer_Program_StatusCode.Withdrawn)]
  private partial ecer_Program_StatusCode MapProgramStatus(ProgramStatus source);

  private ProgramStatus MapProgramStatus(ecer_Program_StatusCode? source) => source switch
  {
    ecer_Program_StatusCode.RequiresReview => ProgramStatus.Draft,
    ecer_Program_StatusCode.UnderRegistryReview => ProgramStatus.UnderReview,
    ecer_Program_StatusCode.RegistryReviewComplete => ProgramStatus.Approved,
    ecer_Program_StatusCode.Denied => ProgramStatus.Denied,
    ecer_Program_StatusCode.Archived => ProgramStatus.Inactive,
    ecer_Program_StatusCode.ChangeRequestInProgress => ProgramStatus.ChangeRequestInProgress,
    ecer_Program_StatusCode.Withdrawn => ProgramStatus.Withdrawn,
    _ => ProgramStatus.Draft,
  };

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ecer_ProgramProfileType MapProgramProfileType(ProgramProfileType source);

  private ProgramProfileType MapProgramProfileType(ecer_ProgramProfileType? source) => source switch
  {
    ecer_ProgramProfileType.ChangeRequest => ProgramProfileType.ChangeRequest,
    _ => ProgramProfileType.AnnualReview,
  };

  private static ecer_PSIProgramType MapOfferedProgramType(string source) => Enum.Parse<ecer_PSIProgramType>(source);
}
