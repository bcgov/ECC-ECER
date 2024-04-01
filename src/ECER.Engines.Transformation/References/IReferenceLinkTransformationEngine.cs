using ECER.Managers.Admin.Contract.References;

namespace ECER.Engines.Transformation.References;

/// <summary>
/// transforms references links
/// </summary>
public interface IReferenceLinkTransformationEngine
{
  Task<GenerateReferenceLinkResponse> Transform(GenerateReferenceLinkRequest request);

  Task<GenerateReferenceLinkRequest> UnTransform(GenerateReferenceLinkResponse response);
}
