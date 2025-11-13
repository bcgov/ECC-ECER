using AutoMapper;
using ECER.Managers.Registry.Contract.PspUsers;
using ECER.Managers.Registry.Contract.Registrants;
using ECER.Resources.Accounts.PspReps;
using ECER.Resources.Accounts.Registrants;
using PspUser = ECER.Resources.Accounts.PspReps.PspUser;

namespace ECER.Managers.Registry.UserRegistrationIdentityService;

internal sealed class BceidRegistrationIdentityService(IRegistrantRepository registrantRepository, IPspRepRepository pspRepRepository, IMapper mapper) : IRegistrationIdentityService
{
  public async Task<string> Resolve(RegisterNewUserCommand command, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(command);

    var registrants = await registrantRepository.Query(new RegistrantQuery
    {
      ByIdentity = command.Identity
    }, cancellationToken);

    if (registrants.Any()) throw new InvalidOperationException($"Registrant with identity {command.Identity} already exists");

    //all Bceid registrants have their identities unverified
    command.Profile.Status = Contract.Registrants.StatusCode.Unverified;

    var registrant = mapper.Map<Resources.Accounts.Registrants.Registrant>(command);

    return await registrantRepository.Create(registrant, cancellationToken);
  }
  
  public async Task<string> Resolve(RegisterNewPspUserCommand command, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(command);

    var pspUsers = await pspRepRepository.Query(new PspRepQuery()
    {
      ByIdentity = command.Identity
    }, cancellationToken);

    if (pspUsers.Any()) throw new InvalidOperationException($"Registrant with identity {command.Identity} already exists");

    var pspUser = mapper.Map<PspUser>(command);

    return await pspRepRepository.AttachIdentity(pspUser, cancellationToken);
  }
}
