using AutoMapper;
using ECER.Engines.Transformation;
using ECER.Engines.Transformation.PortalInvitations;
using ECER.Engines.Validation.Applications;
using ECER.Infrastructure.Common;
using ECER.Managers.Registry.Contract.Applications;
using ECER.Resources.Documents.Applications;
using ECER.Resources.Documents.PortalInvitations;
using ECER.Utilities.DataverseSdk.Model;
using MediatR;

namespace ECER.Managers.Registry;

/// <summary>
/// Message handlers
/// </summary>
public class ApplicationHandlers(
    IPortalInvitationTransformationEngine transformationEngine,
    IPortalInvitationRepository portalInvitationRepository,
     IApplicationRepository applicationRepository,
     IMapper mapper,
     IApplicationValidationEngineResolver validationResolver,
     EcerContext ecerContext)
  : IRequestHandler<SaveDraftApplicationCommand, Contract.Applications.Application?>,
    IRequestHandler<CancelDraftApplicationCommand, string>,
    IRequestHandler<SubmitApplicationCommand, ApplicationSubmissionResult>,
    IRequestHandler<ApplicationsQuery, ApplicationsQueryResults>,
    IRequestHandler<SubmitReferenceCommand, ReferenceSubmissionResult>,
    IRequestHandler<Contract.Applications.OptOutReferenceRequest, ReferenceSubmissionResult>,
    IRequestHandler<Contract.Applications.UpdateWorkExperienceReferenceCommand, UpdateWorkExperienceReferenceResult>,
    IRequestHandler<UpdateCharacterReferenceCommand, UpdateCharacterReferenceResult>,
    IRequestHandler<ResendCharacterReferenceInviteRequest, string>,
    IRequestHandler<ResendWorkExperienceReferenceInviteRequest, string>,
    IRequestHandler<AddProfessionalDevelopmentCommand, AddProfessionalDevelopmentResult>,
    IRequestHandler<SaveApplicationTranscriptCommand, Contract.Applications.Application?>
{
  /// <summary>
  /// Handles submitting a new application use case
  /// </summary>
  /// <param name="request">The command</param>
  /// <param name="cancellationToken">cancellation token</param>
  /// <returns></returns>
  public async Task<Contract.Applications.Application?> Handle(SaveDraftApplicationCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    if (request.Application.Id == null)
    {
      // Check if a draft application already exists for the current user

      var applications = await applicationRepository.Query(new ApplicationQuery
      {
        ByApplicantId = request.Application.RegistrantId,
        ByStatus = [Resources.Documents.Applications.ApplicationStatus.Draft]
      }, cancellationToken);

      var draftApplicationResults = new ApplicationsQueryResults(mapper.Map<IEnumerable<Contract.Applications.Application>>(applications)!);
      var existingDraftApplication = draftApplicationResults.Items.FirstOrDefault();
      if (existingDraftApplication != null)
      {
        // user already has a draft application
        throw new InvalidOperationException($"User already has a draft application with id '{existingDraftApplication.Id}'");
      }
    }
    request.Application.Origin = Contract.Applications.ApplicationOrigin.Portal; // Set application origin to "Portal"
    var applicationId = await applicationRepository.SaveApplication(mapper.Map<Resources.Documents.Applications.Application>(request.Application)!, cancellationToken);

    var freshApplications = await applicationRepository.Query(new ApplicationQuery
    {
      ById = applicationId,
      ByStatus = [Resources.Documents.Applications.ApplicationStatus.Draft]
    }, cancellationToken);

    return mapper.Map<Contract.Applications.Application>(freshApplications.SingleOrDefault())!;
  }

  /// <summary>
  /// Handles submitting a new application use case
  /// </summary>
  /// <param name="request">The command</param>
  /// <param name="cancellationToken">cancellation token</param>
  /// <returns></returns>
  public async Task<Contract.Applications.Application?> Handle(SaveApplicationTranscriptCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var applications = await applicationRepository.Query(new ApplicationQuery
    {
      ById = request.TranscriptDocuments.ApplicationId,
      ByApplicantId = request.TranscriptDocuments.RegistrantId,
      ByStatus = [Resources.Documents.Applications.ApplicationStatus.Submitted]
    }, cancellationToken);

    var applicationResults = new ApplicationsQueryResults(mapper.Map<IEnumerable<Contract.Applications.Application>>(applications)!);
    if (!applicationResults.Items.Any())
    {
      // user does not have a submitted application
      throw new InvalidOperationException($"user does not have a submitted application");
    }

    var applicationId = await applicationRepository.SaveApplicationTranscript(mapper.Map<Resources.Documents.Applications.TranscriptDocuments>(request.TranscriptDocuments)!, cancellationToken);

    var freshApplications = await applicationRepository.Query(new ApplicationQuery
    {
      ById = applicationId,
    }, cancellationToken);

    return mapper.Map<Contract.Applications.Application>(freshApplications.SingleOrDefault())!;
  }

  public async Task<string> Handle(CancelDraftApplicationCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var applications = await applicationRepository.Query(new ApplicationQuery
    {
      ById = request.applicationId,
      ByApplicantId = request.userId,
      ByStatus = [Resources.Documents.Applications.ApplicationStatus.Draft]
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
    ArgumentNullException.ThrowIfNull(request);

    var applications = await applicationRepository.Query(new ApplicationQuery
    {
      ById = request.applicationId,
      ByApplicantId = request.userId,
      ByStatus = [Resources.Documents.Applications.ApplicationStatus.Draft]
    }, cancellationToken);

    var draftApplicationResults = new ApplicationsQueryResults(mapper.Map<IEnumerable<Contract.Applications.Application>>(applications)!);
    if (!draftApplicationResults.Items.Any())
    {
      return new ApplicationSubmissionResult() { ApplicationId = request.applicationId, Error = SubmissionError.DraftApplicationNotFound, ValidationErrors = new List<string>() { "draft application does not exist" } };
    }
    var draftApplication = draftApplicationResults.Items.First();

    var validationEngine = validationResolver?.Resolve(draftApplication.ApplicationType);
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
  /// Handles Reference Submission
  /// </summary>
  /// <param name="request"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  /// <exception cref="InvalidCastException"></exception>
  public async Task<ReferenceSubmissionResult> Handle(SubmitReferenceCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var transformationResponse = await transformationEngine.Transform(new DecryptInviteTokenRequest(request.Token))! as DecryptInviteTokenResponse ?? throw new InvalidCastException("Invalid response type");
    if (transformationResponse.PortalInvitation == Guid.Empty) return ReferenceSubmissionResult.Failure("Invalid Token");

    var portalInvitation = await portalInvitationRepository.Query(new PortalInvitationQuery(transformationResponse.PortalInvitation), cancellationToken);
    if (portalInvitation.StatusCode != PortalInvitationStatusCode.Sent) return ReferenceSubmissionResult.Failure("Portal Invitation is not valid or expired");

    ecerContext.BeginTransaction();

    SubmitReferenceRequest submitReferenceRequest = new SubmitReferenceRequest();
    switch (portalInvitation.InviteType)
    {
      case InviteType.CharacterReference:
        submitReferenceRequest = mapper.Map<Resources.Documents.Applications.CharacterReferenceSubmissionRequest>(request.CharacterReferenceSubmissionRequest);
        break;

      case InviteType.WorkExperienceReference:
        submitReferenceRequest = mapper.Map<Resources.Documents.Applications.WorkExperienceReferenceSubmissionRequest>(request.WorkExperienceReferenceSubmissionRequest);
        break;
    }
    submitReferenceRequest.PortalInvitation = portalInvitation;
    submitReferenceRequest.DateSigned = DateTime.Today;
    await applicationRepository.SubmitReference(submitReferenceRequest, cancellationToken);
    await portalInvitationRepository.Complete(new CompletePortalInvitationCommand(transformationResponse.PortalInvitation), cancellationToken);
    ecerContext.CommitTransaction();
    return ReferenceSubmissionResult.Success();
  }

  /// <summary>
  /// Handles Reference Opt out
  /// </summary>
  /// <param name="request"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  /// <exception cref="InvalidCastException"></exception>
  public async Task<ReferenceSubmissionResult> Handle(Contract.Applications.OptOutReferenceRequest request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var transformationResponse = await transformationEngine.Transform(new DecryptInviteTokenRequest(request.Token))! as DecryptInviteTokenResponse ?? throw new InvalidCastException("Invalid response type");
    if (transformationResponse.PortalInvitation == Guid.Empty) return ReferenceSubmissionResult.Failure("Invalid Token");

    var portalInvitation = await portalInvitationRepository.Query(new PortalInvitationQuery(transformationResponse.PortalInvitation), cancellationToken);

    if (portalInvitation.StatusCode != PortalInvitationStatusCode.Sent) return ReferenceSubmissionResult.Failure("Portal Invitation is not valid or expired");

    var referenceRequest = mapper.Map<Resources.Documents.Applications.OptOutReferenceRequest>(request);
    referenceRequest.PortalInvitation = portalInvitation;

    ecerContext.BeginTransaction();
    await applicationRepository.OptOutReference(referenceRequest, cancellationToken);
    await portalInvitationRepository.Complete(new CompletePortalInvitationCommand(transformationResponse.PortalInvitation), cancellationToken);
    ecerContext.CommitTransaction();
    return ReferenceSubmissionResult.Success();
  }

  /// <summary>
  /// Handles Work Experience Reference update
  /// </summary>
  /// <param name="request"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  public async Task<UpdateWorkExperienceReferenceResult> Handle(Contract.Applications.UpdateWorkExperienceReferenceCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    var WorkExpReference = mapper.Map<Resources.Documents.Applications.WorkExperienceReference>(request.workExperienceRef);
    var UpdatedWorkExReferenceId = await applicationRepository.UpdateWorkExReferenceForSubmittedApplication(WorkExpReference, request.applicationId, request.referenceId, request.userId, cancellationToken);
    return new UpdateWorkExperienceReferenceResult() { ReferenceId = UpdatedWorkExReferenceId, IsSuccess = true };
  }

  /// <summary>
  /// Handles Character Reference update
  /// </summary>
  /// <param name="request"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  public async Task<UpdateCharacterReferenceResult> Handle(Contract.Applications.UpdateCharacterReferenceCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    var CharacterReference = mapper.Map<Resources.Documents.Applications.CharacterReference>(request.characterRef);
    var UpdatedCharacterReferenceId = await applicationRepository.UpdateCharacterReferenceForSubmittedApplication(CharacterReference, request.applicationId, request.referenceId, request.userId, cancellationToken);
    return new UpdateCharacterReferenceResult() { ReferenceId = UpdatedCharacterReferenceId, IsSuccess = true };
  }

  /// <summary>
  /// Handles Character Reference resend invite
  /// </summary>
  /// <param name="request"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  public async Task<string> Handle(ResendCharacterReferenceInviteRequest request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var applications = await applicationRepository.Query(new ApplicationQuery
    {
      ById = request.ApplicationId,
      ByApplicantId = request.UserId,
      ByStatus = [Resources.Documents.Applications.ApplicationStatus.Submitted]
    }, cancellationToken);

    if (!applications.Any())
    {
      throw new InvalidOperationException($"Application not found id '{request.ApplicationId}' or application is past submitted stage");
    }
    ArgumentNullException.ThrowIfNull(request);
    var characterReferenceId = await applicationRepository.ResendCharacterReferenceInvite(new ResendReferenceInviteRequest(request.ReferenceId), cancellationToken);
    return characterReferenceId;
  }

  /// <summary>
  /// Handles Work Experience Reference resend invite
  /// </summary>
  /// <param name="request"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  public async Task<string> Handle(ResendWorkExperienceReferenceInviteRequest request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var applications = await applicationRepository.Query(new ApplicationQuery
    {
      ById = request.ApplicationId,
      ByApplicantId = request.UserId,
      ByStatus = new Resources.Documents.Applications.ApplicationStatus[] { Resources.Documents.Applications.ApplicationStatus.Submitted }
    }, cancellationToken);

    if (!applications.Any())
    {
      throw new InvalidOperationException($"Application not found id '{request.ApplicationId}' or application is past submitted stage");
    }
    ArgumentNullException.ThrowIfNull(request);

    var workExperienceReferenceId = await applicationRepository.ResendWorkExperienceReferenceInvite(new ResendReferenceInviteRequest(request.ReferenceId), cancellationToken);
    return workExperienceReferenceId;
  }

  public async Task<AddProfessionalDevelopmentResult> Handle(AddProfessionalDevelopmentCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var applications = await applicationRepository.Query(new ApplicationQuery
    {
      ById = request.applicationId,
      ByApplicantId = request.userId,
    }, cancellationToken);

    var application = applications.SingleOrDefault();

    if (application == null)
    {
      throw new InvalidOperationException($"Application not found id '{request.applicationId}'");
    }

    // Set the status for the new professional development request
    request.professionalDevelopment.Status = Managers.Registry.Contract.Applications.ProfessionalDevelopmentStatusCode.Submitted;

    // Map and add the professional development to the collection
    application.ProfessionalDevelopments = application.ProfessionalDevelopments.Append(mapper.Map<Resources.Documents.Applications.ProfessionalDevelopment>(request.professionalDevelopment));

    var applicationId = await applicationRepository.SaveApplication(mapper.Map<Resources.Documents.Applications.Application>(application)!, cancellationToken);

    return new AddProfessionalDevelopmentResult() { ApplicationId = applicationId, IsSuccess = true };
  }
}
