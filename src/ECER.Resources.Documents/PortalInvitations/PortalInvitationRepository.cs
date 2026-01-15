using AutoMapper;
using ECER.Utilities.DataverseSdk.Model;

namespace ECER.Resources.Documents.PortalInvitations;

internal class PortalInvitationRepository : IPortalInvitationRepository
{
  private readonly EcerContext context;
  private readonly IMapper mapper;

  public PortalInvitationRepository(EcerContext context, IMapper mapper)
  {
    this.context = context;
    this.mapper = mapper;
  }

  public async Task<PortalInvitation> Query(PortalInvitationQuery query, CancellationToken ct)
  {
    await Task.CompletedTask;
    var portalInvitation = context.ecer_PortalInvitationSet.Single(pi => pi.ecer_PortalInvitationId == query.portalInvitationId);
    var result = mapper.Map<PortalInvitation>(portalInvitation);

    if (portalInvitation != null && portalInvitation.ecer_Type == ecer_PortalInvitationTypes.PSIProgramRepresentative)
    {
      var pspRep = context.ecer_ECEProgramRepresentativeSet.Single(pr => pr.ecer_ECEProgramRepresentativeId == portalInvitation!.ecer_psiprogramrepresentativeid.Id);
      var institute = context.ecer_PostSecondaryInstituteSet.Single(psi => psi.ecer_PostSecondaryInstituteId == pspRep.ecer_PostSecondaryInstitute.Id);
      if (institute.ecer_BusinessBCeID != null)
      {
        result.IsLinked = true;
        result.BceidBusinessName = institute.ecer_BCeIDBusinessName;
      }
    }
    return result!;
  }

  public async Task Complete(CompletePortalInvitationCommand command, CancellationToken ct)
  {
    await Task.CompletedTask;
    var portalInvitation = context.ecer_PortalInvitationSet.Single(pi => pi.ecer_PortalInvitationId == command.portalInvitationId);
    portalInvitation.StatusCode = ecer_PortalInvitation_StatusCode.Completed;
    portalInvitation.StateCode = ecer_portalinvitation_statecode.Inactive;
    context.UpdateObject(portalInvitation);
    context.SaveChanges();
  }
}
