using AutoMapper;
using ECER.Utilities.DataverseSdk.Model;
using ECER.Utilities.DataverseSdk.Queries;

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
    if (query.ByCode != null) countries = countries.Where(r => r.ecer_ShortName == query.ByCode);
    if (query.ByName != null) countries = countries.Where(r => r.ecer_Name == query.ByName);

    return mapper.Map<IEnumerable<Country>>(countries)!.ToList();
  }

  public async Task<IEnumerable<PostSecondaryInstitution>> QueryPostSecondaryInstitutions(PostSecondaryInstitutionsQuery query, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;
    var postSecondaryInstitutions = context.ecer_PostSecondaryInstituteSet.AsQueryable();

    if (query.ById != null) postSecondaryInstitutions = postSecondaryInstitutions.Where(r => r.ecer_PostSecondaryInstituteId == Guid.Parse(query.ById));
    if (query.ByProvinceId != null) postSecondaryInstitutions = postSecondaryInstitutions.Where(r => r.ecer_ProvinceId.Id == Guid.Parse(query.ByProvinceId));
    if (query.ByName != null) postSecondaryInstitutions = postSecondaryInstitutions.Where(r => r.ecer_Name == query.ByName);

    var results = context.From(postSecondaryInstitutions)
      .Execute();

    return mapper.Map<IEnumerable<PostSecondaryInstitution>>(results)!.ToList();
  }

  public async Task<IEnumerable<IdentificationType>> QueryIdentificationTypes(IdentificationTypesQuery query, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;
    var identifications = context.ecer_identificationtypeSet;
    if (query.ById != null) identifications = identifications.Where(r => r.ecer_identificationtypeId == Guid.Parse(query.ById));
    if (query.ForPrimary != null) identifications = identifications.Where(r => r.ecer_ForPrimary == query.ForPrimary);
    if (query.ForSecondary != null) identifications = identifications.Where(r => r.ecer_ForSecondary == query.ForSecondary);

    return mapper.Map<IEnumerable<IdentificationType>>(identifications)!.ToList();
  }

  public async Task<IEnumerable<Province>> QueryProvinces(ProvincesQuery query, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;
    var provinces = context.ecer_ProvinceSet;
    if (query.ById != null) provinces = provinces.Where(r => r.ecer_ProvinceId == Guid.Parse(query.ById));

    return mapper.Map<IEnumerable<Province>>(provinces)!.ToList();
  }

  public async Task<IEnumerable<CertificationComparison>> QueryCertificationComparisons(CertificationComparisonQuery query, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;
    var certificationComparisons = context.ecer_certificationcomparisonSet;
    var results = context.From(certificationComparisons)
     .Join()
     .Include(a => a.ecer_certificationcomparisontransferringcertificate)
     .IncludeNested(a => a.ecer_outofprovincecertificationtype_Province_ecer_province).Execute();

    if (query.ById != null) results = results.Where(r => r.Id == Guid.Parse(query.ById));
    if (query.ByProvinceId != null) results = results.Where(r => r.ecer_certificationcomparisontransferringcertificate.ecer_Province.Id == Guid.Parse(query.ByProvinceId));

    return mapper.Map<IEnumerable<CertificationComparison>>(results)!.ToList();
  }

  public async Task<IEnumerable<SystemMessage>> QuerySystemMessages(SystemMessagesQuery query, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;
    var systemMessages = context.ecer_SystemMessageSet.Where(m => m.StatusCode == ecer_SystemMessage_StatusCode.Active);
    if (query.ById != null) systemMessages = systemMessages.Where(r => r.ecer_SystemMessageId == Guid.Parse(query.ById));

    return mapper.Map<IEnumerable<SystemMessage>>(systemMessages)!.ToList();
  }

  public async Task<string> SetDownloadDate(string fileId, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;

    var documentUrl =
      context.bcgov_DocumentUrlSet.Single(c => c.bcgov_DocumentUrlId == Guid.Parse(fileId));

    if (documentUrl == null) throw new InvalidOperationException($"documentUrl '{fileId}' not found");
    if (documentUrl.ecer_DownloadDate == null)
    {
      documentUrl.ecer_DownloadDate = DateTime.UtcNow;
      context.UpdateObject(documentUrl);
      context.SaveChanges();
    }
    return documentUrl.Id.ToString();
  }

  public async Task<IEnumerable<DefaultContent>> QueryDefaultContents(DefaultContentsQuery query, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;
    var defaultContents = context.ecer_DefaultContentsSet.AsQueryable();

    var results = context.From(defaultContents)
      .Execute();

    return mapper.Map<IEnumerable<DefaultContent>>(results)!.ToList();
  }

  public async Task<IEnumerable<DynamicsConfig>> QueryDynamicsConfiguration(DynamicsConfigQuery query, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;
    var dynamicsConfig = context.bcgov_configSet.Where(config => config.bcgov_Group == "Portal" && config.StateCode == bcgov_config_statecode.Active);

    return mapper.Map<IEnumerable<DynamicsConfig>>(dynamicsConfig)!.ToList();
  }
}
