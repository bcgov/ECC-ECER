using ECER.Managers.Admin.Contract.InviteLinks;

namespace ECER.Engines.Transformation;

public abstract record TransformRequest();
public abstract record TransformResponse();

public record EncryptInviteTokenRequest(Guid portalInvitation, InviteType inviteType, int validDays) : TransformRequest;
public record EncryptInviteTokenResponse(Guid portalInvitation, string verificationToken) : TransformResponse;

public record DecryptInviteTokenRequest(Guid portalInvitation, string verificationToken) : TransformRequest;
public record DecryptInviteTokenResponse(Guid portalInvitation, InviteType inviteType) : TransformResponse;
