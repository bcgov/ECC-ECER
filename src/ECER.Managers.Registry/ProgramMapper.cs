using Riok.Mapperly.Abstractions;
using ContractPrograms = ECER.Managers.Registry.Contract.Programs;
using ContractShared = ECER.Managers.Registry.Contract.Shared;
using ResourcePrograms = ECER.Resources.Documents.Programs;
using ResourceShared = ECER.Resources.Documents.Shared;

namespace ECER.Managers.Registry;

public interface IProgramMapper
{
  ResourcePrograms.Program MapProgram(ContractPrograms.Program source);
  ContractPrograms.Program? MapProgram(ResourcePrograms.Program? source);
  IEnumerable<ContractPrograms.Program> MapPrograms(IEnumerable<ResourcePrograms.Program> source);
  IEnumerable<ResourcePrograms.ProgramStatus> MapProgramStatuses(IEnumerable<ContractPrograms.ProgramStatus> source);
}

[Mapper]
internal partial class ProgramMapper : IProgramMapper
{
  public ResourcePrograms.Program MapProgram(ContractPrograms.Program source) => new(source.Id, source.PostSecondaryInstituteId)
  {
    PortalStage = source.PortalStage,
    Status = MapProgramStatus(source.Status),
    CreatedOn = source.CreatedOn,
    Name = source.Name,
    ProgramName = source.ProgramName,
    PostSecondaryInstituteName = source.PostSecondaryInstituteName,
    StartDate = source.StartDate,
    EndDate = source.EndDate,
    NewBasicTotalHours = MapHours(source.NewBasicTotalHours),
    NewSneTotalHours = MapHours(source.NewSneTotalHours),
    NewIteTotalHours = MapHours(source.NewIteTotalHours),
    DeclarationDate = source.DeclarationDate,
    DeclarationUserName = source.DeclarationUserName,
    ProgramProfileType = MapProgramProfileType(source.ProgramProfileType),
    ProgramTypes = source.ProgramTypes,
    Courses = source.Courses?.Select(MapCourse).ToList(),
    OfferedProgramTypes = source.OfferedProgramTypes,
    ChangesMade = source.ChangesMade,
    FromProgramProfileId = source.FromProgramProfileId,
    ReadyForReview = source.ReadyForReview,
  };

  public ContractPrograms.Program? MapProgram(ResourcePrograms.Program? source) => source == null ? null : new ContractPrograms.Program(source.Id, source.PostSecondaryInstituteId)
  {
    PortalStage = source.PortalStage,
    Status = MapProgramStatus(source.Status),
    CreatedOn = source.CreatedOn,
    Name = source.Name,
    ProgramName = source.ProgramName,
    PostSecondaryInstituteName = source.PostSecondaryInstituteName,
    StartDate = source.StartDate,
    EndDate = source.EndDate,
    NewBasicTotalHours = MapHours(source.NewBasicTotalHours),
    NewSneTotalHours = MapHours(source.NewSneTotalHours),
    NewIteTotalHours = MapHours(source.NewIteTotalHours),
    DeclarationDate = source.DeclarationDate,
    DeclarationUserName = source.DeclarationUserName,
    ProgramProfileType = MapProgramProfileType(source.ProgramProfileType),
    ProgramTypes = source.ProgramTypes,
    Courses = source.Courses?.Select(MapCourse).ToList(),
    OfferedProgramTypes = source.OfferedProgramTypes,
    ChangesMade = source.ChangesMade,
    FromProgramProfileId = source.FromProgramProfileId,
    ReadyForReview = source.ReadyForReview,
  };

  public IEnumerable<ContractPrograms.Program> MapPrograms(IEnumerable<ResourcePrograms.Program> source) => source.Select(program => MapProgram(program)!).ToList();

  public IEnumerable<ResourcePrograms.ProgramStatus> MapProgramStatuses(IEnumerable<ContractPrograms.ProgramStatus> source) => source.Select(MapProgramStatus).ToList();

  private static ResourceShared.Course MapCourse(ContractShared.Course source) => new()
  {
    CourseId = source.CourseId,
    CourseNumber = source.CourseNumber,
    CourseTitle = source.CourseTitle,
    NewCourseNumber = source.NewCourseNumber,
    NewCourseTitle = source.NewCourseTitle,
    CourseAreaOfInstruction = source.CourseAreaOfInstruction?.Select(MapCourseAreaOfInstruction).ToList(),
    ProgramType = source.ProgramType,
  };

  private static ContractShared.Course MapCourse(ResourceShared.Course source) => new()
  {
    CourseId = source.CourseId,
    CourseNumber = source.CourseNumber,
    CourseTitle = source.CourseTitle,
    NewCourseNumber = source.NewCourseNumber,
    NewCourseTitle = source.NewCourseTitle,
    CourseAreaOfInstruction = source.CourseAreaOfInstruction?.Select(MapCourseAreaOfInstruction).ToList(),
    ProgramType = source.ProgramType,
  };

  private static ResourceShared.CourseAreaOfInstruction MapCourseAreaOfInstruction(ContractShared.CourseAreaOfInstruction source) => new()
  {
    CourseAreaOfInstructionId = source.CourseAreaOfInstructionId,
    NewHours = string.IsNullOrWhiteSpace(source.NewHours) ? null : Convert.ToSingle(source.NewHours),
    AreaOfInstructionId = source.AreaOfInstructionId,
  };

  private static ContractShared.CourseAreaOfInstruction MapCourseAreaOfInstruction(ResourceShared.CourseAreaOfInstruction source) => new()
  {
    CourseAreaOfInstructionId = source.CourseAreaOfInstructionId ?? string.Empty,
    NewHours = source.NewHours?.ToString(),
    AreaOfInstructionId = source.AreaOfInstructionId,
  };

  private static float? MapHours(string? source) => string.IsNullOrWhiteSpace(source) ? null : Convert.ToSingle(source);

  private static string? MapHours(float? source) => source?.ToString();

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourcePrograms.ProgramStatus MapProgramStatus(ContractPrograms.ProgramStatus source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractPrograms.ProgramStatus MapProgramStatus(ResourcePrograms.ProgramStatus source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourcePrograms.ProgramProfileType MapProgramProfileType(ContractPrograms.ProgramProfileType source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractPrograms.ProgramProfileType MapProgramProfileType(ResourcePrograms.ProgramProfileType source);
}
