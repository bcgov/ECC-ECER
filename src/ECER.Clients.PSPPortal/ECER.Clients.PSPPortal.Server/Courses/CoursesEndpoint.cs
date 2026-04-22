using AutoMapper;
using ECER.Clients.PSPPortal.Server.Programs;
using ECER.Clients.PSPPortal.Server.Shared;
using ECER.Infrastructure.Common;
using ECER.Infrastructure.Common.Validators;
using ECER.Managers.Registry.Contract.Courses;
using ECER.Managers.Registry.Contract.ProgramApplications;
using ECER.Managers.Registry.Contract.PspUsers;
using ECER.Utilities.Hosting;
using ECER.Utilities.Security;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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

    endpointRouteBuilder.MapPost("/api/courses", async Task<Results<Ok<string>, BadRequest<ProblemDetails>, NotFound>> (AddCourseRequest request, HttpContext ctx, CancellationToken ct, IMediator messageBus, IMapper mapper) =>
      {
        if (request.Type == FunctionType.ProgramProfile) return TypedResults.BadRequest(new ProblemDetails() { Title = "User cannot add courses for a Program Profile" });

        var existing = await messageBus.Send(new ProgramApplicationQuery { ById = request.ApplicationId }, ct);
        if (!existing.Items.Any()) return TypedResults.NotFound();

        var userContext = ctx.User.GetUserContext()!;
        var programRep = (await messageBus.Send<PspRepQueryResults>(new SearchPspRepQuery { ByUserIdentity = userContext.Identity }, ct)).Items.SingleOrDefault();
        if (programRep == null || string.IsNullOrWhiteSpace(programRep.PostSecondaryInstituteId)) return TypedResults.NotFound();

        var mappedCourse = mapper.Map<Managers.Registry.Contract.Shared.Course>(request.Course);

        var result = await messageBus.Send(new SaveCourseCommand(mappedCourse, request.ApplicationId, programRep.PostSecondaryInstituteId), ct);

        if (result.Error == SaveCourseError.ProgramApplicationNotFound)
        {
          return TypedResults.BadRequest(new ProblemDetails() { Title = $"Program application id {request.ApplicationId} not found" });
        }

        if (result.Error == SaveCourseError.IncorrectProgramApplicationTypeToSaveCourse)
        {
          return TypedResults.BadRequest(new ProblemDetails() { Title = $"Unable to add course for application id {request.ApplicationId}" });
        }

        return TypedResults.Ok(result.CourseId);
      })
      .WithOpenApi("Add a course for a program application", string.Empty, "course_post")
      .RequireAuthorization(PolicyNames)
      .WithParameterValidation();

    endpointRouteBuilder.MapDelete("/api/courses/{courseId}", async Task<Results<Ok<string>, BadRequest<string>, NotFound>> (string courseId, string? applicationId, HttpContext ctx, CancellationToken ct, IMediator messageBus) =>
      {
        if (string.IsNullOrWhiteSpace(courseId)) return TypedResults.BadRequest("course id cannot be null or whitespace");
        bool IdIsNotGuid = !Guid.TryParse(courseId, out _);

        if (IdIsNotGuid) return TypedResults.BadRequest("invalid course id");

        var userContext = ctx.User.GetUserContext()!;
        var programRep = (await messageBus.Send<PspRepQueryResults>(new SearchPspRepQuery { ByUserIdentity = userContext.Identity }, ct)).Items.SingleOrDefault();
        if (programRep == null || string.IsNullOrWhiteSpace(programRep.PostSecondaryInstituteId)) return TypedResults.NotFound();

        var deletedCourseId = await messageBus.Send(new DeleteCourseCommand(courseId, programRep.PostSecondaryInstituteId, applicationId), ct);
        return TypedResults.Ok(deletedCourseId);
      })
      .WithOpenApi("Deletes a course", "string.Empty", "course_delete")
      .RequireAuthorization(PolicyNames)
      .WithParameterValidation();

    endpointRouteBuilder.MapGet("/api/courses", async Task<Results<Ok<IEnumerable<Course>>, BadRequest<string>, NotFound>> (FunctionType type, string id, ProgramTypes[]? programTypes, HttpContext ctx, CancellationToken ct, IMediator messageBus, IMapper mapper) =>
    {
      var userContext = ctx.User.GetUserContext()!;
      var programRep = (await messageBus.Send<PspRepQueryResults>(new SearchPspRepQuery { ByUserIdentity = userContext.Identity }, ct)).Items.SingleOrDefault();
      if (programRep == null || string.IsNullOrWhiteSpace(programRep.PostSecondaryInstituteId)) return TypedResults.NotFound();

      var courses = await messageBus.Send(new GetCoursesCommand(id, programRep.PostSecondaryInstituteId, type.Convert<FunctionType, Managers.Registry.Contract.Courses.FunctionType>())
      {
        ProgramTypes = mapper.Map<Managers.Registry.Contract.Programs.ProgramTypes[]>(programTypes)
      }, ct);
      return TypedResults.Ok(mapper.Map<IEnumerable<Course>>(courses));
    })
    .WithOpenApi("Gets courses by program profile id or program application id depending on type", "string.Empty", "courses_get")
    .RequireAuthorization(PolicyNames)
    .AddGuidValidationQueryParams(["id"])
    .WithParameterValidation();
  }
}

public record UpdateCourseRequest(Course Course, FunctionType Type, string Id);

public record AddCourseRequest(Course? Course, FunctionType Type, [ValidGuid][Required] string ApplicationId);

public enum FunctionType
{
  ProgramProfile,
  ProgramApplication
}
