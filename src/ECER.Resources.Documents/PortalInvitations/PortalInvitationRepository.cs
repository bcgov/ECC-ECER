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
    return result!;
  }
}
