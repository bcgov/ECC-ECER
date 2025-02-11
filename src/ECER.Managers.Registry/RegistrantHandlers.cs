using AutoMapper;
using ECER.Managers.Registry.Contract.Registrants;
using ECER.Resources.Accounts.Registrants;
using ECER.Resources.Documents.Certifications;
using ECER.Resources.Documents.MetadataResources;
using MediatR;
using ILogger = Serilog.ILogger;

namespace ECER.Managers.Registry;

/// <summary>
/// User Manager
/// </summary>
public class RegistrantHandlers(IRegistrantRepository registrantRepository, ICertificationRepository certificationRepository, IMetadataResourceRepository metadataResourceRepository, IMapper mapper, ILogger logger)
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

    foreach (var registrant in registrants)
    {
      var certifications = await certificationRepository.Query(new UserCertificationQuery
      {
        ByApplicantId = registrant.Id
      });
      registrant.Profile.IsRegistrant = certifications.Any();
    }

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

    var countries = await metadataResourceRepository.QueryCountries(new CountriesQuery() { ByCode = request.Profile.ResidentialAddress!.Country }, cancellationToken);
    var countryName = countries.FirstOrDefault()?.CountryName ?? string.Empty;

    if (string.IsNullOrEmpty(countryName))
    {
      logger.Warning("Could not find country name for country {0}", request.Profile.ResidentialAddress!.Country);
    }

    request.Profile.ResidentialAddress = request.Profile.ResidentialAddress with { Country = countryName };
    request.Profile.MailingAddress = request.Profile.ResidentialAddress;
    var registrant = mapper.Map<Resources.Accounts.Registrants.Registrant>(request);

    //all BCSC registrants have their identities verified
    registrant.Profile.IDVerificationDecision = IDVerificationDecision.Approve;

    if (string.IsNullOrEmpty(request.Profile.RegistrationNumber))
    {
      /*registrant.Profile.Status = !registrants.Any() ? Resources.Accounts.Registrants.StatusCode.Verified : Resources.Accounts.Registrants.StatusCode.ReadyforRegistrantMatch;*/
      registrant.Profile.IsVerified = !registrants.Any();

      return await registrantRepository.Create(registrant, cancellationToken);
    }
    else
    {
      var matchedRegistrant = registrants.FirstOrDefault();

      if (matchedRegistrant != null)
      {
        matchedRegistrant.Profile.IsVerified = true;
        registrant.Profile.Status = Resources.Accounts.Registrants.StatusCode.Verified;
        matchedRegistrant.Profile.ResidentialAddress = mapper.Map<Resources.Accounts.Registrants.Address>(request.Profile.ResidentialAddress);
        matchedRegistrant.Profile.MailingAddress = mapper.Map<Resources.Accounts.Registrants.Address>(request.Profile.MailingAddress);
        matchedRegistrant.Profile.Email = request.Profile.Email;
        matchedRegistrant.Profile.FirstName = request.Profile.FirstName;
        matchedRegistrant.Profile.LastName = request.Profile.LastName;
        matchedRegistrant.Profile.MiddleName = ECER.Infrastructure.Common.Utility.GetMiddleName(request.Profile.FirstName!, request.Profile.GivenName!);
        matchedRegistrant.Profile.Phone = request.Profile.Phone;
        matchedRegistrant.Identities = registrant.Identities;
        await registrantRepository.Create(matchedRegistrant, cancellationToken);
        return matchedRegistrant.Id;
      }
      else
      {
        registrant.Profile.IsVerified = false;
        registrant.Profile.Status = Resources.Accounts.Registrants.StatusCode.ReadyforRegistrantMatch;
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
