﻿using ECER.Managers.Admin.Contract.PortalInvitations;

namespace ECER.Engines.Transformation;

public abstract record TransformRequest();
public abstract record TransformResponse();

public record EncryptInviteTokenRequest(Guid portalInvitation, InviteType inviteType, int validDays) : TransformRequest;
public record EncryptInviteTokenResponse(Guid portalInvitation, string verificationToken) : TransformResponse;

public record DecryptInviteTokenRequest(string verificationToken) : TransformRequest;
public record DecryptInviteTokenResponse(Guid portalInvitation, InviteType inviteType) : TransformResponse;