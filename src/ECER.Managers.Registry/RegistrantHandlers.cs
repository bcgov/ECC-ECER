using AutoMapper;
using ECER.Managers.Registry.Contract.Registrants;
using ECER.Resources.Accounts.Registrants;

namespace ECER.Managers.Registry;

/// <summary>
/// User Manager
/// </summary>
public static class RegistrantHandlers
{
  /// <summary>
  /// Handles user profile query use case
  /// </summary>
  /// <returns>Query results</returns>
  public static async Task<RegistrantQueryResults> Handle(Contract.Registrants.RegistrantQuery query, IRegistrantRepository registrantRepository, IMapper mapper, CancellationToken ct)
  {
    ArgumentNullException.ThrowIfNull(registrantRepository);
    ArgumentNullException.ThrowIfNull(mapper);
    ArgumentNullException.ThrowIfNull(query);

    var registrants = await registrantRepository.Query(new Resources.Accounts.Registrants.RegistrantQuery
    {
      ByIdentity = query.ByUserIdentity
    }, ct);

    return new RegistrantQueryResults(mapper.Map<IEnumerable<Contract.Registrants.Registrant>>(registrants));
  }

  /// <summary>
  /// Handles registering a new registty user use case
  /// </summary>
  /// <returns>the user id of the newly created user</returns>
  /// <exception cref="InvalidOperationException"></exception>
  public static async Task<string> Handle(RegisterNewUserCommand cmd, IRegistrantRepository registrantRepository, IMapper mapper, CancellationToken ct)
  {
    ArgumentNullException.ThrowIfNull(registrantRepository);
    ArgumentNullException.ThrowIfNull(mapper);
    ArgumentNullException.ThrowIfNull(cmd);

    var registrants = await registrantRepository.Query(new Resources.Accounts.Registrants.RegistrantQuery
    {
      ByIdentity = cmd.Identity
    }, ct);

    if (registrants.Any()) throw new InvalidOperationException($"Registrant with identity {cmd.Identity} already exists");

    return await registrantRepository.Create(mapper.Map<Resources.Accounts.Registrants.Registrant>(cmd), ct);
  }

  /// <summary>
  /// Handles updating an existing registrant profile use case
  /// </summary>
  /// <param name="cmd"></param>
  /// <param name="registrantRepository"></param>
  /// <param name="mapper"></param>
  /// <param name="ct"></param>
  /// <returns></returns>
  public static async Task<string> Handle(UpdateRegistrantProfileCommand cmd, IRegistrantRepository registrantRepository, IMapper mapper, CancellationToken ct)
  {
    ArgumentNullException.ThrowIfNull(registrantRepository);
    ArgumentNullException.ThrowIfNull(mapper);
    ArgumentNullException.ThrowIfNull(cmd);

    var registrant = (await registrantRepository.Query(new Resources.Accounts.Registrants.RegistrantQuery
    {
      ByUserId = cmd.Registrant.UserId
    }, ct)).SingleOrDefault();

    if (registrant == null) throw new InvalidOperationException($"Registrant {cmd.Registrant.UserId} wasn't found");

    var profile = mapper.Map<Resources.Accounts.Registrants.UserProfile>(cmd.Registrant.Profile);

    await registrantRepository.Save(new Resources.Accounts.Registrants.Registrant { Id = cmd.Registrant.UserId, Profile = profile }, ct);

    return cmd.Registrant.UserId;
  }
}
