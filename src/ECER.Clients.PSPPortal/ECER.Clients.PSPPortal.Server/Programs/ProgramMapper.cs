using ECER.Clients.PSPPortal.Server.Shared;
using Riok.Mapperly.Abstractions;
using ContractPrograms = ECER.Managers.Registry.Contract.Programs;
using ContractShared = ECER.Managers.Registry.Contract.Shared;

namespace ECER.Clients.PSPPortal.Server.Programs;

internal interface IProgramMapper
{
  ContractPrograms.Program MapProgram(Program source);
  Program MapProgram(ContractPrograms.Program source);
  IEnumerable<Program> MapPrograms(IEnumerable<ContractPrograms.Program> source);
}

[Mapper]
internal partial class ProgramMapper : IProgramMapper
{
  public ContractPrograms.Program MapProgram(Program source) => new(source.Id, string.Empty)
  {
    PortalStage = source.PortalStage,
    Status = MapProgramStatus(source.Status),
    CreatedOn = source.CreatedOn,
    Name = source.Name,
    ProgramName = source.ProgramName,
    PostSecondaryInstituteName = source.PostSecondaryInstituteName,
    StartDate = source.StartDate,
    EndDate = source.EndDate,
    NewBasicTotalHours = source.NewBasicTotalHours,
    NewSneTotalHours = source.NewSneTotalHours,
    NewIteTotalHours = source.NewIteTotalHours,
    DeclarationDate = source.DeclarationDate,
    DeclarationUserName = source.DeclarationUserName,
    ProgramProfileType = MapProgramProfileType(source.ProgramProfileType),
    ProgramTypes = MapProgramTypes(source.ProgramTypes),
    Courses = source.Courses?.Select(MapCourse).ToList(),
    OfferedProgramTypes = MapProgramTypes(source.OfferedProgramTypes),
    ChangesMade = source.ChangesMade,
    FromProgramProfileId = source.FromProgramProfileId,
    ReadyForReview = source.ReadyForReview,
  };

  public Program MapProgram(ContractPrograms.Program source) => new()
  {
    Id = source.Id,
    PortalStage = source.PortalStage,
    Status = MapProgramStatus(source.Status),
    CreatedOn = source.CreatedOn,
    Name = source.Name,
    ProgramName = source.ProgramName,
    PostSecondaryInstituteName = source.PostSecondaryInstituteName,
    StartDate = source.StartDate,
    EndDate = source.EndDate,
    NewBasicTotalHours = source.NewBasicTotalHours,
    NewSneTotalHours = source.NewSneTotalHours,
    NewIteTotalHours = source.NewIteTotalHours,
    DeclarationDate = source.DeclarationDate,
    DeclarationUserName = source.DeclarationUserName,
    ProgramProfileType = MapProgramProfileType(source.ProgramProfileType),
    ProgramTypes = ParseProgramTypes(source.ProgramTypes),
    Courses = source.Courses?.Select(MapCourse).ToList(),
    OfferedProgramTypes = ParseProgramTypes(source.OfferedProgramTypes),
    ChangesMade = source.ChangesMade,
    FromProgramProfileId = source.FromProgramProfileId,
    ReadyForReview = source.ReadyForReview,
  };

  public IEnumerable<Program> MapPrograms(IEnumerable<ContractPrograms.Program> source) => source.Select(MapProgram).ToList();

  private static ContractShared.Course MapCourse(Course source) => new()
  {
    CourseId = source.CourseId!,
    CourseNumber = source.CourseNumber,
    CourseTitle = source.CourseTitle,
    NewCourseNumber = source.NewCourseNumber,
    NewCourseTitle = source.NewCourseTitle,
    CourseAreaOfInstruction = source.CourseAreaOfInstruction?.Select(MapCourseAreaOfInstruction).ToList(),
    ProgramType = source.ProgramType,
  };

  private static Course MapCourse(ContractShared.Course source) => new()
  {
    CourseId = source.CourseId,
    CourseNumber = source.CourseNumber,
    CourseTitle = source.CourseTitle,
    NewCourseNumber = source.NewCourseNumber,
    NewCourseTitle = source.NewCourseTitle,
    CourseAreaOfInstruction = source.CourseAreaOfInstruction?.Select(MapCourseAreaOfInstruction).ToList(),
    ProgramType = source.ProgramType,
  };

  private static ContractShared.CourseAreaOfInstruction MapCourseAreaOfInstruction(CourseAreaOfInstruction source) => new()
  {
    CourseAreaOfInstructionId = source.CourseAreaOfInstructionId,
    NewHours = source.NewHours,
    AreaOfInstructionId = source.AreaOfInstructionId,
  };

  private static CourseAreaOfInstruction MapCourseAreaOfInstruction(ContractShared.CourseAreaOfInstruction source) => new()
  {
    CourseAreaOfInstructionId = source.CourseAreaOfInstructionId,
    NewHours = source.NewHours,
    AreaOfInstructionId = source.AreaOfInstructionId,
  };

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractPrograms.ProgramStatus MapProgramStatus(ProgramStatus source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ProgramStatus MapProgramStatus(ContractPrograms.ProgramStatus source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractPrograms.ProgramProfileType MapProgramProfileType(ProgramProfileType source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ProgramProfileType MapProgramProfileType(ContractPrograms.ProgramProfileType source);

  private static string[]? MapProgramTypes(IEnumerable<ProgramTypes>? source) => source?.Select(type => type.ToString()).ToArray();

  private static ProgramTypes[]? ParseProgramTypes(IEnumerable<string>? programTypes)
  {
    if (programTypes == null)
    {
      return null;
    }

    return programTypes
      .Select(type => Enum.TryParse<ProgramTypes>(type, true, out var parsed) ? (ProgramTypes?)parsed : null)
      .Where(parsed => parsed.HasValue)
      .Select(parsed => parsed!.Value)
      .ToArray();
  }
}
