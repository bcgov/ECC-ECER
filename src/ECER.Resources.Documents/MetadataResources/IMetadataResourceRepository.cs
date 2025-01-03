﻿namespace ECER.Resources.Documents.MetadataResources;

public interface IMetadataResourceRepository
{
  Task<IEnumerable<Province>> QueryProvinces(ProvincesQuery query, CancellationToken cancellationToken);

  Task<IEnumerable<Country>> QueryCountries(CountriesQuery query, CancellationToken cancellationToken);
}

public record Province(string ProvinceId, string ProvinceName);
public record Country(string CountryId, string CountryName, string CountryCode);
public record ProvincesQuery
{
  public string? ById { get; set; }
}

public record CountriesQuery
{
  public string? ById { get; set; }
  public string? ByCode { get; set; }
  public string? ByName { get; set; }
}
