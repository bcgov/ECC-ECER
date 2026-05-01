using ECER.Engines.Transformation;
using ECER.Engines.Transformation.PortalInvitations;
using ECER.Engines.Validation.Applications;
using ECER.Infrastructure.Common;
using ECER.Managers.Registry.Contract.Applications;
using ECER.Resources.Documents.Applications;
using ECER.Resources.Documents.ICRA;
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
    IApplicationMapper applicationMapper,
    IICRAEligibilityMapper icraEligibilityMapper,
    IApplicationValidationEngineResolver validationResolver,
    EcerContext ecerContext,
    IICRARepository iCRARepository)
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
      var applications = await applicationRepository.Query(new ApplicationQuery
      {
        ByApplicantId = request.Application.RegistrantId,
        ByStatus =
        [
          Resources.Documents.Applications.ApplicationStatus.Draft,
          Resources.Documents.Applications.ApplicationStatus.Submitted,
          Resources.Documents.Applications.ApplicationStatus.Ready,
          Resources.Documents.Applications.ApplicationStatus.Escalated,
          Resources.Documents.Applications.ApplicationStatus.Pending,
          Resources.Documents.Applications.ApplicationStatus.InProgress,
          Resources.Documents.Applications.ApplicationStatus.PendingPSPConsultationNeeded,
          Resources.Documents.Applications.ApplicationStatus.PendingQueue,
        ]
      }, cancellationToken);

      if (applications.Any())
      {
        throw new InvalidOperationException($"User already has an application in progress with id '{applications.SingleOrDefault()!.Id}'");
      }
    }

    request.Application.Origin = Contract.Applications.ApplicationOrigin.Portal;
    var applicationId = await applicationRepository.SaveApplication(applicationMapper.MapApplication(request.Application), cancellationToken);

    var freshApplications = await applicationRepository.Query(new ApplicationQuery
    {
      ById = applicationId,
      ByStatus = [Resources.Documents.Applications.ApplicationStatus.Draft]
    }, cancellationToken);

    return applicationMapper.MapApplication(freshApplications.SingleOrDefault());
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

    var applicationResults = new ApplicationsQueryResults(applicationMapper.MapApplications(applications));
    if (!applicationResults.Items.Any())
    {
      throw new InvalidOperationException($"Registrant {request.TranscriptDocuments.RegistrantId} does not have a submitted application");
    }

    var applicationId = await applicationRepository.SaveApplicationTranscript(applicationMapper.MapTranscriptDocuments(request.TranscriptDocuments), cancellationToken);

    var freshApplications = await applicationRepository.Query(new ApplicationQuery
    {
      ById = applicationId,
    }, cancellationToken);

    return applicationMapper.MapApplication(freshApplications.SingleOrDefault());
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

    return await applicationRepository.Cancel(request.applicationId, cancellationToken);
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
      ByApplicantId = request.userId,
      ByStatus =
      [
        Resources.Documents.Applications.ApplicationStatus.Draft,
        Resources.Documents.Applications.ApplicationStatus.Submitted,
        Resources.Documents.Applications.ApplicationStatus.Ready,
        Resources.Documents.Applications.ApplicationStatus.Escalated,
        Resources.Documents.Applications.ApplicationStatus.Pending,
        Resources.Documents.Applications.ApplicationStatus.InProgress,
        Resources.Documents.Applications.ApplicationStatus.PendingPSPConsultationNeeded,
        Resources.Documents.Applications.ApplicationStatus.PendingQueue,
      ]
    }, cancellationToken);

    var draftApplication = applicationMapper.MapApplication(applications.SingleOrDefault(dst =>
      dst.Id == request.applicationId && dst.Status == Resources.Documents.Applications.ApplicationStatus.Draft));
    var submittedApplications = applicationMapper.MapApplications(applications.Where(dst => dst.Status != Resources.Documents.Applications.ApplicationStatus.Draft));

    if (draftApplication == null)
    {
      return new ApplicationSubmissionResult { Application = null, Error = SubmissionError.DraftApplicationNotFound, ValidationErrors = ["draft application does not exist"] };
    }

    ecerContext.BeginTransaction();
    if (draftApplication.ApplicationType == Contract.Applications.ApplicationTypes.ICRA)
    {
      var result = await LinkIcraEligibilityToIcraApplication(draftApplication, request.userId, cancellationToken);
      if (!result)
      {
        return new ApplicationSubmissionResult { Application = null, Error = SubmissionError.MissingApprovedIcraEligibility, ValidationErrors = ["unable to find approved icra eligibility"] };
      }
    }

    var validationEngine = validationResolver.Resolve(draftApplication.ApplicationType);
    var validationErrors = await validationEngine.Validate(draftApplication);
    if (validationErrors.ValidationErrors.Any())
    {
      return new ApplicationSubmissionResult { Application = null, Error = SubmissionError.DraftApplicationValidationFailed, ValidationErrors = validationErrors.ValidationErrors };
    }

    var applicationId = await applicationRepository.Submit(draftApplication.Id!, cancellationToken);
    ecerContext.CommitTransaction();

    var freshApplications = await applicationRepository.Query(new ApplicationQuery
    {
      ById = applicationId,
      ByStatus = [Resources.Documents.Applications.ApplicationStatus.Submitted]
    }, cancellationToken);
    if (!freshApplications.Any())
    {
      return new ApplicationSubmissionResult { Application = null, Error = SubmissionError.DraftApplicationNotFound, ValidationErrors = ["draft application does not exist"] };
    }

    if (submittedApplications.Any())
    {
      return new ApplicationSubmissionResult { Application = null, Error = SubmissionError.SubmittedApplicationAlreadyExists, ValidationErrors = ["submitted application already exists"] };
    }

    return new ApplicationSubmissionResult { Application = applicationMapper.MapApplications(freshApplications).FirstOrDefault() };
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
    return new ApplicationsQueryResults(applicationMapper.MapApplications(applications));
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

    var transformationResponse = await transformationEngine.Transform(new DecryptInviteTokenRequest(request.Token)) as DecryptInviteTokenResponse
      ?? throw new InvalidCastException("Invalid response type");
    if (transformationResponse.PortalInvitation == Guid.Empty) return ReferenceSubmissionResult.Failure("Invalid Token");

    var portalInvitation = await portalInvitationRepository.Query(new PortalInvitationQuery(transformationResponse.PortalInvitation), cancellationToken);
    if (portalInvitation.StatusCode != PortalInvitationStatusCode.Sent) return ReferenceSubmissionResult.Failure("Portal Invitation is not valid or expired");

    ecerContext.BeginTransaction();

    SubmitReferenceRequest submitReferenceRequest = new SubmitReferenceRequest();
    switch (portalInvitation.InviteType)
    {
      case InviteType.CharacterReference:
        submitReferenceRequest = applicationMapper.MapCharacterReferenceSubmissionRequest(request.CharacterReferenceSubmissionRequest!);
        break;

      case InviteType.WorkExperienceReferenceforApplication:
        if (portalInvitation.ApplicantId is null)
        {
          throw new InvalidOperationException("portal invite applicant id is null");
        }
        if (portalInvitation.WorkexperienceReferenceId is null)
        {
          throw new InvalidOperationException("portal invite work experience reference id is null");
        }

        var workExperience = await applicationRepository.GetWorkExperienceReferenceById(portalInvitation.WorkexperienceReferenceId, portalInvitation.ApplicantId, cancellationToken);
        if (workExperience is null)
        {
          throw new InvalidOperationException($"work experience reference not found for reference id: {portalInvitation.WorkexperienceReferenceId} and applicant id: {portalInvitation.ApplicantId}");
        }

        submitReferenceRequest = workExperience.Type switch
        {
          Resources.Documents.Applications.WorkExperienceTypes.ICRA => applicationMapper.MapIcraWorkExperienceReferenceSubmissionRequest(request.WorkExperienceReferenceSubmissionRequest!),
          Resources.Documents.Applications.WorkExperienceTypes.Is400Hours or Resources.Documents.Applications.WorkExperienceTypes.Is500Hours => applicationMapper.MapWorkExperienceReferenceSubmissionRequest(request.WorkExperienceReferenceSubmissionRequest!),
          _ => throw new InvalidOperationException($"unknown work experience reference type '{workExperience.Type}'")
        };
        break;

      case InviteType.WorkExperienceReferenceforICRA:
        var icraReferenceId = portalInvitation.WorkexperienceReferenceId!;
        var icraPayload = icraEligibilityMapper.MapIcraWorkExperienceReferenceSubmissionRequest(request.ICRAWorkExperienceReferenceSubmissionRequest!);
        icraPayload.DateSigned = DateTime.Today;
        await iCRARepository.SubmitEmploymentReference(icraReferenceId, icraPayload, cancellationToken);
        break;
    }

    submitReferenceRequest.PortalInvitation = portalInvitation;
    submitReferenceRequest.DateSigned = DateTime.Today;
    if (portalInvitation.InviteType == InviteType.CharacterReference || portalInvitation.InviteType == InviteType.WorkExperienceReferenceforApplication)
    {
      await applicationRepository.SubmitReference(submitReferenceRequest, cancellationToken);
    }

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

    var transformationResponse = await transformationEngine.Transform(new DecryptInviteTokenRequest(request.Token)) as DecryptInviteTokenResponse
      ?? throw new InvalidCastException("Invalid response type");
    if (transformationResponse.PortalInvitation == Guid.Empty) return ReferenceSubmissionResult.Failure("Invalid Token");

    var portalInvitation = await portalInvitationRepository.Query(new PortalInvitationQuery(transformationResponse.PortalInvitation), cancellationToken);
    if (portalInvitation.StatusCode != PortalInvitationStatusCode.Sent) return ReferenceSubmissionResult.Failure("Portal Invitation is not valid or expired");

    var referenceRequest = applicationMapper.MapOptOutReferenceRequest(request);
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

    var workExperienceReference = applicationMapper.MapWorkExperienceReference(request.workExperienceRef);
    var updatedWorkExperienceReferenceId = await applicationRepository.UpdateWorkExReferenceForSubmittedApplication(workExperienceReference, request.applicationId, request.referenceId, request.userId, cancellationToken);
    return new UpdateWorkExperienceReferenceResult { ReferenceId = updatedWorkExperienceReferenceId, IsSuccess = true };
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

    var characterReference = applicationMapper.MapCharacterReference(request.characterRef);
    var updatedCharacterReferenceId = await applicationRepository.UpdateCharacterReferenceForSubmittedApplication(characterReference, request.applicationId, request.referenceId, request.userId, cancellationToken);
    return new UpdateCharacterReferenceResult { ReferenceId = updatedCharacterReferenceId, IsSuccess = true };
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

    return await applicationRepository.ResendCharacterReferenceInvite(new ResendReferenceInviteRequest(request.ReferenceId), cancellationToken);
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
      ByStatus = [Resources.Documents.Applications.ApplicationStatus.Submitted]
    }, cancellationToken);

    if (!applications.Any())
    {
      throw new InvalidOperationException($"Application not found id '{request.ApplicationId}' or application is past submitted stage");
    }

    return await applicationRepository.ResendWorkExperienceReferenceInvite(new ResendReferenceInviteRequest(request.ReferenceId), cancellationToken);
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

    request.professionalDevelopment.Status = Contract.Applications.ProfessionalDevelopmentStatusCode.Submitted;
    application.ProfessionalDevelopments = application.ProfessionalDevelopments.Append(applicationMapper.MapProfessionalDevelopment(request.professionalDevelopment));

    var applicationId = await applicationRepository.SaveApplication(application, cancellationToken);
    return new AddProfessionalDevelopmentResult { ApplicationId = applicationId, IsSuccess = true };
  }

  /*This method is specific for ICRA applications where we need to attach the application to icra eligility which will trigger a dynamics power automate flow to link up
   * international certificates and work references */

  private async Task<bool> LinkIcraEligibilityToIcraApplication(Contract.Applications.Application application, string userId, CancellationToken cancellationToken)
  {
    var icraEligibilities = await iCRARepository.Query(new ICRAQuery { ByApplicantId = userId }, cancellationToken);
    var approvedIcraEligibility = icraEligibilities.FirstOrDefault(eligibility => eligibility.Status == ICRAStatus.Eligible);

    if (approvedIcraEligibility == null)
    {
      return false;
    }

    if (application.Id == null || approvedIcraEligibility.Id == null)
    {
      throw new InvalidOperationException($"application id is null {application.Id} or approvedIcraEligibility id is null {approvedIcraEligibility.Id}");
    }

    await iCRARepository.LinkIcraEligibilityToIcraApplication(application.Id, approvedIcraEligibility.Id, cancellationToken);
    return true;
  }
}
