using AutoMapper;
using ECER.Managers.Registry.Contract.Programs;
using ECER.Resources.Documents.Programs;
using MediatR;

namespace ECER.Managers.Registry;

public class ProgramHandlers(
    IProgramRepository programRepository,
    IMapper mapper)
  : IRequestHandler<SaveDraftProgramCommand, Contract.Programs.Program?>,
    IRequestHandler<ProgramsQuery, ProgramsQueryResults>,
    IRequestHandler<UpdateCourseCommand, string>,
    IRequestHandler<UpdateProgramCommand, string>
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
}
