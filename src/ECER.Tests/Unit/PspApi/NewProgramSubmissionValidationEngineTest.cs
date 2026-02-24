using ECER.Engines.Validation.Programs;
using ECER.Managers.Registry.Contract.Programs;
using ECER.Managers.Registry.Contract.Shared;
using ECER.Resources.Documents.MetadataResources;

namespace ECER.Tests.Unit.PspApi;

public class NewProgramSubmissionValidationEngineTest
{
  private readonly NewProgramSubmissionValidationEngine validator;

  public NewProgramSubmissionValidationEngineTest()
  {
    validator = new NewProgramSubmissionValidationEngine();
  }

  [Fact]
  public async Task Validate_CheckForMinimumHours_ProgramTypeBasic_WithMinHours_ReturnsSuccess()
  {
    var areaOfInstruction = new AreaOfInstruction(Guid.NewGuid().ToString(), "Test_Area", ["Basic"], 20, "01");
    var areaOfInstructionList = new[] { areaOfInstruction };

    var program = CreateProgram(areaOfInstruction.Id, areaOfInstruction.ProgramTypes.ToList(), "20.00", true);

    var result = await validator.Validate(program, areaOfInstructionList);
    Assert.Empty(result.ValidationErrors);
  }

  [Fact]
  public async Task Validate_CheckForMinimumHours_ProgramTypeBasic_WithZeroHours_ReturnsValidationError()
  {
    var areaOfInstruction = new AreaOfInstruction(Guid.NewGuid().ToString(), "Test_Area", ["Basic"], 20, "01");
    var areaOfInstructionList = new[] { areaOfInstruction };

    var program = CreateProgram(areaOfInstruction.Id, areaOfInstruction.ProgramTypes.ToList(), "0.00", true);

    var result = await validator.Validate(program, areaOfInstructionList);
    Assert.NotEmpty(result.ValidationErrors);
    var errorMessage = result.ValidationErrors.First();
    Assert.Contains("Minimum hours are required for instruction: Test_A", errorMessage);
  }

  [Fact]
  public async Task Validate_CheckForMinimumHours_ProgramTypeSne_WithZeroHours_ReturnsValidationError()
  {
    var areaOfInstruction = new AreaOfInstruction(Guid.NewGuid().ToString(), "Test_Area", ["SNE"], 0, "01");
    var areaOfInstructionList = new[] { areaOfInstruction };

    var program = CreateProgram(areaOfInstruction.Id, areaOfInstruction.ProgramTypes.ToList(), "0.00", true);

    var result = await validator.Validate(program, areaOfInstructionList);
    Assert.NotEmpty(result.ValidationErrors);
    var errorMessage = result.ValidationErrors.First();
    Assert.Contains("Total hours must be greater than zero: Test_A", errorMessage);
  }

  [Fact]
  public async Task Validate_CheckForMinimumHours_ProgramTypeSne_WithZeroHours_AndUnmappedInstruction_ReturnsValidationError()
  {
    var areaOfInstruction1 = new AreaOfInstruction(Guid.NewGuid().ToString(), "Test_Area", ["SNE"], 450, "01");
    var areaOfInstruction2 = new AreaOfInstruction(Guid.NewGuid().ToString(), "Test_Area", ["SNE"], 0, "02");
    var areaOfInstructionList = new[] { areaOfInstruction1, areaOfInstruction2 };

    var program = CreateProgram(areaOfInstruction1.Id, areaOfInstruction1.ProgramTypes.ToList(), "450.00", true);

    var result = await validator.Validate(program, areaOfInstructionList);
    Assert.NotEmpty(result.ValidationErrors);
    var errorMessage = result.ValidationErrors.First();
    Assert.Contains("Total hours must be greater than zero: Test_A", errorMessage);
  }

  [Fact]
  public async Task Validate_CheckTotalCourseHours_ProgramTypeSne_WithZeroHours_ReturnsValidationError()
  {
    var areaOfInstruction = new AreaOfInstruction(Guid.NewGuid().ToString(), "Test_Area", ["SNE"], 40, "03");
    var areaOfInstructionList = new[] { areaOfInstruction };

    var program = CreateProgram(areaOfInstruction.Id, areaOfInstruction.ProgramTypes.ToList(), "40.00", true);

    var result = await validator.Validate(program, areaOfInstructionList);
    Assert.NotEmpty(result.ValidationErrors);
    var errorMessage = result.ValidationErrors.First();
    Assert.Contains("Total course hours must hit the minimum total required hours for program: SNE", errorMessage);
  }

  [Fact]
  public async Task Validate_NoBasicCourses_ReturnsValidationError()
  {
    var areaOfInstruction = new AreaOfInstruction(Guid.NewGuid().ToString(), "Test_Area", ["Basic"], 40, "04");
    var areaOfInstructionList = new[] { areaOfInstruction };

    var program = CreateProgram(areaOfInstruction.Id, areaOfInstruction.ProgramTypes.ToList(), "0.00", false);

    var result = await validator.Validate(program, areaOfInstructionList);
    Assert.NotEmpty(result.ValidationErrors);
    var errorMessage = result.ValidationErrors.First();
    Assert.Contains("Must have courses for BASIC program type", errorMessage);
  }

  [Fact]
  public async Task Validate_NotAuthorizedToProvideProgramTypeBasic_ReturnsValidationError()
  {
    var areaOfInstruction = new AreaOfInstruction(Guid.NewGuid().ToString(), "Test_Area", ["Basic"], 40, "04");
    var areaOfInstructionList = new[] { areaOfInstruction };

    var program = CreateProgram(areaOfInstruction.Id, areaOfInstruction.ProgramTypes.ToList(), "0.00", false);
    program.ProgramTypes = ["ITE", "SNE"];

    var result = await validator.Validate(program, areaOfInstructionList);
    Assert.NotEmpty(result.ValidationErrors);
    var errorMessage = result.ValidationErrors.First();
    Assert.Contains("Not authorized to provide Basic program type", errorMessage);
  }

  [Fact]
  public async Task Validate_CheckForMinimumHours_WithChildAreaLinkedToParent_CombinedHoursPassMinimum_ReturnsSuccess()
  {
    var parentId = Guid.NewGuid().ToString();
    var childId = Guid.NewGuid().ToString();

    var parentArea = new AreaOfInstruction(parentId, "Parent_Area", ["Basic"], 20, "01");
    var childArea = new AreaOfInstruction(childId, "Child_Area", ["Basic"], null, "01a", parentId);
    var areaOfInstructionList = new[] { parentArea, childArea };

    // 15 hours in parent area alone is below the minimum of 20
    var program = CreateProgram(parentId, ["Basic"], "15.00", true);

    // Add 10 hours in the child area — combined (15 + 10 = 25) should satisfy the parent's minimum
    program.Courses = program.Courses!.Append(new Course()
    {
      CourseId = Guid.NewGuid().ToString(),
      CourseNumber = "201",
      CourseTitle = "Course 201",
      NewCourseNumber = "202",
      NewCourseTitle = "Course 202",
      ProgramType = "Basic",
      CourseAreaOfInstruction = new[]
      {
        new CourseAreaOfInstruction()
        {
          AreaOfInstructionId = childId,
          CourseAreaOfInstructionId = Guid.NewGuid().ToString(),
          NewHours = "10.00"
        }
      }
    }).ToArray();

    var result = await validator.Validate(program, areaOfInstructionList);
    Assert.Empty(result.ValidationErrors);
  }

  [Fact]
  public async Task Validate_CheckForMinimumHours_WithChildAreaLinkedToParent_ChildFailsOwnMinimumIndependently_ReturnsValidationError()
  {
    var parentId = Guid.NewGuid().ToString();
    var childId = Guid.NewGuid().ToString();

    // Parent passes when child hours are included (15 + 10 = 25 >= 20)
    // Child fails on its own hours (10 < 12)
    var parentArea = new AreaOfInstruction(parentId, "Parent_Area", ["Basic"], 20, "01");
    var childArea = new AreaOfInstruction(childId, "Child_Area", ["Basic"], 12, "01a", parentId);
    var areaOfInstructionList = new[] { parentArea, childArea };

    var program = CreateProgram(parentId, ["Basic"], "15.00", true);

    program.Courses = program.Courses!.Append(new Course()
    {
      CourseId = Guid.NewGuid().ToString(),
      CourseNumber = "201",
      CourseTitle = "Course 201",
      NewCourseNumber = "202",
      NewCourseTitle = "Course 202",
      ProgramType = "Basic",
      CourseAreaOfInstruction = new[]
      {
        new CourseAreaOfInstruction()
        {
          AreaOfInstructionId = childId,
          CourseAreaOfInstructionId = Guid.NewGuid().ToString(),
          NewHours = "10.00"
        }
      }
    }).ToArray();

    var result = await validator.Validate(program, areaOfInstructionList);
    Assert.NotEmpty(result.ValidationErrors);
    Assert.Single(result.ValidationErrors);
    Assert.Contains("Minimum hours are required for instruction: Child_Area", result.ValidationErrors.First());
  }

  [Fact]
  public async Task Validate_CheckForMinimumHours_ChildAreaWithZeroMinimumHoursAndNoDirectCourses_SkipsZeroHoursCheck_ReturnsSuccess()
  {
    var parentId = Guid.NewGuid().ToString();
    var childId = Guid.NewGuid().ToString();

    // Child has MinimumHours == 0 and no courses assigned directly to it
    // Parent has enough hours on its own (25 >= 20)
    var parentArea = new AreaOfInstruction(parentId, "Parent_Area", ["Basic"], 20, "01");
    var childArea = new AreaOfInstruction(childId, "Child_Area", ["Basic"], 0, "01a", parentId);
    var areaOfInstructionList = new[] { parentArea, childArea };

    var program = CreateProgram(parentId, ["Basic"], "25.00", true);

    var result = await validator.Validate(program, areaOfInstructionList);
    Assert.Empty(result.ValidationErrors);
  }

  private Program CreateProgram(string areaOfInstructionId, List<string> programTypes, string newHours, bool addCourse)
  {
    return new Program(Guid.NewGuid().ToString(), Guid.NewGuid().ToString())
    {
      PortalStage = "PortalStage",
      OfferedProgramTypes = programTypes,
      ProgramTypes = programTypes,
      Courses = !addCourse ? null : new[]
      {
        new Course()
        {
          CourseId = Guid.NewGuid().ToString(),
          CourseNumber = "101",
          CourseTitle = "Course 101",
          NewCourseNumber = "102",
          NewCourseTitle = "Course 102",
          ProgramType = programTypes.First(),
          CourseAreaOfInstruction = new []
          {
            new CourseAreaOfInstruction()
            {
              AreaOfInstructionId = areaOfInstructionId,
              CourseAreaOfInstructionId = Guid.NewGuid().ToString(),
              NewHours = newHours
            }
          }
        }
      }
    };
  }
}
