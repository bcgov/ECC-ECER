using ECER.Engines.Transformation.References;
using ECER.Managers.Admin.Contract.References;
using MediatR;

namespace ECER.Managers.Admin;

public class ReferenceHandlers(IReferenceLinkTransformationEngine transformationEngine)
  : IRequestHandler<ReferenceLinkQuery, GenerateReferenceLinkResponse>
{
  public async Task<GenerateReferenceLinkResponse> Handle(ReferenceLinkQuery request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    ArgumentNullException.ThrowIfNull(transformationEngine);
    return await transformationEngine.Transform(new GenerateReferenceLinkRequest(request.portalInvitation, request.referenceType))!;
  }
}
