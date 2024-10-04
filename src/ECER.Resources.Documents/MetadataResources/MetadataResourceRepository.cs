using AutoMapper;
using ECER.Utilities.DataverseSdk.Model;

namespace ECER.Resources.Documents.MetadataResources;

internal sealed class MetadataResourceRepository : IMetadataResourceRepository
{
  private readonly EcerContext context;
  private readonly IMapper mapper;

  public MetadataResourceRepository(EcerContext context, IMapper mapper)
  {
    this.context = context;
    this.mapper = mapper;
  }

  public async Task<IEnumerable<Province>> QueryProvinces(ProvincesQuery query, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;
    var provinces = context.ecer_ProvinceSet;
    if (query.ById != null) provinces = provinces.Where(r => r.ecer_ProvinceId == Guid.Parse(query.ById));

    return mapper.Map<IEnumerable<Province>>(provinces)!.ToList();
  }
}
