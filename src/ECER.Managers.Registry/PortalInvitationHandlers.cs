using AutoMapper;
using ECER.Engines.Transformation;
using ECER.Engines.Transformation.PortalInvitations;
using ECER.Managers.Registry.Contract.PortalInvitations;
using ECER.Resources.Accounts.Registrants;
using ECER.Resources.Documents.PortalInvitations;
using MediatR;

namespace ECER.Managers.Registry;

public class PortalInvitationHandlers(IPortalInvitationTransformationEngine transformationEngine, IPortalInvitationRepository portalInvitationRepository, IRegistrantRepository registrantRepository, IMapper mapper)
  : IRequestHandler<PortalInvitationVerificationQuery, PortalInvitationVerificationQueryResult>
{
  public async Task<PortalInvitationVerificationQueryResult> Handle(PortalInvitationVerificationQuery request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    ArgumentNullException.ThrowIfNull(transformationEngine);
    var response = await transformationEngine.Transform(new DecryptInviteTokenRequest(request.VerificationToken))! as DecryptInviteTokenResponse ?? throw new InvalidCastException("Invalid response type");
    if (response.portalInvitation == Guid.Empty) throw new InvalidOperationException("Invalid token");

    var portalInvitation = await portalInvitationRepository.Query(new PortalInvitationQuery(response.portalInvitation), cancellationToken);

    var registrantResult = await registrantRepository.Query(new RegistrantQuery() { ByUserId = portalInvitation.ApplicantId }, cancellationToken);
    var applicant = registrantResult.SingleOrDefault();
    if (applicant == null) throw new InvalidOperationException("Applicant not found");
    var result = mapper.Map<Contract.PortalInvitations.PortalInvitation>(portalInvitation)!;
    result.ApplicantFirstName = applicant.Profile.FirstName;
    result.ApplicantLastName = applicant.Profile.LastName;
    return new PortalInvitationVerificationQueryResult(result);
  }
}
