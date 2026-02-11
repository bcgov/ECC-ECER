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
    endpointRouteBuilder.MapPut("/api/courses/{courseId}", async Task<Results<Ok<string>, BadRequest<string>, NotFound>> (string courseId, UpdateCourseRequest request, HttpContext ctx, CancellationToken ct, IMediator messageBus, IMapper mapper) =>
      {
        if (string.IsNullOrWhiteSpace(request.Id)) return TypedResults.BadRequest("id cannot be null or whitespace");
        bool IdIsNotGuid = !Guid.TryParse(request.Id, out _);
        if (IdIsNotGuid) return TypedResults.BadRequest("invalid id");
        
        if (request.Course.CourseId != courseId) return TypedResults.BadRequest("resource id and payload id do not match");

        var userContext = ctx.User.GetUserContext()!;
        var programRep = (await messageBus.Send<PspRepQueryResults>(new SearchPspRepQuery { ByUserIdentity = userContext.Identity }, ct)).Items.SingleOrDefault();
        if (programRep == null || string.IsNullOrWhiteSpace(programRep.PostSecondaryInstituteId)) return TypedResults.NotFound();
        
        var mappedCourses = mapper.Map<Managers.Registry.Contract.Shared.Course>(request.Course);

        var result = await messageBus.Send(new UpdateCourseCommand(mappedCourses, request.Id, request.Type.ToString(), programRep.PostSecondaryInstituteId), ct);
        return TypedResults.Ok(result);
      })
      .WithOpenApi("Update a course for a program profile", string.Empty, "course_put")
      .RequireAuthorization(PolicyNames)
      .WithParameterValidation();
    
    endpointRouteBuilder.MapPost("/api/courses/{id}", async Task<Results<Ok<string>, BadRequest<string>, NotFound>> (string id, AddCourseRequest request, HttpContext ctx, CancellationToken ct, IMediator messageBus, IMapper mapper) =>
      {
        if (string.IsNullOrWhiteSpace(id)) return TypedResults.BadRequest("id cannot be null or whitespace");
        bool IdIsNotGuid = !Guid.TryParse(id, out _);

        if (IdIsNotGuid) return TypedResults.BadRequest("invalid id");

        if (request.Type == FunctionType.ProgramProfile) return TypedResults.BadRequest("User cannot add courses for a Program Profile");

        var existing = await messageBus.Send(new ProgramApplicationQuery { ById = id }, ct);
        if (!existing.Items.Any()) return TypedResults.NotFound();

        var userContext = ctx.User.GetUserContext()!;
        var programRep = (await messageBus.Send<PspRepQueryResults>(new SearchPspRepQuery { ByUserIdentity = userContext.Identity }, ct)).Items.SingleOrDefault();
        if (programRep == null || string.IsNullOrWhiteSpace(programRep.PostSecondaryInstituteId)) return TypedResults.NotFound();
        
        var mappedCourse = mapper.Map<Managers.Registry.Contract.Shared.Course>(request.Course);

        var result = await messageBus.Send(new SaveCourseCommand(mappedCourse, id, programRep.PostSecondaryInstituteId), ct);
        return TypedResults.Ok(result);
      })
      .WithOpenApi("Add a course for a program profile", string.Empty, "course_post")
      .RequireAuthorization(PolicyNames)
      .WithParameterValidation();
  }
}

public record UpdateCourseRequest(Course Course, FunctionType Type, string Id);

public record AddCourseRequest(Course? Course, FunctionType Type);

public enum FunctionType
{
  ProgramProfile,
  ProgramApplication
}
