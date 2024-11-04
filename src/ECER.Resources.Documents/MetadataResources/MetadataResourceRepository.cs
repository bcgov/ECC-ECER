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

  public async Task<IEnumerable<Country>> QueryCountries(CountriesQuery query, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;
    var countries = context.ecer_CountrySet;
    if (query.ById != null) countries = countries.Where(r => r.ecer_CountryId == Guid.Parse(query.ById));
    if (query.ByCode != null) countries = countries.Where(r => r.ecer_Name == query.ByCode);
    if (query.ByName != null) countries = countries.Where(r => r.ecer_Name == query.ByName);

    return mapper.Map<IEnumerable<Country>>(countries)!.ToList();
  }

  public async Task<IEnumerable<Province>> QueryProvinces(ProvincesQuery query, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;
    var provinces = context.ecer_ProvinceSet;
    if (query.ById != null) provinces = provinces.Where(r => r.ecer_ProvinceId == Guid.Parse(query.ById));

    return mapper.Map<IEnumerable<Province>>(provinces)!.ToList();
  }
}
