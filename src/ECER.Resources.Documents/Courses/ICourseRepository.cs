using ECER.Resources.Documents.Shared;

namespace ECER.Resources.Documents.Courses;

public interface ICourseRepository
{
  Task<string> UpdateCourse(IEnumerable<Course> incomingCourse, string id, CancellationToken cancellationToken);
}
