using ECER.Managers.Registry.Contract.PspUsers;
using ECER.Managers.Registry.Contract.Registrants;
using ECER.Resources.Accounts.PspReps;
using ECER.Resources.Accounts.Registrants;

namespace ECER.Managers.Registry.UserRegistrationIdentityService;

internal sealed class BceidRegistrationIdentityService(
  IRegistrantRepository registrantRepository,
  IPspRepRepository pspRepRepository,
  IRegistrantMapper registrantMapper,
  IPspUserMapper pspUserMapper) : IRegistrationIdentityService
{
  public async Task<string> Resolve(RegisterNewUserCommand command, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(command);

    var registrants = await registrantRepository.Query(new RegistrantQuery
    {
      ByIdentity = command.Identity
    }, cancellationToken);

    if (registrants.Any()) throw new InvalidOperationException($"Registrant with identity {command.Identity} already exists");

    command.Profile.Status = Contract.Registrants.StatusCode.Unverified;

    var registrant = registrantMapper.MapRegisteredRegistrant(command);
    return await registrantRepository.Create(registrant, cancellationToken);
  }

  public async Task<string> Resolve(RegisterNewPspUserCommand command, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(command);

    var pspUsers = await pspRepRepository.Query(new PspRepQuery
    {
      ByIdentity = command.Identity
    }, cancellationToken);

    if (pspUsers.Any()) throw new InvalidOperationException($"Registrant with identity {command.Identity} already exists");

    var pspUser = pspUserMapper.MapRegisteredPspUser(command);
    return await pspRepRepository.AttachIdentity(pspUser, cancellationToken);
  }
}
