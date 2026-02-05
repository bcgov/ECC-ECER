using AutoMapper;
using ECER.Managers.Registry.Contract.Courses;
using ECER.Resources.Documents.Courses;
using ECER.Resources.Documents.Programs;
using MediatR;

namespace ECER.Managers.Registry;

public class CoursesHandler(
  IProgramRepository programRepository,
  ICourseRepository courseRepository,
  IMapper mapper)
:IRequestHandler<UpdateCourseCommand, string>
{
  public async Task<string> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    if (request.Type == nameof(FunctionType.ProgramProfile))
    {
      var programProfile = await programRepository.Query(new ProgramQuery
      {
        ById = request.Id,
        ByPostSecondaryInstituteId = request.PostSecondaryInstituteId,
      }, cancellationToken);
      
      if (!programProfile.Programs!.Any()) throw new InvalidOperationException($"Program profile with '{request.Id}' not found");;
    }
    var programId = await courseRepository.UpdateCourse(mapper.Map<IEnumerable<Resources.Documents.Shared.Course>>(request.Course)!, request.Id, cancellationToken);
    return programId;
  }
}
