using ECER.Managers.Admin.Contract.PortalInvitations;

namespace ECER.Engines.Transformation;

public abstract record TransformRequest();
public abstract record TransformResponse();

public record EncryptInviteTokenRequest(Guid PortalInvitation, InviteType InviteType, int ValidDays) : TransformRequest;
public record EncryptInviteTokenResponse(Guid PortalInvitation, string VerificationToken) : TransformResponse;

public record DecryptInviteTokenRequest(string VerificationToken) : TransformRequest;
public record DecryptInviteTokenResponse(Guid PortalInvitation, InviteType InviteType) : TransformResponse;
