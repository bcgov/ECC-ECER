using AutoMapper;
using ECER.Managers.Registry.Contract;
using ECER.Resources.Accounts.Registrants;

namespace ECER.Managers.Registry;

/// <summary>
/// User Manager
/// </summary>
public static class UserHandlers
{
  /// <summary>
  /// Handles user profile query use case
  /// </summary>
  /// <param name="query">The query</param>
  /// <param name="registrantRepository">DI service</param>
  /// <param name="mapper">DI service</param>
  /// <returns>Query results</returns>
  public static async Task<UserProfileQueryResponse> Handle(UserProfileQuery query, IRegistrantRepository registrantRepository, IMapper mapper)
  {
    ArgumentNullException.ThrowIfNull(registrantRepository);
    ArgumentNullException.ThrowIfNull(mapper);
    ArgumentNullException.ThrowIfNull(query);

    var results = await registrantRepository.Query(new RegistrantQuery
    {
      WithIdentity = query.UserIdentity
    });

    var registrant = results.Items.SingleOrDefault();

    return new UserProfileQueryResponse(registrant?.Id, mapper.Map<Contract.UserProfile?>(registrant?.Profile));
  }

  /// <summary>
  /// Handles registering a new registty user use case
  /// </summary>
  /// <param name="cmd">the command</param>
  /// <param name="registrantRepository">DI service</param>
  /// <param name="mapper">DI service</param>
  /// <returns>the user id of the newly created user</returns>
  /// <exception cref="InvalidOperationException"></exception>
  public static async Task<string> Handle(RegisterNewUserCommand cmd, IRegistrantRepository registrantRepository, IMapper mapper)
  {
    ArgumentNullException.ThrowIfNull(registrantRepository);
    ArgumentNullException.ThrowIfNull(mapper);
    ArgumentNullException.ThrowIfNull(cmd);

    var results = await registrantRepository.Query(new RegistrantQuery
    {
      WithIdentity = cmd.Identity
    });

    if (results.Items.Any()) throw new InvalidOperationException($"Registrant with identity {cmd.Identity} already exists");

    return await registrantRepository.RegisterNew(mapper.Map<NewRegistrantRequest>(cmd));
  }
}
