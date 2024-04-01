using MediatR;

namespace ECER.Managers.Admin.Contract.References;

public record ReferenceLinkQuery(Guid portalInvitation, ReferenceType referenceType) : IRequest<GenerateReferenceLinkResponse>;
public record GenerateReferenceLinkRequest(Guid portalInvitation, ReferenceType referenceType);
public record GenerateReferenceLinkResponse(Guid portalInvitation, string referenceLink);

public enum ReferenceType
{
  CharacterReference,
  WorkExperienceReference
}
