using AutoMapper;
using ECER.Engines.Transformation;
using ECER.Engines.Transformation.PortalInvitations;
using ECER.Managers.Registry.Contract.PortalInvitations;
using ECER.Resources.Accounts.Registrants;
using ECER.Resources.Documents.Applications;
using ECER.Resources.Documents.PortalInvitations;
using MediatR;
using InviteType = ECER.Managers.Registry.Contract.PortalInvitations.InviteType;
using PortalInvitationStatusCode = ECER.Managers.Registry.Contract.PortalInvitations.PortalInvitationStatusCode;

namespace ECER.Managers.Registry;

public class PortalInvitationHandlers(IPortalInvitationTransformationEngine transformationEngine, IPortalInvitationRepository portalInvitationRepository, IApplicationRepository applicationRepository, IRegistrantRepository registrantRepository, IMapper mapper)
  : IRequestHandler<PortalInvitationVerificationQuery, PortalInvitationVerificationQueryResult>
{
  public async Task<PortalInvitationVerificationQueryResult> Handle(PortalInvitationVerificationQuery request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    var response = await transformationEngine.Transform(new DecryptInviteTokenRequest(request.VerificationToken))! as DecryptInviteTokenResponse ?? throw new InvalidCastException("Invalid response type");
    if (response.PortalInvitation == Guid.Empty) return PortalInvitationVerificationQueryResult.Failure("Invalid Token");

    var portalInvitation = await portalInvitationRepository.Query(new PortalInvitationQuery(response.PortalInvitation), cancellationToken);
    if (portalInvitation == null) return PortalInvitationVerificationQueryResult.Failure("Portal Invitation not found");
    
    if (portalInvitation.WorkexperienceReferenceId == null && portalInvitation.CharacterReferenceId == null) return PortalInvitationVerificationQueryResult.Failure("Reference not found");

    var registrantResult = await registrantRepository.Query(new RegistrantQuery() { ByUserId = portalInvitation.ApplicantId }, cancellationToken);

    var applicant = registrantResult.SingleOrDefault();
    if (applicant == null) return PortalInvitationVerificationQueryResult.Failure("Applicant not found");
    

    var applications = await applicationRepository.Query(new ApplicationQuery() { ById = portalInvitation.ApplicationId }, cancellationToken);
    var application = applications.SingleOrDefault();
    if (application == null) return PortalInvitationVerificationQueryResult.Failure("Application not found");

    var result = mapper.Map<Contract.PortalInvitations.PortalInvitation>(portalInvitation);
    
    switch (result.StatusCode)
    {
      case PortalInvitationStatusCode.Completed:
        return PortalInvitationVerificationQueryResult.Failure("Reference has already been submitted.");
      case PortalInvitationStatusCode.Expired:
        return PortalInvitationVerificationQueryResult.Failure("Reference has expired.");
      case PortalInvitationStatusCode.Cancelled:
        return PortalInvitationVerificationQueryResult.Failure("Reference has been cancelled.");
      case PortalInvitationStatusCode.Failed:
        return PortalInvitationVerificationQueryResult.Failure("Reference has failed.");
    }
    
    if (result.InviteType == Contract.PortalInvitations.InviteType.WorkExperienceReference)
    {
      var workExRef =
        application.WorkExperienceReferences.SingleOrDefault(work =>
          work.Id == portalInvitation.WorkexperienceReferenceId);
      if (workExRef != null)
      {
        result.WorkExperienceReferenceHours = workExRef.Hours;
      }
    }
    result.CertificationTypes = mapper.Map<Contract.Applications.Application>(application)!.CertificationTypes!;
    result.ApplicantFirstName = applicant.Profile.FirstName;
    result.ApplicantLastName = applicant.Profile.LastName;

    return PortalInvitationVerificationQueryResult.Success(result);
  }
}
