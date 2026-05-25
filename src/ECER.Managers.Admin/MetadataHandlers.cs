using ECER.Managers.Admin.Contract.Metadatas;
using ECER.Resources.Documents.MetadataResources;
using Mediator;

namespace ECER.Managers.Admin;

public class MetadataHandlers(
   IMetadataResourceRepository metadataResourceRepository,
   IMetadataMapper metadataMapper) : IRequestHandler<Contract.Metadatas.ProvincesQuery, ProvincesQueryResults>,
   IRequestHandler<Contract.Metadatas.CountriesQuery, CountriesQueryResults>,
   IRequestHandler<Contract.Metadatas.AreaOfInstructionsQuery, AreaOfInstructionsQueryResults>,
   IRequestHandler<Contract.Metadatas.CertificationComparisonQuery, CertificationComparisonQueryResults>,
   IRequestHandler<Contract.Metadatas.PostSecondaryInstitutionsQuery, PostSecondaryInstitutionsQueryResults>,
   IRequestHandler<Contract.Metadatas.SystemMessagesQuery, SystemMessagesQueryResults>,
   IRequestHandler<Contract.Metadatas.DefaultContentsQuery, DefaultContentsQueryResults>,
   IRequestHandler<Contract.Metadatas.IdentificationTypesQuery, IdentificationTypesQueryResults>,
   IRequestHandler<Contract.Metadatas.DynamicsConfigQuery, DynamicsConfigQueryResults>
{
  public async ValueTask<PostSecondaryInstitutionsQueryResults> Handle(Contract.Metadatas.PostSecondaryInstitutionsQuery request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var postSecondaryInstitutions = await metadataResourceRepository.QueryPostSecondaryInstitutions(metadataMapper.MapPostSecondaryInstitutionsQuery(request), cancellationToken);
    return new PostSecondaryInstitutionsQueryResults(metadataMapper.MapPostSecondaryInstitutions(postSecondaryInstitutions));
  }

  public async ValueTask<CertificationComparisonQueryResults> Handle(Contract.Metadatas.CertificationComparisonQuery request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var certificationComparisons = await metadataResourceRepository.QueryCertificationComparisons(new Resources.Documents.MetadataResources.CertificationComparisonQuery() { ById = request.ById, ByProvinceId = request.ByProvinceId }, cancellationToken);
    var comparisonRecords = certificationComparisons
      .GroupBy(x => new { x.TransferringCertificate!.Id, x.TransferringCertificate.CertificationType })
      .Select(g => new Contract.Metadatas.ComparisonRecord()
      {
        TransferringCertificate = new Contract.Metadatas.OutOfProvinceCertificationType(g.Key.Id)
        {
          CertificationType = g.Key.CertificationType
        },
        Options = g.Select(item => new Contract.Metadatas.CertificationComparison(item.Id)
        {
          BcCertificate = item.BcCertificate
        })
      .ToList()
      })
      .ToList();

    return new CertificationComparisonQueryResults(comparisonRecords!);
  }

  public async ValueTask<ProvincesQueryResults> Handle(Contract.Metadatas.ProvincesQuery request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var provinces = await metadataResourceRepository.QueryProvinces(new Resources.Documents.MetadataResources.ProvincesQuery() { ById = request.ById }, cancellationToken);
    return new ProvincesQueryResults(metadataMapper.MapProvinces(provinces));
  }

  public async ValueTask<AreaOfInstructionsQueryResults> Handle(Contract.Metadatas.AreaOfInstructionsQuery request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var instructions = await metadataResourceRepository.QueryAreaOfInstructions(new Resources.Documents.MetadataResources.AreaOfInstructionsQuery() { ById = request.ById }, cancellationToken);
    return new AreaOfInstructionsQueryResults(metadataMapper.MapAreaOfInstructions(instructions));
  }

  public async ValueTask<SystemMessagesQueryResults> Handle(Contract.Metadatas.SystemMessagesQuery request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var systemMessages = await metadataResourceRepository.QuerySystemMessages(new Resources.Documents.MetadataResources.SystemMessagesQuery() { ById = request.ById }, cancellationToken);

    // Return only active system messages
    systemMessages = systemMessages.Where(m => m.StartDate < DateTime.Now && m.EndDate > DateTime.Now).ToList();

    return new SystemMessagesQueryResults(metadataMapper.MapSystemMessages(systemMessages));
  }

  public async ValueTask<DefaultContentsQueryResults> Handle(Contract.Metadatas.DefaultContentsQuery request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var defaultContents = await metadataResourceRepository.QueryDefaultContents(new Resources.Documents.MetadataResources.DefaultContentsQuery(), cancellationToken);

    return new DefaultContentsQueryResults(metadataMapper.MapDefaultContents(defaultContents));
  }

  public async ValueTask<CountriesQueryResults> Handle(Contract.Metadatas.CountriesQuery request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var countries = await metadataResourceRepository.QueryCountries(new Resources.Documents.MetadataResources.CountriesQuery() { ById = request.ById, ByCode = request.ByCode, ByName = request.ByName }, cancellationToken);
    return new CountriesQueryResults(metadataMapper.MapCountries(countries));
  }

  public async ValueTask<IdentificationTypesQueryResults> Handle(Contract.Metadatas.IdentificationTypesQuery request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var identificationTypes = await metadataResourceRepository.QueryIdentificationTypes(new Resources.Documents.MetadataResources.IdentificationTypesQuery() { ById = request.ById, ForPrimary = request.ForPrimary, ForSecondary = request.ForSecondary }, cancellationToken);
    return new IdentificationTypesQueryResults(metadataMapper.MapIdentificationTypes(identificationTypes));
  }

  public async ValueTask<DynamicsConfigQueryResults> Handle(Contract.Metadatas.DynamicsConfigQuery request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var dynamicsConfig = await metadataResourceRepository.QueryDynamicsConfiguration(new Resources.Documents.MetadataResources.DynamicsConfigQuery() { }, cancellationToken);
    return new DynamicsConfigQueryResults(metadataMapper.MapDynamicsConfig(dynamicsConfig));
  }
}
