using AutoMapper;
using ECER.Managers.Registry.Contract.Registrants;
using ECER.Resources.Accounts.Registrants;

namespace ECER.Managers.Registry;

/// <summary>
/// User Manager
/// </summary>
public static class RegistrantUserHandlers
{
  /// <summary>
  /// Handles user profile query use case
  /// </summary>
  /// <param name="query">The query</param>
  /// <param name="registrantRepository"></param>
  /// <param name="mapper"></param>
  /// <returns>Query results</returns>
  public static async Task<RegistrantQueryResults> Handle(Contract.Registrants.RegistrantQuery query, IRegistrantRepository registrantRepository, IMapper mapper)
  {
    ArgumentNullException.ThrowIfNull(registrantRepository);
    ArgumentNullException.ThrowIfNull(mapper);
    ArgumentNullException.ThrowIfNull(query);

    var registrants = await registrantRepository.Query(new Resources.Accounts.Registrants.RegistrantQuery
    {
      ByIdentity = query.ByUserIdentity
    });

    return new RegistrantQueryResults(mapper.Map<IEnumerable<Contract.Registrants.Registrant>>(registrants));
  }

  /// <summary>
  /// Handles registering a new registty user use case
  /// </summary>
  /// <param name="cmd">the command</param>
  /// <param name="registrantRepository"></param>
  /// <param name="mapper"></param>
  /// <returns>the user id of the newly created user</returns>
  /// <exception cref="InvalidOperationException"></exception>
  public static async Task<string> Handle(RegisterNewRegistrantCommand cmd, IRegistrantRepository registrantRepository, IMapper mapper)
  {
    ArgumentNullException.ThrowIfNull(registrantRepository);
    ArgumentNullException.ThrowIfNull(mapper);
    ArgumentNullException.ThrowIfNull(cmd);

    var registrants = await registrantRepository.Query(new Resources.Accounts.Registrants.RegistrantQuery
    {
      ByIdentity = cmd.Identity
    });

    if (registrants.Any()) throw new InvalidOperationException($"Registrant with identity {cmd.Identity} already exists");

    return await registrantRepository.Create(mapper.Map<Resources.Accounts.Registrants.Registrant>(cmd));
  }
}
