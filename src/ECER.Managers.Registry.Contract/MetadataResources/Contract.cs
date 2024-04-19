using MediatR;


namespace ECER.Managers.Registry.Contract.MetadataResources;


  /// <summary>
  /// Invokes provinces use case
  /// </summary>
  public record ProvincesQuery : IRequest<ProvincesQueryResults>
  {
    public string? ById { get; set; }
  }
  /// <summary>
  /// Container for <see cref="ProvincesQuery"/> results
  /// </summary>
  /// <param name="Items">The </param>
  public record ProvincesQueryResults(IEnumerable<Province> Items);

public record Province(string ProvinceId, string ProvinceName);

