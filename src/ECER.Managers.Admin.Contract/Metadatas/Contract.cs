﻿using MediatR;

namespace ECER.Managers.Admin.Contract.Metadatas;

/// <summary>
/// Invokes provinces use case
/// </summary>
public record ProvincesQuery : IRequest<ProvincesQueryResults>
{
  public string? ById { get; set; }
}
public record CountriesQuery : IRequest<CountriesQueryResults>
{
  public string? ById { get; set; }
  public string? ByCode { get; set; }
  public string? ByName { get; set; }
}

public record SystemMessagesQuery : IRequest<SystemMessagesQueryResults>
{
  public string? ById { get; set; }
}

public record SystemMessagesQueryResults(IEnumerable<SystemMessage> Items);
public record ProvincesQueryResults(IEnumerable<Province> Items);
public record CountriesQueryResults(IEnumerable<Country> Items);
public record Province(string ProvinceId, string ProvinceName);
public record Country(string CountryId, string CountryName, string CountryCode);
public record SystemMessage(string Name, string Subject, string Message)
{
  public DateTime StartDate { get; set; }
  public DateTime EndDate { get; set; }
  public IEnumerable<PortalTags> PortalTags { get; set; } = Array.Empty<PortalTags>();
}

public enum PortalTags
{
  LOGIN,
  LOOKUP,
  REFERENCES
}
