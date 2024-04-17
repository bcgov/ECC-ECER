using AutoMapper;
using ECER.Engines.Transformation;
using ECER.Engines.Transformation.PortalInvitations;
using ECER.Engines.Validation.Applications;
using ECER.Infrastructure.Common;
using ECER.Managers.Registry.Contract.Applications;
using ECER.Resources.Accounts.Registrants;
using ECER.Resources.Documents.Applications;
using ECER.Resources.Documents.PortalInvitations;
using MediatR;
using ReferenceSubmissionRequest = ECER.Managers.Registry.Contract.Applications.ReferenceSubmissionRequest;

namespace ECER.Managers.Registry;

/// <summary>
/// Message handlers
/// </summary>
public class ApplicationHandlers(IPortalInvitationTransformationEngine transformationEngine, IPortalInvitationRepository portalInvitationRepository, IRegistrantRepository registrantRepository, IApplicationRepository applicationRepository, IMapper mapper, IApplicationSubmissionValidationEngine validationEngine)
  : IRequestHandler<SaveDraftApplicationCommand, string>,
    IRequestHandler<CancelDraftApplicationCommand, string>,
    IRequestHandler<SubmitApplicationCommand, ApplicationSubmissionResult>,
    IRequestHandler<ApplicationsQuery, ApplicationsQueryResults>,
    IRequestHandler<ReferenceSubmissionRequest, ReferenceSubmissionResult>,
    IRequestHandler<ProvincesQuery, ProvincesQueryResults>
{
  /// <summary>
  /// Handles submitting a new application use case
  /// </summary>
  /// <param name="request">The command</param>
  /// <param name="cancellationToken">cancellation token</param>
  /// <returns></returns>
  public async Task<string> Handle(SaveDraftApplicationCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(applicationRepository);
    ArgumentNullException.ThrowIfNull(mapper);
    ArgumentNullException.ThrowIfNull(request);

    if (request.Application.Id == null)
    {
      // Check if a draft application already exists for the current user

      var applications = await applicationRepository.Query(new ApplicationQuery
      {
        ByApplicantId = request.Application.RegistrantId,
        ByStatus = new Resources.Documents.Applications.ApplicationStatus[] { Resources.Documents.Applications.ApplicationStatus.Draft }
      }, cancellationToken);

      var draftApplicationResults = new ApplicationsQueryResults(mapper.Map<IEnumerable<Contract.Applications.Application>>(applications)!);
      var existingDraftApplication = draftApplicationResults.Items.FirstOrDefault();
      if (existingDraftApplication != null)
      {
        // user already has a draft application
        throw new InvalidOperationException($"User already has a draft application with id '{existingDraftApplication.Id}'");
      }
    }
    var applicationId = await applicationRepository.SaveDraft(mapper.Map<Resources.Documents.Applications.Application>(request.Application)!, cancellationToken);
    return applicationId;
  }

  public async Task<string> Handle(CancelDraftApplicationCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(applicationRepository);
    ArgumentNullException.ThrowIfNull(mapper);
    ArgumentNullException.ThrowIfNull(request);

    var applications = await applicationRepository.Query(new ApplicationQuery
    {
      ById = request.applicationId,
      ByApplicantId = request.userId,
      ByStatus = new Resources.Documents.Applications.ApplicationStatus[] { Resources.Documents.Applications.ApplicationStatus.Draft }
    }, cancellationToken);

    if (!applications.Any())
    {
      throw new InvalidOperationException($"Application not found id '{request.applicationId}'");
    }

    var cancelledApplicationId = await applicationRepository.Cancel(request.applicationId, cancellationToken);

    return cancelledApplicationId;
  }

  /// <summary>
  /// Handles submitting a new application use case
  /// </summary>
  /// <param name="request">The command</param>
  /// <param name="cancellationToken">cancellation token</param>
  /// <returns></returns>
  public async Task<ApplicationSubmissionResult> Handle(SubmitApplicationCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(applicationRepository);
    ArgumentNullException.ThrowIfNull(mapper);
    ArgumentNullException.ThrowIfNull(request);

    var applications = await applicationRepository.Query(new ApplicationQuery
    {
      ById = request.applicationId,
      ByApplicantId = request.userId,
      ByStatus = new Resources.Documents.Applications.ApplicationStatus[] { Resources.Documents.Applications.ApplicationStatus.Draft }
    }, cancellationToken);

    var draftApplicationResults = new ApplicationsQueryResults(mapper.Map<IEnumerable<Contract.Applications.Application>>(applications)!);
    if (!draftApplicationResults.Items.Any())
    {
      return new ApplicationSubmissionResult() { ApplicationId = request.applicationId, Error = SubmissionError.DraftApplicationNotFound, ValidationErrors = new List<string>() { "draft application does not exist" } };
    }
    var draftApplication = draftApplicationResults.Items.First();

    var validationErrors = await validationEngine?.Validate(draftApplication)!;
    if (validationErrors.ValidationErrors.Any())
    {
      return new ApplicationSubmissionResult() { ApplicationId = request.applicationId, Error = SubmissionError.DraftApplicationValidationFailed, ValidationErrors = validationErrors.ValidationErrors };
    }
    var applicationId = await applicationRepository.Submit(draftApplication.Id!, cancellationToken);
    return new ApplicationSubmissionResult() { ApplicationId = applicationId };
  }

  /// <summary>
  /// Handles applications query use case
  /// </summary>
  /// <param name="request">The query</param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  public async Task<ApplicationsQueryResults> Handle(ApplicationsQuery request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(applicationRepository);
    ArgumentNullException.ThrowIfNull(mapper);
    ArgumentNullException.ThrowIfNull(request);

    var applications = await applicationRepository.Query(new ApplicationQuery
    {
      ById = request.ById,
      ByApplicantId = request.ByApplicantId,
      ByStatus = request.ByStatus?.Convert<Contract.Applications.ApplicationStatus, Resources.Documents.Applications.ApplicationStatus>(),
    }, cancellationToken);
    return new ApplicationsQueryResults(mapper.Map<IEnumerable<Contract.Applications.Application>>(applications)!);
  }

  /// <summary>
  ///
  /// </summary>
  /// <param name="request"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  public async Task<ProvincesQueryResults> Handle(ProvincesQuery request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(applicationRepository);
    ArgumentNullException.ThrowIfNull(mapper);
    ArgumentNullException.ThrowIfNull(request);

    var provinces = await applicationRepository.QueryProvinces(cancellationToken);
    return new ProvincesQueryResults(mapper.Map<IEnumerable<Contract.Applications.Province>>(provinces)!);
  }

  /// <summary>
  /// Handles Reference Submission
  /// </summary>
  /// <param name="request"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  /// <exception cref="InvalidCastException"></exception>
  public async Task<ReferenceSubmissionResult> Handle(ReferenceSubmissionRequest request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(portalInvitationRepository);
    ArgumentNullException.ThrowIfNull(registrantRepository);
    ArgumentNullException.ThrowIfNull(applicationRepository);
    ArgumentNullException.ThrowIfNull(transformationEngine);
    ArgumentNullException.ThrowIfNull(mapper);
    ArgumentNullException.ThrowIfNull(request);

    var transformationResponse = await transformationEngine.Transform(new DecryptInviteTokenRequest(request.Token))! as DecryptInviteTokenResponse ?? throw new InvalidCastException("Invalid response type");
    if (transformationResponse.PortalInvitation == Guid.Empty) return ReferenceSubmissionResult.Failure("Invalid Token");

    var portalInvitation = await portalInvitationRepository.Query(new PortalInvitationQuery(transformationResponse.PortalInvitation), cancellationToken);
    var registrantResult = await registrantRepository.Query(new RegistrantQuery() { ByUserId = portalInvitation.ApplicantId }, cancellationToken);

    var applicant = registrantResult.SingleOrDefault();
    if (applicant == null) return ReferenceSubmissionResult.Failure("Applicant not found");

    var referenceRequest = mapper.Map<Resources.Documents.Applications.ReferenceSubmissionRequest>(request);
    referenceRequest.PortalInvitation = portalInvitation;
    if (transformationResponse.InviteType == Admin.Contract.PortalInvitations.InviteType.WorkExperienceReference && !string.IsNullOrEmpty(portalInvitation.WorkexperienceReferenceId))
    {
      var result = await applicationRepository.SubmitWorkexperienceReference(referenceRequest, cancellationToken);
      if (!result) return ReferenceSubmissionResult.Failure("Failed to submit work experience reference");
    }
    else if (transformationResponse.InviteType == Admin.Contract.PortalInvitations.InviteType.CharacterReference && !string.IsNullOrEmpty(portalInvitation.CharacterReferenceId))
    {
      var result = await applicationRepository.SubmitCharacterReference(referenceRequest, cancellationToken);
      if (!result) return ReferenceSubmissionResult.Failure("Failed to submit character reference");
    }
    else
    {
      return ReferenceSubmissionResult.Failure("Portal Invitation does not have a character reference or work experience references");
    }
    return ReferenceSubmissionResult.Success();
  }
}
