using ECER.Engines.Validation.ProgramApplications;
using ECER.Managers.Registry.Contract.ProgramApplications;
using ECER.Managers.Registry.Contract.Shared;
using ECER.Resources.Documents.MetadataResources;

namespace ECER.Tests.Unit.PspApi;

public class ProgramApplicationSubmissionValidationEngineTests
{
  private static readonly string[] BasicProgramTypes = ["Basic"];
  private static readonly string[] ITEProgramTypes = ["ITE"];
  private static readonly string[] SNEProgramTypes = ["SNE"];
  private static readonly string[] ITEAndSNEProgramTypes = ["ITE", "SNE"];

  private readonly ProgramApplicationSubmissionValidationEngine engine;

  public ProgramApplicationSubmissionValidationEngineTests()
  {
    engine = new ProgramApplicationSubmissionValidationEngine();
  }

  [Fact]
  public async Task Validate_WithNoGroupsNoProgramTypesAndDeclarationAccepted_ReturnsValid()
  {
    var context = BuildContext(declarationAccepted: true);

    var result = await engine.Validate(context);

    Assert.True(result.IsValid);
    Assert.Empty(result.ValidationErrors);
  }

  [Fact]
  public async Task Validate_WithAllGroupsCompletedAndDeclarationAccepted_ReturnsValid()
  {
    var context = BuildContext(
      groups: new[]
      {
        new ComponentGroupValidationStatus(Guid.NewGuid().ToString(), "Group A", "Completed"),
        new ComponentGroupValidationStatus(Guid.NewGuid().ToString(), "Group B", "COMPLETED"),
      },
      declarationAccepted: true);

    var result = await engine.Validate(context);

    Assert.True(result.IsValid);
    Assert.Empty(result.ValidationErrors);
  }

  [Fact]
  public async Task Validate_WithIncompleteComponentGroup_ReturnsGroupNotCompletedError()
  {
    var context = BuildContext(
      groups: new[] { new ComponentGroupValidationStatus(Guid.NewGuid().ToString(), "Curriculum Section", "InProgress") },
      declarationAccepted: true);

    var result = await engine.Validate(context);

    Assert.False(result.IsValid);
    Assert.Single(result.ValidationErrors);
    Assert.Contains("Curriculum Section has not been completed", result.ValidationErrors);
  }

  [Fact]
  public async Task Validate_WithMultipleIncompleteComponentGroups_ReturnsOneErrorPerIncompleteGroup()
  {
    var context = BuildContext(
      groups: new[]
      {
        new ComponentGroupValidationStatus(Guid.NewGuid().ToString(), "Group A", "Draft"),
        new ComponentGroupValidationStatus(Guid.NewGuid().ToString(), "Group B", "Completed"),
        new ComponentGroupValidationStatus(Guid.NewGuid().ToString(), "Group C", "InProgress"),
      },
      declarationAccepted: true);

    var result = await engine.Validate(context);

    Assert.False(result.IsValid);
    Assert.Equal(2, result.ValidationErrors.Count());
    Assert.Contains("Group A has not been completed", result.ValidationErrors);
    Assert.Contains("Group C has not been completed", result.ValidationErrors);
  }

  [Fact]
  public async Task Validate_WithGroupStatusNullOrEmpty_TreatsGroupAsIncomplete()
  {
    var context = BuildContext(
      groups: new[]
      {
        new ComponentGroupValidationStatus(Guid.NewGuid().ToString(), "Unstaged Group", string.Empty),
      },
      declarationAccepted: true);

    var result = await engine.Validate(context);

    Assert.False(result.IsValid);
    Assert.Contains("Unstaged Group has not been completed", result.ValidationErrors);
  }

  [Fact]
  public async Task Validate_WithDeclarationNotAccepted_ReturnsDeclarationError()
  {
    var context = BuildContext(declarationAccepted: false);

    var result = await engine.Validate(context);

    Assert.False(result.IsValid);
    Assert.Single(result.ValidationErrors);
    Assert.Contains("Declaration must be accepted to submit the program application", result.ValidationErrors);
  }

  [Fact]
  public async Task Validate_WithBasicProgramTypeAndNoCourses_ReturnsMissingCoursesError()
  {
    var areaId = Guid.NewGuid().ToString();
    var context = BuildContext(
      programTypes: new[] { ProgramCertificationType.Basic },
      areasOfInstruction: new[] { new AreaOfInstruction(areaId, "Child Development", BasicProgramTypes, 20, "01") },
      declarationAccepted: true);

    var result = await engine.Validate(context);

    Assert.False(result.IsValid);
    Assert.Contains(result.ValidationErrors, e => e.Contains("Must have courses for Basic program type"));
  }

  [Fact]
  public async Task Validate_WithBasicProgramTypeAndInsufficientMinimumHours_ReturnsMinimumHoursError()
  {
    var areaId = Guid.NewGuid().ToString();
    var context = BuildContext(
      programTypes: new[] { ProgramCertificationType.Basic },
      courses: new[] { BuildCourse("Basic", areaId, 10m) },
      areasOfInstruction: new[] { new AreaOfInstruction(areaId, "Child Development", BasicProgramTypes, 20, "01") },
      declarationAccepted: true);

    var result = await engine.Validate(context);

    Assert.False(result.IsValid);
    Assert.Contains(result.ValidationErrors, e => e.Contains("Minimum hours are required for instruction: Child Development"));
  }

  [Fact]
  public async Task Validate_WithBasicProgramTypeAndSufficientMinimumHours_ReturnsValid()
  {
    var areaId = Guid.NewGuid().ToString();
    var context = BuildContext(
      programTypes: new[] { ProgramCertificationType.Basic },
      courses: new[] { BuildCourse("Basic", areaId, 25m) },
      areasOfInstruction: new[] { new AreaOfInstruction(areaId, "Child Development", BasicProgramTypes, 20, "01") },
      declarationAccepted: true);

    var result = await engine.Validate(context);

    Assert.True(result.IsValid);
    Assert.Empty(result.ValidationErrors);
  }

  [Fact]
  public async Task Validate_WithBasicProgramType_DoesNotEnforceTotalHoursMinimum()
  {
    var areaId = Guid.NewGuid().ToString();
    var context = BuildContext(
      programTypes: new[] { ProgramCertificationType.Basic },
      courses: new[] { BuildCourse("Basic", areaId, 200m) },
      areasOfInstruction: new[] { new AreaOfInstruction(areaId, "ECE Core", BasicProgramTypes, 0, "01") },
      declarationAccepted: true);

    var result = await engine.Validate(context);

    Assert.True(result.IsValid);
    Assert.Empty(result.ValidationErrors);
  }

  [Fact]
  public async Task Validate_WithITEProgramTypeAndNoCourses_ReturnsMissingCoursesError()
  {
    var context = BuildContext(
      programTypes: new[] { ProgramCertificationType.ITE },
      declarationAccepted: true);

    var result = await engine.Validate(context);

    Assert.False(result.IsValid);
    Assert.Contains(result.ValidationErrors, e => e.Contains("Must have courses for ITE program type"));
  }

  [Fact]
  public async Task Validate_WithITEProgramTypeAndTotalHoursBelowMinimum_ReturnsTotalHoursError()
  {
    var areaId = Guid.NewGuid().ToString();
    var context = BuildContext(
      programTypes: new[] { ProgramCertificationType.ITE },
      courses: new[] { BuildCourse("ITE", areaId, 300m) },
      areasOfInstruction: new[] { new AreaOfInstruction(areaId, "ECE Core", ITEProgramTypes, null, "01") },
      declarationAccepted: true);

    var result = await engine.Validate(context);

    Assert.False(result.IsValid);
    Assert.Contains(result.ValidationErrors, e => e.Contains("Total course hours must hit the minimum total required hours for program: ITE"));
  }

  [Fact]
  public async Task Validate_WithITEProgramTypeAndSufficientTotalHours_ReturnsValid()
  {
    var areaId = Guid.NewGuid().ToString();
    var context = BuildContext(
      programTypes: new[] { ProgramCertificationType.ITE },
      courses: new[] { BuildCourse("ITE", areaId, 450m) },
      areasOfInstruction: new[] { new AreaOfInstruction(areaId, "ECE Core", ITEProgramTypes, null, "01") },
      declarationAccepted: true);

    var result = await engine.Validate(context);

    Assert.True(result.IsValid);
    Assert.Empty(result.ValidationErrors);
  }

  [Fact]
  public async Task Validate_WithSNEProgramTypeAndNoCourses_ReturnsMissingCoursesError()
  {
    var context = BuildContext(
      programTypes: new[] { ProgramCertificationType.SNE },
      declarationAccepted: true);

    var result = await engine.Validate(context);

    Assert.False(result.IsValid);
    Assert.Contains(result.ValidationErrors, e => e.Contains("Must have courses for SNE program type"));
  }

  [Fact]
  public async Task Validate_WithSNEProgramTypeAndTotalHoursBelowMinimum_ReturnsTotalHoursError()
  {
    var areaId = Guid.NewGuid().ToString();
    var context = BuildContext(
      programTypes: new[] { ProgramCertificationType.SNE },
      courses: new[] { BuildCourse("SNE", areaId, 200m) },
      areasOfInstruction: new[] { new AreaOfInstruction(areaId, "Special Needs", SNEProgramTypes, null, "01") },
      declarationAccepted: true);

    var result = await engine.Validate(context);

    Assert.False(result.IsValid);
    Assert.Contains(result.ValidationErrors, e => e.Contains("Total course hours must hit the minimum total required hours for program: SNE"));
  }

  [Fact]
  public async Task Validate_WithSNEProgramTypeAndSufficientTotalHours_ReturnsValid()
  {
    var areaId = Guid.NewGuid().ToString();
    var context = BuildContext(
      programTypes: new[] { ProgramCertificationType.SNE },
      courses: new[] { BuildCourse("SNE", areaId, 450m) },
      areasOfInstruction: new[] { new AreaOfInstruction(areaId, "Special Needs", SNEProgramTypes, null, "01") },
      declarationAccepted: true);

    var result = await engine.Validate(context);

    Assert.True(result.IsValid);
    Assert.Empty(result.ValidationErrors);
  }

  [Fact]
  public async Task Validate_WithMultipleProgramTypesAllLackingCourses_ReturnsErrorPerProgramType()
  {
    var context = BuildContext(
      programTypes: new[] { ProgramCertificationType.Basic, ProgramCertificationType.ITE },
      areasOfInstruction: Array.Empty<AreaOfInstruction>().ToList().AsReadOnly(),
      declarationAccepted: true);

    var result = await engine.Validate(context);

    Assert.False(result.IsValid);
    Assert.Contains(result.ValidationErrors, e => e.Contains("Must have courses for Basic program type"));
    Assert.Contains(result.ValidationErrors, e => e.Contains("Must have courses for ITE program type"));
  }

  [Fact]
  public async Task Validate_WithITEAndSNECoursesButInsufficientTotalHours_ReturnsTotalHoursErrorForBoth()
  {
    var areaId = Guid.NewGuid().ToString();
    var context = BuildContext(
      programTypes: new[] { ProgramCertificationType.ITE, ProgramCertificationType.SNE },
      courses: new[]
      {
        BuildCourse("ITE", areaId, 100m),
        BuildCourse("SNE", areaId, 100m),
      },
      areasOfInstruction: new[]
      {
        new AreaOfInstruction(areaId, "ECE Core", ITEAndSNEProgramTypes, null, "01"),
      },
      declarationAccepted: true);

    var result = await engine.Validate(context);

    Assert.False(result.IsValid);
    Assert.Contains(result.ValidationErrors, e => e.Contains("Total course hours must hit the minimum total required hours for program: ITE"));
    Assert.Contains(result.ValidationErrors, e => e.Contains("Total course hours must hit the minimum total required hours for program: SNE"));
  }

  [Fact]
  public async Task Validate_WithAllRulesFailing_ReturnsErrorsFromAllRules()
  {
    var context = BuildContext(
      groups: new[] { new ComponentGroupValidationStatus(Guid.NewGuid().ToString(), "Missing Section", "Draft") },
      programTypes: new[] { ProgramCertificationType.Basic },
      areasOfInstruction: Array.Empty<AreaOfInstruction>().ToList().AsReadOnly(),
      declarationAccepted: false);

    var result = await engine.Validate(context);

    Assert.False(result.IsValid);
    Assert.Contains(result.ValidationErrors, e => e.Contains("Missing Section has not been completed"));
    Assert.Contains(result.ValidationErrors, e => e.Contains("Must have courses for Basic program type"));
    Assert.Contains(result.ValidationErrors, e => e.Contains("Declaration must be accepted to submit the program application"));
  }

  private static ProgramApplicationValidationContext BuildContext(
    IEnumerable<ComponentGroupValidationStatus>? groups = null,
    IEnumerable<Course>? courses = null,
    IEnumerable<ProgramCertificationType>? programTypes = null,
    IReadOnlyCollection<AreaOfInstruction>? areasOfInstruction = null,
    bool declarationAccepted = true) =>
    new(
      ComponentGroupStatuses: groups ?? Enumerable.Empty<ComponentGroupValidationStatus>(),
      Courses: courses ?? Enumerable.Empty<Course>(),
      ProgramApplication: new ProgramApplication(null, "test-psi")
      {
        ProgramTypes = programTypes?.ToList(),
        InstituteInfoEntryProgress = "Completed",
      },
      AreasOfInstruction: areasOfInstruction ?? new List<AreaOfInstruction>().AsReadOnly(),
      DeclarationAccepted: declarationAccepted);

  private static Course BuildCourse(string programType, string areaOfInstructionId, decimal hours) =>
    new()
    {
      CourseId = Guid.NewGuid().ToString(),
      CourseNumber = "101",
      CourseTitle = "Test Course",
      ProgramType = programType,
      CourseAreaOfInstruction = new[]
      {
        new CourseAreaOfInstruction
        {
          CourseAreaOfInstructionId = Guid.NewGuid().ToString(),
          AreaOfInstructionId = areaOfInstructionId,
          NewHours = hours.ToString("F2"),
        },
      },
    };
}
