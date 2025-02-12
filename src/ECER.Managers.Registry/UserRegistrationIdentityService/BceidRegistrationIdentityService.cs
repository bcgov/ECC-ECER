using AutoMapper;
using ECER.Managers.Registry.Contract.Registrants;
using ECER.Resources.Accounts.Registrants;

namespace ECER.Managers.Registry.UserRegistrationIdentityService;

internal sealed class BceidRegistrationIdentityService(IRegistrantRepository registrantRepository, IMapper mapper) : IRegistrationIdentityService
{
  public async Task<string> Resolve(RegisterNewUserCommand command, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(command);

    //always unverified
    //last name info
    //address
    //call create

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
}
