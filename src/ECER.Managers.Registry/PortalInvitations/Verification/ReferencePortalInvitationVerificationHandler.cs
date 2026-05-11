using ECER.Managers.Registry.Contract.PortalInvitations;
using ECER.Managers.Registry.PortalInvitations;
using ECER.Resources.Accounts.Registrants;
using ECER.Resources.Documents.Applications;
using ECER.Resources.Documents.Certifications;

namespace ECER.Managers.Registry;

public class ReferencePortalInvitationVerificationHandler(
  IRegistrantRepository registrantRepository,
  ICertificationRepository certificationRepository,
  IApplicationRepository applicationRepository,
  ICertificationMapper certificationMapper,
  IPortalInvitationMapper portalInvitationMapper)
  : IPortalInvitationVerificationHandler
{
  public bool CanHandle(InviteType? inviteType)
  {
    return inviteType is InviteType.CharacterReference
      or InviteType.WorkExperienceReferenceforApplication;
  }

  public async Task<PortalInvitationVerificationQueryResult> Verify(PortalInvitation portalInvitation, CancellationToken cancellationToken)
  {
    if (HasMissingReference(portalInvitation))
    {
      return PortalInvitationVerificationQueryResult.Failure("Reference not found");
    }

    var registrantResult = await registrantRepository.Query(new RegistrantQuery { ByUserId = portalInvitation.ApplicantId }, cancellationToken);
    var applicant = registrantResult.SingleOrDefault();
    if (applicant == null)
    {
      return PortalInvitationVerificationQueryResult.Failure("Applicant not found");
    }

    var applications = await applicationRepository.Query(new ApplicationQuery { ById = portalInvitation.ApplicationId }, cancellationToken);
    var application = applications.SingleOrDefault();
    if (application == null)
    {
      return PortalInvitationVerificationQueryResult.Failure("Application not found");
    }

    var failureMessage = GetFailureMessage(portalInvitation.StatusCode);
    if (failureMessage != null)
    {
      return PortalInvitationVerificationQueryResult.Failure(failureMessage);
    }

    var certifications = await certificationRepository.Query(new UserCertificationQuery
    {
      ByApplicantId = applicant.Id,
      ById = !string.IsNullOrEmpty(application.FromCertificate?.ToString()) ? application.FromCertificate.ToString() : null
    });

    var result = portalInvitation;
    ApplyWorkExperienceReferenceDetails(result, application, portalInvitation);

    var fromCertificate = certifications.FirstOrDefault();
    if (fromCertificate is not null)
    {
      result.FromCertificate = certificationMapper.MapCertification(fromCertificate);
    }

    result.CertificationTypes = portalInvitationMapper.MapCertificationTypes(application.CertificationTypes);
    result.ApplicantFirstName = applicant.Profile.FirstName;
    result.ApplicantLastName = applicant.Profile.LastName;
    result.ApplicationSubmittedOn = application.SubmittedOn;

    return PortalInvitationVerificationQueryResult.Success(result);
  }

  private static bool HasMissingReference(PortalInvitation? portalInvitation)
  {
    var emptyGuidString = Guid.Empty.ToString();
    var missingWorkExperienceReference = string.IsNullOrEmpty(portalInvitation?.WorkexperienceReferenceId)
      || portalInvitation.WorkexperienceReferenceId == emptyGuidString;
    var missingCharacterReference = string.IsNullOrEmpty(portalInvitation?.CharacterReferenceId)
      || portalInvitation.CharacterReferenceId == emptyGuidString;

    return portalInvitation == null || (missingWorkExperienceReference && missingCharacterReference);
  }

  private static string? GetFailureMessage(Contract.PortalInvitations.PortalInvitationStatusCode? statusCode) => statusCode switch
  {
    Contract.PortalInvitations.PortalInvitationStatusCode.Completed => "Reference has already been submitted.",
    Contract.PortalInvitations.PortalInvitationStatusCode.Expired => "Reference has expired.",
    Contract.PortalInvitations.PortalInvitationStatusCode.Cancelled => "Reference has been cancelled.",
    Contract.PortalInvitations.PortalInvitationStatusCode.Failed => "Reference has failed.",
    _ => null,
  };

  private void ApplyWorkExperienceReferenceDetails(PortalInvitation result, Application application, PortalInvitation portalInvitation)
  {
    if (result.InviteType != Contract.PortalInvitations.InviteType.WorkExperienceReferenceforApplication)
    {
      return;
    }

    var workExRef = application.WorkExperienceReferences.SingleOrDefault(work => work.Id == portalInvitation.WorkexperienceReferenceId);
    if (workExRef == null)
    {
      return;
    }

    result.WorkExperienceReferenceHours = workExRef.Hours;
    if (workExRef.Type != null)
    {
      result.WorkExperienceType = portalInvitationMapper.MapWorkExperienceType(workExRef.Type);
    }
  }
}
