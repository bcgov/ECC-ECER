using ECER.Managers.Admin.Contract.InviteLinks;

namespace ECER.Engines.Transformation.InviteLinks;

/// <summary>
/// transforms references links
/// </summary>
public interface IInviteLinkTransformationEngine
{
  Task<TransformResponse> Transform(TransformRequest request);
}
