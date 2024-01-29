using AutoMapper;
using ECER.Managers.Registry.Contract.Applications;
using ECER.Resources.Applications;

namespace ECER.Managers.Registry;

/// <summary>
/// Message handlers
/// </summary>
public static class ApplicationHandlers
{
  /// <summary>
  /// Handles submitting a new application use case
  /// </summary>
  /// <param name="cmd">The command</param>
  /// <param name="applicationRepository">DI service</param>
  /// <param name="mapper">DI service</param>
  /// <returns></returns>
  public static async Task<string> Handle(SaveDraftCertificationApplicationCommand cmd, IApplicationRepository applicationRepository, IMapper mapper)
  {
    ArgumentNullException.ThrowIfNull(applicationRepository);
    ArgumentNullException.ThrowIfNull(mapper);
    ArgumentNullException.ThrowIfNull(cmd);

    var applicationId = await applicationRepository.SaveDraft(mapper.Map<Resources.Applications.CertificationApplication>(cmd.Application));
    return applicationId;
  }

  /// <summary>
  /// Handles applications query use case
  /// </summary>
  /// <param name="query">The query</param>
  /// <param name="applicationRepository">DI service</param>
  /// <param name="mapper">DI service</param>
  /// <returns></returns>
  public static async Task<ApplicationsQueryResults> Handle(CertificationApplicationsQuery query, IApplicationRepository applicationRepository, IMapper mapper)
  {
    ArgumentNullException.ThrowIfNull(applicationRepository);
    ArgumentNullException.ThrowIfNull(mapper);
    ArgumentNullException.ThrowIfNull(query);

    var applications = await applicationRepository.Query(new CertificationApplicationQuery
    {
      ById = query.ById
    });
    return new ApplicationsQueryResults(mapper.Map<IEnumerable<Contract.Applications.CertificationApplication>>(applications));
  }
}
