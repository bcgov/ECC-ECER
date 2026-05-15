using ECER.Managers.Registry.Contract.Registrants;
using ECER.Managers.Registry.UserRegistrationIdentityService;
using ECER.Resources.Accounts.Registrants;
using ECER.Resources.Documents.Certifications;
using ECER.Resources.Documents.MetadataResources;
using Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace ECER.Managers.Registry;

/// <summary>
/// User Manager
/// </summary>
public class RegistrantHandlers(
  IRegistrantRepository registrantRepository,
  ICertificationRepository certificationRepository,
  IMetadataResourceRepository metadataResourceRepository,
  IRegistrantMapper registrantMapper,
  IServiceProvider serviceProvider)
  : IRequestHandler<SearchRegistrantQuery, RegistrantQueryResults>,
    IRequestHandler<RegisterNewUserCommand, string>,
    IRequestHandler<UpdateRegistrantProfileCommand, string>,
    IRequestHandler<UpdateRegistrantProfileIdentificationCommand, string>
{
  /// <summary>
  /// Handles search registrants use case
  /// </summary>
  /// <returns>Query results</returns>
  public async ValueTask<RegistrantQueryResults> Handle(SearchRegistrantQuery request, CancellationToken cancellationToken)
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

    return new RegistrantQueryResults(registrantMapper.MapRegistrants(registrants));
  }

  /// <summary>
  /// Handles register a new registy user use case
  /// </summary>
  /// <returns>the user id of the newly created user</returns>
  /// <exception cref="InvalidOperationException"></exception>
  public async ValueTask<string> Handle(RegisterNewUserCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    IRegistrationIdentityService resolver = request.Identity.IdentityProvider switch
    {
      "bcsc" => serviceProvider.GetRequiredService<BcscRegistrationIdentityService>(),
      "bceidbasic" => serviceProvider.GetRequiredService<BceidRegistrationIdentityService>(),
      _ => throw new ArgumentOutOfRangeException("request.Identity.IdentityProvider", request.Identity.IdentityProvider, null)
    };

    return await resolver.Resolve(request, cancellationToken);
  }

  /// <summary>
  /// Handles update an existing registrant profile use case
  /// </summary>
  /// <param name="request"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  public async ValueTask<string> Handle(UpdateRegistrantProfileCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var registrant = (await registrantRepository.Query(new RegistrantQuery
    {
      ByUserId = request.Registrant.UserId
    }, cancellationToken)).SingleOrDefault();

    if (registrant == null) throw new InvalidOperationException($"Registrant {request.Registrant.UserId} wasn't found");

    var profile = registrantMapper.MapUserProfile(request.Registrant.Profile);
    await registrantRepository.Save(new Resources.Accounts.Registrants.Registrant { Id = request.Registrant.UserId, Profile = profile }, cancellationToken);

    return request.Registrant.UserId;
  }

  /// <summary>
  /// Handles add identification to an existing registrant profile use case
  /// </summary>
  /// <param name="request"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  public async ValueTask<string> Handle(UpdateRegistrantProfileIdentificationCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var registrant = (await registrantRepository.Query(new RegistrantQuery
    {
      ByUserId = request.Identification.RegistrantId
    }, cancellationToken)).SingleOrDefault();

    if (registrant == null) throw new InvalidOperationException($"Registrant {request.Identification.RegistrantId} wasn't found");

    var primaryIdOption = (await metadataResourceRepository.QueryIdentificationTypes(new IdentificationTypesQuery
    {
      ById = request.Identification.PrimaryIdTypeObjectId
    }, cancellationToken)).SingleOrDefault();
    if (primaryIdOption == null) throw new InvalidOperationException($"PrimaryIdOption {request.Identification.PrimaryIdTypeObjectId} wasn't found");

    var secondaryIdOption = (await metadataResourceRepository.QueryIdentificationTypes(new IdentificationTypesQuery
    {
      ById = request.Identification.SecondaryIdTypeObjectId
    }, cancellationToken)).SingleOrDefault();
    if (secondaryIdOption == null) throw new InvalidOperationException($"SecondaryIdOption {request.Identification.SecondaryIdTypeObjectId} wasn't found");

    var profileIdentification = new Resources.Accounts.Registrants.ProfileIdentification
    {
      PrimaryIdTypeObjectId = primaryIdOption.Id,
      SecondaryIdTypeObjectId = secondaryIdOption.Id,
      PrimaryIds = registrantMapper.MapIdentityDocuments(request.Identification.PrimaryIds),
      SecondaryIds = registrantMapper.MapIdentityDocuments(request.Identification.SecondaryIds)
    };
    await registrantRepository.SaveIdentityIds(registrant, profileIdentification, cancellationToken);

    return request.Identification.RegistrantId;
  }
}
