using AutoMapper;
using ECER.Managers.Registry.Contract.Registrants;
using ECER.Resources.Accounts.Registrants;
using ECER.Resources.Documents.MetadataResources;
using Serilog;

namespace ECER.Managers.Registry.UserRegistrationIdentityService;

internal sealed class BcscRegistrationIdentityService(IRegistrantRepository registrantRepository, IMetadataResourceRepository metadataResourceRepository, IMapper mapper, ILogger logger) : IRegistrationIdentityService
{
  public async Task<string> Resolve(RegisterNewUserCommand command, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(command);

    var registrants = await registrantRepository.Query(new RegistrantQuery
    {
      ByIdentity = command.Identity
    }, cancellationToken);

    if (registrants.Any()) throw new InvalidOperationException($"Registrant with identity {command.Identity} already exists");

    // If no ECE number, query based on last name and date of birth otherwise add registrationNumber.
    registrants = await registrantRepository.Query(new RegistrantQuery
    {
      ByLastName = command.Profile.LastName,
      ByDateOfBirth = command.Profile.DateOfBirth,
      ByRegistrationNumber = string.IsNullOrEmpty(command.Profile.RegistrationNumber) ? null : command.Profile.RegistrationNumber,
    }, cancellationToken);
    var countries = await metadataResourceRepository.QueryCountries(new CountriesQuery() { ByCode = command.Profile.ResidentialAddress!.Country }, cancellationToken);
    var countryName = countries.FirstOrDefault()?.CountryName ?? string.Empty;

    if (string.IsNullOrEmpty(countryName))
    {
      logger.Warning("Could not find country name for country {0}", command.Profile.ResidentialAddress!.Country);
    }

    command.Profile.ResidentialAddress = command.Profile.ResidentialAddress with { Country = countryName };
    command.Profile.MailingAddress = command.Profile.ResidentialAddress;
    var registrant = mapper.Map<Resources.Accounts.Registrants.Registrant>(command);

    //all BCSC registrants have their identities verified
    registrant.Profile.IDVerificationDecision = IDVerificationDecision.Approve;

    if (string.IsNullOrEmpty(command.Profile.RegistrationNumber))
    {
      registrant.Profile.Status = !registrants.Any() ? Resources.Accounts.Registrants.StatusCode.Verified : Resources.Accounts.Registrants.StatusCode.ReadyforRegistrantMatch;
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
        matchedRegistrant.Profile.ResidentialAddress = mapper.Map<Resources.Accounts.Registrants.Address>(command.Profile.ResidentialAddress);
        matchedRegistrant.Profile.MailingAddress = mapper.Map<Resources.Accounts.Registrants.Address>(command.Profile.MailingAddress);
        matchedRegistrant.Profile.Email = command.Profile.Email;
        matchedRegistrant.Profile.FirstName = command.Profile.FirstName;
        matchedRegistrant.Profile.LastName = command.Profile.LastName;
        matchedRegistrant.Profile.MiddleName = Infrastructure.Common.Utility.GetMiddleName(command.Profile.FirstName!, command.Profile.GivenName!);
        matchedRegistrant.Profile.Phone = command.Profile.Phone;
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
}
