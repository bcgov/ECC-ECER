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
    var response = await transformationEngine.Transform(request)!;
    return response! as GenerateInviteLinkCommandResponse ?? throw new InvalidCastException("Invalid response type");
  }

  public async Task<VerifyInviteLinkCommandResponse> Handle(VerifyInviteLinkCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    ArgumentNullException.ThrowIfNull(transformationEngine);
    var response = await transformationEngine.Transform(request)!;
    return response! as VerifyInviteLinkCommandResponse ?? throw new InvalidCastException("Invalid response type");
  }
}
