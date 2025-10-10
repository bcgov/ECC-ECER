using ECER.Utilities.Security;
using MediatR;

namespace ECER.Managers.Registry.Contract.PspUsers;

public record PspUserProfile
{
  public string Id { get; set; } = null!;
  public string? FirstName { get; set; }
  public string? LastName { get; set; }
  public string? Email { get; set; } = null!;
  public string? BceidBusinessId { get; set; }
  public string? ProgramRepresentativeId { get; set; }
};

public record RegisterPspUserCommand(PspUserProfile pspUserProfile) : IRequest<string>;
