using ECER.Engines.Transformation.InviteLinks;
using ECER.Managers.Admin.Contract.InviteLinks;
using MediatR;

namespace ECER.Managers.Admin;

public class InviteLinkHandlers(IInviteLinkTransformationEngine transformationEngine)
  : IRequestHandler<GenerateInviteLinkCommand, GenerateInviteLinkCommandResponse>, IRequestHandler<VerifyInviteLinkCommand, VerifyInviteLinkCommandResponse>
{
  public async Task<GenerateInviteLinkCommandResponse> Handle(GenerateInviteLinkCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    ArgumentNullException.ThrowIfNull(transformationEngine);
    return await transformationEngine.Transform(request)!;
  }

  public async Task<VerifyInviteLinkCommandResponse> Handle(VerifyInviteLinkCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    ArgumentNullException.ThrowIfNull(transformationEngine);
    return await transformationEngine.Transform(request)!;
  }
}
