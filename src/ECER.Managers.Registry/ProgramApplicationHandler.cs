using AutoMapper;
using ECER.Resources.Documents.ProgramApplications;
using MediatR;
using ApplicationStatus = ECER.Resources.Documents.ProgramApplications.ApplicationStatus;
using ProgramApplicationQuery = ECER.Managers.Registry.Contract.ProgramApplications.ProgramApplicationQuery;
using ProgramApplicationQueryResults = ECER.Managers.Registry.Contract.ProgramApplications.ProgramApplicationQueryResults;

namespace ECER.Managers.Registry;

public class ProgramApplicationHandler(
    IProgramApplicationRepository programApplicationRepository,
    IMapper mapper)
  : IRequestHandler<ProgramApplicationQuery, ProgramApplicationQueryResults>
{
  public async Task<ProgramApplicationQueryResults> Handle(ProgramApplicationQuery request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var statusFilter = request.ByStatus != null
      ? mapper.Map<IEnumerable<ApplicationStatus>>(request.ByStatus)
      : null;

    var result = await programApplicationRepository.Query(new Resources.Documents.ProgramApplications.ProgramApplicationQuery()
    {
      ById = request.ById,
      ByPostSecondaryInstituteId = request.ByPostSecondaryInstituteId,
      ByStatus = statusFilter,
      PageNumber = request.PageNumber,
      PageSize = request.PageSize,   
    }, cancellationToken);

    return new ProgramApplicationQueryResults(mapper.Map<IEnumerable<Contract.ProgramApplications.ProgramApplication>>(result.Items), result.Count);
  }
}
