using AutoMapper;
using ECER.Utilities.DataverseSdk.Model;
using Microsoft.Xrm.Sdk.Client;

namespace ECER.Resources.Accounts.PortalInvitations;

internal class PortalInvitationRepository : IPortalInvitationRepository
{
  private readonly EcerContext context;
  private readonly IMapper mapper;

  public PortalInvitationRepository(EcerContext context, IMapper mapper)
  {
    this.context = context;
    this.mapper = mapper;
  }

  public async Task<IEnumerable<PortalInvitation>> Query(PortalInvitationQuery query)
  {
    await Task.CompletedTask;
    var portalInvitations = context.ecer_PortalInvitationSet.Where(pi => pi.ecer_PortalInvitationId == query.portalInvitationId);

    context.LoadProperties(portalInvitations, ecer_PortalInvitation.Fields.ecer_portalinvitation_ApplicantId);
    var result = mapper.Map<IEnumerable<PortalInvitation>>(portalInvitations);
    return result!;
  }
}
