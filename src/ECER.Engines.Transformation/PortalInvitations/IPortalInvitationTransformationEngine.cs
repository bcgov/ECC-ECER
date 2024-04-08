namespace ECER.Engines.Transformation.PortalInvitations;

/// <summary>
/// transforms references links
/// </summary>
public interface IPortalInvitationTransformationEngine
{
  Task<TransformResponse> Transform(TransformRequest request);
}
