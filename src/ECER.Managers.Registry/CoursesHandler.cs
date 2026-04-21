using AutoMapper;
using ECER.Engines.Validation;
using ECER.Infrastructure.Common;
using ECER.Managers.Registry.Contract.Courses;
using ECER.Resources.Documents.Courses;
using ECER.Resources.Documents.MetadataResources;
using ECER.Resources.Documents.ProgramApplications;
using ECER.Resources.Documents.Programs;
using ECER.Utilities.DataverseSdk.Model;
using MediatR;

namespace ECER.Managers.Registry;

public class CoursesHandler(
  IProgramRepository programRepository,
  ICourseRepository courseRepository,
  IProgramApplicationRepository programApplicationRepository,
  IMetadataResourceRepository metadataResourceRepository,
  ICourseProgressEvaluator courseProgressEvaluator,
  IMapper mapper,
  EcerContext ecerContext)
: IRequestHandler<UpdateCourseCommand, string>,
  IRequestHandler<SaveCourseCommand, SaveCourseCommandResult>,
  IRequestHandler<DeleteCourseCommand, string>,
  IRequestHandler<GetCoursesCommand, IEnumerable<Contract.Shared.Course>>
{
  public async Task<string> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    ArgumentNullException.ThrowIfNull(request.Course);

    if (request.Type == nameof(Contract.Courses.FunctionType.ProgramProfile))
    {
      var programProfile = await programRepository.Query(new ProgramQuery
      {
        ById = request.Id,
        ByPostSecondaryInstituteId = request.PostSecondaryInstituteId
      }, cancellationToken);

      if (!programProfile.Programs!.Any()) throw new InvalidOperationException($"Program profile with '{request.Id}' not found");

      ecerContext.BeginTransaction();
      await courseRepository.UpdateCourse(mapper.Map<Resources.Documents.Shared.Course>(request.Course)!, request.Id, false, cancellationToken);
      ecerContext.CommitTransaction();
      return request.Id;
    }

    if (request.Type == nameof(Contract.Courses.FunctionType.ProgramApplication))
    {
      var programApplicationResult = await programApplicationRepository.Query(new ProgramApplicationQuery
      {
        ById = request.Id,
        ByPostSecondaryInstituteId = request.PostSecondaryInstituteId
      }, cancellationToken);
      if (!programApplicationResult.Items!.Any()) throw new InvalidOperationException($"Program application with '{request.Id}' not found");

      var existingCourses = await GetCoursesAsync(request.Id, request.PostSecondaryInstituteId, cancellationToken);
      var adjustedCourses = existingCourses.Where(c => c.CourseId != request.Course.CourseId).Append(request.Course).ToList();
      var areasOfInstruction = await GetAreasOfInstructionAsync(cancellationToken);

      ecerContext.BeginTransaction();
      await courseRepository.UpdateCourse(mapper.Map<Resources.Documents.Shared.Course>(request.Course)!, request.Id, true, cancellationToken);
      await UpdateCourseProgressAsync(request.Id, adjustedCourses, programApplicationResult.Items.First().ProgramTypes, areasOfInstruction, cancellationToken);
      ecerContext.CommitTransaction();
      return request.Id;
    }

    throw new InvalidOperationException("Operation not allowed");
  }

  public async Task<SaveCourseCommandResult> Handle(SaveCourseCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    ArgumentNullException.ThrowIfNull(request.Course);
    ArgumentNullException.ThrowIfNull(request.Course.CourseAreaOfInstruction);

    var programApplications = await programApplicationRepository.Query(new ProgramApplicationQuery
    {
      ById = request.Id,
      ByPostSecondaryInstituteId = request.PostSecondaryInstituteId
    }, cancellationToken);
    if (!programApplications.Items!.Any())
    {
      return new SaveCourseCommandResult() { Error = SaveCourseError.ProgramApplicationNotFound };
    }

    var programApplication = programApplications.Items.First();
    if (programApplication.ProgramApplicationType != ApplicationType.NewBasicECEPostBasicProgram)
    {
      return new SaveCourseCommandResult() { Error = SaveCourseError.IncorrectProgramApplicationTypeToSaveCourse };
    }

    var existingCourses = await GetCoursesAsync(request.Id, request.PostSecondaryInstituteId, cancellationToken);
    var adjustedCourses = existingCourses.Append(request.Course).ToList();
    var areasOfInstruction = await GetAreasOfInstructionAsync(cancellationToken);

    ecerContext.BeginTransaction();
    var courseId = await courseRepository.AddCourse(mapper.Map<Resources.Documents.Shared.Course>(request.Course)!, request.Id, request.PostSecondaryInstituteId, cancellationToken);
    await UpdateCourseProgressAsync(request.Id, adjustedCourses, programApplication.ProgramTypes, areasOfInstruction, cancellationToken);
    ecerContext.CommitTransaction();
    return new SaveCourseCommandResult() { CourseId = courseId };
  }

  public async Task<IEnumerable<ECER.Managers.Registry.Contract.Shared.Course>> Handle(GetCoursesCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var courses = await courseRepository.GetCourses(new GetCoursesRequest(request.Id, request.PostSecondaryInstituteId, request.Type.Convert<Contract.Courses.FunctionType, Resources.Documents.Courses.FunctionType>())
    {
      ProgramTypes = mapper.Map<IEnumerable<ProgramType>>(request.ProgramTypes)
    }, cancellationToken);
    return mapper.Map<IEnumerable<Contract.Shared.Course>>(courses);
  }

  public async Task<string> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    ArgumentNullException.ThrowIfNull(request.CourseId);

    if (request.ApplicationId != null)
    {
      var programApplicationResult = await programApplicationRepository.Query(new ProgramApplicationQuery
      {
        ById = request.ApplicationId,
        ByPostSecondaryInstituteId = request.PostSecondaryInstituteId
      }, cancellationToken);

      var existingCourses = await GetCoursesAsync(request.ApplicationId, request.PostSecondaryInstituteId, cancellationToken);
      var adjustedCourses = existingCourses.Where(c => c.CourseId != request.CourseId).ToList();
      var areasOfInstruction = await GetAreasOfInstructionAsync(cancellationToken);

      ecerContext.BeginTransaction();
      await courseRepository.DeleteCourse(request.CourseId, request.PostSecondaryInstituteId, cancellationToken);
      await UpdateCourseProgressAsync(request.ApplicationId, adjustedCourses, programApplicationResult.Items.FirstOrDefault()?.ProgramTypes, areasOfInstruction, cancellationToken);
      ecerContext.CommitTransaction();
    }
    else
    {
      ecerContext.BeginTransaction();
      await courseRepository.DeleteCourse(request.CourseId, request.PostSecondaryInstituteId, cancellationToken);
      ecerContext.CommitTransaction();
    }

    return request.CourseId;
  }

  private async Task<IList<Contract.Shared.Course>> GetCoursesAsync(string applicationId, string postSecondaryInstituteId, CancellationToken cancellationToken)
  {
    var resourceCourses = await courseRepository.GetCourses(
      new GetCoursesRequest(applicationId, postSecondaryInstituteId, Resources.Documents.Courses.FunctionType.ProgramApplication),
      cancellationToken);
    return mapper.Map<IList<Contract.Shared.Course>>(resourceCourses);
  }

  private async Task<IReadOnlyCollection<AreaOfInstruction>> GetAreasOfInstructionAsync(CancellationToken cancellationToken)
  {
    return (await metadataResourceRepository.QueryAreaOfInstructions(
      new AreaOfInstructionsQuery { ById = null },
      cancellationToken)).ToList().AsReadOnly();
  }

  private async Task UpdateCourseProgressAsync(
    string applicationId,
    IList<Contract.Shared.Course> courses,
    IEnumerable<ProgramCertificationType>? programTypes,
    IReadOnlyCollection<AreaOfInstruction> areasOfInstruction,
    CancellationToken cancellationToken)
  {
    if (programTypes == null) return;

    var offeredTypes = programTypes.ToList();
    if (offeredTypes.Count == 0) return;

    var basicProgress = offeredTypes.Contains(ProgramCertificationType.Basic)
      ? courseProgressEvaluator.EvaluateProgress(courses, nameof(ProgramCertificationType.Basic), areasOfInstruction, checkTotalHours: false)
      : null;

    var iteProgress = offeredTypes.Contains(ProgramCertificationType.ITE)
      ? courseProgressEvaluator.EvaluateProgress(courses, nameof(ProgramCertificationType.ITE), areasOfInstruction, checkTotalHours: true)
      : null;

    var sneProgress = offeredTypes.Contains(ProgramCertificationType.SNE)
      ? courseProgressEvaluator.EvaluateProgress(courses, nameof(ProgramCertificationType.SNE), areasOfInstruction, checkTotalHours: true)
      : null;

    await programApplicationRepository.UpdateCourseProgress(applicationId, basicProgress, iteProgress, sneProgress, cancellationToken);
  }
}
