using AutoMapper;
using ECER.Managers.Admin.Contract.MetadataResources;
using ECER.Resources.Documents.MetadataResources;
using MediatR;

namespace ECER.Managers.Admin;

public class MetadataResourceHandlers(
   IMetadataResourceRepository metadataResourceRepository,
   IMapper mapper
  ) : IRequestHandler<Contract.MetadataResources.ProvincesQuery, ProvincesQueryResults>
{
  public async Task<ProvincesQueryResults> Handle(Contract.MetadataResources.ProvincesQuery request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(metadataResourceRepository);
    ArgumentNullException.ThrowIfNull(mapper);
    ArgumentNullException.ThrowIfNull(request);

    var provinces = await metadataResourceRepository.QueryProvinces(new Resources.Documents.MetadataResources.ProvincesQuery() { ById = request.ById }, cancellationToken);
    return new ProvincesQueryResults(mapper.Map<IEnumerable<Contract.MetadataResources.Province>>(provinces)!);
  }
}
