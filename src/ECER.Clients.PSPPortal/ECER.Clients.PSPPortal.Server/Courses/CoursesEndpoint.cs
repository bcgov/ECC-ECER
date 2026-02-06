using System.ComponentModel.DataAnnotations;
using AutoMapper;
using ECER.Clients.PSPPortal.Server.Shared;
using ECER.Managers.Registry.Contract.PspUsers;
using ECER.Utilities.Hosting;
using ECER.Utilities.Security;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using UpdateCourseCommand = ECER.Managers.Registry.Contract.Courses.UpdateCourseCommand;

namespace ECER.Clients.PSPPortal.Server.Courses;

public class CoursesEndpoint : IRegisterEndpoints
{
  public void Register(IEndpointRouteBuilder endpointRouteBuilder)
  {
    const string PolicyNames = "psp_user";
    endpointRouteBuilder.MapPut("/api/courses/{id}", async Task<Results<Ok<string>, BadRequest<string>, NotFound>> (string id, UpdateCourseRequest request, HttpContext ctx, CancellationToken ct, IMediator messageBus, IMapper mapper) =>
      {
        if (string.IsNullOrWhiteSpace(id)) return TypedResults.BadRequest("id cannot be null or whitespace");
        bool IdIsNotGuid = !Guid.TryParse(id, out _);

        if (IdIsNotGuid) return TypedResults.BadRequest("invalid id");

        var userContext = ctx.User.GetUserContext()!;
        var programRep = (await messageBus.Send<PspRepQueryResults>(new SearchPspRepQuery { ByUserIdentity = userContext.Identity }, ct)).Items.SingleOrDefault();
        if (programRep == null || string.IsNullOrWhiteSpace(programRep.PostSecondaryInstituteId)) return TypedResults.NotFound();
        
        var mappedCourses = mapper.Map<IEnumerable<Managers.Registry.Contract.Shared.Course>>(request.Courses);

        var result = await messageBus.Send(new UpdateCourseCommand(mappedCourses, id, request.Type.ToString(), programRep.PostSecondaryInstituteId), ct);
        return TypedResults.Ok(result);
      })
      .WithOpenApi("Update a course for a program profile", string.Empty, "course_put")
      .RequireAuthorization(PolicyNames)
      .WithParameterValidation();
  }
}

public record UpdateCourseRequest(IEnumerable<Course>? Courses, FunctionType Type);

public enum FunctionType
{
  ProgramProfile,
  ProgramApplication
}
