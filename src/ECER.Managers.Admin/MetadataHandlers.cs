using AutoMapper;
using ECER.Managers.Admin.Contract.Metadatas;
using ECER.Resources.Documents.MetadataResources;
using MediatR;

namespace ECER.Managers.Admin;

public class MetadataHandlers(
   IMetadataResourceRepository metadataResourceRepository,
   IMapper mapper) : IRequestHandler<Contract.Metadatas.ProvincesQuery, ProvincesQueryResults>,
   IRequestHandler<Contract.Metadatas.CountriesQuery, CountriesQueryResults>,
   IRequestHandler<Contract.Metadatas.SystemMessagesQuery, SystemMessagesQueryResults>
{
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
}
