using AutoMapper;
using ECER.Managers.Registry.Contract.PortalInvitations;
using ECER.Resources.Accounts.Registrants;
using ECER.Resources.Documents.Applications;
using ECER.Resources.Documents.Certifications;

namespace ECER.Managers.Registry;

public class ReferencePortalInvitationVerificationHandler(
  IRegistrantRepository registrantRepository,
  ICertificationRepository certificationRepository,
  IApplicationRepository applicationRepository,
  IMapper mapper)
  : IPortalInvitationVerificationHandler
{
  public bool CanHandle(InviteType? inviteType)
  {
    return inviteType is InviteType.CharacterReference
      or InviteType.WorkExperienceReferenceforApplication;
  }

  public async Task<PortalInvitationVerificationQueryResult> Verify(PortalInvitation portalInvitation, CancellationToken cancellationToken)
  {
    var emptyGuidString = Guid.Empty.ToString();
    if (portalInvitation == null || (portalInvitation.WorkexperienceReferenceId == emptyGuidString && portalInvitation.CharacterReferenceId == emptyGuidString))
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

    var applications = await applicationRepository.Query(new ApplicationQuery() { ById = portalInvitation.ApplicationId }, cancellationToken);
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
}
