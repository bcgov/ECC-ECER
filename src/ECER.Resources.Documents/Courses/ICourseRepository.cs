using ECER.Resources.Documents.Programs;
using ECER.Resources.Documents.Shared;

namespace ECER.Resources.Documents.Courses;

public interface ICourseRepository
{
  Task<string> UpdateCourse(Course course, string id, bool isProgramApplication, CancellationToken cancellationToken);

  Task<string> AddCourse(Course incomingCourse, string id, string postSecondaryInstituteId, CancellationToken cancellationToken);

  Task<string> DeleteCourse(string courseId, string postSecondaryInstituteId, CancellationToken cancellationToken);

  Task<IEnumerable<Course>> GetCourses(GetCoursesRequest getCoursesRequest, CancellationToken cancellationToken);
}

public record GetCoursesRequest(string Id, string PostSecondaryInstituteId, FunctionType Type)
{
  public IEnumerable<ProgramType>? ProgramTypes { get; set; }
};

public enum FunctionType
{
  ProgramProfile,
  ProgramApplication
}
