using AutoMapper;
using ECER.Engines.Transformation;
using ECER.Engines.Transformation.PortalInvitations;
using ECER.Managers.Registry.Contract.References;
using ECER.Resources.Accounts.Registrants;
using ECER.Resources.Documents.PortalInvitations;
using ECER.Resources.Documents.References;
using MediatR;
using ReferenceSubmissionRequest = ECER.Managers.Registry.Contract.References.ReferenceSubmissionRequest;

namespace ECER.Managers.Registry;

public class ReferenceHandlers(IPortalInvitationTransformationEngine transformationEngine, IPortalInvitationRepository portalInvitationRepository, IRegistrantRepository registrantRepository, IReferenceRepository referenceRepository, IMapper mapper) : IRequestHandler<ReferenceSubmissionRequest, ReferenceSubmissionResult>
{
  public async Task<ReferenceSubmissionResult> Handle(ReferenceSubmissionRequest request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(portalInvitationRepository);
    ArgumentNullException.ThrowIfNull(registrantRepository);
    ArgumentNullException.ThrowIfNull(referenceRepository);
    ArgumentNullException.ThrowIfNull(transformationEngine);
    ArgumentNullException.ThrowIfNull(mapper);
    ArgumentNullException.ThrowIfNull(request);

    var response = await transformationEngine.Transform(new DecryptInviteTokenRequest(request.Token))! as DecryptInviteTokenResponse ?? throw new InvalidCastException("Invalid response type");
    if (response.portalInvitation == Guid.Empty) return ReferenceSubmissionResult.Failure("Invalid Token");

    var portalInvitation = await portalInvitationRepository.Query(new PortalInvitationQuery(response.portalInvitation), cancellationToken);
    var registrantResult = await registrantRepository.Query(new RegistrantQuery() { ByUserId = portalInvitation.ApplicantId }, cancellationToken);

    var applicant = registrantResult.SingleOrDefault();
    if (applicant == null) return ReferenceSubmissionResult.Failure("Applicant not found");

    var result = await referenceRepository.SubmitReferenceResponse(mapper.Map<Resources.Documents.References.ReferenceSubmissionRequest>(request), cancellationToken);
    if (!result) return ReferenceSubmissionResult.Failure("Failed to submit reference");
    return ReferenceSubmissionResult.Success();
  }
}
