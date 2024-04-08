using AutoMapper;
using ECER.Utilities.DataverseSdk.Model;

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
    var portalInvitations = from pi in context.ecer_PortalInvitationSet
                            join c in context.ContactSet on pi.ecer_portalinvitation_ApplicantId.Id equals c.ContactId
                            where pi.ecer_PortalInvitationId == query.portalInvitationId
                            select new { pi, c };

    var data = portalInvitations.Select(r => r.pi).ToList();
    var result = mapper.Map<IEnumerable<PortalInvitation>>(data);
    return result!;
  }
}
