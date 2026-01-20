using AutoMapper;
using ECER.Engines.Validation.Programs;
using ECER.Managers.Registry.Contract.Programs;
using ECER.Resources.Documents.MetadataResources;
using ECER.Resources.Documents.Programs;
using MediatR;
using ProgramStatus = ECER.Resources.Documents.Programs.ProgramStatus;

namespace ECER.Managers.Registry;

public class ProgramHandlers(
    IProgramRepository programRepository,
    IMetadataResourceRepository metadataResourceRepository,
    IMapper mapper,
    IProgramValidationEngineResolver validationResolver)
  : IRequestHandler<SaveDraftProgramCommand, Contract.Programs.Program?>,
    IRequestHandler<ProgramsQuery, ProgramsQueryResults>,
    IRequestHandler<UpdateCourseCommand, string>,
    IRequestHandler<UpdateProgramCommand, string>,
    IRequestHandler<SubmitProgramCommand, SubmitProgramResult>
{
  public async Task<Contract.Programs.Program?> Handle(SaveDraftProgramCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var programId = await programRepository.Save(mapper.Map<Resources.Documents.Programs.Program>(request.Program)!, cancellationToken);

    var result = (await programRepository.Query(new ProgramQuery
    {
      ById = programId,
      ByPostSecondaryInstituteId = request.Program.PostSecondaryInstituteId
    }, cancellationToken)).SingleOrDefault();

    return mapper.Map<Contract.Programs.Program>(result);
  }

  public async Task<string> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    var programId = await programRepository.UpdateCourse(mapper.Map<IEnumerable<Resources.Documents.Programs.Course>>(request.Course)!, request.Id, cancellationToken);
    return programId;
  }

  public async Task<ProgramsQueryResults> Handle(ProgramsQuery request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var statusFilter = request.ByStatus != null
      ? mapper.Map<IEnumerable<Resources.Documents.Programs.ProgramStatus>>(request.ByStatus)
      : null;

    var programs = await programRepository.Query(new ProgramQuery
    {
      ById = request.ById,
      ByPostSecondaryInstituteId = request.ByPostSecondaryInstituteId,
      ByStatus = statusFilter
    }, cancellationToken);

    return new ProgramsQueryResults(mapper.Map<IEnumerable<Contract.Programs.Program>>(programs)!);
  }
  
  public async Task<string> Handle(UpdateProgramCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    var programId = await programRepository.UpdateProgram(mapper.Map<Resources.Documents.Programs.Program>(request.Program)!, cancellationToken);
    return programId;
  }

  public async Task<SubmitProgramResult> Handle(SubmitProgramCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    
    var programs = await programRepository.Query(new ProgramQuery
    {
      ById = request.ProgramId
    }, cancellationToken);

    if (programs.Count() > 1)
    {
      return new SubmitProgramResult { ProgramId = null, Error = ProgramSubmissionError.NonUniqueProgramProfiles, ValidationErrors = new List<string>() { "Multiple program profiles exist for the given ID" } };
    }
    
    var draftProgram = mapper.Map<Contract.Programs.Program>(programs.SingleOrDefault(dst => dst.Status == ProgramStatus.Draft));
    if (draftProgram == null)
    {
      return new SubmitProgramResult { ProgramId = null, Error = ProgramSubmissionError.DraftApplicationNotFound, ValidationErrors = new List<string>() { "Draft program profile does not exist" } };
    }
    
    var instructions = await metadataResourceRepository.QueryAreaOfInstructions(new AreaOfInstructionsQuery() { ById = null }, cancellationToken);
    
    var validationEngine = validationResolver?.resolve(draftProgram.ProgramProfileType);
    var validationErrors = await validationEngine?.Validate(draftProgram, instructions.ToList())!;
    if (validationErrors.ValidationErrors.Any())
    {
      return new SubmitProgramResult { ProgramId = null, Error = ProgramSubmissionError.DraftApplicationValidationFailed, ValidationErrors = validationErrors.ValidationErrors };
    }

    var programId = await programRepository.SubmitProgramProfile(draftProgram.Id!, cancellationToken);
    return new SubmitProgramResult { ProgramId = programId };
  }
}
