using AutoMapper;
using ECER.Infrastructure.Common;
using ECER.Managers.Registry.Contract.PspUsers;
using ECER.Utilities.Hosting;
using ECER.Utilities.Security;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using ContractProgram = ECER.Managers.Registry.Contract.Programs.Program;
using ContractProgramStatus = ECER.Managers.Registry.Contract.Programs.ProgramStatus;
using ContractProgramsQuery = ECER.Managers.Registry.Contract.Programs.ProgramsQuery;
using SaveDraftProgramCommand = ECER.Managers.Registry.Contract.Programs.SaveDraftProgramCommand;

namespace ECER.Clients.PSPPortal.Server.Programs;

public class ProgramsEndpoints : IRegisterEndpoints
{
  public void Register(IEndpointRouteBuilder endpointRouteBuilder)
  {
    endpointRouteBuilder.MapPut("/api/draftprograms/{id?}", async Task<Results<Ok<DraftProgramResponse>, BadRequest<string>, NotFound>> (string? id, SaveDraftProgramRequest request, HttpContext ctx, CancellationToken ct, IMediator messageBus, IMapper mapper) =>
    {
      bool IdIsNotGuid = !Guid.TryParse(id, out _); if (IdIsNotGuid && id != null) { id = null; }
      bool ProgramIdIsNotGuid = !Guid.TryParse(request.Program.Id, out _); if (ProgramIdIsNotGuid && request.Program.Id != null) { request.Program.Id = null; }

      if (request.Program.Id != id) return TypedResults.BadRequest("resource id and payload id do not match");

      var userContext = ctx.User.GetUserContext()!;
      var programRep = (await messageBus.Send<PspRepQueryResults>(new SearchPspRepQuery { ByUserIdentity = userContext.Identity }, ct)).Items.SingleOrDefault();
      if (programRep == null || string.IsNullOrWhiteSpace(programRep.PostSecondaryInstituteId)) return TypedResults.NotFound();

      var draftProgram = mapper.Map<ContractProgram>(request.Program)
        with { PostSecondaryInstituteId = programRep.PostSecondaryInstituteId, Status = ContractProgramStatus.Draft };

      if (id != null)
      {
        var existing = await messageBus.Send(new ContractProgramsQuery
        {
          ById = id,
          ByPostSecondaryInstituteId = programRep.PostSecondaryInstituteId,
          ByStatus = new[] { ContractProgramStatus.Draft }
        }, ct);

        if (!existing.Items.Any()) return TypedResults.NotFound();
      }

      var program = await messageBus.Send(new SaveDraftProgramCommand(draftProgram), ct);
      var mappedProgram = mapper.Map<Program>(program);
      if (mappedProgram == null) return TypedResults.NotFound();

      return TypedResults.Ok(new DraftProgramResponse(mappedProgram));
    })
    .WithOpenApi("Save a draft program for the current user", string.Empty, "draftprogram_put")
    .RequireAuthorization("psp_user")
    .WithParameterValidation();

    endpointRouteBuilder.MapGet("/api/programs/{id?}", async Task<Results<Ok<IEnumerable<Program>>, NotFound>> (string? id, ProgramStatus[]? byStatus, HttpContext ctx, IMediator messageBus, IMapper mapper, CancellationToken ct) =>
    {
      bool IdIsNotGuid = !Guid.TryParse(id, out _); if (IdIsNotGuid) { id = null; }

      var userContext = ctx.User.GetUserContext()!;
      var programRep = (await messageBus.Send<PspRepQueryResults>(new SearchPspRepQuery { ByUserIdentity = userContext.Identity }, ct)).Items.SingleOrDefault();
      if (programRep == null || string.IsNullOrWhiteSpace(programRep.PostSecondaryInstituteId)) return TypedResults.NotFound();

      var statusFilter = (byStatus != null && byStatus.Length > 0)
        ? byStatus.Convert<ProgramStatus, ContractProgramStatus>()
        : null;

      var results = await messageBus.Send(new ContractProgramsQuery
      {
        ById = id,
        ByPostSecondaryInstituteId = programRep.PostSecondaryInstituteId,
        ByStatus = statusFilter
      }, ct);

      return TypedResults.Ok(mapper.Map<IEnumerable<Program>>(results.Items));
    })
    .WithOpenApi("Handles program queries", string.Empty, "program_get")
    .RequireAuthorization("psp_user")
    .WithParameterValidation();
  }
}

public record SaveDraftProgramRequest(Program Program);

public record DraftProgramResponse(Program Program);

public record Program
{
  public string? Id { get; set; }

  [Required]
  public string PortalStage { get; set; } = null!;
  public ProgramStatus Status { get; set; } = ProgramStatus.Draft;
  public DateTime? CreatedOn { get; set; }
  public string? Name { get; set; }
  public string? PostSecondaryInstituteName { get; set; }
  public DateTime? StartDate { get; set; }
  public DateTime? EndDate { get; set; }
  public IEnumerable<ProgramTypes>? ProgramTypes { get; set; }
}

public enum ProgramStatus
{
  Draft,
  UnderReview,
  Approved,
  Denied,
  Inactive
}
public enum ProgramTypes
{
  Basic,
  SNE,
  ITE
}
