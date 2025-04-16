using AutoMapper;
using ECER.Managers.Admin.Contract.Metadatas;
using ECER.Resources.Documents.MetadataResources;
using MediatR;

namespace ECER.Managers.Admin;

public class MetadataHandlers(
   IMetadataResourceRepository metadataResourceRepository,
   IMapper mapper) : IRequestHandler<Contract.Metadatas.ProvincesQuery, ProvincesQueryResults>,
   IRequestHandler<Contract.Metadatas.CountriesQuery, CountriesQueryResults>,
   IRequestHandler<Contract.Metadatas.CertificationComparisonQuery, CertificationComparisonQueryResults>,
   IRequestHandler<Contract.Metadatas.PostSecondaryInstitutionsQuery, PostSecondaryInstitutionsQueryResults>,
   IRequestHandler<Contract.Metadatas.SystemMessagesQuery, SystemMessagesQueryResults>,
   IRequestHandler<Contract.Metadatas.IdentificationTypesQuery, IdentificationTypesQueryResults>
{
  public async Task<PostSecondaryInstitutionsQueryResults> Handle(Contract.Metadatas.PostSecondaryInstitutionsQuery request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var postSecondaryInstitutions = await metadataResourceRepository.QueryPostSecondaryInstitutions(mapper.Map<Resources.Documents.MetadataResources.PostSecondaryInstitutionsQuery>(request), cancellationToken);
    return new PostSecondaryInstitutionsQueryResults(mapper.Map<IEnumerable<Contract.Metadatas.PostSecondaryInstitution>>(postSecondaryInstitutions)!);
  }

  public async Task<CertificationComparisonQueryResults> Handle(Contract.Metadatas.CertificationComparisonQuery request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var certificationComparisons = await metadataResourceRepository.QueryCertificationComparisons(new Resources.Documents.MetadataResources.CertificationComparisonQuery() { ById = request.ById, ByProvinceId = request.ByProvinceId }, cancellationToken);
    return new CertificationComparisonQueryResults(mapper.Map<IEnumerable<Contract.Metadatas.CertificationComparison>>(certificationComparisons)!);
  }

  public async Task<ProvincesQueryResults> Handle(Contract.Metadatas.ProvincesQuery request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var provinces = await metadataResourceRepository.QueryProvinces(new Resources.Documents.MetadataResources.ProvincesQuery() { ById = request.ById }, cancellationToken);
    return new ProvincesQueryResults(mapper.Map<IEnumerable<Contract.Metadatas.Province>>(provinces)!);
  }

  public async Task<SystemMessagesQueryResults> Handle(Contract.Metadatas.SystemMessagesQuery request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var systemMessages = await metadataResourceRepository.QuerySystemMessages(new Resources.Documents.MetadataResources.SystemMessagesQuery() { ById = request.ById }, cancellationToken);

    // Return only active system messages
    systemMessages = systemMessages.Where(m => m.StartDate < DateTime.Now && m.EndDate > DateTime.Now).ToList();

    return new SystemMessagesQueryResults(mapper.Map<IEnumerable<Contract.Metadatas.SystemMessage>>(systemMessages)!);
  }

  public async Task<CountriesQueryResults> Handle(Contract.Metadatas.CountriesQuery request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var countries = await metadataResourceRepository.QueryCountries(new Resources.Documents.MetadataResources.CountriesQuery() { ById = request.ById, ByCode = request.ByCode, ByName = request.ByName }, cancellationToken);
    return new CountriesQueryResults(mapper.Map<IEnumerable<Contract.Metadatas.Country>>(countries)!);
  }

  public async Task<IdentificationTypesQueryResults> Handle(Contract.Metadatas.IdentificationTypesQuery request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var identificationTypes = await metadataResourceRepository.QueryIdentificationTypes(new Resources.Documents.MetadataResources.IdentificationTypesQuery() { ById = request.ById, ForPrimary = request.ForPrimary, ForSecondary = request.ForSecondary }, cancellationToken);
    return new IdentificationTypesQueryResults(mapper.Map<IEnumerable<Contract.Metadatas.IdentificationType>>(identificationTypes)!);
  }

}
