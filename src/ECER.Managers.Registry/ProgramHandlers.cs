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
    }, cancellationToken));
    
    var program = result.Programs?.SingleOrDefault();

    return mapper.Map<Contract.Programs.Program>(program);
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

    var result = await programRepository.Query(new ProgramQuery
    {
      ById = request.ById,
      ByPostSecondaryInstituteId = request.ByPostSecondaryInstituteId,
      ByStatus = statusFilter,
      PageNumber = request.PageNumber,
      PageSize = request.PageSize,   
    }, cancellationToken);

    return new ProgramsQueryResults(mapper.Map<IEnumerable<Contract.Programs.Program>>(result.Programs), result.TotalProgramsCount);
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
    
    var programResult = await programRepository.Query(new ProgramQuery
    {
      ById = request.ProgramId,
      ByStatus = new[] { ProgramStatus.Draft }
    }, cancellationToken);
    
    if (!programResult.Programs!.Any())
    {
      return new SubmitProgramResult { ProgramId = null, Error = ProgramSubmissionError.DraftApplicationNotFound, ValidationErrors = new List<string>() { "Draft program profile does not exist" } };
    }
    
    var draftProgram = mapper.Map<Contract.Programs.Program>(programResult.Programs!.First());
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
