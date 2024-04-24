namespace ECER.Resources.Documents.MetadataResources;

public interface IMetadataResourceRepository
{
  Task<IEnumerable<Province>> QueryProvinces(ProvincesQuery query, CancellationToken cancellationToken);
}
public record Province(string ProvinceId, string ProvinceName);
public record ProvincesQuery
{
  public string? ById { get; set; }
}


