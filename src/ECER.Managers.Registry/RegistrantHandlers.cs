using Amazon;
using AutoMapper;
using ECER.Managers.Registry.Contract.Registrants;
using ECER.Resources.Accounts.Registrants;
using MediatR;
using Microsoft.Xrm.Sdk.Query;
using System.ServiceModel.Channels;

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

    // If no ECE number, query based on last name and date of birth otherwise add registrationNumber.
    registrants = await registrantRepository.Query(new RegistrantQuery
    {
      ByLastName = request.Profile.LastName,
      ByDateOfBirth = request.Profile.DateOfBirth,
      ByRegistrationNumber = string.IsNullOrEmpty(request.Profile.RegistrationNumber) ? null : request.Profile.RegistrationNumber,
    }, cancellationToken);

    var registrant = mapper.Map<Resources.Accounts.Registrants.Registrant>(request);

    // Logic for the 'No' ECE case
    if (string.IsNullOrEmpty(request.Profile.RegistrationNumber))
    {
      registrant.Profile.IsVerified = !registrants.Any();
      return await registrantRepository.Create(registrant, cancellationToken);
    }
    // Logic for the 'Yes' ECE case
    else
    {
      var matchedRegistrant = registrants.FirstOrDefault();

      if (matchedRegistrant != null)
      {
        // Update the existing contact record
        matchedRegistrant.Profile.IsVerified = true;
        matchedRegistrant.Profile.ResidentialAddress = mapper.Map<Resources.Accounts.Registrants.Address>(request.Profile.ResidentialAddress);
        matchedRegistrant.Profile.Email = request.Profile.Email;
        matchedRegistrant.Profile.FirstName = request.Profile.FirstName;
        matchedRegistrant.Profile.LastName = request.Profile.LastName;
        matchedRegistrant.Profile.MiddleName = ECER.Infrastructure.Common.Utility.GetMiddleName(request.Profile.FirstName!, request.Profile.GivenName!);
        matchedRegistrant.Profile.Phone = request.Profile.Phone;

        // Update existing registrant
        await registrantRepository.Save(new Resources.Accounts.Registrants.Registrant { Id = matchedRegistrant.Id, Profile = matchedRegistrant.Profile }, cancellationToken);
        return matchedRegistrant.Id;
      }
      else
      {
        // No matching contact, create a new record
        registrant.Profile.IsVerified = false;
        return await registrantRepository.Create(registrant, cancellationToken);
      }
    }
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
