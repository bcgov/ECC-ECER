using MediatR;

namespace ECER.Managers.Admin.Contract.PortalInvitations;

public record GenerateInviteLinkCommand(Guid PortalInvitation, int ValidDays) : IRequest<GenerateInviteLinkCommandResponse>;
public record GenerateInviteLinkCommandResponse(Guid PortalInvitation, string VerificationLink);
