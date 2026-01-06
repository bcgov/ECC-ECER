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
    IRequestHandler<ProgramDetailCommand, Contract.Programs.ProgramDetail>
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

  public async Task<Contract.Programs.ProgramDetail> Handle(ProgramDetailCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var program = await programRepository.GetProgramById(new ProgramDetailQuery(request.ProgramId, request.PostSecondaryInstituteId), cancellationToken);

    return mapper.Map<Contract.Programs.ProgramDetail>(program);
  }
}
