using AutoMapper;
using ECER.Managers.Registry.Contract.Courses;
using ECER.Resources.Documents.Courses;
using ECER.Resources.Documents.ProgramApplications;
using ECER.Resources.Documents.Programs;
using MediatR;

namespace ECER.Managers.Registry;

public class CoursesHandler(
  IProgramRepository programRepository,
  ICourseRepository courseRepository,
  IProgramApplicationRepository  programApplicationRepository,
  IMapper mapper)
: IRequestHandler<UpdateCourseCommand, string>, 
  IRequestHandler<SaveCourseCommand, string>
{
  public async Task<string> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    ArgumentNullException.ThrowIfNull(request.Course);
    Infrastructure.Common.Utility.ThrowIfNullOrEmpty(request.Course, nameof(request.Course));

    if (request.Type == nameof(FunctionType.ProgramProfile))
    {
      var programProfile = await programRepository.Query(new ProgramQuery
      {
        ById = request.Id,
        ByPostSecondaryInstituteId = request.PostSecondaryInstituteId
      }, cancellationToken);
      
      if (!programProfile.Programs!.Any()) throw new InvalidOperationException($"Program profile with '{request.Id}' not found");
      
      var programId = await courseRepository.UpdateCourse(mapper.Map<IEnumerable<Resources.Documents.Shared.Course>>(request.Course)!, request.Id, cancellationToken);
      return programId;
    } else if (request.Type == nameof(FunctionType.ProgramApplication))
    {
      var programApplication = await programApplicationRepository.Query(new ProgramApplicationQuery
        {
          ById = request.Id,
          ByPostSecondaryInstituteId = request.PostSecondaryInstituteId
        }, cancellationToken);
      if (!programApplication.Items!.Any()) throw new InvalidOperationException($"Program application with '{request.Id}' not found");
      
      var programId = await courseRepository.UpdateCourse(mapper.Map<IEnumerable<Resources.Documents.Shared.Course>>(request.Course)!, request.Id, cancellationToken);
      return programId;
    }
    throw new InvalidOperationException("Operation not allowed");
  }
  
  public async Task<string> Handle(SaveCourseCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    ArgumentNullException.ThrowIfNull(request.Course);
    ArgumentNullException.ThrowIfNull(request.Course.CourseAreaOfInstruction);
    Infrastructure.Common.Utility.ThrowIfNullOrEmpty(request.Course.CourseAreaOfInstruction, nameof(request.Course.CourseAreaOfInstruction));
    
    var courseId = await courseRepository.AddCourse(mapper.Map<Resources.Documents.Shared.Course>(request.Course)!, request.Id, request.PostSecondaryInstituteId, cancellationToken);
    return courseId;
  }
}
