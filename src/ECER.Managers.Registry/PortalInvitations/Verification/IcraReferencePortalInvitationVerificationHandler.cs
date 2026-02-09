using AutoMapper;
using ECER.Managers.Registry.Contract.PortalInvitations;
using ECER.Resources.Accounts.Registrants;

namespace ECER.Managers.Registry;

public class IcraReferencePortalInvitationVerificationHandler(
  IRegistrantRepository registrantRepository,
  IMapper mapper)
  : IPortalInvitationVerificationHandler
{
  public bool CanHandle(InviteType? inviteType)
  {
    return inviteType == InviteType.WorkExperienceReferenceforICRA;
  }

  public async Task<PortalInvitationVerificationQueryResult> Verify(PortalInvitation portalInvitation, CancellationToken cancellationToken)
  {
    var emptyGuidString = Guid.Empty.ToString();
    if (portalInvitation == null || portalInvitation.WorkexperienceReferenceId == emptyGuidString)
    {
      return PortalInvitationVerificationQueryResult.Failure("Reference not found");
    }

    var registrantResult = await registrantRepository.Query(new RegistrantQuery() { ByUserId = portalInvitation.ApplicantId }, cancellationToken);
    var applicant = registrantResult.SingleOrDefault();
    if (applicant == null)
    {
      return PortalInvitationVerificationQueryResult.Failure("Applicant not found");
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

    // For ICRA references there is no ApplicationId; do not attempt to load application or certification data
    result.ApplicantFirstName = applicant.Profile.FirstName;
    result.ApplicantLastName = applicant.Profile.LastName;

    return PortalInvitationVerificationQueryResult.Success(result);
  }
}


