using AutoMapper;
using ECER.Managers.Registry.Contract.Registrants;
using ECER.Resources.Accounts.Registrants;
using MediatR;

namespace ECER.Managers.Registry;

/// <summary>
/// User Manager
/// </summary>
public class RegistrantHandlers(IRegistrantRepository registrantRepository, IMapper mapper)
  : IRequestHandler<SearchRegistrantQuery, RegistrantQueryResults>,
    IRequestHandler<RegisterNewUserCommand, string>,
    IRequestHandler<UpdateRegistrantProfileCommand, string>

{
  /// <summary>
  /// Handles search registrants use case
  /// </summary>
  /// <returns>Query results</returns>
  public async Task<RegistrantQueryResults> Handle(SearchRegistrantQuery request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var registrants = await registrantRepository.Query(new RegistrantQuery
    {
      ByIdentity = request.ByUserIdentity
    }, cancellationToken);

    return new RegistrantQueryResults(mapper.Map<IEnumerable<Contract.Registrants.Registrant>>(registrants)!);
  }

  /// <summary>
  /// Handles register a new registty user use case
  /// </summary>
  /// <returns>the user id of the newly created user</returns>
  /// <exception cref="InvalidOperationException"></exception>
  public async Task<string> Handle(RegisterNewUserCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var registrants = await registrantRepository.Query(new RegistrantQuery
    {
      ByIdentity = request.Identity
    }, cancellationToken);

    if (registrants.Any()) throw new InvalidOperationException($"Registrant with identity {request.Identity} already exists");

    return await registrantRepository.Create(mapper.Map<Resources.Accounts.Registrants.Registrant>(request)!, cancellationToken);
  }

  /// <summary>
  /// Handles update an existing registrant profile use case
  /// </summary>
  /// <param name="request"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  public async Task<string> Handle(UpdateRegistrantProfileCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var registrant = (await registrantRepository.Query(new RegistrantQuery
    {
      ByUserId = request.Registrant.UserId
    }, cancellationToken)).SingleOrDefault();

    if (registrant == null) throw new InvalidOperationException($"Registrant {request.Registrant.UserId} wasn't found");
    var profile = mapper.Map<Resources.Accounts.Registrants.UserProfile>(request.Registrant.Profile)!;
    await registrantRepository.Save(new Resources.Accounts.Registrants.Registrant { Id = request.Registrant.UserId, Profile = profile }, cancellationToken);

    return request.Registrant.UserId;
  }
}
