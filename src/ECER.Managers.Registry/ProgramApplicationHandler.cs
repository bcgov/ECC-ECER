using AutoMapper;
using ECER.Managers.Registry.Contract.ProgramApplications;
using ECER.Resources.Documents.ProgramApplications;
using MediatR;
using ApplicationStatus = ECER.Resources.Documents.ProgramApplications.ApplicationStatus;
using ComponentGroupMetadata = ECER.Managers.Registry.Contract.ProgramApplications.ComponentGroupMetadata;
using ComponentGroupQuery = ECER.Managers.Registry.Contract.ProgramApplications.ComponentGroupQuery;
using CreateProgramApplicationCommand = ECER.Managers.Registry.Contract.ProgramApplications.CreateProgramApplicationCommand;
using ProgramApplicationQuery = ECER.Managers.Registry.Contract.ProgramApplications.ProgramApplicationQuery;
using ProgramApplicationQueryResults = ECER.Managers.Registry.Contract.ProgramApplications.ProgramApplicationQueryResults;

namespace ECER.Managers.Registry;

public class ProgramApplicationHandler(
    IProgramApplicationRepository programApplicationRepository,
    IMapper mapper)
  : IRequestHandler<CreateProgramApplicationCommand, Contract.ProgramApplications.ProgramApplication?>,
    IRequestHandler<ProgramApplicationQuery, ProgramApplicationQueryResults>,
    IRequestHandler<UpdateProgramApplicationCommand, string>,
    IRequestHandler<ComponentGroupQuery, IEnumerable<ComponentGroupMetadata>>
{
  public async Task<Contract.ProgramApplications.ProgramApplication?> Handle(CreateProgramApplicationCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var resourcesApplication = mapper.Map<Resources.Documents.ProgramApplications.ProgramApplication>(request.ProgramApplication);
    var id = await programApplicationRepository.Create(resourcesApplication!, cancellationToken);

    var result = await programApplicationRepository.Query(new Resources.Documents.ProgramApplications.ProgramApplicationQuery
    {
      ById = id,
      ByPostSecondaryInstituteId = request.ProgramApplication.PostSecondaryInstituteId,
    }, cancellationToken);

    var created = result.Items.FirstOrDefault();
    return created != null ? mapper.Map<Contract.ProgramApplications.ProgramApplication>(created) : null;
  }


  public async Task<string> Handle(UpdateProgramApplicationCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    var programId = await programApplicationRepository.UpdateProgramApplication(mapper.Map<Resources.Documents.ProgramApplications.ProgramApplication>(request.ProgramApplication)!, cancellationToken);
    return programId;
  }

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
  
  public async Task<IEnumerable<ComponentGroupMetadata>> Handle(ComponentGroupQuery request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    var result = await programApplicationRepository.QueryComponentGroups(new Resources.Documents.ProgramApplications.ComponentGroupQuery()
    {
      ByProgramApplicationId = request.ByProgramApplicationId, 
    }, cancellationToken);
    return mapper.Map<IEnumerable<ComponentGroupMetadata>>(result);
  }
}
