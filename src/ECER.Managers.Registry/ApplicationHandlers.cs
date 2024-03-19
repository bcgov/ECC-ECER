using AutoMapper;
using ECER.Engines.Validation.Applications;
using ECER.Infrastructure.Common;
using ECER.Managers.Registry.Contract.Applications;
using ECER.Resources.Documents.Applications;

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
  /// <param name="cancellationToken">cancellation token</param>
  /// <returns></returns>
  public static async Task<string> Handle(SaveDraftApplicationCommand cmd, IApplicationRepository applicationRepository, IMapper mapper, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(applicationRepository);
    ArgumentNullException.ThrowIfNull(mapper);
    ArgumentNullException.ThrowIfNull(cmd);

    if (cmd.Application.Id == null)
    {
      // Check if a draft application already exists for the current user

      var applications = await applicationRepository.Query(new ApplicationQuery
      {
        ByApplicantId = cmd.Application.RegistrantId,
        ByStatus = new Resources.Documents.Applications.ApplicationStatus[] { Resources.Documents.Applications.ApplicationStatus.Draft }
      });

      var draftApplicationResults = new ApplicationsQueryResults(mapper.Map<IEnumerable<Contract.Applications.Application>>(applications)!);
      var existingDraftApplication = draftApplicationResults.Items.FirstOrDefault();
      if (existingDraftApplication != null)
      {
        // user already has a draft application
        throw new InvalidOperationException($"User already has a draft application with id '{existingDraftApplication.Id}'");
      }
    }
    cancellationToken.ThrowIfCancellationRequested();
    var applicationId = await applicationRepository.SaveDraft(mapper.Map<Resources.Documents.Applications.Application>(cmd.Application)!, cancellationToken);
    return applicationId;
  }

  /// <summary>
  /// Handles submitting a new application use case
  /// </summary>
  /// <param name="cmd">The command</param>
  /// <param name="applicationRepository">DI service</param>
  /// <param name="validationEngine">validationEngine</param>
  /// <param name="mapper">DI service</param>
  /// <param name="cancellationToken">cancellation token</param>
  /// <returns></returns>
  public static async Task<ApplicationSubmissionResult> Handle(SubmitApplicationCommand cmd, IApplicationSubmissionValidationEngine validationEngine, IApplicationRepository applicationRepository, IMapper mapper, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(applicationRepository);
    ArgumentNullException.ThrowIfNull(mapper);
    ArgumentNullException.ThrowIfNull(cmd);

    var applications = await applicationRepository.Query(new ApplicationQuery
    {
      ById = cmd.applicationId,
      ByStatus = new Resources.Documents.Applications.ApplicationStatus[] { Resources.Documents.Applications.ApplicationStatus.Draft }
    });

    var draftApplicationResults = new ApplicationsQueryResults(mapper.Map<IEnumerable<Contract.Applications.Application>>(applications)!);
    if (!draftApplicationResults.Items.Any())
    {
      throw new InvalidOperationException("draft application does not exist");
    }
    var draftApplication = draftApplicationResults.Items.First();

    var validationErrors = await validationEngine?.Validate(draftApplication)!;
    if (validationErrors.ValidationErrors.Any())
    {
      return new ApplicationSubmissionResult() { ValidationErrors = validationErrors.ValidationErrors };
    }
    cancellationToken.ThrowIfCancellationRequested();
    var applicationId = await applicationRepository.Submit(draftApplication.Id!, cancellationToken);
    return new ApplicationSubmissionResult() { ApplicationId = applicationId };
  }

  /// <summary>
  /// Handles applications query use case
  /// </summary>
  /// <param name="query">The query</param>
  /// <param name="applicationRepository">DI service</param>
  /// <param name="mapper">DI service</param>
  /// <returns></returns>
  public static async Task<ApplicationsQueryResults> Handle(ApplicationsQuery query, IApplicationRepository applicationRepository, IMapper mapper)
  {
    ArgumentNullException.ThrowIfNull(applicationRepository);
    ArgumentNullException.ThrowIfNull(mapper);
    ArgumentNullException.ThrowIfNull(query);

    var applications = await applicationRepository.Query(new ApplicationQuery
    {
      ById = query.ById,
      ByApplicantId = query.ByApplicantId,
      ByStatus = query.ByStatus?.Convert<Contract.Applications.ApplicationStatus, Resources.Documents.Applications.ApplicationStatus>(),
    });
    return new ApplicationsQueryResults(mapper.Map<IEnumerable<Contract.Applications.Application>>(applications)!);
  }
}
