using AutoMapper;
using ECER.Clients.PSPPortal.Server.Shared;
using ECER.Managers.Registry.Contract.Courses;
using ECER.Managers.Registry.Contract.ProgramApplications;
using ECER.Managers.Registry.Contract.PspUsers;
using ECER.Utilities.Hosting;
using ECER.Utilities.Security;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

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
    
    endpointRouteBuilder.MapPost("/api/courses", async Task<Results<Ok<string>, BadRequest<string>, NotFound>> (AddCourseRequest request, HttpContext ctx, CancellationToken ct, IMediator messageBus, IMapper mapper) =>
      {
        if (string.IsNullOrWhiteSpace(request.ApplicationId)) return TypedResults.BadRequest("id cannot be null or whitespace");
        bool IdIsNotGuid = !Guid.TryParse(request.ApplicationId, out _);

        if (IdIsNotGuid) return TypedResults.BadRequest("invalid id");

        if (request.Type == FunctionType.ProgramProfile) return TypedResults.BadRequest("User cannot add courses for a Program Profile");

        var existing = await messageBus.Send(new ProgramApplicationQuery { ById = request.ApplicationId }, ct);
        if (!existing.Items.Any()) return TypedResults.NotFound();

        var userContext = ctx.User.GetUserContext()!;
        var programRep = (await messageBus.Send<PspRepQueryResults>(new SearchPspRepQuery { ByUserIdentity = userContext.Identity }, ct)).Items.SingleOrDefault();
        if (programRep == null || string.IsNullOrWhiteSpace(programRep.PostSecondaryInstituteId)) return TypedResults.NotFound();
        
        var mappedCourse = mapper.Map<Managers.Registry.Contract.Shared.Course>(request.Course);

        var result = await messageBus.Send(new SaveCourseCommand(mappedCourse, request.ApplicationId, programRep.PostSecondaryInstituteId), ct);
        return TypedResults.Ok(result);
      })
      .WithOpenApi("Add a course for a program profile", string.Empty, "course_post")
      .RequireAuthorization(PolicyNames)
      .WithParameterValidation();
    
    endpointRouteBuilder.MapDelete("/api/courses/{courseId}", async Task<Results<Ok<string>, BadRequest<string>>> (string courseId, HttpContext ctx, CancellationToken ct, IMediator messageBus) =>
      {
        if (string.IsNullOrWhiteSpace(courseId)) return TypedResults.BadRequest("course id cannot be null or whitespace");
        bool IdIsNotGuid = !Guid.TryParse(courseId, out _);

        if (IdIsNotGuid) return TypedResults.BadRequest("invalid course id");
        
        var deletedCourseId = await messageBus.Send(new DeleteCourseCommand(courseId), ct);
        return TypedResults.Ok(deletedCourseId);
      })
      .WithOpenApi("Deletes a course", "string.Empty", "course_delete")
      .RequireAuthorization(PolicyNames)
      .WithParameterValidation();
  }
}

public record UpdateCourseRequest(IEnumerable<Course>? Courses, FunctionType Type);
public record AddCourseRequest(Course? Course, FunctionType Type, string ApplicationId);

public enum FunctionType
{
  ProgramProfile,
  ProgramApplication
}
