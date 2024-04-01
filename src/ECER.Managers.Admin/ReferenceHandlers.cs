using ECER.Engines.Transformation.References;
using ECER.Managers.Admin.Contract.References;

namespace ECER.Managers.Admin;

public static class ReferenceHandlers
{
  public static async Task<GenerateReferenceLinkResponse> Handle(ReferenceLinkQuery cmd, IReferenceLinkTransformationEngine transformationEngine, CancellationToken ct)
  {
    ArgumentNullException.ThrowIfNull(cmd);
    ArgumentNullException.ThrowIfNull(transformationEngine);
    return await transformationEngine.Transform(new GenerateReferenceLinkRequest(cmd.portalInvitation, cmd.referenceType))!;
  }
}
