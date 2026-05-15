using ECER.Engines.Validation.Programs;
using ECER.Managers.Registry.Contract.Programs;
using ECER.Resources.Documents.MetadataResources;
using ECER.Resources.Documents.Programs;
using Mediator;
using ProgramStatus = ECER.Resources.Documents.Programs.ProgramStatus;

namespace ECER.Managers.Registry;

public class ProgramHandlers(
    IProgramRepository programRepository,
    IMetadataResourceRepository metadataResourceRepository,
    IProgramMapper programMapper,
    IProgramValidationEngineResolver validationResolver)
  : IRequestHandler<SaveDraftProgramCommand, Contract.Programs.Program?>,
    IRequestHandler<ProgramsQuery, ProgramsQueryResults>,
    IRequestHandler<UpdateProgramCommand, string>,
    IRequestHandler<SubmitProgramCommand, SubmitProgramResult>,
    IRequestHandler<ChangeProgramCommand, string>
{
  public async ValueTask<Contract.Programs.Program?> Handle(SaveDraftProgramCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var programId = await programRepository.Save(programMapper.MapProgram(request.Program), cancellationToken);

    var result = (await programRepository.Query(new ProgramQuery
    {
      ById = programId,
      ByPostSecondaryInstituteId = request.Program.PostSecondaryInstituteId
    }, cancellationToken));

    var program = result.Programs?.SingleOrDefault();

    return programMapper.MapProgram(program);
  }

  public async ValueTask<ProgramsQueryResults> Handle(ProgramsQuery request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var statusFilter = request.ByStatus != null
      ? programMapper.MapProgramStatuses(request.ByStatus)
      : null;

    Resources.Documents.Programs.ProgramProfileType? profileTypeFilter = request.ByProgramProfileType.HasValue
      ? programMapper.MapProgramProfileType(request.ByProgramProfileType.Value)
      : null;

    var result = await programRepository.Query(new ProgramQuery
    {
      ById = request.ById,
      ByPostSecondaryInstituteId = request.ByPostSecondaryInstituteId,
      ByStatus = statusFilter,
      ByProgramProfileType = profileTypeFilter,
      ByFromProgramProfileId = request.ByFromProgramProfileId,
      ByCampusId = request.ByCampusId,
      PageNumber = request.PageNumber,
      PageSize = request.PageSize,
    }, cancellationToken);

    return new ProgramsQueryResults(programMapper.MapPrograms(result.Programs ?? Array.Empty<Resources.Documents.Programs.Program>()), result.TotalProgramsCount);
  }

  public async ValueTask<string> Handle(UpdateProgramCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    var programId = await programRepository.UpdateProgram(programMapper.MapProgram(request.Program), cancellationToken);
    return programId;
  }

  public async ValueTask<string> Handle(ChangeProgramCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    var programId = await programRepository.ChangeProgram(programMapper.MapProgram(request.Program), cancellationToken);
    return programId;
  }

  public async ValueTask<SubmitProgramResult> Handle(SubmitProgramCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var programResult = await programRepository.Query(new ProgramQuery
    {
      ById = request.ProgramId,
      ByStatus = new[] { ProgramStatus.Draft }
    }, cancellationToken);

    if (!programResult.Programs!.Any())
    {
      return new SubmitProgramResult { ProgramId = null, Error = ProgramSubmissionError.DraftApplicationNotFound, ValidationErrors = new List<string>() { "Draft program profile does not exist" } };
    }

    var draftProgram = programMapper.MapProgram(programResult.Programs!.First())!;
    var instructions = await metadataResourceRepository.QueryAreaOfInstructions(new AreaOfInstructionsQuery() { ById = null }, cancellationToken);

    var validationEngine = validationResolver?.resolve(draftProgram.ProgramProfileType);
    var validationErrors = await validationEngine?.Validate(draftProgram, instructions.ToList())!;
    if (validationErrors.ValidationErrors.Any())
    {
      return new SubmitProgramResult { ProgramId = null, Error = ProgramSubmissionError.DraftApplicationValidationFailed, ValidationErrors = validationErrors.ValidationErrors };
    }

    var programId = await programRepository.SubmitProgramProfile(draftProgram.Id!, request.UserId, cancellationToken);
    return new SubmitProgramResult { ProgramId = programId };
  }
}
