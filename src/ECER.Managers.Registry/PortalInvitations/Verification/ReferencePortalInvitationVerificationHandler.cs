using AutoMapper;
using ECER.Managers.Registry.Contract.PortalInvitations;
using ECER.Resources.Accounts.Registrants;
using ECER.Resources.Documents.Applications;
using ECER.Resources.Documents.Certifications;
using ECER.Resources.Documents.PortalInvitations;
using MediatR;
using InviteType = ECER.Resources.Documents.PortalInvitations.InviteType;
using PortalInvitation = ECER.Resources.Documents.PortalInvitations.PortalInvitation;
using PortalInvitationStatusCode = ECER.Managers.Registry.Contract.PortalInvitations.PortalInvitationStatusCode;

namespace ECER.Managers.Registry;

public class PortalInvitationHandlers(IPortalInvitationTransformationEngine transformationEngine,
    IPortalInvitationRepository portalInvitationRepository, IApplicationRepository applicationRepository,
    IRegistrantRepository registrantRepository, ICertificationRepository certificationRepository, IMapper mapper)
  : IRequestHandler<PortalInvitationVerificationQuery, PortalInvitationVerificationQueryResult>
{
  public async Task<PortalInvitationVerificationQueryResult> Handle(PortalInvitationVerificationQuery request,
    CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    var response =
      await transformationEngine.Transform(new DecryptInviteTokenRequest(request.VerificationToken))! as
        DecryptInviteTokenResponse ?? throw new InvalidCastException("Invalid response type");
    if (response.PortalInvitation == Guid.Empty)
    {
      return PortalInvitationVerificationQueryResult.Failure("Invalid Token");
    }

    var portalInvitation =
      await portalInvitationRepository.Query(new PortalInvitationQuery(response.PortalInvitation), cancellationToken);
    if (portalInvitation == null)
    {
      return PortalInvitationVerificationQueryResult.Failure("Portal Invitation not found");
    }

    switch (portalInvitation.InviteType)
    {
      case InviteType.PSIProgramRepresentative:
        return await HandlePspInvitation(portalInvitation, cancellationToken);
      case InviteType.CharacterReference:
      case InviteType.WorkExperienceReferenceforApplication:
      case InviteType.WorkExperienceReferenceforICRA:
        return await HandleReferenceInvitation(portalInvitation, cancellationToken);
      default:
        return PortalInvitationVerificationQueryResult.Failure("Unsupported invite type.");
    }
  }

  private async Task<PortalInvitationVerificationQueryResult> HandleReferenceInvitation(
    PortalInvitation portalInvitation, CancellationToken cancellationToken)
  {
    var emptyGuidString = Guid.Empty.ToString();
    if (portalInvitation.WorkexperienceReferenceId == emptyGuidString &&
        portalInvitation.CharacterReferenceId == emptyGuidString)
    {
      return PortalInvitationVerificationQueryResult.Failure("Reference not found");
    }

    var registrantResult = await registrantRepository.Query(new RegistrantQuery() { ByUserId = portalInvitation.ApplicantId }, cancellationToken);
    var applicant = registrantResult.SingleOrDefault();
    if (applicant == null)
    {
      return PortalInvitationVerificationQueryResult.Failure("Applicant not found");
    }

    var certifications = await certificationRepository.Query(new UserCertificationQuery() { ByApplicantId = applicant.Id });
    var latestCertification = certifications.FirstOrDefault();

    var applications =
      await applicationRepository.Query(new ApplicationQuery() { ById = portalInvitation.ApplicationId },
        cancellationToken);
    var application = applications.SingleOrDefault();
    if (application == null)
    {
      return PortalInvitationVerificationQueryResult.Failure("Application not found");
    }

    var result = mapper.Map<Contract.PortalInvitations.PortalInvitation>(portalInvitation);

    switch (result.StatusCode)
    {
      case Contract.PortalInvitations.PortalInvitationStatusCode.Completed:
        return PortalInvitationVerificationQueryResult.Failure("Reference has already been submitted.");
      case Contract.PortalInvitations.PortalInvitationStatusCode.Expired:
        return PortalInvitationVerificationQueryResult.Failure("Reference has expired.");
      case Contract.PortalInvitations.PortalInvitationStatusCode.Cancelled:
        return PortalInvitationVerificationQueryResult.Failure("Reference has been cancelled.");
      case Contract.PortalInvitations.PortalInvitationStatusCode.Failed:
        return PortalInvitationVerificationQueryResult.Failure("Reference has failed.");
    }

    if (result.InviteType == Contract.PortalInvitations.InviteType.WorkExperienceReferenceforApplication)
    {
      var workExRef = application.WorkExperienceReferences.SingleOrDefault(work => work.Id == portalInvitation.WorkexperienceReferenceId);
      if (workExRef != null)
      {
        result.WorkExperienceReferenceHours = workExRef.Hours;
        if (workExRef.Type != null)
        {
          result.WorkExperienceType = mapper.Map<Contract.Applications.WorkExperienceTypes>(workExRef.Type);
        }
      }
    }

    if (latestCertification != null)
    {
      result.LatestCertification = mapper.Map<Contract.Certifications.Certification>(latestCertification);
    }

    result.CertificationTypes = mapper.Map<Contract.Applications.Application>(application)!.CertificationTypes!;
    result.ApplicantFirstName = applicant.Profile.FirstName;
    result.ApplicantLastName = applicant.Profile.LastName;

    return PortalInvitationVerificationQueryResult.Success(result);
  }

  private Task<PortalInvitationVerificationQueryResult> HandlePspInvitation(PortalInvitation portalInvitation,
    CancellationToken cancellationToken)
  {
    var emptyGuidString = Guid.Empty.ToString();
    if (portalInvitation.PspProgramRepresentativeId == emptyGuidString)
    {
      return Task.FromResult(PortalInvitationVerificationQueryResult.Failure("Psp Program Representative not found"));
    }

    var result = mapper.Map<Contract.PortalInvitations.PortalInvitation>(portalInvitation);

    return Task.FromResult(PortalInvitationVerificationQueryResult.Success(result));
  }
}


